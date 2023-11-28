using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;
public class IngameUIManager
{
    #region [�ӽ� ENUM ���� ����]
    public enum InGameMode
    {
        None = 0,
        ThirdToThird // 3vs3
    }

    public enum IngameCanvasType
    {
        RapidlyUpdate = 0,
        SlowlyUpdate,
    }
    #endregion

    #region [Singleton]
    private static IngameUIManager _instance;
    public static bool IsExistence = false;
    private static readonly object lockObject = new object();

    public static IngameUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new IngameUIManager();
                    }
                }
            }
            return _instance;
        }
    }
    private IngameUIManager() {
        IsExistence = true;
    }
    #endregion

    #region [GetSet_Property]
    public bool _isInitialze { get; private set; } = false;
    public IngameInputSystem InputSystem { get; private set; } = null;
    public UIBaseTest1 test1 { get; private set; } = null;
    public UIBaseTest2 test2 { get; private set; } = null;

    #endregion

    #region [Property]
    private InGameMode _mode;
    private Dictionary<IngameCanvasType, Transform> _canvasDic = new();
    private List<GameObject> createdUIElements = new(); // ���� ������ UI ������Ʈ ������
    #endregion

    #region [Public]
    // �� ���Ӿ� ���Խ� ���� ���� ��������� �ʱ�ȭ��Ű�� ���� �Լ�.
    public async void Initialize()
    {
        // ��带 �о�´�.
        await SetMode();
        CreateUISequence().Forget();
    }

    // �� ��ȯ�� Ingame���� ����ϴ� UIManager�� �޸𸮸� ȸ���ϱ� ���� �Լ�.
    // ���� �۾��� ���� ���ٰ�.
    public static void DestroyInstance()
    {
        _instance.InputSystem = null;
        _instance = null;
        IsExistence = false;
    }
    #endregion

    #region [Private]
    private async UniTask CreateUISequence()
    {
        await CreateCanvas();
        await SetMode();
        await CreateUIBasedOnMode();
        _isInitialze = true;
    }
    private async UniTask SetMode()
    {
        // ��带 �о�´�.
        _mode = InGameMode.ThirdToThird; //(�ӽ�)
    }

    private async UniTask CreateCanvas()
    {
        GameObject root = new GameObject("UIManager");
        // ĵ���� ����
        foreach (IngameCanvasType canvasType in Enum.GetValues(typeof(IngameCanvasType)))
        {
            var canvas = await InstantiateAndInitializeCanvas(canvasType, root.transform);
            //SafeAreaFitter safeArea = await InstantiateAndInitializeSafeArea(canvas.transform);
            _canvasDic.Add(canvasType, canvas.transform);
            createdUIElements.Add(canvas.gameObject);
        }
    }

    private async UniTask CreateUIBasedOnMode()
    {
        // ��庰 UI ����
        switch (_mode)
        {
            case InGameMode.None:
                break;
            case InGameMode.ThirdToThird:
                await CreateUIThirdToThird();
                break;
            default:
                break;
        }
    }


    #region [Create_UI_Function_Based_On_Mode]
    private async UniTask CreateUIThirdToThird()
    {
        InputSystem = await InstantiateRapidlyUpdateUI<IngameInputSystem>("Assets/Prefabs/IngameInputSystem.prefab");
        //InputSystem.OnHide();
        test1 = await InstantiateRapidlyUpdateUI<UIBaseTest1>("Assets/Prefabs/UIBaseTest1.prefab");
        test1.OnHide();
        test2 = await InstantiateRapidlyUpdateUI<UIBaseTest2>("Assets/Prefabs/UIBaseTest2.prefab");
        test2.OnHide();
    }
    #endregion


    
    private async UniTask<T> InstantiateRapidlyUpdateUI<T>(string argPath, Action<T> onComplete = null) where T : Object
    {
        T result = await Instantiate(argPath, _canvasDic[IngameCanvasType.RapidlyUpdate], onComplete);
        return result;
    }

    private async UniTask<T> InstantiateSlowlyUpdateUI<T>(string argPath, Action<T> onComplete = null) where T : Object
    {
        T result = await Instantiate(argPath, _canvasDic[IngameCanvasType.SlowlyUpdate], onComplete);
        return result;
    }

    private async UniTask<Canvas> InstantiateAndInitializeCanvas(IngameCanvasType argCanvasType, Transform argParent)
    {
        string name = SetCanvasName();
        GameObject canvasGameObject = new GameObject(name);
        canvasGameObject.transform.SetParent(argParent);
        Canvas canvas = canvasGameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        CanvasScaler scaler = canvasGameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        // �ػ󵵴� �̸� �����ؼ� ������� �����ؼ� ���� ���� ���� ���δ�.
        scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGameObject.AddComponent<GraphicRaycaster>();
        return canvas;

        string SetCanvasName()
        {
            switch (argCanvasType)
            {
                case IngameCanvasType.RapidlyUpdate:
                    return "RapidlyUpdateCanvas";
                case IngameCanvasType.SlowlyUpdate:
                    return "SlowlyUpdateCanvas";
            }
            return string.Empty;
        }
    }

    private async UniTask<SafeAreaFitter> InstantiateAndInitializeSafeArea(Transform argParent)
    {
        SafeAreaFitter safeArea = await Instantiate<SafeAreaFitter>("Assets/Prefabs/SafeArea.prefab", argParent);
        return safeArea;
    }
    
    // ���� ������� ��巹����� ��ü����
    private async UniTask<T> Instantiate<T>(string argPath, Transform argParent = null, Action<T> onComplete = null) where T : Object
    {
        GameObject assetInstance = null;
        bool isProcessing = true;

        if (true == string.IsNullOrEmpty(argPath))
        {
            Debug.LogError("ArgumentException : Path is Empty");
            return null;
        }

        Addressables.InstantiateAsync(argPath, argParent).Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                assetInstance = handle.Result;
            }
            else
            {
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessing = false;
        };

        await UniTask.WaitUntil(() => true != isProcessing);

        if (assetInstance == null)
        {
            Debug.LogError("Failed to instantiate asset");
            return null;
        }

        bool isRequiresGetComponent = (typeof(T) == typeof(GameObject)) || typeof(T) == typeof(Transform);
        if (isRequiresGetComponent)
        {
            return typeof(T) == typeof(GameObject) ? assetInstance as T : assetInstance.transform as T;
        }

        T result = assetInstance.GetComponent<T>();
        if (result == null)
        {
            Debug.LogError($"Failed to get component of type {typeof(T).Name} from the instantiated asset. Please ensure the asset contains a component of this type.");
            return null;
        }    

        if (onComplete != null)
        {
            onComplete(result);
        }

        createdUIElements.Add(assetInstance);
        return result;
    }
    #endregion
}
