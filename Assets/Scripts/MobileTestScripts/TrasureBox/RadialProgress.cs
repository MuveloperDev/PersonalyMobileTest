using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RadialProgress
{
    [SerializeField] private Image _radiusImg;
    [SerializeField] private bool _cacledForce;
    [SerializeField] private bool _isProgress;
    [SerializeField] private float _curPercentValue;
    [SerializeField] private float _duration;
    [SerializeField] private float _speed;
    [SerializeField] private MonoBehaviour _coroutineRunner;
    [SerializeField] private CancellationTokenSource _cts;

    public RadialProgress(float argDuration, MonoBehaviour argCoroutineRunner, Image argRadiusImg)
    {
        _duration = argDuration;
        _coroutineRunner = argCoroutineRunner;
        _radiusImg = argRadiusImg;
        _cts = new CancellationTokenSource();
        Initialized();
    }

    public void Interaction()
    {
        SetActive(true);
        _isProgress = true;
        Progress().Forget();
    }

    public void Cancle()
    {
        _cacledForce = true;
    }

    public CancellationTokenSource GetCTS() => _cts;
    public bool GetProgress() => _isProgress;
    public void SetTime(float argTime) => _duration = argTime;
    public void SetActive(bool isActive) => _radiusImg.gameObject.SetActive(isActive);

    private void Initialized()
    {
        _curPercentValue = 100;
        _speed = _curPercentValue / _duration;
        _cacledForce = false;
        _isProgress = false;
    }

    private async UniTask Progress(CancellationToken cancellationToken = default(CancellationToken))
    {
        while (true)
        {
            await UniTask.WaitForEndOfFrame(_coroutineRunner);

            if (true == cancellationToken.IsCancellationRequested)
                break;

            if (true == _cacledForce)
            {
                Initialized();
                break;
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
        SetActive(false);
    }
}
