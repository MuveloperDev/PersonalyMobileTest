using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Minimap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CameraMove cm;
    [SerializeField] private Image myImage;
    [SerializeField] private RectTransform miniMapRect;
    [SerializeField] private RectTransform cmViewUI;
    [SerializeField] private Vector2 miniMapSize;
    [SerializeField] private Vector2 worldMapSize = new Vector2(60,60);
    // Start is called before the first frame update
    void Start()
    {
        myImage= GetComponent<Image>();
        miniMapSize = miniMapRect.sizeDelta;
        worldMapSize = new Vector2(820, 820);
    }

    // Pointer Down �̺�Ʈ �ݹ�
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 rectPosition = ScreenPointToLocalPointInRectangle(eventData);
        SetPositionCMView(rectPosition);
        cm.MoveCamera(ConvertMiniMapPositionToWorldPosition(rectPosition));

        var worldPosition = ScreenPointToLocalPointInRectangleToWorldPosition(eventData);
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.position = new Vector3(worldPosition.x, 1, worldPosition.y);
    }

    // Pointer Up �̺�Ʈ �ݹ�
    public void OnPointerUp(PointerEventData eventData)
    {
        // Pointer Up �̺�Ʈ�� �߻����� �� ����� �ڵ�
        //Debug.Log($"OnPointerUp : x - {eventData.position.x} / y - {eventData.position.y}");
        cm.MoveCamera(offset);
        Vector2 rectPosition = ConvertWorldPositionToMiniMapPosition(offset);
        SetPositionCMView(rectPosition);
    }

    // Drag �̺�Ʈ �ݹ�
    public void OnDrag(PointerEventData eventData)
    {
        // Drag �̺�Ʈ�� �߻����� �� ����� �ڵ�
        Vector2 rectPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, eventData.position, eventData.pressEventCamera, out rectPosition))
        {
            bool xExceeded = Mathf.Abs(rectPosition.x) > miniMapSize.x / 2;
            bool yExceeded = Mathf.Abs(rectPosition.y) > miniMapSize.y / 2;

            // X���� �ʰ��Ǿ��ٸ� Y���� ������Ʈ
            if (xExceeded)
            {
                rectPosition.x = Mathf.Clamp(rectPosition.x, -miniMapSize.x / 2, miniMapSize.x / 2);
            }

            // Y���� �ʰ��Ǿ��ٸ� X���� ������Ʈ
            if (yExceeded)
            {
                rectPosition.y = Mathf.Clamp(rectPosition.y, -miniMapSize.y / 2, miniMapSize.y / 2);
            }

            SetPositionCMView(rectPosition);
            cm.MoveCamera(ConvertMiniMapPositionToWorldPosition(rectPosition));
            Debug.Log($"OnDrag : x - {rectPosition.x} / y - {rectPosition.y}");
        }
    }

    // �̺κ��� ĳ���� ��ġ�� �����ؾ���.
    Vector2 offset = new Vector2(410, 410);
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag : x - {eventData.position.x} / y - {eventData.position.y}");
        cm.MoveCamera(offset);
        Vector2 rectPosition = ConvertWorldPositionToMiniMapPosition(offset);
        SetPositionCMView(rectPosition);
    }

    /// <summary>
    /// Convert ui position to world position
    /// </summary>
    /// <param name="uiPosition"></param>
    /// <returns></returns>
    private Vector2 ConvertMiniMapPositionToWorldPosition(Vector2 uiPosition)
    {
        // ���� ����� 0,0�϶��� ����
        //Vector2 uiRatio = new Vector2(uiPosition.x / miniMapSize.x, uiPosition.y / miniMapSize.y);
        //Vector2 worldPosition = new Vector2(uiRatio.x * worldMapSize.x, uiRatio.y * worldMapSize.y);
        //return worldPosition;

        // ���� �����ϴ��� 0,0�϶��� ����.
        Vector2 uiRatio = new Vector2(uiPosition.x / miniMapSize.x, uiPosition.y / miniMapSize.y);
        Vector2 worldPosition = new Vector2(uiRatio.x * worldMapSize.x + (worldMapSize.x * 0.5f),
                                            uiRatio.y * worldMapSize.y + (worldMapSize.y * 0.5f));
        return worldPosition;
    }

    /// <summary>
    /// Convert world position to ui position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    private Vector2 ConvertWorldPositionToMiniMapPosition(Vector2 worldPosition)
    {
        Vector2 worldRatio = new Vector2((worldPosition.x - (worldMapSize.x * 0.5f)) / worldMapSize.x,
                                 (worldPosition.y - (worldMapSize.y * 0.5f)) / worldMapSize.y);

        Vector2 uiPosition = new Vector2(worldRatio.x * miniMapSize.x, worldRatio.y * miniMapSize.y);
        return uiPosition;
    }

    /// <summary>
    /// UIPostionToWorldPostion
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    private Vector2 ScreenPointToLocalPointInRectangleToWorldPosition(PointerEventData eventData)
    {
        Vector2 rectPos;
        Vector2 worldPosition = Vector2.one;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, eventData.position, eventData.pressEventCamera, out rectPos))
        {
            worldPosition = ConvertMiniMapPositionToWorldPosition(rectPos);
        }
        return worldPosition;
    }

    private Vector2 ScreenPointToLocalPointInRectangle(PointerEventData eventData)
    {
        Vector2 rectPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, eventData.position, eventData.pressEventCamera, out rectPosition))
        {
            return rectPosition;
        }
        return Vector2.zero;
    }

    #region [ Camera View Rect Functions ]
    private void SetPositionCMView(Vector2 rectPosition)
    {
        Vector2 adjustPos = rectPosition;
        // �ƽ� ���� ����.
        //adjustPos.x = Mathf.Clamp(localPosition.x, (-miniMapSize.x / 2) + (cmViewUI.sizeDelta.x / 2), (miniMapSize.x / 2) - (cmViewUI.sizeDelta.x / 2));
        //adjustPos.y = Mathf.Clamp(localPosition.y, (-miniMapSize.y / 2) + (cmViewUI.sizeDelta.y / 2), (miniMapSize.y / 2) - (cmViewUI.sizeDelta.y / 2));
        cmViewUI.anchoredPosition = adjustPos;
    }
    public void SetPositionCMViewToWorldPosition(Vector2 worldPosition)
    {
        Vector2 rectPos = ConvertWorldPositionToMiniMapPosition(worldPosition);
        cmViewUI.anchoredPosition = rectPos;
    }
    #endregion
}
