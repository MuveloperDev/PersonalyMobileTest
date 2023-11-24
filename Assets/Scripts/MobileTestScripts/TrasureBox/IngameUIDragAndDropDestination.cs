using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngameUIDragAndDropDestination : MonoBehaviour
{
    private Action _onPointerEnter;
    public void OnPointerEnterAddLitener(Action argEvent) => _onPointerEnter += argEvent;

    private Action _onPointerExit;
    public void OnPointerExitAddLitener(Action argEvent) => _onPointerExit += argEvent;


    public void OnPointerEnter()
    {
        Debug.Log("OnPointerEnter");
        // �ѿ��� �̺�Ʈ �߻�
        if (null != _onPointerEnter)
        {
            _onPointerEnter();
        }
    }

    public void OnPointerExit()
    {
        // �Ѿƿ� �̺�Ʈ �߻�
        Debug.Log("OnPointerExit");
        if (null != _onPointerExit)
        {
            _onPointerExit();
        }
    }
}
