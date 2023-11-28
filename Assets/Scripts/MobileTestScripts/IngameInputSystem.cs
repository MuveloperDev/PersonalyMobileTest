using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static IngameUIInputSystemHandedManager;

public class IngameInputSystem : IngameUIBase
{

    // root�� ���� ��ġ���� ���� default���� ������ �� ���Ұ� ���� �ӽĹ�


    [SerializeField] private ActionButtonManager _actionButtonManager;
    [SerializeField] private VirtualJoystick _joystick;
    [SerializeField] private IngameUIInputSystemHandedManager _handedManager;

    private void Awake()
    {
        var _joystickRect = _joystick.GetComponent<RectTransform>();
        var _actionButtonManagerRect = _actionButtonManager.GetComponent<RectTransform>();
        var rects = GetComponentsInChildren<RectTransform>();
        _handedManager = new IngameUIInputSystemHandedManager(rects.ToList<RectTransform>());
        gameObject.AddComponent<SafeAreaFitter>();

    }
    private void OnValidate()
    {
        if (IngameUIInputSystemHandedManager.HandedType.Left == _handedManager.GetHandedType())
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
class IngameUIInputSystemHandedManager
{
    public enum HandedType
    {
        Left,
        Right
    }

    [Header("HANDED TYPE")]
    [SerializeField] private HandedType _handedType = HandedType.Right;

    [Header("TARGET UILIST")]
    [SerializeField] private List<RectTransform> _uiList;

    [Header("TARGET ANCHOR POS BASED ON HANDED TYPE")]
    [SerializeField] private List<Vector2> _leftAnchorPositions = new ();
    [SerializeField] private List<Vector2> _rightAnchorPositions = new ();

    public IngameUIInputSystemHandedManager(List<RectTransform> argList)
    {
        _uiList = argList;
        foreach (var rect in _uiList)
        {
            _rightAnchorPositions.Add(rect.anchoredPosition);
            var leftRect = new Vector2(-1 * rect.anchoredPosition.x, rect.anchoredPosition.y);
            _leftAnchorPositions.Add(leftRect);
        }

        // ����� HandedPosition�� ������ �����Ѵ�.
    }

    public HandedType GetHandedType() => _handedType;
    public void SetHandedType(HandedType argHandedType) => _handedType = argHandedType;

    public void ChangeHandedType(HandedType argHandedType)
    {
        _handedType = argHandedType;
        SetHanded(argHandedType);
    }

    // �� ��� UI�� Pivot�� �߰����� ���õ��־�� �Ѵ�.
    public void SetHanded(HandedType argType)
    {
        switch (argType)
        {
            case HandedType.Left:
                SetPosition(_leftAnchorPositions);
                break;
            case HandedType.Right:
                SetPosition(_rightAnchorPositions);
                break;
        }
    }

    private void SetPosition(List<Vector2> argList)
    {
        for (int i = 0; i < _uiList.Count; i++)
        {
            _uiList[i].anchoredPosition = argList[i];
        }
    }
}
