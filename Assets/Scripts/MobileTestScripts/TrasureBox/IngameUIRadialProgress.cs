using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngameUIRadialProgress
{
    [SerializeField] private Image _radiusImg;
    [SerializeField] private bool _isProgress;
    [SerializeField] private float _curPercentValue;
    [SerializeField] private float _duration;
    [SerializeField] private float _speed;
    [SerializeField] private MonoBehaviour _coroutineRunner;
    [SerializeField] private CancellationTokenSource _cts;

    public IngameUIRadialProgress(float argDuration, MonoBehaviour argCoroutineRunner, Image argRadiusImg)
    {
        _duration = argDuration;
        _coroutineRunner = argCoroutineRunner;
        _radiusImg = argRadiusImg;
        Initialized();
    }

    public void Interaction(Action onAction = null)
    {
        _radiusImg.gameObject.SetActive(true);
        Progress(onAction).Forget();
    }

    public void Reset()
    {
        if (true == _isProgress)
        {
            _cts.Cancel();
        }
    }

    public void Cancle()
    {
        _cts.Cancel();
    }

    public CancellationTokenSource GetCTS() => _cts;
    public bool GetProgress() => _isProgress;
    public void SetTime(float argTime)
    {
        _duration = argTime;
        Initialized();
    }
    public int GetTime() => (int)Math.Ceiling( _curPercentValue / (_speed * Time.deltaTime));
    public void SetActive(bool isActive) => _radiusImg.gameObject.SetActive(isActive);

    private void Initialized()
    {
        _curPercentValue = 100;
        _speed = _curPercentValue / _duration;
        _isProgress = false;
        _cts = new CancellationTokenSource();
    }

    // onAction 은 버튼 프로세스용
    private async UniTask Progress(Action onAction = null)
    {
        _isProgress = true;
        while (true)
        {
            await UniTask.WaitForEndOfFrame(_coroutineRunner);

            if (true == _cts.Token.IsCancellationRequested)
            {
                Debug.Log("CancleProgress");
                Initialized();
                SetActive(false);
                return;
            }

            if (_curPercentValue > 0)
            {
                _curPercentValue -= _speed * (Time.deltaTime);
            }

            _radiusImg.fillAmount = _curPercentValue / 100;

            if (_curPercentValue <= 0)
            {
                break;
            }
        }
        Initialized();
        _radiusImg.gameObject.SetActive(false);
        onAction?.Invoke();
    }
}
