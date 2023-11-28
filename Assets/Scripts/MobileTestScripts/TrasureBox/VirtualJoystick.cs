using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : IngameVirtualJoystickBase, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] IngameUIButtonBase _button;
    [SerializeField] Vector2 _offset;
    private void Awake()
    {
        _button =GetComponent<IngameUIButtonBase>();
        if (null != _button)
        {
            _button.OnClickDownAddLitener(DownEvent);
            _button.OnClickUpAddLitener(UpEvent);
        }
        _offset = new Vector2(joystickBackground.anchoredPosition.x, joystickBackground.anchoredPosition.y);
    }
    public override void OnShow(PointerEventData eventData)
    {
        base.OnShow(eventData);
    }
    public override void OnHide()
    {
        base.OnHide();
    }
    private void DownEvent(PointerEventData eventData)
    {
        MoveJoystickPos(eventData.position);
    }
    private void UpEvent(PointerEventData eventData)
    {
        ResetJoystickPos(_offset);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Start");
        //OnShow(eventData);
        //OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        base.HandleJoystickInput(eventData);
    }

}
