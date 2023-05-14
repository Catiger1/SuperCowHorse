using Assets.Scripts.Common;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerInputManager :  NetworkBehaviour
{
    public enum Status
    {
        Idle = 1<<0,
        Move = 1<<1,
        Jump = 1<<2,
        DoubleJump = 1 << 3,
        Fall = 1 << 4,
        OnGround = 1 << 5,
        BeHit= 1 << 6,

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
    //private Vector2 boxSize;
    //private readonly float boxHeight = 0.2f;

    //key-vibration eliminate
    private bool buttonJump = true;

    //Double Jump Detection
    private bool isAddDoubleJumpDetection = false;
    //Fall GravityScale
    private float fallMultiplier = 2.5f;
    private float jumpMultiplier = 2f;

    //Frition
    public PhysicsMaterial2D ZeroFrition;
    public PhysicsMaterial2D NormalFrition;

    void Start()
    {
        InitComponent();
        SetComponent(0);
    }
    public override void OnStartAuthority()
    {
        this.enabled = true;
    }

    public override void OnStopAuthority()
    {
        this.enabled = false;
    }
    private void AddInputDetection()
    {
        InputDetectionList.Clear();
        //Add Move Detection        
        AddDetection(() => { return true; }, Move);
        //Add Jump Detection
        AddDetection(GetJump, Jump);
        AddDetection(() => { return _rigidbody.velocity.y < 0.1f; }, Fall);
        //Add Double Jump Detection
        AddDetection(() => { return (status & (int)Status.OnGround) == 0; }, AddDoubleJump2Command);
        //Add OnGround Detection
        AddDetection(() => { return true; }, OnGround);
    }

    private bool GetJump()
    {
        return Input.GetButton("Jump");
    }
    private void InitComponent()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        playerSize = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size;
    }
    private void SetComponent(int index)
    {
        Transform child = transform.GetChild(index);
        _animator = child.GetComponent<Animator>();
        AddInputDetection();
        //boxSize = new Vector2(playerSize.x * 0.6f, boxHeight);
    }

    private void Move()
    {
        direction = Input.GetAxis("Horizontal");
        if (direction != 0)
        {
            _rigidbody.velocity = new Vector2(direction* speed, _rigidbody.velocity.y);
            _rigidbody.transform.rotation = Quaternion.Euler(0,180*(direction>0?0:1),0);
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
        if((status & (int)Status.Fall) == 0&& (status & (int)Status.OnGround) == 0)
        {
            status |= (int)Status.Jump;
            status |= (int)Status.Fall;
            _animator.SetBool(AnimationName.Fall, true);
            _rigidbody.gravityScale = fallMultiplier;
        }
    }
    private IEnumerator WaitForJump()
    {
        yield return new WaitForSeconds(0.2f);
        buttonJump = true;
    }
    private void Jump()
    {
        if ((status & (int)Status.Jump) == 0&&((status &= (int)Status.Jump) == 0)&& buttonJump)
        {
            buttonJump = false;
            StartCoroutine(WaitForJump());
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
                Delay = 0.2f,
                condition = GetJump,
                removeCondition = () => { return (status & (int)Status.OnGround) != 0; },
                command = DoubleJump
            });
        }
    }
    private void DoubleJump()
    {
        status |= (int)Status.DoubleJump;
        _rigidbody.AddForce(Vector2.up * (jumpVelocity-_rigidbody.velocity.y), ForceMode2D.Impulse);
        _animator.SetBool(AnimationName.DoubleJump,true);
    }
    private void OnGround()
    {
        //whether leave ground
        if(Physics2D.Raycast((Vector2)transform.position, Vector2.down, playerSize.y * 0.5f,mask))
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
                _rigidbody.sharedMaterial = NormalFrition;
            }
        }
        else
        {
            status &= ~(int)Status.OnGround;
            _rigidbody.sharedMaterial = ZeroFrition;
        }
    }
    public void AddDetection(Func<bool> condition,Action call)
    {
        InputDetectionList.Add(new KeyValuePair<Func<bool>, Action>(condition, call)); 
    }

    int i;
    // Update is called once per frame
    private void FixedUpdate()
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
