using Assets.Scripts.Common;
using Assets.Scripts.GameManager;
using Assets.Scripts.StateMachine;
using Mirror.Examples.NetworkRoomExt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    public class PlayerInputManager : NetworkBehaviour
    {
        public enum Status
        {
            Idle = 1 << 0,
            Move = 1 << 1,
            Jump = 1 << 2,
            DoubleJump = 1 << 3,
            Fall = 1 << 4,
            OnGround = 1 << 5,
            BeHit = 1 << 6,

        }
        //Input Detect List
        private List<KeyValuePair<Func<bool>, Action>> InputDetectionList = new List<KeyValuePair<Func<bool>, Action>>();
        private readonly float speed = 5f;
        private readonly float jumpVelocity = 11f;
        private int status;
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private float direction;

        //OnGround Detection
        public LayerMask mask;
        private Vector2 playerSize;
        //key-vibration eliminate
        private bool buttonJump = true;

        //Double Jump Detection
        private bool isAddDoubleJumpDetection = false;
        //Fall GravityScale
        private float fallMultiplier = 2.5f;
        private float jumpMultiplier = 2f;

        //Fire Detection
        private float lastFireTime;
        public float FireInterval = 1f;
        public Transform FireTF;
        public GameObject Bullet;
        //Death Height
        private float DeathHeight = -10;
        public bool CanControl { get; set; }
        public override void OnStartAuthority()
        {
            InitComponent();
            this.enabled = true;
            Debug.Log("Script Active");
            SetComponent();
        }
        public override void OnStopAuthority()
        {
            InputDetectionList.Clear();
            this.enabled = false;
        }
        private void AddInputDetection()
        {
            //Add Move Detection        
            AddDetection(() => { return true; }, Move);
            //Add Jump Detection
            AddDetection(GetJump, Jump);
            AddDetection(() => { return _rigidbody.velocity.y < 0.1f; }, Fall);
            AddDetection(() => { return _rigidbody.velocity.y > 0.1f; }, AnimJump);
            //Add Double Jump Detection
            AddDetection(() => { return (status & (int)Status.OnGround) == 0; }, AddDoubleJump2Command);
            //Add OnGround Detection
            AddDetection(() => { return true; }, OnGround);
            AddDetection(Fire,OnFire);
            AddDetection(FallOverDeathLine, Death);
        }

        private bool FallOverDeathLine()
        {
            //Debug.Log("Death="+DeathHeight+" transform.y="+transform.position.y);
            return (transform.position.y < DeathHeight)&&CanControl;
        }
        [Command(requiresAuthority = false)]
        private void Death()
        {
            GetComponent<PlayerScore>().Death();
            NetworkRoomManagerExt.singleton.AliveCount--;
            if (NetworkRoomManagerExt.singleton.AliveCount <= 1)
                SendChangeStateMessage();
        }
        public void SendChangeStateMessage()
        {
            NetworkServer.SendToAll(new StateMessage
            {
                newStateID = (int)GameStateID.Result
            });
        }
        private bool Fire()
        {
            return Input.GetKeyDown(KeyCode.J)&&(Time.time- lastFireTime>= FireInterval)&& CanControl;
        }
        private void OnFire()
        {
            lastFireTime = Time.time;
            AudioManager.Play(SoundName.Fire);
            GenerateBulletOnServer();//NetworkServer.Spawn(tf.gameObject);
            ReloadForFirePrefabOnServer();
        }
        [Command(requiresAuthority = false)]
        public void GenerateBulletAndHideOnServer()
        {
            Vector3 rotation = transform.rotation.eulerAngles.y > 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, -90);
            NetworkPrefabPoolManager.Instance.CreateObjectAndHide(GetComponent<PlayerScore>().index, Bullet, FireTF.position, Quaternion.Euler(rotation), gameObject);
        }
        [Command(requiresAuthority = false)]
        public void GenerateBulletOnServer()
        {
            Vector3 rotation = transform.rotation.eulerAngles.y > 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, -90);
            NetworkPrefabPoolManager.Instance.CreateObject(GetComponent<PlayerScore>().index, Bullet, FireTF.position, Quaternion.Euler(rotation),gameObject);
        }
        
        [Command(requiresAuthority = false)]
        public void ReloadForFirePrefabOnServer()
        {
            NetworkPrefabPoolManager.Instance.ReloadPrefab(GetComponent<PlayerScore>().index, gameObject);
        }

        private bool GetJump()
        {
            return Input.GetButton("Jump");
        }
        private void InitComponent()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            playerSize = GetComponent<SpriteRenderer>().bounds.size;
            _animator = GetComponent<Animator>();
        }
        public void SetComponent()
        {
            AddInputDetection();
        }

        private void Move()
        {
            direction = Input.GetAxis("Horizontal");
            if (!CanControl) direction =0;
            if (direction != 0)
            {
                if (Mathf.Abs(_rigidbody.velocity.x) < 5)
                    _rigidbody.AddForce(new Vector2(direction * speed, 0));

                _rigidbody.transform.rotation = Quaternion.Euler(0, 180 * (direction > 0 ? 0 : 1), 0);
                _animator.SetBool(AnimationName.Run, true);
                status |= (int)Status.Move;
            }
            else
            {
                _animator.SetBool(AnimationName.Run, false);
                status &= ~(int)Status.Move;
            }
        }
        private void Fall()
        {
            if ((status & (int)Status.Fall) == 0 && (status & (int)Status.OnGround) == 0)
            {
                status |= (int)Status.Jump;
                status |= (int)Status.Fall;
                _animator.SetBool(AnimationName.Fall, true);
                _rigidbody.gravityScale = fallMultiplier;
            }
        }
        private void AnimJump()
        {
            if((status & (int)Status.OnGround) == 0)
                _animator.SetBool(AnimationName.Jump, true);
        }

        private IEnumerator WaitForJump()
        {
            yield return new WaitForSeconds(0.2f);
            buttonJump = true;
        }
        private void Jump()
        {
            if (!CanControl) return;
            if ((status & (int)Status.Jump) == 0 && ((status &= (int)Status.Jump) == 0) && buttonJump)
            {
                AudioManager.Play(SoundName.Jump);
                buttonJump = false;
                StartCoroutine(WaitForJump());
                if (_rigidbody.velocity.y != 0&&((_rigidbody.velocity.x>0&& direction<0)||(_rigidbody.velocity.x < 0 && direction > 0)))
                    _rigidbody.velocity = Vector2.zero;
                else
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x,0);
                _rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);

                _animator.SetBool(AnimationName.Jump, true);
                status |= (int)Status.Jump;
            }
        }

        private void AddDoubleJump2Command()
        {
            if (!isAddDoubleJumpDetection)
            {
                isAddDoubleJumpDetection = true;
                //DoubleJump
                CommandDispatcher.PushCommand(new CommandData()
                {
                    DelayRemove = true,
                    Delay = 0.3f,
                    condition = GetJump,
                    removeCondition = () => { return (status & (int)Status.OnGround) != 0; },
                    command = DoubleJump
                });
            }
        }
        //int idd =0;
        private void DoubleJump()
        {
            AudioManager.Play(SoundName.Jump);
            status |= (int)Status.DoubleJump;
            //idd++;
            //Debug.Log("DoubleJump"+ idd);
            ////float y = _rigidbody.velocity.y > 0? jumpVelocity:jumpVelocity - _rigidbody.velocity.y;
            //Debug.Log("x=" + _rigidbody.velocity.x);
            //Debug.Log("direction=" + direction);
            if (_rigidbody.velocity.y != 0 && ((_rigidbody.velocity.x > 0 && direction < 0) || (_rigidbody.velocity.x < 0 && direction > 0)))
                _rigidbody.velocity = Vector2.zero;
            else
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(Vector2.up * jumpVelocity,ForceMode2D.Impulse);
            _animator.SetBool(AnimationName.DoubleJump, true);
        }
        private void OnGround()
        {
            //whether leave ground
            if (Physics2D.Raycast((Vector2)transform.position - new Vector2(playerSize.x * 0.4f,0), Vector2.down, playerSize.y * 0.5f, mask)
                || Physics2D.Raycast((Vector2)transform.position + new Vector2(playerSize.x * 0.4f, 0), Vector2.down, playerSize.y * 0.5f, mask)
                )
            {
                if ((status &= (int)Status.OnGround) == 0)
                {
                    status &= ~(int)Status.Jump;
                    status &= ~(int)Status.DoubleJump;
                    status &= ~(int)Status.Fall;
                    status |= (int)Status.OnGround;
                    _animator.SetBool(AnimationName.Fall, false);
                    _animator.SetBool(AnimationName.Jump, false);
                    _animator.SetBool(AnimationName.DoubleJump, false);
                    isAddDoubleJumpDetection = false;
                    _rigidbody.gravityScale = jumpMultiplier;
                }
            }
            else
            {
                status &= ~(int)Status.OnGround;
            }
        }
        public void AddDetection(Func<bool> condition, Action call)
        {
            InputDetectionList.Add(new KeyValuePair<Func<bool>, Action>(condition, call));
        }

        int i;
        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer) return;

            for (i = InputDetectionList.Count - 1; i >= 0; i--)
            {
                if (InputDetectionList[i].Key())
                {
                    InputDetectionList[i].Value();
                }
            }
        }


        //private void OnDrawGizmos()
        //{
        //    if((status& (int)Status.OnGround)==0)
        //    {
        //        Gizmos.color = Color.green;
        //    }else
        //    {
        //        Gizmos.color = Color.red;
        //    }
        //    Vector2 boxCenter = (Vector2)_target.position + (Vector2.down * playerSize.y * 0.5f);
        //    Gizmos.DrawCube(boxCenter, boxSize);
        //}
    }
}
