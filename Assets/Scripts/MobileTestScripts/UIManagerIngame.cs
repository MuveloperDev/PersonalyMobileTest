using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerIngame : MonoBehaviour
{
    #region [Singleton]
    private static readonly object lockObject = new object();
    private static UIManagerIngame instance;

    public static UIManagerIngame Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManagerIngame>();
                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject("UIManager(Ingame)");
                        gameObject.AddComponent<UIManagerIngame>();
                        DontDestroyOnLoad(gameObject);
                    }
                }

                return instance;
            }
        }
    }
    #endregion

    #region [CreateUIManager]
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateUIManager()
    {
        if (null != instance)
            return;

        GameObject gameObject = new GameObject("UIManager(Ingame)");
        gameObject.AddComponent<UIManagerIngame>();
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region [GetProperty]
    #endregion


    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }


}
