using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.VersionControl;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets.Build;
public enum Directories
{
    Prefabs,
    Tables
}
public class AdrressablesLoader
{

    struct DirectoryInfoStruct
    {
        public string path;
        public string name;
    }

    static Dictionary<Directories, DirectoryInfoStruct> fileAdressDic = new Dictionary<Directories, DirectoryInfoStruct>();

    [MenuItem("CustomTool/Load Addressable Assets")]
    static public async void LoadAdressables()
    {
        await SetDirectoriesPath();
        Dictionary<Directories, List<AssetData>> pathDatas = new Dictionary<Directories, List<AssetData>>();


        foreach (var Direcoty in fileAdressDic.Values)
        {
            // Addressable Asset Settings�� �����ɴϴ�.
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // ���࿡ �׷��� �ִٸ� ������ �ݴϴ�.
            AddressableAssetGroup group = settings.FindGroup(Direcoty.name);
            if (null != group)
            {
                settings.RemoveGroup(group);
            }
            // ���ο� Addressable Asset Group�� �����մϴ�.
            group = settings.CreateGroup(Direcoty.name, false, false, true, null);

            // Ư�� ������ ��� ������Ʈ�� �����ɴϴ�.
            string[] guids = AssetDatabase.FindAssets("", new[] { Direcoty.path });

            foreach (string guid in guids)
            {
                // GUID�� �̿��� �ּ��� ��θ� �����ɴϴ�.
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("GUID : " + assetPath);
                string[] nameTokens = assetPath.Split('/');
                string nameToken = nameTokens[nameTokens.Length -1].Split('.')[0];
                Debug.Log("token : " + nameToken);
                SetScrptableData(pathDatas, nameToken, assetPath, Direcoty.name);

                // �ּ��� Addressable Asset Group�� �߰��մϴ�.
                settings.CreateOrMoveEntry(guid, group);
            }

            // ��������� �����մϴ�.
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        }
        string json = JsonConvert.SerializeObject(pathDatas, Formatting.Indented);
        File.WriteAllText("Assets/Tables/PathData.json", json);
        // �ּ��� �ε��ϰ� ĳ��

    }

    static async UniTask SetDirectoriesPath()
    {
        fileAdressDic.Clear();
        DirectoryInfo di = new DirectoryInfo(Application.dataPath);

        foreach (FileInfo file in di.GetFiles())
        {
            StringBuilder sb = new StringBuilder();

            string[] fileTokens = file.Name.Split('.');
            string fileName = fileTokens[0];
            foreach (var directory in Enum.GetValues(typeof(Directories)))
            {
                if (fileName == directory.ToString())
                {
                    sb.Append("Assets/").Append(fileName);
                    DirectoryInfoStruct directoryInfoStruct = new DirectoryInfoStruct() {
                        path = sb.ToString(),
                        name = fileName
                    };
                    fileAdressDic.Add((Directories)directory, directoryInfoStruct);
                }
            }
        }
    }

    static void SetScrptableData(Dictionary<Directories, List<AssetData>> PathDatas, string argName, string argAssetPath, string argDirectoryStr)
    {
        AssetData assetData = new AssetData()
        {
            path = argAssetPath,
            name = argName
        };
        Directories key = (Directories)Enum.Parse(typeof(Directories), argDirectoryStr);

        if (null == PathDatas)
        {
            Debug.LogError("NullException : ArgAsset is null....");
            return;
        }

        if (false == PathDatas.ContainsKey(key))
        {
            List<AssetData> list = new();
            list.Add(assetData);
            PathDatas.Add(key, list);
            return;
        }

        PathDatas[key].Add(assetData);
    }


}
public struct AssetData
{
    public string path;
    public string name;
}