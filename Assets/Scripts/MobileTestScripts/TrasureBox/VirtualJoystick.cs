using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : IngameVirtualJoystickBase, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public override void OnShow(PointerEventData eventData)
    {
        base.OnShow(eventData);
    }
    public override void OnHide()
    {
        base.OnHide();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Start");
        //OnShow(eventData);
        //OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        base.HandleJoystickInput(eventData);
    }

}
