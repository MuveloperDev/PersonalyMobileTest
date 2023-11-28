using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class SkillActionButtonBase : MonoBehaviour
{
    #region [ENUM]
    public enum InputType
    {
        SingleTouch,
        DragReleaseTouch,
        ComboTouch,
        StackTouch
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

    #endregion
    //#region SkillInfo
    //// 추후 구조체로 만들어서 사용 예정.
    //[Header("SKILL INFOMATION")]
    //// 매니저에서 테이블데이터를 캐싱 후 데이터를 넘겨 받을것 
    //[SerializeField] private string _skillName;
    //[SerializeField] private string _skillIconPath;
    //[SerializeField] private string _skillSimpleDescription;

    //[Header("Combo")]
    //[SerializeField] private float _skillComboMaxCount; // 스킬 콤보의 최대 수치

    //[Header("SKILL")]
    //[SerializeField] private int _skillSlotType;
    //[SerializeField] private int _skillCategoryType;
    //[SerializeField] private int _skillMainType;
    //[SerializeField] private int _skillSubType;

    //[SerializeField] private float _skillRange;
    //[SerializeField] private float _skillRangePositionOffset;
    //[SerializeField] private float[] _generateSkillRangeTimeSec;
    //[SerializeField] private float _useSkillMaxRange;

    //[Header("CHARGE")]
    //[SerializeField] private int _chargeType;
    //[SerializeField] private float _chargeMoveDistance;

    //[Header("Motion")]
    //[SerializeField] private float _motionCancellableTimeSec;


    //[Header("CASTING")]
    //[SerializeField] private bool _useCasting;
    //[SerializeField] private float _astingTimeSec;

    //[Header("NOTIFICATION")]
    //[SerializeField] private string _skillRangeNotificationFxPrefabPath;
    //[SerializeField] private float _skillRangeNotificationTimeSec;

    //[Header("INDICATOR")]
    //[SerializeField] private string _skillGuidLineFXPrefabPath;
    //[SerializeField] private float _castingTimeSec;

    //[Header("CHANNELING")]
    //[SerializeField] private bool _startChannelingTick;
    //[SerializeField] private float _channelingDuration;
    //[SerializeField] private int _channelingTickCount;
    //[SerializeField] private float _channelingTickIntervalTimeSec;

    //[Header("EXPLOSION")]
    //[SerializeField] private int[] _explosionType; // enum으로 변환예정
    //[SerializeField] private int[] _generateExplosionPositionType; // 폭발 생성위치 설정 enum으로 변환예정
    //[SerializeField] private float _exlosionTimeSec; // 폭발하기전 유지시간을 설정한다.
    //[SerializeField] private float _explosionRagneType; // 생성 시킬 폭발 범위 타입을 지정한다.
    //[SerializeField] private float _explosionRange; // 생성시킬 폭발 범위를 설정한다. enum으로 변환예정
    //[SerializeField] private float _explosionNotificationTimeSec; // 폭발 범위 노티 표시 시간을 몇초 표시해줄지 설정한다.
    //[SerializeField] private float _explosionNotificationFxPrefabPath; // 폭발 생성 위치에 모든 유저에게 노출되는 노티 리소스 경로를 지정한다.

    //[Header("COOLTIME")]
    //[SerializeField] private int _coolTimeType; //enum으로 변환예정
    //[SerializeField] private bool _useCoolTimeGauge;
    //[SerializeField] private float _globalCoolTimeSec; // 초반 시작 전체 쿨타임
    //[SerializeField] private float _coolTimeSec; // 쿨타임 시간 지정
    //[SerializeField] private int _stackMaxCount; // 스택의 최대치를 설정
    //[SerializeField] private int _unavailableAwhileTimeSec; // 스택의 최대치를 설정
    //[Header("SKILL CHANGE TYPE")]
    //[SerializeField] private int _skillChangeType; // 스택의 최대치를 설정
    //[SerializeField] private float _maintainTimeSec; // 스킬이 변경된 후 유지 시간 설정
    //#endregion

    [Header("DEPENDENCY")]
    [SerializeField] protected IngameUIButtonBase _button;
    [Space]
    [SerializeField] protected Image _radialImageSkill;
    [SerializeField] protected IngameUIRadialProgress _radialProgressSkill;
    [Space]
    [SerializeField] protected Image _radialImageStackOrCombo;
    [SerializeField] protected IngameUIRadialProgress _radialProgressStackOrCombo;
    [SerializeField] protected TextMeshProUGUI _stackComboText;
    [Space]
    [SerializeField] protected IngameVirtualJoystickBase _joystick;
    [Space]
    [SerializeField] protected IngameUIAimComboActionButton _aimComboProcess;
    [Space]
    [SerializeField] protected IngameUICoolTime _coolTime;
    [Space]
    [SerializeField] protected Image _dimd;
    [Space]
    [SerializeField] protected TextMeshProUGUI _textCoolTime;

    [Header("TYPE INFO")]
#if UNITY_EDITOR
    [SerializeField] private InputType _prevInputType;
#endif
    [SerializeField] private InputType _inputType;


    [SerializeField] private bool _isIgonreUpEvent;

    //[SerializeField] protected IngameUIToolTipBox _toolTipBox;
    //[SerializeField] private CancellationTokenSource _cts = new();

    #region [Button_Interaction_Handler]
    public Action<PointerEventData> _onPointerDownEvent;
    public Action<PointerEventData> _onPointerUpEvent;
    public Action<PointerEventData> _onBeginDragEvent;
    public Action<PointerEventData> _onDragEvent;
    public Action<PointerEventData> _onEndDragEvent;
    #endregion

    // 스킬 타입을 인자로 받는다.(매니저에서)
    // 스킬 타입에 대한 데이터를 인자로 받는다 (매니저 -> ActionButton).
    // 거기에 따른 타입 설정.
    void OnValidate()
    {
        switch (_prevInputType)
        {
            case InputType.SingleTouch:
                _coolTime.Reset();
                break;
            case InputType.DragReleaseTouch:
                _coolTime.Reset();
                break;
            case InputType.ComboTouch:
                _coolTime.Reset();
                _aimComboProcess.Reset();
                break;
            case InputType.StackTouch:
                break;
        }

        switch (_inputType)
        {
            case InputType.SingleTouch:
                _coolTime.RefreshData(1);
                break;
            case InputType.DragReleaseTouch:
                _coolTime.RefreshData(1);
                break;
            case InputType.ComboTouch:
                _coolTime.RefreshData(20);
                _aimComboProcess.RefreshData(3,3,20);
                break;
            case InputType.StackTouch:
                break;
        }
        _prevInputType = _inputType;
    }
    public virtual void Initialize(InputType argType)
    {
        _inputType = argType;
#if UNITY_EDITOR
        _prevInputType = _inputType;
#endif
        AllocatedConstructorBasedOnInputType();
        AddLitener();
    }

    public virtual void ResetCoolTime()
    { 
    }
    public virtual void DecreaseCoolTime()
    {
    }

    #region [GET FUNCTION]
    public virtual Vector2 GetJoystickVector2() => _joystick.GetVector2();
    public virtual Vector2 GetCorrectedDeadZoneVector2() => _joystick.GetCorrectedDeadZoneVector2();
    #endregion

#pragma warning disable CS1998
    private async UniTask AttackButtonEffect()
    {
        Debug.Log(" 어택 버튼 이펙트 !!!");
    }
    // 쿨타임
    private async UniTask CoolTimeEndEffect()
    {
        Debug.Log(" 쿨타임 끝 이펙트 !!!");
    }

    private void DownProcessBasedOnTouchType(PointerEventData eventData)
    {
        switch (_inputType)
        {
            case InputType.SingleTouch:
                break;
            case InputType.DragReleaseTouch:
                _joystick.OnShow(eventData);
                OnDragReleaseDownEvent(eventData);
                break; 
            case InputType.ComboTouch:
                _joystick.OnShow(eventData);
                break;
            case InputType.StackTouch: 
                _joystick.OnShow(eventData);
                //_aimComboProcess.DownProcessCombo().Forget();
                break;
        }
    }
    private void UpProcessBasedOnTouchType(PointerEventData eventData)
    {
        switch (_inputType)
        {
            case InputType.SingleTouch:
                break;
            case InputType.DragReleaseTouch:
                _joystick.OnHide();
                OnDragReleaseUpEvent(eventData);
                break;
            case InputType.ComboTouch:
                _joystick.OnHide();
                break;
            case InputType.StackTouch:
                _joystick.OnHide();
                //_aimComboProcess.UpProcessCombo().Forget();
                break;
        }
    }




    #region [DRAG RELEASE EVENT FUNC]
    private void OnDragReleaseDownEvent(PointerEventData eventData)
    {}

    private async void OnDragReleaseUpEvent(PointerEventData eventData)
    {
        AttackButtonEffect().Forget();
        _radialProgressSkill.Interaction();
        await _coolTime.Interaction();
        CoolTimeEndEffect().Forget();
    }
    #endregion

    private void AllocatedConstructorBasedOnInputType(/*구조체를 받아와 초기값을 할당한다 */)
    {
        switch (_inputType)
        {
            case InputType.SingleTouch:
                break;
            case InputType.DragReleaseTouch:
                // 필요한 기능은 버튼의 터치 방식에 따라서 결정
                _radialProgressSkill = new IngameUIRadialProgress(20, this, _radialImageSkill);
                _coolTime = new IngameUICoolTime(20, _dimd, _textCoolTime);
                //_aimComboProcess = new IngameUIAimComboActionButton(_coolTime, 3, 3, 20);
                break;
            case InputType.ComboTouch:
                break;
            case InputType.StackTouch:
                break;
            default:
                break;
        }
    }

    #region [BUTTON EVENET]
    private void OnPointerUpEvent(PointerEventData eventData)
    {
        bool isVaild = IsVaildEvent();
        if (false == isVaild)
            return;

        if (false == _isIgonreUpEvent)
        {
            _onPointerUpEvent?.Invoke(eventData);
            UpProcessBasedOnTouchType(eventData);
        }
        else _isIgonreUpEvent = false;

    }

    private void OnPointerDownEvent(PointerEventData eventData)
    {
        bool isVaild = IsVaildEvent();
        if (false == isVaild)
            return;

        _onPointerDownEvent?.Invoke(eventData);
        DownProcessBasedOnTouchType(eventData);
    }

    private void OnDragEvent(PointerEventData eventData)
    {
        bool isVaild = IsVaildEvent();
        if (false == isVaild)
            return;

        _joystick.HandleJoystickInput(eventData);
        _onDragEvent?.Invoke(eventData);
    }

    private void OnBeginDragEvent(PointerEventData eventData)
    {
        bool isVaild = IsVaildEvent();
        if (false == isVaild)
            return;

        Debug.Log("StartDrag");
        _onDragEvent?.Invoke(eventData);
    }
    private void OnEndDragEvent(PointerEventData eventData)
    {
        bool isVaild = IsVaildEvent();
        if (false == isVaild)
            return;

        _onBeginDragEvent?.Invoke(eventData);
    }
    private bool IsVaildEvent()
    {
        if (true == _coolTime.IsCoolTime())
            return false;

        return true;
    }

    private void AddLitener()
    {
        _button.OnClickUpRemoveLitener(OnPointerUpEvent);
        _button.OnClickDownRemoveLitener(OnPointerDownEvent);
        _button.OnDragRemoveLitener(OnDragEvent);
        _button.OnBeginDragRemoveLitener(OnBeginDragEvent);
        _button.OnEndDragRemoveLitener(OnEndDragEvent);

        _button.OnClickUpAddLitener(OnPointerUpEvent);
        _button.OnClickDownAddLitener(OnPointerDownEvent);
        _button.OnDragAddLitener(OnDragEvent);
        _button.OnBeginDragAddLitener(OnBeginDragEvent);
        _button.OnEndDragAddLitener(OnEndDragEvent);
    }
    #endregion

    #region DescriptionBox Base

    //private void OnPointerClickUpEvnetToolTip()
    //{
    //    if (true == isTooltipProcess)
    //    {
    //        _toolTipBox.OnHide();
    //        _cts.Cancel();
    //    }
    //    else
    //    {
    //        _toolTipBox.OnHide();
    //    }

    //    if (true == _cts.IsCancellationRequested)
    //    {
    //        _cts = new();
    //    }

    //    isTooltipProcess = false;
    //}

    //// button을 누르는 와중 콜백되는 이벤트
    //bool isTooltipProcess = false;
    //private async UniTask TimerAfterOnClickDownEvent(PointerEventData eventData)
    //{
    //    isTooltipProcess = true;
    //    await UniTask.Delay(2000, false, PlayerLoopTiming.Update ,_cts.Token);
    //    if (true == _cts.Token.IsCancellationRequested)
    //    {
    //        return;
    //    }
    //    _toolTipBox.SetPosition(eventData);
    //    _toolTipBox.OnShow();
    //    isTooltipProcess = false;
    //}
    #endregion
}
