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
        StraightLine_UserCenteredCasting, // ���� - ����� �߽� ����
        StraightLine_UserCenteredDash, // ���� - ����� �߽� ����
        FanShaped_UserCenteredCasting, // ��ä�� - ����� �߽� ����
        Circular_UserCenteredCasting, // ���� - ����� �߽� ����
        Circular_FreeRangeDesignation, // ���� - ���� ���� ����
        ToggleForm // ��� ����
    }

    #endregion
    //#region SkillInfo
    //// ���� ����ü�� ���� ��� ����.
    //[Header("SKILL INFOMATION")]
    //// �Ŵ������� ���̺����͸� ĳ�� �� �����͸� �Ѱ� ������ 
    //[SerializeField] private string _skillName;
    //[SerializeField] private string _skillIconPath;
    //[SerializeField] private string _skillSimpleDescription;

    //[Header("Combo")]
    //[SerializeField] private float _skillComboMaxCount; // ��ų �޺��� �ִ� ��ġ

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
    //[SerializeField] private int[] _explosionType; // enum���� ��ȯ����
    //[SerializeField] private int[] _generateExplosionPositionType; // ���� ������ġ ���� enum���� ��ȯ����
    //[SerializeField] private float _exlosionTimeSec; // �����ϱ��� �����ð��� �����Ѵ�.
    //[SerializeField] private float _explosionRagneType; // ���� ��ų ���� ���� Ÿ���� �����Ѵ�.
    //[SerializeField] private float _explosionRange; // ������ų ���� ������ �����Ѵ�. enum���� ��ȯ����
    //[SerializeField] private float _explosionNotificationTimeSec; // ���� ���� ��Ƽ ǥ�� �ð��� ���� ǥ�������� �����Ѵ�.
    //[SerializeField] private float _explosionNotificationFxPrefabPath; // ���� ���� ��ġ�� ��� �������� ����Ǵ� ��Ƽ ���ҽ� ��θ� �����Ѵ�.

    //[Header("COOLTIME")]
    //[SerializeField] private int _coolTimeType; //enum���� ��ȯ����
    //[SerializeField] private bool _useCoolTimeGauge;
    //[SerializeField] private float _globalCoolTimeSec; // �ʹ� ���� ��ü ��Ÿ��
    //[SerializeField] private float _coolTimeSec; // ��Ÿ�� �ð� ����
    //[SerializeField] private int _stackMaxCount; // ������ �ִ�ġ�� ����
    //[SerializeField] private int _unavailableAwhileTimeSec; // ������ �ִ�ġ�� ����
    //[Header("SKILL CHANGE TYPE")]
    //[SerializeField] private int _skillChangeType; // ������ �ִ�ġ�� ����
    //[SerializeField] private float _maintainTimeSec; // ��ų�� ����� �� ���� �ð� ����
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

    // ��ų Ÿ���� ���ڷ� �޴´�.(�Ŵ�������)
    // ��ų Ÿ�Կ� ���� �����͸� ���ڷ� �޴´� (�Ŵ��� -> ActionButton).
    // �ű⿡ ���� Ÿ�� ����.
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
        Debug.Log(" ���� ��ư ����Ʈ !!!");
    }
    // ��Ÿ��
    private async UniTask CoolTimeEndEffect()
    {
        Debug.Log(" ��Ÿ�� �� ����Ʈ !!!");
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

    private void AllocatedConstructorBasedOnInputType(/*����ü�� �޾ƿ� �ʱⰪ�� �Ҵ��Ѵ� */)
    {
        switch (_inputType)
        {
            case InputType.SingleTouch:
                break;
            case InputType.DragReleaseTouch:
                // �ʿ��� ����� ��ư�� ��ġ ��Ŀ� ���� ����
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

    //// button�� ������ ���� �ݹ�Ǵ� �̺�Ʈ
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
