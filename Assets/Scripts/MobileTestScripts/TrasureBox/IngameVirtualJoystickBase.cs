using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class IngameVirtualJoystickBase : MonoBehaviour
{
    [Header("DEPENDENCY")]
    [SerializeField] protected RectTransform joystickBackground;
    [SerializeField] protected RectTransform joystickHandle;
    [SerializeField] protected RectTransform _rectTransform;

    [Header("INPUT VECTOR")]
    [SerializeField] protected Vector2 inputVector;
    [SerializeField] protected Vector2 inputDir;

    public float GetHorizontal() => inputVector.x;

    public float GetVertical() => inputVector.y;

    public virtual void OnShow(PointerEventData eventData)
    {
        if (null == _rectTransform)
            _rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(true);
        Vector3 pos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            _rectTransform.position = pos;
        }
    }

    public virtual void OnHide()
    {
        _rectTransform.position = Vector3.zero;
        inputVector = Vector2.zero;
        gameObject.SetActive(false);
    }

    public virtual void HandleJoystickInput(PointerEventData eventData)
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
        inputDir = eventData.position - joystickBackground.anchoredPosition;
        //Debug.Log($"inputDir.x :{GetHorizontal()} / inputDir.y : {GetVertical()}");
    }
}
