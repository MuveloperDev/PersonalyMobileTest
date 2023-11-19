using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IngameUIButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum InGameButtonMode
    {
        None = -1,
        Drag,
        DragAndDrop,
        JoystickAndDragDrop,
        Customize   // ���� ��������.
    }
    [Header("MODE")]
    [SerializeField] private InGameButtonMode _mode;

    [Header("DEPENDENCY INFORMATION")]
    [SerializeField] private Button button;
    [SerializeField] private IngameDragAndDropButton _dragAndDropDestBtn;
    [SerializeField] private RectTransform _dragAndDropDestBtnRect;
    [SerializeField] private Image _dragGhostImage;
    [SerializeField] private IngameVirtualJoystickBase _joystick;

    [Header("FLAGS")]
    [SerializeField] private bool _isArriveAtTheDest = false;

    #region [Button_Interaction_Handler]
    private Action _onClickDownEvent;
    public void OnClickDownAddLitener(Action argEvent) => _onClickDownEvent += argEvent;

    private Action _onClickUpEvent;
    public void OnClickUpAddLitener(Action argEvent) => _onClickUpEvent += argEvent;

    private Action<PointerEventData> _onClickDragEvent;
    public void OnClickDragAddLitener(Action<PointerEventData> argEvent) => _onClickDragEvent += argEvent;

    private Vector2 _dragPos;
    public Vector2 dragPos { get { return _dragPos; } private set { } }
    #endregion

    void Awake()
    {
        button = GetComponent<Button>();
        // Ÿ�� ��ư�� ���߿� �Ŵ����޿��� ĳ���ؿð�. (�׽�Ʈ��)
        _mode = InGameButtonMode.None;
        SetDragAndDropDestButton(FindObjectOfType<IngameDragAndDropButton>());
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("TouchDown");
        // ��ġ �ٿ� �̺�Ʈ ó�� �ڵ�
        if (null != _onClickDownEvent)
            _onClickDownEvent();

        OnClickDownEventBasedOnMode(eventData);

        if (true == _isArriveAtTheDest)
            _isArriveAtTheDest = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ��ġ �� �̺�Ʈ ó�� �ڵ�
        if (null != _onClickUpEvent && false == _isArriveAtTheDest)
            _onClickUpEvent();

        Debug.Log("TouchUp");
        OnClickUpEventBasedOnMode();
        _dragPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (null != _onClickDragEvent)
            _onClickDragEvent(eventData);

        _dragPos = eventData.position;
        OnDragEventBasedOnMode(eventData);
    }

    public void SetMode(InGameButtonMode argMode) => _mode = argMode;

    public void SetDragAndDropDestButton(IngameDragAndDropButton argCancleBtn)
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
                ActiveDragGhost();
                break;
            case InGameButtonMode.JoystickAndDragDrop:
                ActiveDragGhost();
                _joystick.OnShow(eventData);
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
                OnPointerUpEventDragGhost();
                break;
            case InGameButtonMode.JoystickAndDragDrop:
                OnPointerUpEventDragGhost();
                _joystick.OnHide();
                break;
            case InGameButtonMode.Customize:
                break;
            default:
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
                UpdateGhostDragImagePosition();
                DetectRollOverDragAndDropDestButton();
                break;
            case InGameButtonMode.JoystickAndDragDrop:
                UpdateGhostDragImagePosition();
                DetectRollOverDragAndDropDestButton();
                _joystick.HandleJoystickInput(eventData);
                break;
            case InGameButtonMode.Customize:
                break;
            default:
                break;
        }
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
        if (null != _dragAndDropDestBtn && true == _isArriveAtTheDest)
        {
            // �� ĵ�� �κ��� ���ʴ��̶� �̾߱� �غ���.
            // �ϴ��� ĵ����ư�� ��������Ʈ�� ���ξ����� ��뼺�� ���� �����غ���.
            _dragAndDropDestBtn.OnPointerExit();
        }
        DisableGhostDragImage();
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
        }
        else
        {
            if (true == _isArriveAtTheDest)
                _isArriveAtTheDest = false;
        }
    }

    private void InstatiateGhost()
    {
        // ��巹����� ��ü����
        _dragGhostImage = Instantiate(GetComponent<Image>(), transform.parent);
        var button = _dragGhostImage.GetComponent<IngameUIButtonBase>();
        button.enabled = false;
        _dragGhostImage.color = new Color(1, 1, 1, 0.5f);
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

}
