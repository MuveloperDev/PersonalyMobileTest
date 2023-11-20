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





    #region [Button_Interaction_Handler]
    public Action<PointerEventData> _onClickDownEvent;
    public Action<PointerEventData> _onClickUpEvent;
    public Action<PointerEventData> _onBeginDragEvent;
    public Action<PointerEventData> _onDragEvent;
    public Action<PointerEventData> _onEndDragEvent;
    #endregion
    private CancellationTokenSource _cts = new CancellationTokenSource();


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
