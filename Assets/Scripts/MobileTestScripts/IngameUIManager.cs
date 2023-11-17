using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class IngameUIManager
{
    // �ӽ� enum ���� ���� ����.
    public enum InGameMode
    {
        None = 0,
        ThirdToThird // 3vs3
    }

    public enum IngameCanvasType
    {
        RapidlyUpdate = 0,
        SlowlyUpdate
    }

    #region [Singleton]
    private static IngameUIManager instance;
    private static readonly object lockObject = new object();

    public static IngameUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new IngameUIManager();
                    }
                }
            }
            return instance;
        }
    }
    #endregion

    #region [UI_Property]
    #endregion

    #region [Property]
    private InGameMode _mode;
    #endregion

    #region [Initialize]
    private IngameUIManager()
    {
        // �ʱ�ȭ
        Init();
    }

    private void Init()
    {
        // ��带 �о�´�.
        CreateUISequence().Forget();
    }
    #endregion

    #region [Private]
    private async UniTask CreateUISequence()
    {
        await CreateCanvas();
        await SetMode();
        await CreateUIByMode();
    }
    private async UniTask SetMode()
    {
        // ��带 �о�´�.
    }
    private async UniTask CreateCanvas()
    {
        // ĵ���� ����
        foreach (IngameCanvasType canvasType in Enum.GetValues(typeof(IngameCanvasType)))
        {
            // SafeArea�� ���� ����.
        }
    }
    private async UniTask CreateUIByMode()
    {
        // ��庰 UI ����
        switch (_mode)
        {
            case InGameMode.None:
                break;
            case InGameMode.ThirdToThird:
                break;
            default:
                break;
        }
    }

    #endregion
}
