using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTweenFadeInOut : MonoBehaviour
{

    public CanvasGroup group;
    public float duration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            group.DOFade(1f, duration);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            group.DOFade(0f, duration);
        }
    }
}
