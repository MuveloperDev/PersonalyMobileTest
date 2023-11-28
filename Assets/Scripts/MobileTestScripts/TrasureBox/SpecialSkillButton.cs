using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkillButton : SkillActionButtonBase
{
    private void Awake()
    {
        Initialize();
    }
    public void Initialize(/*����ü ������ �����͸� �޾ƿ´�.*/)
    {
        // ������ �����͸� �޾ƿ;��Ѵ�.
        base.Initialize(InputType.DragReleaseTouch);
    }

    #region [GET FUNCTION]
    public override Vector2 GetJoystickVector2()
    => base.GetJoystickVector2();

    public override Vector2 GetCorrectedDeadZoneVector2()
    => base.GetCorrectedDeadZoneVector2();

    public override void ResetCoolTime()
    {
        base.ResetCoolTime();
    }
    public override void DecreaseCoolTime()
    {
        base.DecreaseCoolTime();
    }
    #endregion

}
