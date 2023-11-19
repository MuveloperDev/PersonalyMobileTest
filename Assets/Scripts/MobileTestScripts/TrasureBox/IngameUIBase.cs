using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUIBase : MonoBehaviour
{
    //[SerializeField] protected float _duration;
    //[SerializeField] protected CanvasGroup _group;
    public virtual void OnShow()
    {
        if (true == gameObject.activeSelf)
            return;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        //FadeInOut(true);
    }

    public virtual void OnHide()
    {
        if (false == gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        //FadeInOut(false);
    }

    //private void FadeInOut(bool isFadeIn)
    //{
    //    if (true == isFadeIn)
    //    {
    //        _group.DOFade(1f, _duration);
    //    }
    //}

}
