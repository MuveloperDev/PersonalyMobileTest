using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public partial class IngameUIStatusEffect : IngameUIEventHandler
{
    public enum IngameUIStatusEffectType
    {
        Normal,
        StackLimitMaxCnt,
        StackPassive,
        StackEffectAmountToMaxStack
    }

    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _dimd;
    [SerializeField] private IngameUIRadialProgress _radius;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private IngameUIStatusEffect _statusEffect;

    [Header("INFORMATION")]
    [SerializeField] private IngameUIStatusEffectType _type;
    [SerializeField] private StatusEffectData _data;

    [Header("TYPE CLASSES")]
    [SerializeField] private IngameUIStatusEffectStackTypeFunction _stackTypeFunc;
    [SerializeField] private IngameUIStatusEffectNormalTypeFunction _normalTypeFunc;

    public Action<IngameUIStatusEffect> onDurationEnd;

    public void Initialize()
    {
        _radius = new IngameUIRadialProgress(0, this, _dimd);
    }

    public void OnShow(StatusEffectData argData)
    {
        gameObject.SetActive(true);
        SetData(argData);
        switch (_data.type)
        {
            case IngameUIStatusEffectType.Normal:
                _normalTypeFunc = new(this);
                _normalTypeFunc.OnShow();
                break;
            case IngameUIStatusEffectType.StackLimitMaxCnt:
            case IngameUIStatusEffectType.StackPassive:
            case IngameUIStatusEffectType.StackEffectAmountToMaxStack:
                _stackTypeFunc = new(this);
                _stackTypeFunc.OnShow();
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

        switch (_data.type)
        {
            case IngameUIStatusEffectType.Normal:
                if (null == _normalTypeFunc)
                    break;
                _normalTypeFunc.OnHide();
                break;
            case IngameUIStatusEffectType.StackLimitMaxCnt:
            case IngameUIStatusEffectType.StackPassive:
            case IngameUIStatusEffectType.StackEffectAmountToMaxStack:
                if (null == _stackTypeFunc)
                    break;
                _stackTypeFunc.OnHide();
                break;
        }
        _stackTypeFunc = null;
        _normalTypeFunc = null;
        onDurationEnd?.Invoke(this);
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
        _type = argData.type;
        _data = argData;
        _radius.SetTime(_data.duration);
    }

    private void OnPointerDownEvent()
    {

    }
    private void OnPointerUpEvent()
    { 

    }
    private async UniTask OnTextBox()
    { 
        
    }
}


