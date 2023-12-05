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
    public class IngameUIStatusEffectPassiveTypeFunction : IngameUIStatusEffectTypeFunction
    {
        [SerializeField] private StackType _stackType;
        [SerializeField] private int _stackMaxCount;
        [SerializeField] private int[] _stackCheckValue;
        [SerializeField] private int[] _stackEnableStatusEffectIds;
        // 패시브 상태가 아닌 인스턴스인 경우 듀레이션이 끝나면 쌓여있던 스택은 전부 삭제.
        // MaxCount까지 스택 쌓을때 마다 상태이상이 Duration 과 틱인터벌타임세컨이 초기화한다.
        // 맥스 카운트까지 쌓인 상태에서 같은 상태이상이 Duratiom과 틱인터벌 타임 세컨이 초기화된다.
        // 피해와 스탯 관련 변경 여부는 중첩한다.
        public IngameUIStatusEffectPassiveTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow(IngameUIStatusEffect statusEffect)
        {
            base.OnShow(statusEffect);
            _stackText.gameObject.SetActive(true);
            _stackText.text = _data.stack.ToString();
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
    public class IngameUIStatusEffectInstanceTypeFunction : IngameUIStatusEffectTypeFunction
    {
        [SerializeField] private StackType _stackType;
        [SerializeField] private int _stackMaxCount;
        [SerializeField] private int[] _stackCheckValue;
        [SerializeField] private int[] _stackEnableStatusEffectIds;

        public IngameUIStatusEffectInstanceTypeFunction(IngameUIStatusEffect argStatusEffect)
         => Initialize(argStatusEffect);
        public override void Initialize(IngameUIStatusEffect argStatusEffect)
        => base.Initialize(argStatusEffect);

        public override void OnShow(IngameUIStatusEffect statusEffect)
        {
            base.OnShow(statusEffect);
            _timeText.text = _data.groupId.ToString();
            _radius.SetTime(_data.duration);
            _radius.Interaction(() => {});
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
    public StatusEffectOverlabType overlabType;
    public int stack;
    public void Init()
    {
        groupId = -1;
        duration = -1;
        maintainType = IngameUIStatusEffect.IngameUIStatusEffectMaintainType.None;
        buffType = IngameUIStatusEffect.IngameStatusEffectBuffType.None;
        overlabType = StatusEffectOverlabType.None;
        stack = -1;
    }
}

public enum StatusEffectOverlabType
{
    None = -1,
    LongerHoldingTime,          // 같은 그룹 상태이상일 경우 현재 유지시간을 비교 후 교체 여부를 판단
    NewStatusEffect,            // 새로 들어오는 상태이상으로 교체
    MaintainEexistingStatusQuo, // 기존 상태이상 유지 및 신규상태이상 삭제
    GroupPropertiesAreHigh,     // 기존 상태이상과 GroupPriority 를 비교하여 숫자가 높거나 같을 경우 교체
    Max
}