#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
#pragma warning disable CS1998
public class DamageFontData
{
    public int Id;
    public string Description = "";
    public int DamageType;
    public int Ref_ValueStringId;
    public string SizeCurvedExport = "";
    public string PositionXCurvedExport = "";
    public string PositionYCurvedExport = "";
    public float Duration;
    public float[] FadeTimeSec;
    public float[][] StartPositionXY;
    public float[][] EndPositionXY;
    public float Height;
    public bool UseBold;
    public float[] PrefabScale;
    public string FontColorHex = "";
    public bool UseIcon;
    public string IconPath = "";
}


    [System.Serializable]
public class IngameUIDamageFontTool : MonoBehaviour
{
    public List<DamageFontData> dataList = new List<DamageFontData>();
    public int tableId = 1;
    public bool useTable = false;
    public int damageType;

    public int refValueStringId;
    public float height;
    public float duration;
    public string positionCurvedXExport;
    public string positionCurvedYExport;
    public string scaleCurvedExport;
    public float[] fadeTimeSec = new float[2] { 0, 0 };
    public bool useRandomPositionArray;
    public float[] startPositionX = new float[2] { 0, 0 };
    public float[] startPositionY = new float[2] { 0, 0 };
    public float[] endPositionX = new float[2] { 0, 0 };
    public float[] endPositionY = new float[2] { 0, 0 };
    public bool useBold;
    public float[] prefabScale;
    public string fontColorHex;
    public bool useIcon;
    public string iconPath;
    public AnimationCurve positionXCurve;
    public AnimationCurve positionYCurve;
    public AnimationCurve scaleCurve;

    private static RectTransform rect;
    private static CanvasGroup canvasGroup;
    //private static StringLocalizer stringLocalizer;
    private static TextMeshProUGUI text;
    private static Image Icon;
    private void Awake()
    {
        //var loader = new TableLoader();
        //loader.LoadAll();

        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //stringLocalizer = GetComponentInChildren<StringLocalizer>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        Icon = transform.GetChild(0).GetComponent<Image>();
    }
    void Start()
    {

    }


    void Update()
    {

    }
    public static void StartDamageFont(bool usePositionArray, DamageFontData data, AnimationCurve positionXCurve, AnimationCurve positionYCurve, AnimationCurve scaleCurve)
    {
        SetData(data).Forget();
        if (true == usePositionArray)
        {
            PositionCurve(data, positionXCurve, positionYCurve).Forget();
        }
        else
        {
            NonRandomPosPositionCurve(data, positionXCurve, positionYCurve).Forget();
        }

        ScaleCurve(data.PrefabScale, data.Duration, scaleCurve).Forget();
    }

    private static async UniTask SetData(DamageFontData data)
    {
        //SetText();
        SetBold();
        //SetFontColorHex();
        //SetIcon().Forget();

        void SetText()
        {
            //if (data.DamageType < 7000)
            //{
            //    //{0}에서 수치값을 출력해준다.
            //    var stringData = StringData.Table.TryGet(data.Ref_ValueStringId);
            //}
            //else
            //{
            //    stringLocalizer.UpdateString(data.Ref_ValueStringId);
            //}

        }
        void SetBold()
        {
            if (true == data.UseBold)
            {
                FontStyles bold = FontStyles.Bold;
                text.fontStyle = bold;
                return;
            }
            FontStyles normal = FontStyles.Normal;
            text.fontStyle = normal;
        }
        void SetFontColorHex()
        {
            Color color;
            if (true == ColorUtility.TryParseHtmlString(data.FontColorHex, out color))
            {
                text.color = color;
            }
        }
        async UniTask SetIcon()
        {
            if (true == data.UseIcon)
            {
                Icon.gameObject.SetActive(true);
                Icon.sprite = await Addressables.LoadAssetAsync<Sprite>(data.IconPath);
            }
            else
            {
                Icon.gameObject.SetActive(false);
            }
        }
    }

    private static async UniTask PositionCurve(DamageFontData data, AnimationCurve positionXCurve, AnimationCurve positionYCurve)
    {
        float startX = Random.Range(data.StartPositionXY[0][0], data.StartPositionXY[0][1]);
        float startY = Random.Range(data.StartPositionXY[1][0], data.StartPositionXY[1][1]);
        float endX = Random.Range(data.EndPositionXY[0][0], data.EndPositionXY[0][1]);
        float endY = Random.Range(data.EndPositionXY[1][0], data.EndPositionXY[1][1]);

        Vector2 startCoord = new Vector2(startX, startY);
        Vector2 endCoord = new Vector2(endX, endY);

        //AnimationCurve ByteData를 받아 역직렬화한다.

        // 최초지점과 끝 지점 시간에 해당하는 키프레임 변경
        SetStartAndEndKeyFrame();
        // 만약 커브값이 존재한다면...
        #region [CEHCK EXIST KEY]
        SetMiddleKeyFrameYCurve();
        #endregion

        FadeIn(data.FadeTimeSec[0]).Forget();

        float timeElapsed = 0.0f;
        bool processingFadeOut = false;
        while (timeElapsed < data.Duration)
        {
            await UniTask.Yield();
            timeElapsed += Time.deltaTime;

            if (data.FadeTimeSec[1] >= data.Duration - timeElapsed && false == processingFadeOut)
            {
                processingFadeOut = true;
                //Debug.Log("duration - timeElapsed : " + duration - timeElapsed);
                FadeOut(data.FadeTimeSec[1]).Forget();
            }

            float curveValueY = positionYCurve.Evaluate(timeElapsed);
            float curveValueX = positionXCurve.Evaluate(timeElapsed);

            float currentY = curveValueY;
            float currentX = curveValueX;

            rect.localPosition = new Vector3(currentX, currentY, rect.localPosition.z);
        }
        void SetStartAndEndKeyFrame()
        {
            if (2 <= positionYCurve.keys.Length)
            {
                Keyframe firstYKey = new Keyframe(0, startCoord.y);
                Keyframe lastYKey = new Keyframe(data.Duration, endCoord.y);
                positionYCurve.MoveKey(0, firstYKey);
                positionYCurve.MoveKey(positionXCurve.keys.Length - 1, lastYKey);
            }
            else
            {
                // 만약 커브값이 존재하지않는다면...
                Keyframe firstYKey = new Keyframe(0, startCoord.y);
                Keyframe lastYKey = new Keyframe(data.Duration, endCoord.y);
                positionYCurve.AddKey(firstYKey);
                positionYCurve.AddKey(lastYKey);
            }

            if (2 <= positionXCurve.keys.Length)
            {
                Keyframe firstYKey = new Keyframe(0, startCoord.x);
                Keyframe lastYKey = new Keyframe(data.Duration, endCoord.x);
                positionXCurve.MoveKey(0, firstYKey);
                positionXCurve.MoveKey(positionXCurve.keys.Length - 1, lastYKey);
            }
            else
            {
                // 만약 커브값이 존재하지않는다면...
                Keyframe firstYKey = new Keyframe(0, startCoord.x);
                Keyframe lastYKey = new Keyframe(data.Duration, endCoord.x);
                positionXCurve.AddKey(firstYKey);
                positionXCurve.AddKey(lastYKey);
            }
        }
        void SetMiddleKeyFrameYCurve()
        {
            if (positionYCurve.length - 1 < 2)
            {
                // middleKeyFrame이 없는 경우
                float midTime = (positionYCurve.keys[positionYCurve.length - 1].time) / 2;
                positionYCurve.AddKey(midTime, data.Height);
            }
            else
            {
                // middleKeyFrame이 있는 경우
                Keyframe middleYKey = new Keyframe(positionYCurve.keys[1].time, data.Height);
                positionYCurve.MoveKey(1, middleYKey);
            }
        }
    }

    private static async UniTask ScaleCurve(float[] scaleData, float duration, AnimationCurve scaleCurve)
    {
        // 만약 커브값이 존재한다면...
        if (1 <= scaleCurve.keys.Length)
        {
            Keyframe firstKey = new Keyframe(0, scaleData[0]);
            Keyframe lastKey = new Keyframe(duration, scaleData[1]);

            scaleCurve.MoveKey(0, firstKey);
            scaleCurve.MoveKey(scaleCurve.keys.Length - 1, lastKey);
        }
        else
        {
            // 만약 커브값이 존재하지않는다면...
            scaleCurve.AddKey(0, scaleData[0]);
            scaleCurve.AddKey(duration, scaleData[1]);
        }

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            await UniTask.Yield();
            timeElapsed += Time.deltaTime;
            float curveValue = scaleCurve.Evaluate(timeElapsed);
            rect.localScale = new Vector3(curveValue, curveValue, 1);
        }
    }

    private static async UniTask NonRandomPosPositionCurve(DamageFontData data, AnimationCurve positionXCurve, AnimationCurve positionYCurve)
    {
        //AnimationCurve ByteData를 받아 역직렬화한다.

        // 만약 커브값이 존재한다면...
        #region [CEHCK EXIST KEY]
        SetMiddleKeyFrameYCurve();
        #endregion

        FadeIn(data.FadeTimeSec[0]).Forget();

        float timeElapsed = 0.0f;
        bool processingFadeOut = false;
        while (timeElapsed < data.Duration)
        {
            await UniTask.Yield();
            timeElapsed += Time.deltaTime;

            if (data.FadeTimeSec[1] >= data.Duration - timeElapsed && false == processingFadeOut)
            {
                processingFadeOut = true;
                FadeOut(data.FadeTimeSec[1]).Forget();
            }

            float curveValueY = positionYCurve.Evaluate(timeElapsed);
            float curveValueX = positionXCurve.Evaluate(timeElapsed);

            rect.localPosition = new Vector3(curveValueX, curveValueY, rect.localPosition.z);
        }

        void SetMiddleKeyFrameYCurve()
        {
            if (positionYCurve.length - 1 < 2)
            {
                // middleKeyFrame이 없는 경우
                float midTime = (positionYCurve.keys[positionYCurve.length - 1].time) / 2;
                positionYCurve.AddKey(midTime, data.Height);
            }
            else
            {
                // middleKeyFrame이 있는 경우
                Keyframe middleYKey = new Keyframe(positionYCurve.keys[1].time, data.Height);
                positionYCurve.MoveKey(1, middleYKey);
            }
        }
    }

    public static async UniTask FadeIn(float duration)
    {
        canvasGroup.alpha = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / duration;

            canvasGroup.alpha = alpha;

            await UniTask.Yield();
        }

        canvasGroup.alpha = 1f;
    }
    private static async UniTask FadeOut(float duration)
    {
        canvasGroup.alpha = 1f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / duration);

            canvasGroup.alpha = alpha;

            await UniTask.Yield();

        }

        canvasGroup.alpha = 0f;
    }
}

[CustomEditor(typeof(IngameUIDamageFontTool))]
public class IngameUIDamageFontToolCustomEditor : Editor
{
    SerializedProperty dataList;
    SerializedProperty tableId;
    SerializedProperty useTable;
    SerializedProperty refValueStringId;
    SerializedProperty positionCurvedXExport;
    SerializedProperty positionCurvedYExport;
    SerializedProperty scaleCurvedExport;
    SerializedProperty height;
    SerializedProperty duration;
    SerializedProperty fadeTimeSec;
    SerializedProperty useRandomPositionArray;
    SerializedProperty startPositionX;
    SerializedProperty startPositionY;
    SerializedProperty endPositionX;
    SerializedProperty endPositionY;
    SerializedProperty useBold;
    SerializedProperty prefabScale;
    SerializedProperty fontColorHex;
    SerializedProperty useIcon;
    SerializedProperty iconPath;
    SerializedProperty positionXCurve;
    SerializedProperty positionYCurve;
    SerializedProperty scaleCurve;

    void OnEnable()
    {
        dataList = serializedObject.FindProperty("dataList");
        tableId = serializedObject.FindProperty("tableId");
        useTable = serializedObject.FindProperty("useTable");
        refValueStringId = serializedObject.FindProperty("refValueStringId");
        positionCurvedXExport = serializedObject.FindProperty("positionCurvedXExport");
        positionCurvedYExport = serializedObject.FindProperty("positionCurvedYExport");
        scaleCurvedExport = serializedObject.FindProperty("scaleCurvedExport");
        height = serializedObject.FindProperty("height");
        duration = serializedObject.FindProperty("duration");
        fadeTimeSec = serializedObject.FindProperty("fadeTimeSec");
        useRandomPositionArray = serializedObject.FindProperty("useRandomPositionArray");
        startPositionX = serializedObject.FindProperty("startPositionX");
        startPositionY = serializedObject.FindProperty("startPositionY");
        endPositionX = serializedObject.FindProperty("endPositionX");
        endPositionY = serializedObject.FindProperty("endPositionY");
        useBold = serializedObject.FindProperty("useBold");
        prefabScale = serializedObject.FindProperty("prefabScale");
        fontColorHex = serializedObject.FindProperty("fontColorHex");
        useIcon = serializedObject.FindProperty("useIcon");
        iconPath = serializedObject.FindProperty("iconPath");
        positionXCurve = serializedObject.FindProperty("positionXCurve");
        positionYCurve = serializedObject.FindProperty("positionYCurve");
        scaleCurve = serializedObject.FindProperty("scaleCurve");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.PropertyField(dataList, new GUIContent("DAMAGE FONT TABLE DATA LIST"));

        DrawLineAndLabel("PROPERTY");

        EditorGUILayout.PropertyField(tableId, new GUIContent("TABLE ID"));
        EditorGUILayout.PropertyField(useTable, new GUIContent("USE TABLE"));
        if (false == useTable.boolValue)
        {
            EditorGUILayout.PropertyField(refValueStringId, new GUIContent("REF_VALUE STRING ID"));
            EditorGUILayout.PropertyField(height, new GUIContent("HEIGHT"));
            EditorGUILayout.PropertyField(duration, new GUIContent("DURATION"));
            EditorGUILayout.PropertyField(fadeTimeSec, new GUIContent("FADE TIME SEC"));
            EditorGUILayout.PropertyField(useRandomPositionArray, new GUIContent("USE POSITION ARRAY"));
            if (true == useRandomPositionArray.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30);
                EditorGUILayout.BeginVertical();

                EditorGUILayout.PropertyField(startPositionX, new GUIContent("START POSITION XY_1"));
                EditorGUILayout.PropertyField(startPositionY, new GUIContent("START POSITION XY_2"));
                EditorGUILayout.PropertyField(endPositionX, new GUIContent("END POSITION XY_1"));
                EditorGUILayout.PropertyField(endPositionY, new GUIContent("END POSITION XY_2"));

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(useBold, new GUIContent("USE BOLD"));
            EditorGUILayout.PropertyField(prefabScale, new GUIContent("PREFAB SCALE"));
            EditorGUILayout.PropertyField(fontColorHex, new GUIContent("FONT COLOR HEX"));
            EditorGUILayout.PropertyField(useIcon, new GUIContent("USE ICON"));
            EditorGUILayout.PropertyField(iconPath, new GUIContent("ICON PATH"));
            EditorGUILayout.PropertyField(positionXCurve, new GUIContent("CURVE POSITION X"));
            EditorGUILayout.PropertyField(positionYCurve, new GUIContent("CURVE POSITION Y"));
            EditorGUILayout.PropertyField(scaleCurve, new GUIContent("CURVE SCALE"));

        }

        if (GUILayout.Button("PLAY"))
        {
            if (false == EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
                //sss
            }
            else
            {
                OnClickEventPlay();
            }
        }
        if (GUILayout.Button("STOP"))
        {
        }

        DrawLineAndLabel("LOAD AND PRINT");

        if (GUILayout.Button("LOAD TABLE DATA"))
        {

        }

        if (GUILayout.Button("PRINT SHAKE DATA"))
        {

        }
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawLineAndLabel(string label)
    {
        GUILayout.Space(10);

        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        rect.height = 2;
        EditorGUI.DrawRect(rect, Color.cyan);
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.green;
        labelStyle.fontSize = 15;
        GUILayout.Label(label, labelStyle);
        GUILayout.Space(10);
    }

    public void DrawJaggieArray(SerializedProperty list)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty listProperty = list.GetArrayElementAtIndex(i);

            for (int j = 0; j < listProperty.arraySize; j++)
            {
                SerializedProperty floatProperty = listProperty.GetArrayElementAtIndex(j);
                EditorGUILayout.PropertyField(floatProperty);
            }
        }
    }

    public void OnClickEventPlay()
    {
        float fadeInTimeSec = fadeTimeSec.GetArrayElementAtIndex(0).floatValue;
        float fadeOutTimeSec = fadeTimeSec.GetArrayElementAtIndex(1).floatValue;

        float startXMin = startPositionX.GetArrayElementAtIndex(0).floatValue;
        float startXMax = startPositionX.GetArrayElementAtIndex(1).floatValue;
        float startYMin = startPositionY.GetArrayElementAtIndex(0).floatValue;
        float startYMax = startPositionY.GetArrayElementAtIndex(1).floatValue;

        float endXMin = endPositionX.GetArrayElementAtIndex(0).floatValue;
        float endXMax = endPositionX.GetArrayElementAtIndex(1).floatValue;
        float endYMin = endPositionY.GetArrayElementAtIndex(0).floatValue;
        float endYMax = endPositionY.GetArrayElementAtIndex(1).floatValue;

        float startScale =  prefabScale.GetArrayElementAtIndex(0).floatValue;
        float endScale =  prefabScale.GetArrayElementAtIndex(1).floatValue;

        DamageFontData data = new DamageFontData()
        {
            Id = 9999,
            DamageType = 0,
            Ref_ValueStringId = refValueStringId.intValue,
            PositionXCurvedExport = positionCurvedXExport.stringValue,
            PositionYCurvedExport = positionCurvedYExport.stringValue,
            SizeCurvedExport = scaleCurvedExport.stringValue,
            Height = height.floatValue,
            Duration = duration.floatValue,
            FadeTimeSec = new float[] { fadeInTimeSec, fadeOutTimeSec },
            StartPositionXY = new float[][]
            {
                new float[]
                {
                    startXMin, startXMax
                },
                new float[]
                {
                    startYMin, startYMax
                }
            },
            EndPositionXY = new float[][]
            {
                new float[]
                {
                    endXMin, endXMax
                },
                new float[]
                {
                    endYMin, endYMax
                }
            },
            UseBold = useBold.boolValue,
            // 이부분 내일 추가할 것
            PrefabScale = new float[] { startScale , endScale },
            FontColorHex = fontColorHex.stringValue,
            UseIcon = useIcon.boolValue,
            IconPath = iconPath.stringValue,

        };
        IngameUIDamageFontTool.StartDamageFont(useRandomPositionArray.boolValue,
            data, positionXCurve.animationCurveValue,
            positionYCurve.animationCurveValue,
            scaleCurve.animationCurveValue);
    }
}
#endif