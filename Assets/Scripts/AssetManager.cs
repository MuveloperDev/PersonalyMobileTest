using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ObjectType
{
    GameObject,
    Material
}
public class AssetManager
{
    #region Singleton
    private static AssetManager instance;
    private AssetManager() { }

    public static AssetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AssetManager();
            }
            return instance;
        }
    }
    #endregion
    public Dictionary<Directories, List<AssetData>> PathDatas = new();
    private bool _isLoadedData = false;
    public async UniTask ScriptableDataLoad()
    {
        TextAsset json = null;
        // �ּ��� �ε��ϰ� ĳ��
        Addressables.LoadAssetAsync<TextAsset>("Assets/Tables/PathData.json").Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // �ּ� �ε� ����
                json = handle.Result;
            }
            else
            {
                // �ּ� �ε� ����
                Debug.LogError("Failed to load asset: ");
            }
        };

        await UniTask.WaitUntil(()=> null != json);
        Debug.Log("���");
        PathDatas = JsonConvert.DeserializeObject<Dictionary<Directories, List<AssetData>>>(json.text);

        foreach (var datas in PathDatas)
        {
            Debug.Log("Datas key : " + datas.Key);
            foreach (var data in datas.Value)
            {
                Debug.Log($"data name : {data.name} / data path : {data.path}");
            }
        }

        _isLoadedData = true;
    }
    public async UniTask<T> LoadAsset<T>(Directories folderName, string prefabName) where T: class
    {
        if (false == _isLoadedData)
            await UniTask.WaitUntil(()=> false != _isLoadedData);

        string[] tokens = prefabName.Split(')');
        string token = tokens[0];

        string path = string.Empty;
        foreach (var data in PathDatas[folderName])
        {
            if (prefabName == data.name)
            {
                path = data.path;
            }
        }

        if (true == string.IsNullOrEmpty(path))
            return null;

        T asset = null;
        Addressables.LoadAssetAsync<T>(path).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // �ּ� �ε� ����
                asset = handle.Result;
            }
            else
            {
                // �ּ� �ε� ����
                Debug.LogError("Failed to load asset: ");
            }
        };
        
        await UniTask.WaitUntil(()=>null != asset);
        return asset;
    }
    public async UniTask<UnityEngine.GameObject> GameObjectInstantiate(Directories folderName, string prefabName, Transform parent, Action<GameObject> onComplete = null)
    {
        GameObject assetInstance = null;
        bool isProcessing = true;

        string path = $"Assets/{folderName}/{prefabName}.prefab";
       // string path = GetPath(folderName, prefabName);
        if (true == string.IsNullOrEmpty(path))
            return null;

        Addressables.InstantiateAsync(path, parent).Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // �ּ� �ν��Ͻ� ���� ����
                assetInstance = handle.Result;
                // ���⿡�� assetInstance�� ����Ͽ� �ʿ��� ó���� ����
            }
            else
            {
                // �ּ� �ν��Ͻ� ���� ����
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessing = false;
        };

        await UniTask.WaitUntil(() => true != isProcessing);

        if (onComplete != null)
        {
            onComplete(assetInstance);
        }
        if (assetInstance != null)
        {
            return assetInstance;
        }
        return null;
    }

    public async UniTask<T> Instantiate<T>(Directories folderName, string prefabName, Action<T> onComplete = null) where T : class
    {
        GameObject assetInstance = null;
        bool isProcessing = true;

        string path = GetPath(folderName, prefabName);
        if (true == string.IsNullOrEmpty(path))
            return null;

        Addressables.InstantiateAsync(path).Completed += (AsyncOperationHandle<GameObject> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // �ּ� �ν��Ͻ� ���� ����
                assetInstance = handle.Result;
            }
            else
            {
                // �ּ� �ν��Ͻ� ���� ����
                Debug.LogError("Failed to instantiate asset: " + handle.OperationException);
            }
            isProcessing = false;
        };

        await UniTask.WaitUntil(() => true != isProcessing);

        if (typeof(GameObject) == typeof(T))
        {
            if (onComplete != null)
            {
                onComplete(assetInstance as T);
            }
            return assetInstance as T;
        }

        if (assetInstance.TryGetComponent<T>(out T component))
        {
            if (onComplete != null)
            {
                onComplete(component);
            }
            return component;
        }
        

        return null;
    }

    string GetPath(Directories folderName, string prefabName)
    {
        string path = string.Empty;
        foreach (var data in PathDatas[folderName])
        {
            if (prefabName == data.name)
            {
                path = data.path;
            }
        }

        if (true == string.IsNullOrEmpty(path))
            return string.Empty;

        return path;
    }

    public async void InstantiateAddressableAsset()
    {
        // InstantiateAsync �޼��带 ����Ͽ� ������ �񵿱������� �ε��ϰ� �ν��Ͻ�ȭ
        var operationHandle = Addressables.InstantiateAsync("");

        // �񵿱� �۾��� �Ϸ�� ������ ���
        await operationHandle.Task;

        if (operationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            GameObject instantiatedObject = operationHandle.Result;

            // ������ GameObject�� RectTransform�� ����
            RectTransform rectTransform = instantiatedObject.GetComponent<RectTransform>();

            // RectTransform�� �Ӽ� ����
            rectTransform.anchoredPosition = Vector2.zero;

            // �θ� ���� (Canvas �� UI �θ� ������Ʈ)
            rectTransform.SetParent(GameObject.Find("Canvas").transform, false);
        }
        else
        {
            Debug.LogError("Failed to load and instantiate asset.");
        }
    }
}
