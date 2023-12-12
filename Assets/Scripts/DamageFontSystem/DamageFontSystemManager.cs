using Cysharp.Threading.Tasks;
using UnityEngine;

public class DamageFontSystemManager : MonoBehaviour
{
    [SerializeField] private Transform _characterSpin;
    [SerializeField] private RectTransform _DamageFont;
    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private AnimationCurve _scaleCurve;

    // 데미지폰트 스크립트에 들어가야함.
    [SerializeField] private Vector3 _characterSpinPos;
    [SerializeField] private float timeElapsed = 0;


    private void Awake()
    {
        Debug.Log($"_positionCurve length : {_positionCurve.length} \n");
        int i =0;
        foreach (var key in _positionCurve.keys)
        {
            Debug.Log($"key{i}Data \n" +
                $"key.value : {key.value} \n" +
                $"key.inTangent : {key.inTangent} \n" +
                $"key.weightedMode : {key.weightedMode} \n" +
                $"key.outTangent : {key.outTangent} \n" +
                $"key.time : {key.time} \n" +
                $"key.inWeight : {key.inWeight} \n" +
                $"key.outWeight : {key.outWeight} \n");
            i++;
        }
    }

    void Start()
    {

        //Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, adjustPos);
        //_characterImage.GetComponent<RectTransform>().position = pos;
        //Vector2 canvasPos = new Vector2();
        //if (true == RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, pos, Camera.main, out canvasPos))
        //{
        //    // 좌측 하단을 원점으로 위치 계산
        //    canvasPos.x += _characterImage.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        //    canvasPos.y += _characterImage.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        //    // imageRect의 위치 변경
        //    _characterImage.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        //    //_characterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x, canvasPos.y);

        //}
    }
    [SerializeField] bool isStart = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _characterSpinPos = _characterSpin.localPosition;
            Vector3 adjustPos = new Vector3(_characterSpinPos.x - 0.3f, _characterSpinPos.y - 0.2f);
            _DamageFont.gameObject.SetActive(true);
            _DamageFont.anchoredPosition = adjustPos;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            float totalDuration = 1.5f;
            //ScaleCurve(new float[]{2.0f, 1.0f}, totalDuration).Forget();
            float[][] startCoord = new float[][]{
                new float[] {0.1f, 0.2f},
                new float[] {-0.1f, -0.2f}
            };
            float[][] endCoord = new float[][]{
                new float[] { 0.5f, 0.8f},
                new float[] { -0.5f, -0.8f }
            };
            PositionCurve(startCoord, endCoord, totalDuration).Forget();
            //isStart = true;
        }
    }

    // 높이를 받으면 포지션 이동에 제약이 생긴다.
    // 이부분은 커브 애니메이션을 건드는게 아니라 
    // 테이블로 관리되어야 한다.
    private async UniTask PositionCurve(float[][] argStartPos, float[][] argEndPos, float duration)
    {
        float startX = Random.Range(argStartPos[0][0], argStartPos[0][1]);
        float startY = Random.Range(argStartPos[1][0], argStartPos[1][1]);
        float endX = Random.Range(argEndPos[0][0], argEndPos[0][1]);
        float endY = Random.Range(argEndPos[1][0], argEndPos[1][1]);

        Vector2 startCoord = new Vector2(startX, startY);
        Vector2 endCoord = new Vector2(endX, endY);

        // 중간 시간에 해당하는 키프레임 추가
        Keyframe lastKey = new Keyframe(duration, endCoord.x);
        Keyframe firstKey = new Keyframe(0, startCoord.x);
        _positionCurve.MoveKey(_positionCurve.keys.Length - 1, lastKey);
        _positionCurve.MoveKey(0, firstKey);

        float midTime = (_positionCurve.keys[_positionCurve.length - 1].time) / 2;
        _positionCurve.AddKey(midTime, 2.0f); // 높이는 임의로 1.0f로 설정

        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            await UniTask.Yield();
            timeElapsed += Time.deltaTime;

            float curveValue = _positionCurve.Evaluate(timeElapsed);

            float currentX = Mathf.Lerp(startCoord.x, endCoord.x, curveValue);
            float currentY = Mathf.Lerp(startCoord.y, endCoord.y, curveValue);

            _DamageFont.localPosition = new Vector3(currentX, currentY, _DamageFont.localPosition.z);
        }
    }

    private async UniTask ScaleCurve(float[] scaleData, float duration)
    {
        // 만약 커브값이 존재한다면...
        if (1 <= _scaleCurve.keys.Length)
        {
            Keyframe firstKey = new Keyframe(0, scaleData[0]);
            Keyframe lastKey = new Keyframe(duration, scaleData[1]);

            _scaleCurve.MoveKey(0, firstKey);
            _scaleCurve.MoveKey(_scaleCurve.keys.Length - 1, lastKey);
        }
        else
        {
            // 만약 커브값이 존재하지않는다면...
            _scaleCurve.AddKey(0, scaleData[0]);
            _scaleCurve.AddKey(duration, scaleData[1]);
        }

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            await UniTask.Yield();
            timeElapsed += Time.deltaTime;
            float curveValue = _scaleCurve.Evaluate(timeElapsed);
            _DamageFont.localScale = new Vector2(curveValue, curveValue);
        }
    }
}
