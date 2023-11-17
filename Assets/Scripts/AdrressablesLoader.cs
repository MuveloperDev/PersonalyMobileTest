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
            // Addressable Asset Settings를 가져옵니다.
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            // 만약에 그룹이 있다면 삭제해 줍니다.
            AddressableAssetGroup group = settings.FindGroup(Direcoty.name);
            if (null != group)
            {
                settings.RemoveGroup(group);
            }
            // 새로운 Addressable Asset Group을 생성합니다.
            group = settings.CreateGroup(Direcoty.name, false, false, true, null);

            // 특정 폴더의 모든 오브젝트를 가져옵니다.
            string[] guids = AssetDatabase.FindAssets("", new[] { Direcoty.path });

            foreach (string guid in guids)
            {
                // GUID를 이용해 애셋의 경로를 가져옵니다.
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("GUID : " + assetPath);
                string[] nameTokens = assetPath.Split('/');
                string nameToken = nameTokens[nameTokens.Length -1].Split('.')[0];
                Debug.Log("token : " + nameToken);
                SetScrptableData(pathDatas, nameToken, assetPath, Direcoty.name);

                // 애셋을 Addressable Asset Group에 추가합니다.
                settings.CreateOrMoveEntry(guid, group);
            }

            // 변경사항을 저장합니다.
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
        }
        string json = JsonConvert.SerializeObject(pathDatas, Formatting.Indented);
        File.WriteAllText("Assets/Tables/PathData.json", json);
        // 애셋을 로드하고 캐싱

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