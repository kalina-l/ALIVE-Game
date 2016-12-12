using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class DragItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragItemContainer itemBeingDragged;
    public static bool itemIsDragged;

    private DragItemContainer _item;

    private Vector3 _startPos;

    public void Setup(DragItemContainer item)
    {
        _item = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = _item;
        itemIsDragged = true;
        _item.Drop.SetDropZone(true);
        _startPos = transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        _item.Dragging();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemIsDragged)
        {
            _item.DropItem();
        }

        itemBeingDragged = null;
        itemIsDragged = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        transform.position = _startPos;

        _item.Drop.SetDropZone(false);

        Debug.Log("EndDrag");
    }
}
