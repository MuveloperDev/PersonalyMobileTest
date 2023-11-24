using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngameUIComboActionButtonProcess : MonoBehaviour
{
    [SerializeField] private int _maxComboCnt;
    [SerializeField] private int _curComboCnt;
    [SerializeField] private int _offsetCoolTimeValue;



    private async UniTask ComboProcess()
    {
        _curComboCnt = _maxComboCnt;
        for (int i = 0; i < _maxComboCnt; i++)
        {

            Debug.Log($"_curComboCnt : {_curComboCnt}");
        }
    }
}
