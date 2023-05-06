using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputManager : MonoSingleton<PlayerInputManager>
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
    private float jumpVelocity = 5f;
    private int status;
    public Transform _target;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private float direction;

    //OnGround Detection
    public LayerMask mask;
    private Vector2 boxSize;
    private Vector2 playerSize;
    private readonly float boxHeight = 0.4f;

    //Double Jump Detection
    private bool isAddDoubleJumpDetection = false;
    void Start()
    {
        InitComponent();
        //Add Move Detection        
        AddDetection(() => { return true; }, Move);
        //Add Jump Detection
        AddDetection(GetJump, Jump);
        AddDetection(() => { return _rigidbody.velocity.y < 0; }, Fall);
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
        _rigidbody = _target.GetComponent<Rigidbody2D>();
        _animator = _target.GetComponent<Animator>();

        playerSize = _target.GetComponent<SpriteRenderer>().bounds.size;
        boxSize = new Vector2(playerSize.x * 0.6f, boxHeight);
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
        }
    }
    private void Jump()
    {
        if ((status & (int)Status.Jump) == 0)
        {
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
                Delay = 0.5f,
                condition = GetJump,
                removeCondition = () => { return (status & (int)Status.OnGround) != 0; },
                command = DoubleJump
            });
        }
    }
    private void DoubleJump()
    {
        status |= (int)Status.DoubleJump;
        //status &=~(int)Status.Fall;
        if((status & (int)Status.Fall)== 0)
            _rigidbody.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        else
            _rigidbody.AddForce(Vector2.up *2* jumpVelocity, ForceMode2D.Impulse);
        _animator.SetBool(AnimationName.Fall, false);
        _animator.SetTrigger(AnimationName.DoubleJump);
    }
    private void OnGround()
    {
        Vector2 boxCenter = (Vector2)_target.position + (Vector2.down * playerSize.y * 0.5f);
        if(Physics2D.OverlapBox(boxCenter, boxSize, 0, mask))
        {
            status &= ~(int)Status.Jump;
            status &= ~(int)Status.DoubleJump;
            status &= ~(int)Status.Fall;
            status |= (int)Status.OnGround;
            _animator.SetBool(AnimationName.Fall, false);
            _animator.SetBool(AnimationName.Jump, false);
            isAddDoubleJumpDetection = false;
        }
        else
        {
            status &= ~(int)Status.OnGround;
        }
    }
    public static void AddDetection(Func<bool> condition,Action call)
    {
        Instance.InputDetectionList.Add(new KeyValuePair<Func<bool>, Action>(condition, call)); 
    }

    int i;
    // Update is called once per frame
    private void FixedUpdate()
    {
        for (i = InputDetectionList.Count - 1; i >= 0; i--)
        {
            if (InputDetectionList[i].Key())
            {
                InputDetectionList[i].Value();
            }
        }
    }


    private void OnDrawGizmos()
    {
        if((status& (int)Status.OnGround)==0)
        {
            Gizmos.color = Color.green;
        }else
        {
            Gizmos.color = Color.red;
        }
        Vector2 boxCenter = (Vector2)_target.position + (Vector2.down * playerSize.y * 0.5f);
        Gizmos.DrawCube(boxCenter, boxSize);
    }
}
