using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region [Singleton]
    private static readonly object lockObject = new object();
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>();
                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject("UIManager(Ingame)");
                        gameObject.AddComponent<UIManager>();
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
        gameObject.AddComponent<UIManager>();
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
