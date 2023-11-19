using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType
{
    None,
    RadialProgress,
    CoolTime,

}
public class ActionButton : MonoBehaviour
{

    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _radialImage;
    [SerializeField] private RadialProgress _radialProgress;
    [Header("ACTIONTYPE")]
    [SerializeField] private ActionType _type;

    public Action onAction;

    void Start()
    {
        _radialProgress = new RadialProgress(1f, this, _radialImage);
        _button.OnClickUpAddLitener(() => {
            if (onAction != null)
                onAction();

            OnTouchButtonEvent();
        });
    }


    public void OnTouchButtonEvent()
    { 
        switch (_type)
        {
            case ActionType.RadialProgress:
            {
                bool isProcess = _radialProgress.GetProgress();
                if (true == isProcess)
                {
                    _radialProgress.Cancle();
                    return;
                }

                _radialProgress.Interaction();
                break;
            }
            case ActionType.CoolTime:
            {
                break;
            }
            default:
            {
                break;    
            }
        }
    }

    public void SetType(ActionType argType) => _type = argType;
    public IngameUIButtonBase GetButton() => _button;
}
