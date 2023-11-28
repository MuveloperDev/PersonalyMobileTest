using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SkillActionButtonBase;

public class ActionButtonManager : MonoBehaviour
{
    public enum ActionButtons
    {
        BasicSkillButton = 0,
        SpecialSkillButton,
        UltimateSkillButton,
    }
    [SerializeField] private List<SkillActionButtonBase> _actionBtnList;

    private Dictionary<ActionButtons, SkillActionButtonBase> _actionBtnDic;
    // Start is called before the first frame update
    void Awake()
    {
        _actionBtnDic = new();
        for (int i = 0; i < _actionBtnList.Count; i++)
        {
            _actionBtnDic.Add((ActionButtons)i, _actionBtnList[i]);
            //_actionBtnList[i].Initialize(InputType.DoubleTouch);
        }
        
    }

    public Dictionary<ActionButtons, SkillActionButtonBase> GetBtnDic() => _actionBtnDic;
    
}
