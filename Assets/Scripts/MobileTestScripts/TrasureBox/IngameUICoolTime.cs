using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngameUICoolTime
{
    [Header("DPENDENCY")]
    [SerializeField] private Image _coolTimeRootObject;
    [SerializeField] private TextMeshProUGUI _textTime;

    [Header("COOLTIME INFORMATION")]
    [SerializeField] private bool _isCoolTime;
    [SerializeField] private int _maxCoolTimeValue;
    [SerializeField] private int _curCoolTimeValue;
    [Header("TOKEN")]
    [SerializeField] private CancellationTokenSource _cts;
    public IngameUICoolTime(int argMaxCoolTimeValue, Image _argCoolTimeObj = null, TextMeshProUGUI _argCoolTimeText = null)
    {
        _maxCoolTimeValue= argMaxCoolTimeValue;
        if (null != _argCoolTimeObj)
        {
            _coolTimeRootObject = _argCoolTimeObj;
        }
        if (null != _argCoolTimeText)
        {
            _textTime = _argCoolTimeText;
        }
        Initialize();
    }

    public async UniTask Interaction(Action onAction = null)
    {
        _coolTimeRootObject.gameObject.SetActive(true);
        _textTime.gameObject.SetActive(true);
        await Process();
    }

    public bool IsCoolTime() => _isCoolTime;
    public void SetMaxCoolTimeValue(int argMaxCoolTimeValue) => _maxCoolTimeValue= argMaxCoolTimeValue;
    public int GetMaxCoolTimeValue() => _maxCoolTimeValue;

    // 쿨타임 초기화용
    public void Reset()
    {
        if (_isCoolTime == true)
        {
            _cts.Cancel();
        }
    }

    private void Initialize()
    {
        _curCoolTimeValue = _maxCoolTimeValue;
        _isCoolTime= false;
        _cts = new CancellationTokenSource();
        _coolTimeRootObject.gameObject.SetActive(false);
        _textTime.gameObject.SetActive(false);
    }

    public void RefreshData(int argMaxCoolTimeValue)
    {
        _maxCoolTimeValue = argMaxCoolTimeValue;
        _curCoolTimeValue = _maxCoolTimeValue;
    }

    private async UniTask Process()
    {
        _isCoolTime = true;
        _curCoolTimeValue = _maxCoolTimeValue;
        while (_curCoolTimeValue > 0)
        {
            Debug.Log("Remaining: " + _curCoolTimeValue + "s");
            _textTime.text = _curCoolTimeValue.ToString();
            await UniTask.Delay(1000, false, PlayerLoopTiming.Update, _cts.Token, false);

            if (_cts.Token.IsCancellationRequested)
                break;

            _curCoolTimeValue--;
        }
        Initialize();
        Debug.Log("Countdown finished!");
    }
}
