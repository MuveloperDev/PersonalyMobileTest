using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public enum SkillSlotType
    { 
        None = 0,
        Basic,
        Special,
        Ultimate,
        Passive
    }

    public enum StateType
    {
        None = -1,
        RadialProgress,
        CoolTime,

    }

    public enum SkillCategoryType
    {
        Active = 0,
        Passive
    }

    public enum SkillMainType
    {
        None = 0,
        StraightLine_UserCenteredCasting, // 직선 - 사용자 중심 시전
        StraightLine_UserCenteredDash, // 직선 - 사용자 중심 돌진
        FanShaped_UserCenteredCasting, // 부채꼴 - 사용자 중심 시전
        Circular_UserCenteredCasting, // 원형 - 사용자 중심 시전
        Circular_FreeRangeDesignation, // 원형 - 자유 범위 지정
        ToggleForm // 토글 형태
    }

    //public enum SkillRange
    //{
    //    StraightLine = 1,
    //    FanShaped,
    //    Circular_Radius_3,
    //    Circular_Radius_4,
    //    Circular_Radius_5,
    //}

    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _radialImage;
    [SerializeField] private RadialProgress _radialProgress;
    [SerializeField] private IngameVirtualJoystickBase _joystick;

    [Header("STATE TYPES")]
    [SerializeField] private List<StateType> _types;
    // 추후 구조체로 만들어서 사용 예정.
    [Header("SKILL INFOMATION")]
    // 매니저에서 테이블데이터를 캐싱 후 데이터를 넘겨 받을것 
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillIconPath;
    [SerializeField] private string _skillSimpleDescription;

    [Header("Combo")]
    [SerializeField] private float _skillComboMaxCount; // 스킬 콤보의 최대 수치

    [Header("SKILL")]
    [SerializeField] private int _skillSlotType;
    [SerializeField] private int _skillCategoryType;
    [SerializeField] private int _skillMainType;
    [SerializeField] private int _skillSubType;

    [SerializeField] private float _skillRange; 
    [SerializeField] private float _skillRangePositionOffset;
    [SerializeField] private float[] _generateSkillRangeTimeSec;
    [SerializeField] private float _useSkillMaxRange;

    [Header("CHARGE")]
    [SerializeField] private int _chargeType;
    [SerializeField] private float _chargeMoveDistance;

    [Header("Motion")]
    [SerializeField] private float _motionCancellableTimeSec;


    [Header("CASTING")]
    [SerializeField] private bool _useCasting;
    [SerializeField] private float _astingTimeSec;

    [Header("NOTIFICATION")]
    [SerializeField] private string _skillRangeNotificationFxPrefabPath;
    [SerializeField] private float _skillRangeNotificationTimeSec;

    [Header("INDICATOR")]
    [SerializeField] private string _skillGuidLineFXPrefabPath;
    [SerializeField] private float _castingTimeSec;

    [Header("CHANNELING")]
    [SerializeField] private bool _startChannelingTick;
    [SerializeField] private float _channelingDuration;
    [SerializeField] private int _channelingTickCount;
    [SerializeField] private float _channelingTickIntervalTimeSec;

    [Header("EXPLOSION")]
    [SerializeField] private int[] _explosionType; // enum으로 변환예정
    [SerializeField] private int[] _generateExplosionPositionType; // 폭발 생성위치 설정 enum으로 변환예정
    [SerializeField] private float _exlosionTimeSec; // 폭발하기전 유지시간을 설정한다.
    [SerializeField] private float _explosionRagneType; // 생성 시킬 폭발 범위 타입을 지정한다.
    [SerializeField] private float _explosionRange; // 생성시킬 폭발 범위를 설정한다. enum으로 변환예정
    [SerializeField] private float _explosionNotificationTimeSec; // 폭발 범위 노티 표시 시간을 몇초 표시해줄지 설정한다.
    [SerializeField] private float _explosionNotificationFxPrefabPath; // 폭발 생성 위치에 모든 유저에게 노출되는 노티 리소스 경로를 지정한다.

    [Header("COOLTIME")]
    [SerializeField] private int _coolTimeType; //enum으로 변환예정
    [SerializeField] private bool _useCoolTimeGauge; 
    [SerializeField] private float _globalCoolTimeSec; // 초반 시작 전체 쿨타임
    [SerializeField] private float _coolTimeSec; // 쿨타임 시간 지정
    [SerializeField] private int _stackMaxCount; // 스택의 최대치를 설정
    [SerializeField] private int _unavailableAwhileTimeSec; // 스택의 최대치를 설정
    [Header("SKILL CHANGE TYPE")]
    [SerializeField] private int _skillChangeType; // 스택의 최대치를 설정
    [SerializeField] private float _maintainTimeSec; // 스킬이 변경된 후 유지 시간 설정





    #region [Button_Interaction_Handler]
    public Action<PointerEventData> _onClickDownEvent;
    public Action<PointerEventData> _onClickUpEvent;
    public Action<PointerEventData> _onBeginDragEvent;
    public Action<PointerEventData> _onDragEvent;
    public Action<PointerEventData> _onEndDragEvent;
    #endregion
    private CancellationTokenSource _cts = new CancellationTokenSource();


    // 스킬 타입을 인자로 받는다.(매니저에서)
    // 스킬 타입에 대한 데이터를 인자로 받는다 (매니저 -> ActionButton).
    // 거기에 따른 타입 설정.
    public void Initialize(List<StateType> argTypes)
    {
        _types = argTypes;

        foreach (var stateType in _types)
        {
            switch (stateType)
            {
                case StateType.RadialProgress:
                    {
                        if (null == _radialImage)
                        {
                            Debug.LogError($"{GetType()} radialImage is null");
                            return;
                        }
                        _radialProgress = new RadialProgress(1f, this, _radialImage);
                        break;
                    }
                case StateType.CoolTime:
                    {
                        break;
                    }
            }
        }

        AddLitener();
        //_button.SetUseProcessPressing(true);
    }

    public void SetType(List<StateType> argTypes) => _types = argTypes;
    public IngameUIButtonBase GetButton() => _button;



    private void OnClickUpEvent(PointerEventData eventData)
    {
        _onClickUpEvent?.Invoke(eventData);
        //_cts.Cancel();
        //Debug.Log("Cancle");
        //_cts = new CancellationTokenSource();
        //switch (_type)
        //{
        //    case StateType.RadialProgress:
        //    {
        //        bool isProcess = _radialProgress.GetProgress();
        //        if (true == isProcess)
        //        {
        //            _radialProgress.Cancle();
        //            return;
        //        }

        //        _radialProgress.Interaction();
        //        break;
        //    }
        //    case StateType.CoolTime:
        //        break;

        //}
    }

    private void OnClickDownEvent(PointerEventData eventData)
    {
        _onClickDownEvent?.Invoke(eventData);
        //TimerAfterOnClickDownEvent(2000, TextBox, _cts.Token).Forget();
    }

    private void OnDragEvent(PointerEventData eventData)
    {
        _onDragEvent?.Invoke(eventData);

        //switch (_type)
        //{
        //    case StateType.RadialProgress:
        //        break;
        //    case StateType.CoolTime:
        //        break;

        //}
    }

    private void OnBeginDragEvent(PointerEventData eventData)
    {
        _onDragEvent?.Invoke(eventData);

        //switch (_type)
        //{
        //    case StateType.RadialProgress:
        //        break;
        //    case StateType.CoolTime:
        //        break;

        //}
    }
    private void OnEndDragEvent(PointerEventData eventData)
    {
        _onBeginDragEvent?.Invoke(eventData);

        //switch (_type)
        //{
        //    case StateType.RadialProgress:
        //        break;
        //    case StateType.CoolTime:
        //        break;

        //}
    }
    private void AddLitener()
    {
        _button.OnClickUpRemoveLitener(OnClickUpEvent);
        _button.OnClickDownRemoveLitener(OnClickDownEvent);
        _button.OnDragRemoveLitener(OnDragEvent);
        _button.OnBeginDragRemoveLitener(OnBeginDragEvent);
        _button.OnEndDragRemoveLitener(OnEndDragEvent);

        _button.OnClickUpAddLitener(OnClickUpEvent);
        _button.OnClickDownAddLitener(OnClickDownEvent);
        _button.OnDragAddLitener(OnDragEvent);
        _button.OnBeginDragAddLitener(OnBeginDragEvent);
        _button.OnEndDragAddLitener(OnEndDragEvent);
    }

    #region DescriptionBox Base
    private void TextBox()
    {
        Debug.LogError("ActionACtion");
    }

    // button을 누르는 와중 콜백되는 이벤트
    private async UniTask TimerAfterOnClickDownEvent(int delay, Action onAction, CancellationToken token = default(CancellationToken))
    {
        await UniTask.Delay(delay, cancellationToken: token);
        if (true == token.IsCancellationRequested)
        {
            Debug.LogError("CANCLE");
            return;
        }
        onAction?.Invoke();
    }
    #endregion
}
