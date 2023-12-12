using Cysharp.Threading.Tasks;
using UnityEngine;

public class DamageFontSystemManager : MonoBehaviour
{
    [SerializeField] private Transform _characterSpin;
    [SerializeField] private RectTransform _DamageFont;
    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private AnimationCurve _scaleCurve;

    // ��������Ʈ ��ũ��Ʈ�� ������.
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
        //    // ���� �ϴ��� �������� ��ġ ���
        //    canvasPos.x += _characterImage.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        //    canvasPos.y += _characterImage.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        //    // imageRect�� ��ġ ����
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

    // ���̸� ������ ������ �̵��� ������ �����.
    // �̺κ��� Ŀ�� �ִϸ��̼��� �ǵ�°� �ƴ϶� 
    // ���̺�� �����Ǿ�� �Ѵ�.
    private async UniTask PositionCurve(float[][] argStartPos, float[][] argEndPos, float duration)
    {
        float startX = Random.Range(argStartPos[0][0], argStartPos[0][1]);
        float startY = Random.Range(argStartPos[1][0], argStartPos[1][1]);
        float endX = Random.Range(argEndPos[0][0], argEndPos[0][1]);
        float endY = Random.Range(argEndPos[1][0], argEndPos[1][1]);

        Vector2 startCoord = new Vector2(startX, startY);
        Vector2 endCoord = new Vector2(endX, endY);

        // �߰� �ð��� �ش��ϴ� Ű������ �߰�
        Keyframe lastKey = new Keyframe(duration, endCoord.x);
        Keyframe firstKey = new Keyframe(0, startCoord.x);
        _positionCurve.MoveKey(_positionCurve.keys.Length - 1, lastKey);
        _positionCurve.MoveKey(0, firstKey);

        float midTime = (_positionCurve.keys[_positionCurve.length - 1].time) / 2;
        _positionCurve.AddKey(midTime, 2.0f); // ���̴� ���Ƿ� 1.0f�� ����

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
        // ���� Ŀ�갪�� �����Ѵٸ�...
        if (1 <= _scaleCurve.keys.Length)
        {
            Keyframe firstKey = new Keyframe(0, scaleData[0]);
            Keyframe lastKey = new Keyframe(duration, scaleData[1]);

            _scaleCurve.MoveKey(0, firstKey);
            _scaleCurve.MoveKey(_scaleCurve.keys.Length - 1, lastKey);
        }
        else
        {
            // ���� Ŀ�갪�� ���������ʴ´ٸ�...
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
