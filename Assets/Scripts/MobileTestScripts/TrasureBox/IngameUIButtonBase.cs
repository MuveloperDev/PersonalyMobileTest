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
        Customize   // 추후 구현예정.
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
        // 타겟 버튼은 나중에 매니저급에서 캐싱해올것. (테스트용)
        _mode = InGameButtonMode.None;
        SetDragAndDropDestButton(FindObjectOfType<IngameDragAndDropButton>());
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("TouchDown");
        // 터치 다운 이벤트 처리 코드
        if (null != _onClickDownEvent)
            _onClickDownEvent();

        OnClickDownEventBasedOnMode(eventData);

        if (true == _isArriveAtTheDest)
            _isArriveAtTheDest = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 터치 업 이벤트 처리 코드
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
            // 이 캔슬 부분은 동필님이랑 이야기 해볼것.
            // 일단은 캔슬버튼에 델리게이트로 빼두었지만 사용성을 위해 생각해볼것.
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
            // 드랍 이벤트 한번만 호출한다.
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
        // 어드레서블로 교체예정
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
