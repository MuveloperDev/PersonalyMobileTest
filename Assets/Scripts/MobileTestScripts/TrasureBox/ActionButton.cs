using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
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
        StraightLine_UserCenteredCasting, // ���� - ����� �߽� ����
        StraightLine_UserCenteredDash, // ���� - ����� �߽� ����
        FanShaped_UserCenteredCasting, // ��ä�� - ����� �߽� ����
        Circular_UserCenteredCasting, // ���� - ����� �߽� ����
        Circular_FreeRangeDesignation, // ���� - ���� ���� ����
        ToggleForm // ��� ����
    }

    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _radialImage;
    [SerializeField] private IngameUIRadialProgress _radialProgress;
    [SerializeField] private IngameVirtualJoystickBase _joystick;
    [SerializeField] private IngameUIAimComboActionButton _aimComboProcess;
    [SerializeField] private IngameUICoolTime _coolTIme;
    [SerializeField] private GameObject _coolTimeObject;
    [SerializeField] private TextMeshProUGUI _textCoolTime;

    [Header("STATE TYPES")]
    [SerializeField] private List<StateType> _types;

    #region SkillInfo
    // ���� ����ü�� ���� ��� ����.
    [Header("SKILL INFOMATION")]
    // �Ŵ������� ���̺����͸� ĳ�� �� �����͸� �Ѱ� ������ 
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillIconPath;
    [SerializeField] private string _skillSimpleDescription;

    [Header("Combo")]
    [SerializeField] private float _skillComboMaxCount; // ��ų �޺��� �ִ� ��ġ

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
    [SerializeField] private int[] _explosionType; // enum���� ��ȯ����
    [SerializeField] private int[] _generateExplosionPositionType; // ���� ������ġ ���� enum���� ��ȯ����
    [SerializeField] private float _exlosionTimeSec; // �����ϱ��� �����ð��� �����Ѵ�.
    [SerializeField] private float _explosionRagneType; // ���� ��ų ���� ���� Ÿ���� �����Ѵ�.
    [SerializeField] private float _explosionRange; // ������ų ���� ������ �����Ѵ�. enum���� ��ȯ����
    [SerializeField] private float _explosionNotificationTimeSec; // ���� ���� ��Ƽ ǥ�� �ð��� ���� ǥ�������� �����Ѵ�.
    [SerializeField] private float _explosionNotificationFxPrefabPath; // ���� ���� ��ġ�� ��� �������� ����Ǵ� ��Ƽ ���ҽ� ��θ� �����Ѵ�.

    [Header("COOLTIME")]
    [SerializeField] private int _coolTimeType; //enum���� ��ȯ����
    [SerializeField] private bool _useCoolTimeGauge;
    [SerializeField] private float _globalCoolTimeSec; // �ʹ� ���� ��ü ��Ÿ��
    [SerializeField] private float _coolTimeSec; // ��Ÿ�� �ð� ����
    [SerializeField] private int _stackMaxCount; // ������ �ִ�ġ�� ����
    [SerializeField] private int _unavailableAwhileTimeSec; // ������ �ִ�ġ�� ����
    [Header("SKILL CHANGE TYPE")]
    [SerializeField] private int _skillChangeType; // ������ �ִ�ġ�� ����
    [SerializeField] private float _maintainTimeSec; // ��ų�� ����� �� ���� �ð� ����
    #endregion

    #region [Button_Interaction_Handler]
    public Action<PointerEventData> _onPointerDownEvent;
    public Action<PointerEventData> _onPointerUpEvent;
    public Action<PointerEventData> _onBeginDragEvent;
    public Action<PointerEventData> _onDragEvent;
    public Action<PointerEventData> _onEndDragEvent;
    #endregion

    public enum InputType
    {
        NormalTouch_Aim,
        NormalTouch_Immediately,
        TouchDownAndUp,
        DoubleTouch,
        MultipleTouch,
        Swipe // �̰� ����.
    }

    enum StateProcess
    {
        Touch,
        AttackEffect,
        CoolTime,
        CoolTimeEndEvent,

    }
#if UNITY_EDITOR
    [SerializeField] private InputType _prevInputType;
#endif

    [SerializeField] private InputType _inputType;
    // ��ų Ÿ���� ���ڷ� �޴´�.(�Ŵ�������)
    // ��ų Ÿ�Կ� ���� �����͸� ���ڷ� �޴´� (�Ŵ��� -> ActionButton).
    // �ű⿡ ���� Ÿ�� ����.
    void OnValidate()
    {
        switch (_prevInputType)
        {
            case InputType.NormalTouch_Aim:
                _coolTIme.Reset();
                break;
            case InputType.NormalTouch_Immediately:
                _coolTIme.Reset();
                break;
            case InputType.TouchDownAndUp:
                _coolTIme.Reset();
                break;
            case InputType.DoubleTouch:
                break;
            case InputType.MultipleTouch:
                _aimComboProcess.Reset();
                break;
        }

        switch (_inputType)
        {
            case InputType.NormalTouch_Aim:
                _coolTIme.RefreshData(1);
                break;
            case InputType.NormalTouch_Immediately:
                _coolTIme.RefreshData(1);
                break;
            case InputType.TouchDownAndUp:
                _coolTIme.RefreshData(20);
                break;
            case InputType.DoubleTouch:
                break;
            case InputType.MultipleTouch:
                _aimComboProcess.RefreshData(3,3,20);
                break;
        }
    }
    public void Initialize(InputType argType)
    {
        _inputType = argType;

#if UNITY_EDITOR
        _prevInputType = _inputType;
#endif

        // �ʿ��� ����� ��ư�� ��ġ ��Ŀ� ���� ����
        _radialProgress = new IngameUIRadialProgress(5f, this, _radialImage);
        _coolTIme = new IngameUICoolTime(20, _coolTimeObject, _textCoolTime);
        _aimComboProcess = new IngameUIAimComboActionButton(_coolTIme, 3, 3, 20);

        AddLitener();
    }

    public void SetType(List<StateType> argTypes) => _types = argTypes;

    public Vector2 GetJoystickVector2() => _joystick.GetVector2();

#pragma warning disable CS1998
    private async UniTask AttackButtonEffect()
    {
        Debug.Log(" ���� ��ư ����Ʈ !!!");
    }
    // ��Ÿ��
    #region COOLTIME
    private async UniTask CoolTimeEndEffect()
    {
        Debug.Log(" ��Ÿ�� �� ����Ʈ !!!");
    }
    #endregion

    private async UniTask TouchHoldEvnet(PointerEventData eventData)
    {
        _radialProgress.Interaction(()=> { 
            _isIgonreUpEvent = true; 
            _onPointerUpEvent?.Invoke(eventData);
            UpProcessBasedOnTouchType(eventData);
        });
    }

    private void DownProcessBasedOnTouchType(PointerEventData eventData)
    {
        switch (_inputType)
        {
            case InputType.NormalTouch_Aim:
                _joystick.OnShow(eventData);
                break;
            case InputType.NormalTouch_Immediately:
                _onPointerUpEvent?.Invoke(eventData);
                NormalTouchImmediatelyProcess();
                break; 
            case InputType.TouchDownAndUp:
                _joystick.OnShow(eventData);
                DownProcessOfTouchDownAndUp(eventData);
                break;
            case InputType.DoubleTouch: 
                _joystick.OnShow(eventData);
                break;
            case InputType.MultipleTouch:
                _joystick.OnShow(null);
                _aimComboProcess.DownProcessCombo().Forget();
                break;
        }
    }
    private void UpProcessBasedOnTouchType(PointerEventData eventData)
    {
        switch (_inputType)
        {
            case InputType.NormalTouch_Aim:
                _joystick.OnHide();
                NormalTouchImmediatelyProcess();
                break;
            case InputType.NormalTouch_Immediately:
                break;
            case InputType.TouchDownAndUp:
                _joystick.OnHide();
                UpProcessOfTouchDownAndUp();
                break;
            case InputType.DoubleTouch:
                _joystick.OnHide();
                break;
            case InputType.MultipleTouch:
                _joystick.OnHide();
                _aimComboProcess.UpProcessCombo().Forget();
                break;
        }
    }


    private async void NormalTouchImmediatelyProcess()
    {
        AttackButtonEffect().Forget();  // ����Ʈ�� ���� ������.
        await _coolTIme.Interaction();
        CoolTimeEndEffect().Forget();
    }


    [SerializeField] private bool _isIgonreUpEvent;
    private async void DownProcessOfTouchDownAndUp(PointerEventData eventData)
    {
        TouchHoldEvnet(eventData).Forget();
    }
    private async void UpProcessOfTouchDownAndUp()
    {
        bool isProcess = _radialProgress.GetProgress();
        if (true == isProcess)
        {
            _radialProgress.Cancle();
        }
        await _coolTIme.Interaction();
        CoolTimeEndEffect().Forget();
    }


    private void OnPointerUpEvent(PointerEventData eventData)
    {
        if (true == _coolTIme.IsCoolTime())
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
        if (true == _coolTIme.IsCoolTime())
            return;

        _onPointerDownEvent?.Invoke(eventData);
        DownProcessBasedOnTouchType(eventData);
    }

    private void OnDragEvent(PointerEventData eventData)
    {
        if (true == _coolTIme.IsCoolTime())
            return;

        _joystick.HandleJoystickInput(eventData);
        _onDragEvent?.Invoke(eventData);
    }

    private void OnBeginDragEvent(PointerEventData eventData)
    {
        if (true == _coolTIme.IsCoolTime())
            return;

        _onDragEvent?.Invoke(eventData);
    }
    private void OnEndDragEvent(PointerEventData eventData)
    {
        if (true == _coolTIme.IsCoolTime())
            return;

        _onBeginDragEvent?.Invoke(eventData);
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

    #region DescriptionBox Base
    private void TextBox()
    {
        Debug.LogError("ActionACtion");
    }

    // button�� ������ ���� �ݹ�Ǵ� �̺�Ʈ
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
