using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.EventSystems;

public class IngameUIToolTipBox : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _textRects;
    [SerializeField] private RectTransform _textRect; 
    [SerializeField] private RectTransform _bgRect; 
    [SerializeField] private RectTransform _rect; 
    [SerializeField] private Image _bgImage; 
    [SerializeField] private VerticalLayoutGroup _layoutGroup; 

    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private TextMeshProUGUI _textCoolTime;
    [SerializeField] private TextMeshProUGUI _textDescription;

    [SerializeField] private float _widthAlign = 0;
    string title = "TTTTITLE";
    string _coolTime = "86";
    string _description = "{EnemyHit:0} / {EnemyExplo:0} / {AllyHit:0} / {AllyExplo:0}" +
        "{EnemyHitStatusD:0} / {EnemyHitStatusTick:0} / {EnemyHitStatusV:0,0}" +
        "{EnemyExploStatusD:0} / {EnemyExploStatusTick:0} / {EnemyExploStatus:0,0}" +
        "{SelfStatusD:0} / {SelfStatusTick:0} / {SelfStatus:0,0}" +
        "{AllyHitDuration:0} / {AllyHitTickTime:0} / {AllyHitStatus:0,0}" +
        "{AllyExploDuration:0} / {AllyExploTickTime:0} / {AllyExploStatus:0,0}";

    string pattern = @"\{(.*?)\}";

    //private void OnValidate()
    //{
    //    OnShow();
    //}

    void Start()
    {
        _textRects = GetComponentsInChildren<RectTransform>(true).ToList();
        _rect = GetComponent<RectTransform>();
        _textTitle.text = title;
        _textCoolTime.text = _coolTime;
        _textDescription.text = _description;
        OnHide();
    }
    private void Update()
    {

    }
    public void OnShow(PointerEventData eventData)
    {
        // 위치 세팅
        if (null != eventData)
        {
            Vector2 temp = new Vector2(0, 60) + eventData.position;
            _rect.position = temp;
        }
        // text세팅

        // description 및 BG size 세팅
        _textRect.sizeDelta = new Vector2(_textRect.sizeDelta.x + _widthAlign, _textDescription.preferredHeight);

        float totalHeight = 0;
        for (int i = 1; i < _textRects.Count; i++)
        {
            totalHeight += _textRects[i].sizeDelta.y;
        }
 

        totalHeight += _layoutGroup.padding.top * 4;

        _bgRect.sizeDelta = new Vector2((_textRect.sizeDelta.x + _widthAlign), totalHeight);
        ReconstructionText();
        gameObject.SetActive(true);
    }

    public void OnHide()
    {
        gameObject.SetActive(false);
    }

    // 위치 조정.
    public void SetPosition(PointerEventData eventData)
    { 
        GetComponent<RectTransform>().pivot = new Vector2(1, 0);
        GetComponent<RectTransform>().position = new Vector2(eventData.position.x - 50, eventData.position.y + 50);
    }

    public void SetTextData(string argTitle, string argCoolTime, string argDescription)
    {
        _textTitle.text = argTitle;
        _textCoolTime.text = argCoolTime;
        _textDescription.text = argDescription;
    }

    private void ReconstructionText()
    {
        // 텍스트 색상을 바꾸는 코드
        MatchCollection matches = Regex.Matches(_textDescription.text, pattern);
        List<string> cachedStrings = new List<string>();

        foreach (Match match in matches)
        {
            cachedStrings.Add(match.Groups[0].Value);
        }

        foreach (string str in cachedStrings)
        {
            // 여기서 타입을 구분하여 텍스트와 색상을 변경하면 된다.
            // str을 원하는 스트링으로 변경하면 된다.
            string coloredStr = string.Empty;
            switch (str)
            {
                case "{EnemyHit:0}":
                case "{EnemyExplo:0}":
                case "{AllyHit:0}":
                case "{AllyExplo:0}":
                    coloredStr = $"<color=red>{str}</color>";
                    break;
                case "{EnemyHitStatusD:0}":
                case "{EnemyHitStatusTick:0}":
                case "{EnemyHitStatusV:0,0}":
                    coloredStr = $"<color=yellow>{str}</color>";
                    break;
                case "{EnemyExploStatusD:0}":
                case "{EnemyExploStatusTick:0}":
                case "{EnemyExploStatus:0,0}":
                    coloredStr = $"<color=blue>{str}</color>";
                    break;
                case "{SelfStatusD:0}":
                case "{SelfStatusTick:0}":
                case "{SelfStatus:0,0}":
                    coloredStr = $"<color=grey>{str}</color>";
                    break;
                case "{AllyHitDuration:0}":
                case "{AllyHitTickTime:0}":
                case "{AllyHitStatus:0,0}":
                    coloredStr = $"<color=green>{str}</color>";
                    break;
                case "{AllyExploDuration:0}":
                case "{AllyExploTickTime:0}":
                case "{AllyExploStatus:0,0}":
                    coloredStr = $"<color=black>{str}</color>";
                    break;
            }
            // 여기서 타입을 구분하여 텍스트와 색상을 변경하면 된다.
            _textDescription.text = _textDescription.text.Replace(str, coloredStr);
        }
    }
}
