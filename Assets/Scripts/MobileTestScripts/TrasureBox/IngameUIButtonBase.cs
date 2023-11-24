using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IngameUIButtonBase : IngameUIEventHandler
{
    public enum InGameButtonMode
    {
        None = -1,
        Drag,
        DragAndDrop,
        Customize   // 추후 구현예정.
    }
    [Header("MODE")]
    [SerializeField] private InGameButtonMode _mode;

    [Header("DEPENDENCY INFORMATION")]
    [SerializeField] private Button button;
    [SerializeField] private IngameUIDragAndDropDestination _dragAndDropDestBtn;
    [SerializeField] private RectTransform _dragAndDropDestBtnRect;
    [SerializeField] private Image _dragGhostImage;

    [Header("Flags")]
    [SerializeField] private bool _useGhostImage;


    void Awake()
    {
        button = GetComponent<Button>();
        // 타겟 버튼은 나중에 매니저급에서 캐싱해올것. (테스트용)
        _mode = InGameButtonMode.None;
        if (_mode == InGameButtonMode.DragAndDrop)
        {
            SetDragAndDropDestButton(FindObjectOfType<IngameUIDragAndDropDestination>());
        }
    }

    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnClickDownEventBasedOnMode(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _dragPos = transform.position;
        OnClickUpEventBasedOnMode();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        _dragPos = eventData.position;
        OnDragEventBasedOnMode(eventData);
    }

    public void SetMode(InGameButtonMode argMode) => _mode = argMode;

    public void SetDragAndDropDestButton(IngameUIDragAndDropDestination argCancleBtn)
    { 
        // 드래그앤드랍 사용시 필수적으로 호출해야하는 함수.
        _dragAndDropDestBtn = argCancleBtn;
        _dragAndDropDestBtnRect = argCancleBtn.GetComponent<RectTransform>();
    }

    private void OnClickDownEventBasedOnMode(PointerEventData eventData)
    {
        switch (_mode)
        {
            case InGameButtonMode.None:
                break;
            case InGameButtonMode.Drag:
                break;
            case InGameButtonMode.DragAndDrop:
                if (true == _useGhostImage)
                    ActiveDragGhost();
                break;
            case InGameButtonMode.Customize:
                break;
            default:
                break;
        }
    }

    private void OnClickUpEventBasedOnMode()
    {
        switch (_mode)
        {
            case InGameButtonMode.None:
                break;
            case InGameButtonMode.Drag:
                break;
            case InGameButtonMode.DragAndDrop:
                if (null != _dragAndDropDestBtn && true == _isArriveAtTheDest)
                    _dragAndDropDestBtn.OnPointerExit();

                if (true == _useGhostImage)
                    OnPointerUpEventDragGhost();
                break;
            case InGameButtonMode.Customize:
                break;
        }
    }

    private void OnDragEventBasedOnMode(PointerEventData eventData)
    {
        switch (_mode)
        {
            case InGameButtonMode.None:
                break;
            case InGameButtonMode.Drag:
                break;
            case InGameButtonMode.DragAndDrop:
                DetectRollOverDragAndDropDestButton();

                if (true == _useGhostImage)
                    UpdateGhostDragImagePosition();
                break;
            case InGameButtonMode.Customize:
                break;
        }
    }


    private void DetectRollOverDragAndDropDestButton()
    {
        if (_dragAndDropDestBtn == null)
        {
            Debug.LogError("NullException : _dragAndDropDestBtn is null...");
            return;
        }

        if (true == RectTransformUtility.RectangleContainsScreenPoint(_dragAndDropDestBtnRect, Input.mousePosition))
        {
            // 드랍 이벤트 한번만 호출한다.
            if (true == _isArriveAtTheDest)
                return;

            _isArriveAtTheDest = true;
            _dragAndDropDestBtn.OnPointerEnter();
            return;
        }

        if (true == _isArriveAtTheDest)
            _isArriveAtTheDest = false;
    }


    #region [Ghost_Function]
    private void InstatiateGhost()
    {
        // 어드레서블로 교체예정
        _dragGhostImage = Instantiate(GetComponent<Image>(), transform.parent);
        var button = _dragGhostImage.GetComponent<IngameUIButtonBase>();
        button.enabled = false;
        _dragGhostImage.color = new Color(1, 1, 1, 0.5f);
    }
    private void ActiveDragGhost()
    {
        if (null == _dragGhostImage)
        {
            InstatiateGhost();
        }
        else
        {
            _dragGhostImage.gameObject.SetActive(true);
        }
    }

    private void OnPointerUpEventDragGhost()
    {
        DisableGhostDragImage();
    }

    private void DisableGhostDragImage()
    {
        if (_dragGhostImage == null)
            return;

        _dragGhostImage.transform.position = transform.position;
        _dragGhostImage.gameObject.SetActive(false);
    }

    private void UpdateGhostDragImagePosition()
    {
        if (_dragGhostImage == null)
            return;

        _dragGhostImage.transform.position = Input.mousePosition;
    }
    #endregion
}
