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

    #region SkillInfo
    [Header("STATE TYPES")]
    [SerializeField] private List<StateType> _types;
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
    public Action<PointerEventData> _onClickDownEvent;
    public Action<PointerEventData> _onClickUpEvent;
    public Action<PointerEventData> _onBeginDragEvent;
    public Action<PointerEventData> _onDragEvent;
    public Action<PointerEventData> _onEndDragEvent;
    #endregion
    private CancellationTokenSource _cts = new CancellationTokenSource();

    enum InputType
    {
        NormalTouch,
        TouchDownAndUp,
        DoubleTouch,
        Swipe // �̰� ����.
    }

    enum StateProcess
    {
        Touch,
        AttackEffect,
        CoolTime,
        CoolTimeEndEvent,

    }
    InputType currentInputType = InputType.TouchDownAndUp;
    // ��ų Ÿ���� ���ڷ� �޴´�.(�Ŵ�������)
    // ��ų Ÿ�Կ� ���� �����͸� ���ڷ� �޴´� (�Ŵ��� -> ActionButton).
    // �ű⿡ ���� Ÿ�� ����.
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
                        _radialProgress = new RadialProgress(5f, this, _radialImage);
                        break;
                    }
                case StateType.CoolTime:
                    {
                        break;
                    }
            }
        }

        AddLitener();
        _startCoolTimeValue = 30;
        //_button.SetUseProcessPressing(true);
    }

    public void SetType(List<StateType> argTypes) => _types = argTypes;
    public IngameUIButtonBase GetButton() => _button;

    // ��ġ �̺�Ʈ���� ���� Ŭ������ ���� ������ ���� ����

#pragma warning disable CS1998
    private async UniTask AttackButtonEffect()
    {
        Debug.Log(" ���� ��ư ����Ʈ !!!");
    }

    // ��Ÿ��
    #region COOLTIME
    [SerializeField] private bool _isCoolTime;
    [SerializeField] private int _startCoolTimeValue;
    private async UniTask CoolTime(CancellationToken token = default(CancellationToken))
    {
        _isCoolTime = true;
        int second = _startCoolTimeValue;
        while (second > 0)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            Debug.Log("Remaining: " + second + "s");
            await UniTask.Delay(1000);
            second--;
        }
        _isCoolTime = false;
        Debug.Log("Countdown finished!");
    }
    private async UniTask CoolTimeEndEffect()
    {
        Debug.Log(" ��Ÿ�� �� ����Ʈ !!!");
    }
    #endregion

    private async UniTask TouchHoldEvnet()
    {
        _radialProgress.Interaction(()=> { 
            _isIgonreUpEvent = true; 
            _onClickUpEvent?.Invoke(null);
            UpProcessBasedOnTouchType();
        });
    }

    private void DownProcessBasedOnTouchType()
    {
        switch (currentInputType)
        {
            case InputType.NormalTouch:
                NormalTouchEffect();
                break; 
            case InputType.TouchDownAndUp:
                DownProcessOfTouchDownAndUp();
                break;
            case InputType.DoubleTouch: 
                break;
        }
    }
    private void UpProcessBasedOnTouchType()
    {
        switch (currentInputType)
        {
            case InputType.NormalTouch:
                break;
            case InputType.TouchDownAndUp:
                UpProcessOfTouchDownAndUp();
                break;
            case InputType.DoubleTouch:
                break;
        }
    }


    private async void NormalTouchEffect()
    {
        AttackButtonEffect().Forget();  // ����Ʈ�� ���� ������.
        await CoolTime();
        CoolTimeEndEffect().Forget();
    }

    [SerializeField] private bool _isIgonreUpEvent;
    private async void DownProcessOfTouchDownAndUp()
    {
        TouchHoldEvnet().Forget();
    }
    private async void UpProcessOfTouchDownAndUp()
    {
        bool isProcess = _radialProgress.GetProgress();
        if (true == isProcess)
        {
            _radialProgress.Cancle();
        }
        await CoolTime();
        CoolTimeEndEffect().Forget();
    }


    private void OnClickUpEvent(PointerEventData eventData)
    {
        if (true == _isCoolTime)
            return;

        if (false == _isIgonreUpEvent)
        { 
            _onClickUpEvent?.Invoke(eventData);
            UpProcessBasedOnTouchType();
        }
        else _isIgonreUpEvent = false;

    }

    private void OnClickDownEvent(PointerEventData eventData)
    {
        if (true == _isCoolTime)
            return;

        _onClickDownEvent?.Invoke(eventData);
        DownProcessBasedOnTouchType();
    }

    private void OnDragEvent(PointerEventData eventData)
    {
        if (true == _isCoolTime)
            return;

        _onDragEvent?.Invoke(eventData);
    }

    private void OnBeginDragEvent(PointerEventData eventData)
    {
        if (true == _isCoolTime)
            return;

        _onDragEvent?.Invoke(eventData);
    }
    private void OnEndDragEvent(PointerEventData eventData)
    {
        if (true == _isCoolTime)
            return;

        _onBeginDragEvent?.Invoke(eventData);
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
