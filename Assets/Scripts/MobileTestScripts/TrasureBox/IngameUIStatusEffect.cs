using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIStatusEffect : MonoBehaviour
{
    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _dimd;
    [SerializeField] private IngameUIRadialProgress _radius;
    [SerializeField] private TextMeshProUGUI _timeText;

    [Header("INFORMATION")]
    [SerializeField] private StatusEffectData data;

    public Action<IngameUIStatusEffect> endDurationCallback;
    private void Awake()
    {
        _radius = new IngameUIRadialProgress(0, this, _dimd); 
        gameObject.SetActive(false);
    }

    public void OnShow(StatusEffectData argData)
    {
        gameObject.SetActive(true);
        SetData(argData);
        _timeText.text = argData.groupId.ToString();
        _radius.SetTime(data.duration);
        _radius.Interaction(() => {
            OnHide();
        });
    }

    public void ReplaceData(StatusEffectData argData)
    {
        data = argData;
        _radius.SetTime(data.duration);
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
        _radius.Reset();
        endDurationCallback?.Invoke(this);
        Initialize();
    }

    public int GetRemainningDuration()
    {
        // 남은 시간 반환
        return _radius.GetTime();
    }
    public int GetGroupId() => data.groupId;
    public StatusEffectData GetData() => data;


    private void SetData(StatusEffectData argData)
    { 
        data = argData;
        _radius.SetTime(data.duration);
    }

    private void Initialize() => data.Init();
}

[Serializable]
public struct StatusEffectData
{
    public int groupId;
    public int duration;

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
