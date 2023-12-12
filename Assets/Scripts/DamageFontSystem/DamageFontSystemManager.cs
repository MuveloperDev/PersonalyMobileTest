using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DamageFontSystemManager : MonoBehaviour
{
    [SerializeField] private Transform _characterSpin;
    [SerializeField] private RectTransform _DamageFont;

    // 데미지폰트 스크립트에 들어가야함.
    [SerializeField] private Vector3 _characterSpinPos;
    private void Awake()
    {

    }

    void Start()
    {

        //Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.main, adjustPos);
        //_characterImage.GetComponent<RectTransform>().position = pos;
        //Vector2 canvasPos = new Vector2();
        //if (true == RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, pos, Camera.main, out canvasPos))
        //{
        //    // 좌측 하단을 원점으로 위치 계산
        //    canvasPos.x += _characterImage.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
        //    canvasPos.y += _characterImage.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        //    // imageRect의 위치 변경
        //    _characterImage.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        //    //_characterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasPos.x, canvasPos.y);

        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _characterSpinPos = _characterSpin.localPosition;
            Vector3 adjustPos = new Vector3(_characterSpinPos.x - 0.3f, _characterSpinPos.y - 0.2f);
            _DamageFont.gameObject.SetActive(true);
            _DamageFont.anchoredPosition = adjustPos;
        }
    }
}
