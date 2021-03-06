﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class DropItemHandler : MonoBehaviour, IDropHandler
{
    private CanvasGroup _interaction;
    private Image img;

    void Awake()
    {
        _interaction = gameObject.AddComponent<CanvasGroup>();
        _interaction.blocksRaycasts = false;
        _interaction.interactable = false;

        img = gameObject.AddComponent<Image>();
        img.raycastTarget = false;
        img.color = GraphicsHelper.Instance.SpriteColorWhiteHidden; 
    }

    public void SetDropZone(bool activate)
    {
        _interaction.blocksRaycasts = activate;
        _interaction.interactable = activate;
        img.raycastTarget = activate;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragItemHandler.itemIsDragged)
        {
            DragItemHandler.itemBeingDragged.GiveItem();
            DragItemHandler.itemIsDragged = false;
        }
    }
}
