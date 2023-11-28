using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public partial class IngameUIStatusEffect
{
    [Serializable]
    public class IngameUIStatusEffectStackTypeFunction : IngameUIStatusEffectTypeFunction
    {
        public IngameUIStatusEffectStackTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow()
        {
            base.OnShow();
            _stackText.gameObject.SetActive(true);
            switch (_data.type)
            {
                case IngameUIStatusEffectType.StackLimitMaxCnt:
                    break;
                case IngameUIStatusEffectType.StackPassive:
                    break;
                case IngameUIStatusEffectType.StackEffectAmountToMaxStack:
                    break;
                default:
                    break;
            }
        }

        public override void OnHide()
        {
            base.OnHide();
        }


        public void StackUp()
        {

        }

        public void StackReset()
        {

        }

        public void StackEffectAmountToStackCheckValue()
        {
        }

        public void StackProcessAmountToMaxCnt()
        {

        }
    }

    [Serializable]
    public class IngameUIStatusEffectNormalTypeFunction : IngameUIStatusEffectTypeFunction
    {
        public IngameUIStatusEffectNormalTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow()
        {
            base.OnShow();
            _timeText.text = _data.groupId.ToString();
            _radius.SetTime(_data.duration);
            _radius.Interaction(() => {
                _statusEffect.OnHide();
            });
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }


    [Serializable]
    public class IngameUIStatusEffectTypeFunction
    {
        [Header("DEPENDENCY")]
        [SerializeField] protected Image _dimd;
        [SerializeField] protected IngameUIRadialProgress _radius;
        [SerializeField] protected TextMeshProUGUI _timeText;
        [SerializeField] protected TextMeshProUGUI _stackText;
        [SerializeField] protected IngameUIStatusEffect _statusEffect;

        [Header("INFORMATION")]
        [SerializeField] protected IngameUIStatusEffect.IngameUIStatusEffectType _type;
        [SerializeField] protected StatusEffectData _data;

        public virtual void Initialize(IngameUIStatusEffect argStatusEffect)
        {
            _statusEffect = argStatusEffect;
            _dimd = _statusEffect._dimd;
            _radius = _statusEffect._radius;
            _timeText = _statusEffect._timeText;
            _stackText = _statusEffect._stackText;
            _type = _statusEffect._type;
            _data = _statusEffect._data;
        }

        public virtual void OnShow()
        {
        }
        public virtual void OnHide()
        { }


    }
}



[Serializable]
public struct StatusEffectData
{
    public int groupId;
    public int duration;
    public IngameUIStatusEffect.IngameUIStatusEffectType type;
    public int stack;
    public void Init()
    {
        groupId = -1;
        duration = -1;
    }
}

public enum StatucChangeType
{
    DurationCompareChange = 0,
    ImeditlyChange = 1,
}