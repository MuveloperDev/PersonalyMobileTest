using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIStatusEffect : MonoBehaviour
{
    [Header("DEPENDENCY")]
    [SerializeField] private IngameUIButtonBase _button;
    [SerializeField] private Image _dimd;
    //[SerializeField] private IngameCoolTIme 
    [SerializeField] private RadialProgress _radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShow()
    { 
    }
    public void OnHide()
    {

    }
}
