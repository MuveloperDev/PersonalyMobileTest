using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInputSystem : IngameUIBase
{
    [SerializeField] private ActionButtonManager _actionButtonManager;
    [SerializeField] private VirtualJoystick _joystick;

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
