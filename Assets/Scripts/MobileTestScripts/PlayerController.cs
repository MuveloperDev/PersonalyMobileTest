using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AnimState
{
    None,
    Idle,
    IsRun,
    IsJump,
}
public class PlayerController : MonoBehaviour
{

    [Header("DEPENDENCY")]
    [SerializeField] private Animator _animator;
    [SerializeField] private VirtualJoystick _joystick;
    [SerializeField] private ActionButtonManager _buttonManager;

    [Header("CURRENT STATE")]
    [SerializeField] private AnimState _curState;
    
    [Header("PLAYER INFO")]
    [SerializeField] float speed = 5f;  // 캐릭터 이동 속도
    [SerializeField] private float moveHorizontal;
    [SerializeField] private float moveVertical;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private bool _isInitialized = false;
    private async void Awake()
    {
        await UniTask.WaitUntil(() => null != IngameUIManager.Instance.InputSystem);
        //await UniTask.WaitUntil( ()=> 0 != _buttonManager.GetBtnDic().Count);
        Initialized();
    }

    void Start()
    {
        ChangeState(AnimState.Idle);
    }

    void Update()
    {
        if (false == _isInitialized)
            return;

        Move();
        SetAnim();
    }

    private void Move()
    {
        bool isValid = this.isValid();
        if (true == isValid)
            return;
        moveHorizontal = _joystick.GetHorizontal();  // 수평 이동
        moveVertical = _joystick.GetVertical();  // 수직 이동

        // 카메라의 회전 적용하기
        Camera cam = Camera.main;  // 메인 카메라 가져오기
        Vector3 camForward = cam.transform.forward;  // 카메라의 전방향 벡터 가져오기
        camForward.y = 0;  // y 축은 0으로 설정 (수평 이동만 고려)
        camForward.Normalize();  // 정규화 (길이를 1로 만들기)

        Vector3 camRight = cam.transform.right;  // 카메라의 오른쪽 방향 벡터 가져오기
        camRight.y = 0;  // y 축은 0으로 설정
        camRight.Normalize();  // 정규화

        // 이동 벡터를 카메라의 방향에 맞게 변환
        Vector3 movement = camRight * moveHorizontal + camForward * moveVertical;

        if (movement != Vector3.zero) // movement가 0이 아닐 때만 회전
        {
            Quaternion toRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position += movement * speed * Time.deltaTime;
    }

    private void SetAnim()
    {
        bool isValid = this.isValid();
        if (false == isValid && (0f != moveHorizontal || 0f != moveVertical))
        {
            ChangeState(AnimState.IsRun);
            //Debug.Log(nameof(AnimState.IsRun));
        }
        else if (false == isValid && 0f == moveHorizontal && 0f == moveVertical)
        {
            ChangeState(AnimState.Idle);
            //Debug.Log(nameof(AnimState.Idle));
        }
    }

    public void ActionAnim(AnimState argState)=> ResetState(argState).Forget();

    private async UniTask ResetState(AnimState argState)
    {
        SetFlag(argState, true);
        ChangeState(argState);
        await UniTask.WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length - 1f);
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
        _buttonManager = IngameUIManager.Instance.InputSystem.GetActionButtonManager();
        _joystick = IngameUIManager.Instance.InputSystem.GetJoystick();

        var buttonDic = _buttonManager.GetBtnDic();

        //buttonDic[ActionButtonManager.ActionButtons.NormalAttack].onAction = ()=> { };
        buttonDic[ActionButtonManager.ActionButtons.NormalAttack]._onClickUpEvent += (eventData) => { ActionAnim(AnimState.IsJump); };

        _isInitialized = true;
    }

    void ChangeState(AnimState argState)
    {
        if (argState == _curState)
            return;

        _curState = argState;
        _animator.Play(argState.ToString());
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
