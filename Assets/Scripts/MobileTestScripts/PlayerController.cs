using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum AnimState
    {
        None,
        Idle,
        IsRun,
        IsJump,
    }

    [Header("ANIMATOR COMPONENT")]
    [SerializeField] private Animator _animator;
    [Header("CURRENT STATE")]
    [SerializeField] private AnimState _curState;
    [Header("PLAYER CONTROLLER")]
    [SerializeField] private VirtualJoystick _joystick;
    [Header("PLAYER INFO")]
    [SerializeField] float speed = 5f;  // 캐릭터 이동 속도
    [SerializeField] private float moveHorizontal;
    [SerializeField] private float moveVertical;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool _isJumping = false;

    private void Awake()
    {
        Initialized();
    }

    void Start()
    {
        ChangeState(AnimState.Idle);
    }

    void Update()
    {
        Move();
        SetAnim();
    }

    private void Move()
    {
        bool isValid = this.isValid();
        if (true == isValid)
            return;
        //moveHorizontal = Input.GetAxis("Horizontal");  // 수평 이동
        //moveVertical = Input.GetAxis("Vertical");  // 수직 이동
        moveHorizontal = _joystick.GetHorizontal();  // 수평 이동
        moveVertical = _joystick.GetVertical();  // 수직 이동

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        if (movement != Vector3.zero) // movement가 0이 아닐 때만 회전
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position += new Vector3(moveHorizontal, 0, moveVertical) * speed * Time.deltaTime;
    }

    private void SetAnim()
    {
        bool isValid = this.isValid();
        if (false == isValid && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ResetState(AnimState.IsJump));
            //Debug.Log(nameof(AnimState.IsJump));
        }
        else if (false == isValid && (0f != moveHorizontal || 0f != moveVertical))
        {
            //_animator.Play(nameof(AnimState.IsRun));
            ChangeState(AnimState.IsRun);
            //Debug.Log(nameof(AnimState.IsRun));
        }
        else if (false == isValid && 0f == moveHorizontal && 0f == moveVertical)
        {
            ChangeState(AnimState.Idle);
            //Debug.Log(nameof(AnimState.Idle));
        }
    }

    private IEnumerator ResetState(AnimState argState)
    {
        SetFlag(argState, true);
        ChangeState(argState);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        SetFlag(argState, false);
    }

    private bool isValid()
    {
        if (true == _isJumping)
        {
            return true;
        }

        return false;
    }

    private void Initialized()
    {
        _curState = AnimState.None;
    }

    void ChangeState(AnimState argState)
    {
        _curState = argState;
        _animator.Play(argState.ToString());
        Debug.Log(argState);
    }

    void SetFlag(AnimState argState, bool argValue)
    {
        switch (argState)
        {
            case AnimState.IsJump:
                { 
                    _isJumping = argValue;
                    break;
                }
            default:
                break;
        }
    }
}
