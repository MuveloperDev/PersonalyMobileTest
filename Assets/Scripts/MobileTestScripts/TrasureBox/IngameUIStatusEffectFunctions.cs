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
    public enum StackType
    {
        None,
        MaxCount,
        MaxCountStackCheckValueMaintain,
        MaxCountSteckCheckValueDelete
    }

    [Serializable]
    public class IngameUIStatusEffectStackTypeFunction : IngameUIStatusEffectTypeFunction
    {
        [SerializeField] private StackType _stackType;
        [SerializeField] private int _stackMaxCount;
        [SerializeField] private int[] _stackCheckValue;
        [SerializeField] private int[] _stackEnableStatusEffectIds;
        // �нú� ���°� �ƴ� �ν��Ͻ��� ��� �෹�̼��� ������ �׿��ִ� ������ ���� ����.
        // MaxCount���� ���� ������ ���� �����̻��� Duration �� ƽ���͹�Ÿ�Ӽ����� �ʱ�ȭ�Ѵ�.
        // �ƽ� ī��Ʈ���� ���� ���¿��� ���� �����̻��� Duratiom�� ƽ���͹� Ÿ�� ������ �ʱ�ȭ�ȴ�.
        // ���ؿ� ���� ���� ���� ���δ� ��ø�Ѵ�.
        public IngameUIStatusEffectStackTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow(IngameUIStatusEffect statusEffect)
        {
            base.OnShow(statusEffect);
            _stackText.gameObject.SetActive(true);
            switch (_data.maintainType)
            {
                case IngameUIStatusEffectMaintainType.Instance:
                    break;
                case IngameUIStatusEffectMaintainType.Passive:
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
        [SerializeField] private StackType _stackType;
        [SerializeField] private int _stackMaxCount;
        [SerializeField] private int[] _stackCheckValue;
        [SerializeField] private int[] _stackEnableStatusEffectIds;

        public IngameUIStatusEffectNormalTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow(IngameUIStatusEffect statusEffect)
        {
            base.OnShow(statusEffect);
            _timeText.text = _data.groupId.ToString();
            _radius.SetTime(_data.duration);
            _radius.Interaction(() => {
                onDurationEnd?.Invoke(statusEffect);
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
        [SerializeField] protected IngameUIStatusEffect.IngameUIStatusEffectMaintainType _type;
        [SerializeField] protected StatusEffectData _data;
        public Action<IngameUIStatusEffect> onDurationEnd;
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

        public virtual void OnShow(IngameUIStatusEffect statusEffect)
        { }
        public virtual void OnHide()
        { }
    }
}

[Serializable]
public struct StatusEffectData
{
    public int groupId;
    public int duration;
    public IngameUIStatusEffect.IngameUIStatusEffectMaintainType maintainType;
    public IngameUIStatusEffect.IngameStatusEffectBuffType buffType;
    public StatucEffectOverlabType overlabType;
    public int stack;
    public void Init()
    {
        groupId = -1;
        duration = -1;
    }
}

public enum StatucEffectOverlabType
{
    None = 0,
    LongerHoldingTime,
    NewStatusEffect,
    MaintainEexistingStatusQuo,
    GroupPropertiesAreHigh
}