using System;
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
    [SerializeField] protected Vector2 _inputVector;
    [SerializeField] protected Vector2 _correctedInputVector;
    [SerializeField] protected Vector2 _inputDir;

    [Header("SETTINGS")]
    [SerializeField] protected float MovBtnDeadZoneRange = 0f;

    public float GetHorizontal() => _inputVector.x;
    public float GetVertical() => _inputVector.y;
    public Vector2 GetVector2() => _inputVector;
    public Vector2 GetCorrectedDeadZoneVector2() => _correctedInputVector;

    public virtual void OnShow(PointerEventData eventData)
    {
        if (null == _rectTransform)
            _rectTransform = GetComponent<RectTransform>();

        _rectTransform.position = eventData.position;

        gameObject.SetActive(true);
        MovBtnDeadZoneRange = 0.5f;
    }

    public virtual void OnHide()
    {
        _rectTransform.anchoredPosition = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        _inputVector = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void MoveJoystickPos(Vector2 argDestPos)
    {
        joystickBackground.position = argDestPos;
    }

    public void ResetJoystickPos(Vector2 argOffsetPos)
    {
        joystickBackground.anchoredPosition = argOffsetPos;
    }

    public virtual void HandleJoystickInput(PointerEventData eventData)
    {
        MovBtnDeadZoneRange = 0.5f;
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / joystickBackground.sizeDelta.x) + 0.5f;
            pos.y = (pos.y / joystickBackground.sizeDelta.y) + 0.5f;

            _inputVector = new Vector2(pos.x * 2 - 1, pos.y * 2 - 1);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;


            joystickHandle.anchoredPosition = new Vector2(_inputVector.x * (joystickBackground.sizeDelta.x / 3), _inputVector.y * (joystickBackground.sizeDelta.y / 3));

            float x = _inputVector.x;
            float y = _inputVector.y;
            bool onDeadZone = x <= MovBtnDeadZoneRange && y <= MovBtnDeadZoneRange && x >= -MovBtnDeadZoneRange && y >= -MovBtnDeadZoneRange;
            if (true == onDeadZone)
            {
                _inputVector = Vector2.zero;
            }
            else
            {
                _correctedInputVector = CorrectInputVector(_inputVector, MovBtnDeadZoneRange);
            }

        }
        _inputDir = eventData.position - joystickBackground.anchoredPosition;
    }

    private Vector2 CorrectInputVector(Vector2 inputVector, float deadZone)
    {
        float correctedX = (inputVector.x - deadZone) / (1 - deadZone);
        float correctedY = (inputVector.y - deadZone) / (1 - deadZone);
        return new Vector2(Mathf.Clamp(correctedX, 0, 1), Mathf.Clamp(correctedY, 0, 1));
    }
}
