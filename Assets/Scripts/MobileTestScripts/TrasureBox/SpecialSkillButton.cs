using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkillButton : SkillActionButtonBase
{
    private void Awake()
    {
        Initialize();
    }
    public void Initialize(/*구조체 형식의 데이터를 받아온다.*/)
    {
        // 뭔가의 데이터를 받아와야한다.
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
