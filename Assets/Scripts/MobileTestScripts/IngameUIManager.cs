using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class IngameUIManager
{
    // 임시 enum 추후 삭제 예정.
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
        // 초기화
        Init();
    }

    private void Init()
    {
        // 모드를 읽어온다.
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
        // 모드를 읽어온다.
    }
    private async UniTask CreateCanvas()
    {
        // 캔버스 생성
        foreach (IngameCanvasType canvasType in Enum.GetValues(typeof(IngameCanvasType)))
        {
            // SafeArea도 같이 생성.
        }
    }
    private async UniTask CreateUIByMode()
    {
        // 모드별 UI 생성
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
