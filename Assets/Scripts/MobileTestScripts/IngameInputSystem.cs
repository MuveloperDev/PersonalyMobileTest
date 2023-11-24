using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static HandedManager;

public class IngameInputSystem : IngameUIBase
{

    // root의 대한 위치값은 추후 default값이 나왔을 때 셋할것 밑은 임식밧


    [SerializeField] private ActionButtonManager _actionButtonManager;
    [SerializeField] private VirtualJoystick _joystick;
    [SerializeField] private HandedManager _handedManager;

    private void Awake()
    {
        var _joystickRect = _joystick.GetComponent<RectTransform>();
        var _actionButtonManagerRect = _actionButtonManager.GetComponent<RectTransform>();
        var rects = GetComponentsInChildren<RectTransform>();
        _handedManager = new HandedManager(rects.ToList<RectTransform>());

    }
    private void OnValidate()
    {
        if (HandedManager.HandedType.Left == _handedManager.GetHandedType())
        {
            _handedManager.SetHanded(HandedType.Left);
        }
        else
        {
            _handedManager.SetHanded(HandedType.Right);
        }
    }

    public override void OnShow()
    {
        base.OnShow();
    }

    public override void OnHide() 
    { 
        base.OnHide();
    }

    public ActionButtonManager GetActionButtonManager() => _actionButtonManager;
    public VirtualJoystick GetJoystick() => _joystick;
}

[Serializable]
struct HandedTypeStruct
{
    public Vector2 joystickRootPosition;
    public Vector2 actionButtonsPosition;
}
[Serializable]
class HandedManager
{
    public enum HandedType
    {
        None,
        Left,
        Right
    }

    [Header("HANDED TYPE")]
    [SerializeField] private HandedType _handedType = HandedType.None;

    [SerializeField] private List<RectTransform> _uiList;

    [SerializeField] private List<Vector2> _leftAnchorPositions = new ();
    [SerializeField] private List<Vector2> _rightAnchorPositions = new ();
    [SerializeField] private Vector3 _rightLocalSacle = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 _leftLocalSacle = new Vector3(-1, 1, 1);

    public HandedManager(List<RectTransform> argList)
    {
        _uiList = argList;
        foreach (var rect in _uiList)
        {
            _rightAnchorPositions.Add(rect.anchoredPosition);
            var leftRect = new Vector2(-1 * rect.anchoredPosition.x, rect.anchoredPosition.y);
            _leftAnchorPositions.Add(leftRect);
        }
    }

    public HandedType GetHandedType() => _handedType;
    public void SetHandedType(HandedType argHandedType) => _handedType = argHandedType;

    public void SetHanded(HandedType argType)
    {
        switch (argType)
        {
            case HandedType.Left:
                SetLocalScale(_leftAnchorPositions);
                //SetLocalScale(_leftLocalSacle);
                break;
            case HandedType.Right:
                SetLocalScale(_rightAnchorPositions);
                //SetLocalScale(_rightLocalSacle);
                break;
        }
    }

    void SetLocalScale(List<Vector2> argList)
    {
        for (int i = 0; i < _uiList.Count; i++)
        {
            _uiList[i].anchoredPosition = argList[i];
        }
    }
    void SetLocalScale(Vector3 localScale)
    {
        foreach (var ui in _uiList)
        {
            ui.localScale = localScale;
        }
    }
}

//interface ICustomizeUI
//{
//    void SavePosition()
//    { 

//    }

//    void LoadPosition() 
//    {
        
//    }

//    void UpdatePosition()
//    { 
    
//    }

//}