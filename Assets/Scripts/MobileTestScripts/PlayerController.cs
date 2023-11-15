using System.Collections;
using System.Collections.Generic;
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

    [Header("PLAYER INFO")]
    [SerializeField] float speed = 5f;  // 캐릭터 이동 속도
    [SerializeField] private float moveHorizontal;
    [SerializeField] private float moveVertical;
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
    }

    private void FixedUpdate()
    {

    }

    private void Move()
    {
        moveHorizontal = Input.GetAxis("Horizontal");  // 수평 이동
        moveVertical = Input.GetAxis("Vertical");  // 수직 이동
        Debug.Log($"moveHorizontal : {moveHorizontal} / moveVertical / {moveVertical}");
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        transform.Translate(movement * speed * Time.deltaTime);

        SetAnim();
    }

    private void SetAnim()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(AnimState.IsJump);
        }
        else if (0f != moveHorizontal || 0f != moveVertical)
        {
            ChangeState(AnimState.IsRun);
        }
        else if (0f == moveHorizontal && 0f == moveVertical)
        {
            ChangeState(AnimState.Idle);
        }
    }

    private void Initialized()
    {
        _curState = AnimState.None;
    }

    void ChangeState(AnimState argState)
    {
        if (argState == _curState)
            return;

        StartCoroutine(ChangeStateCor(argState));
    }
    IEnumerator ChangeStateCor(AnimState argState)
    {
        _animator.StopPlayback();
        _curState = argState;
        _animator.CrossFadeInFixedTime(nameof(argState), 0.1f, -1, 0f);
        //_animator.Play(nameof(argState), -1, 0f);
        yield return null;
    }
}
