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
        Instance =0,
        Passive
    }
    public enum IngameStatusEffectBuffType
    { 
        Buff = 0,
        DeBuff
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
    [SerializeField] private IngameUIStatusEffectStackTypeFunction _stackTypeFunc;
    [SerializeField] private IngameUIStatusEffectNormalTypeFunction _normalTypeFunc;

    public Action<IngameUIStatusEffect> onDurationEnd;

    public void Initialize(IngameUIToolTipBox argToolTipBox)
    {
        _radius = new IngameUIRadialProgress(0, this, _dimd, _radialEffectRect);
        _toolTipBox = argToolTipBox;
        _normalTypeFunc = new(this);
        _stackTypeFunc = new(this);
        _normalTypeFunc.onDurationEnd = onDurationEnd;
        _stackTypeFunc.onDurationEnd = onDurationEnd;

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
                _normalTypeFunc.OnShow(this);
                break;
            case IngameUIStatusEffectMaintainType.Passive:
                _stackTypeFunc.OnShow(this);
                break;
        }
    }

    public void ReplaceData(StatusEffectData argData)
    {
        _data = argData;
        _radius.SetTime(_data.duration);
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
        _radius.Reset();

        switch (_data.maintainType)
        {
            case IngameUIStatusEffectMaintainType.Instance:
                if (null == _normalTypeFunc)
                    break;
                _normalTypeFunc.OnHide();
                break;
            case IngameUIStatusEffectMaintainType.Passive:
                if (null == _stackTypeFunc)
                    break;
                _stackTypeFunc.OnHide();
                break;
        }
        _stackTypeFunc = null;
        _normalTypeFunc = null;
        //onDurationEnd?.Invoke(this);
        _data.Init();
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

    // TODO : 그거
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


