using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class IngameUIEventHandler :MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    protected Action<PointerEventData> _onClickDownEvent;
    protected Action<PointerEventData> _onClickUpEvent;
    protected Action<PointerEventData> _onBeginDragEvent;
    protected Action<PointerEventData> _onDragEvent;
    protected Action<PointerEventData> _onEndDragEvent;

    public void OnClickDownAddLitener(Action<PointerEventData> argEvent) => _onClickDownEvent += argEvent;
    public void OnClickDownRemoveLitener(Action<PointerEventData> argEvent) => _onClickDownEvent -= argEvent;

    public void OnClickUpAddLitener(Action<PointerEventData> argEvent) => _onClickUpEvent += argEvent;
    public void OnClickUpRemoveLitener(Action<PointerEventData> argEvent) => _onClickUpEvent -= argEvent;

    public void OnBeginDragAddLitener(Action<PointerEventData> argEvent) => _onBeginDragEvent += argEvent;
    public void OnBeginDragRemoveLitener(Action<PointerEventData> argEvent) => _onBeginDragEvent -= argEvent;

    public void OnDragAddLitener(Action<PointerEventData> argEvent) => _onDragEvent += argEvent;
    public void OnDragRemoveLitener(Action<PointerEventData> argEvent) => _onDragEvent -= argEvent;

    public void OnEndDragAddLitener(Action<PointerEventData> argEvent) => _onEndDragEvent += argEvent;
    public void OnEndDragRemoveLitener(Action<PointerEventData> argEvent) => _onEndDragEvent -= argEvent;

    [SerializeField] public bool _isArriveAtTheDest = false;
    [SerializeField] public bool _isPressed = false;

    protected Vector2 _dragPos;
    public Vector2 dragPos { get { return _dragPos; } private set { } }

#pragma warning disable CS1998
    public async virtual void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;

        if (true == _isArriveAtTheDest)
            _isArriveAtTheDest = false;

        _onClickDownEvent?.Invoke(eventData);
    }

    public async virtual void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        if (true == _isArriveAtTheDest)
            return;

        _onClickUpEvent?.Invoke(eventData);
    }


    public async virtual void OnBeginDrag(PointerEventData eventData)
    {
        _onBeginDragEvent?.Invoke(eventData);
    }
    public async virtual void OnDrag(PointerEventData eventData)
    {
        _onDragEvent?.Invoke(eventData);
    }

    public async virtual void OnEndDrag(PointerEventData eventData)
    {
        _onEndDragEvent?.Invoke(eventData);
    }
}
