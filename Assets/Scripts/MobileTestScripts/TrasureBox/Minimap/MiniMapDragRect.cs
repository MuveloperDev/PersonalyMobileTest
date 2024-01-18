using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MiniMapDragRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private CameraMove cm;
    [SerializeField] private Image myImage;
    [SerializeField] private RectTransform dragRect;
    [SerializeField] private RectTransform cmViewUI;
    [SerializeField] private Vector2 miniMapSize;
    [SerializeField] private Vector2 worldMapSize = new Vector2(60, 60);
    Vector2 offset = new Vector2(410, 410);

    [SerializeField] private Vector2 _curCharacterPosition { 
        get { 
            return cm.transform.position;
        } 
    }
    void Start()
    {
        myImage = GetComponent<Image>();
        miniMapSize = dragRect.sizeDelta;
        worldMapSize = new Vector2(820, 820);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 rectPos = ScreenPointToLocalPointInRectangle(eventData);
        Debug.Log($"OnBeginDrag : {rectPos.x} / {rectPos.y}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 rectPos = ScreenPointToLocalPointInRectangle(eventData);
        Debug.Log($"OnDrag : {rectPos.x} / {rectPos.y}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    private Vector2 ScreenPointToLocalPointInRectangle(PointerEventData eventData)
    {
        Vector2 rectPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect, eventData.position, eventData.pressEventCamera, out rectPosition))
        {
            return rectPosition;
        }
        return Vector2.zero;
    }
}
