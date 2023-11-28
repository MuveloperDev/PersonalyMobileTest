using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IngameUIRadialProgress
{
    [Header("DEPENDENCY")]
    [SerializeField] private Image _radiusImg;
    [SerializeField] private RectTransform _effectTransform;
    [SerializeField] private MonoBehaviour _coroutineRunner;
    [SerializeField] private CancellationTokenSource _cts;

    [Header("INFORMATION")]
    [SerializeField] private bool _isProgress;
    [SerializeField] private bool _isUpdateEffectObj;
    [SerializeField] private float _curPercentValue;
    [SerializeField] private float _duration;
    [SerializeField] private float _speed;



    public IngameUIRadialProgress(float argDuration, MonoBehaviour argCoroutineRunner,
        Image argRadiusImg, RectTransform argEffectTransform)
    {
        _duration = argDuration;
        _coroutineRunner = argCoroutineRunner;
        _radiusImg = argRadiusImg;
        _effectTransform = argEffectTransform;
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
            if (_curPercentValue <= 0)
            {
                break;
            }

            if (_curPercentValue > 0)
            {
                _curPercentValue -= _speed * (Time.deltaTime);
            }

            _radiusImg.fillAmount = _curPercentValue / 100;

            // 이펙트 원을 그리며 따라가는 로직
            if (true == _isUpdateEffectObj)
            {
                float adjust = -15f;
                float imageWidth = _radiusImg.rectTransform.rect.width + adjust;
                float imageHeight = _radiusImg.rectTransform.rect.height + adjust;
                float radian = Mathf.PI - _radiusImg.fillAmount * Mathf.PI * 2;
                float x = imageWidth / 2 * Mathf.Sin(radian);
                float y = imageHeight / 2 * -Mathf.Cos(radian);

                _effectTransform.localPosition = new Vector3(x, y, _effectTransform.localPosition.z);
                _effectTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * radian);
            }

        }
        Initialized();
        _radiusImg.gameObject.SetActive(false);
        onAction?.Invoke();
    }
}
