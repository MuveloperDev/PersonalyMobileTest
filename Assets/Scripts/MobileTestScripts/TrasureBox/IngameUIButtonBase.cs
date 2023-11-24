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
        Customize   // ���� ��������.
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
        // Ÿ�� ��ư�� ���߿� �Ŵ����޿��� ĳ���ؿð�. (�׽�Ʈ��)
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
        // �巡�׾ص�� ���� �ʼ������� ȣ���ؾ��ϴ� �Լ�.
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
            // ��� �̺�Ʈ �ѹ��� ȣ���Ѵ�.
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
        // ��巹����� ��ü����
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
