using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class IngameUIStatusEffect : MonoBehaviour
{
    public enum IngameUIStatusEffectMaintainType
    {
        None = -1,
        Instance =0,
        Passive,
        Max
    }
    public enum IngameStatusEffectBuffType
    {
        None = -1,
        Buff = 0,
        DeBuff,
        Max
    }

    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _dimd;
    [SerializeField] private IngameUIRadialProgress _radius;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private IngameUIStatusEffect _statusEffect;
    [SerializeField] private RectTransform _radialEffectRect;
    [SerializeField] private IngameUIToolTipBox _toolTipBox;

    [Header("INFORMATION")]
    [SerializeField] private IngameUIStatusEffectMaintainType _type;
    [SerializeField] private StatusEffectData _data;

    [Header("TYPE CLASSES")]
    [SerializeField] private IngameUIStatusEffectPassiveTypeFunction _passiveTypeFunc;
    [SerializeField] private IngameUIStatusEffectInstanceTypeFunction _instanceTypeFunc;

    public void Initialize(IngameUIToolTipBox argToolTipBox)
    {
        _radius = new IngameUIRadialProgress(0, this, _dimd, _radialEffectRect);
        _toolTipBox = argToolTipBox;
        _instanceTypeFunc = new(this);
        _passiveTypeFunc = new(this);

        _button.OnClickDownAddLitener(OnPointerDownEvent);
        _button.OnClickUpAddLitener(OnPointerUpEvent);
    }

    public void OnShow(StatusEffectData argData)
    {
        gameObject.SetActive(true);
        SetData(argData);
        switch (_data.maintainType)
        {
            case IngameUIStatusEffectMaintainType.Instance:
                _instanceTypeFunc.Initialize(this);
                _instanceTypeFunc.OnShow(this);
                break;
            case IngameUIStatusEffectMaintainType.Passive:
                _passiveTypeFunc.Initialize(this);
                _passiveTypeFunc.OnShow(this);
                break;
        }
    }

    public void ReplaceData(StatusEffectData argData)
    {
        _data = argData;
        _radius.SetTime(_data.duration);
    }

    public void ResetData()
    {
        _radius.Cancle();
        _data.Init();
        _dimd.gameObject.SetActive(false);

    }

    public void OnHide()
    {
        gameObject.SetActive(false);
        switch (_data.maintainType)
        {
            case IngameUIStatusEffectMaintainType.Instance:
                if (null == _instanceTypeFunc)
                    break;
                _instanceTypeFunc.OnHide();
                break;
            case IngameUIStatusEffectMaintainType.Passive:
                if (null == _passiveTypeFunc)
                    break;
                _passiveTypeFunc.OnHide();
                break;
        }
        ResetData();
    }

    #region [GET SET FUNCTION]
    public int GetRemainningDuration()
    {
        // 남은 시간 반환
        return _radius.GetTime();
    }
    public int GetGroupId() => _data.groupId;
    public StatusEffectData GetData() => _data;
    #endregion


    private void SetData(StatusEffectData argData)
    { 
        _type = argData.maintainType;
        _data = argData;
        _radius.SetTime(_data.duration);
    }

    private void OnPointerDownEvent(PointerEventData eventData)
    {
        _toolTipBox.OnShow(eventData);
    }
    private void OnPointerUpEvent(PointerEventData eventData)
    { 
        _toolTipBox.OnHide();
    }
    private async UniTask OnTextBox()
    { 
        
    }
}


