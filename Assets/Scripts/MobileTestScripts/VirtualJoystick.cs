using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private Vector2 inputVector;

    public float GetHorizontal()
    {
        return inputVector.x;
    }

    public float GetVertical()
    {
        return inputVector.y;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Start");
        //OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / joystickBackground.sizeDelta.x) + 0.5f;
            pos.y = (pos.y / joystickBackground.sizeDelta.y) + 0.5f;

            inputVector = new Vector2(pos.x * 2 - 1, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystickHandle.anchoredPosition = new Vector2(inputVector.x * (joystickBackground.sizeDelta.x / 3), inputVector.y * (joystickBackground.sizeDelta.y / 3));
        }
        Vector2 inputDir = eventData.position - joystickBackground.anchoredPosition;
        Debug.Log($"inputDir.x :{GetHorizontal()} / inputDir.y : {GetVertical()}");
    }
}
