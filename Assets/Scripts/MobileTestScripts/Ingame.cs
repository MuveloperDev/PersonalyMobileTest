using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame : MonoBehaviour
{
    //public IngameUIButtonBase testBtn;
    private void Awake()
    {
        IngameUIManager.Instance.Initialize();
        //testBtn.OnClickDownAddLitener(() => {
        //    Debug.Log("Down");
        //});
        //testBtn.OnClickUpAddLitener(() => {
        //    Debug.Log("Up");
        //});
        //testBtn.OnClickDragAddLitener(() => {
        //    //Debug.Log("Drag");
        //});
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (false == IngameUIManager.Instance.InputSystem.gameObject.activeSelf)
            {
                IngameUIManager.Instance.InputSystem.OnShow();

            }
            else
            {
                IngameUIManager.Instance.InputSystem.OnHide();
            }
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (false == IngameUIManager.Instance.test1.gameObject.activeSelf)
            {
                IngameUIManager.Instance.test1.OnShow();

            }
            else
            {
                IngameUIManager.Instance.test1.OnHide();
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (false == IngameUIManager.Instance.test2.gameObject.activeSelf)
            {
                IngameUIManager.Instance.test2.OnShow();

            }
            else
            {
                IngameUIManager.Instance.test2.OnHide();
            }
        }

    }
}
