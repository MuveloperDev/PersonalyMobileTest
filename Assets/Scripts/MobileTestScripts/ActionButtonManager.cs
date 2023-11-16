using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonManager : MonoBehaviour
{
    public enum ActionButtons
    {
        NormalAttack = 0,
        Skill1,
        Skill2,
        Skill3
    }
    [SerializeField] private List<ActionButton> _actionBtnList;

    private Dictionary<ActionButtons, ActionButton> _actionBtnDic;
    // Start is called before the first frame update
    void Awake()
    {
        _actionBtnDic = new();
        for (int i = 0; i < _actionBtnList.Count; i++)
        {
            _actionBtnDic.Add((ActionButtons)i, _actionBtnList[i]);
        }
    }

    public Dictionary<ActionButtons, ActionButton> GetBtnDic() => _actionBtnDic;
    
}
