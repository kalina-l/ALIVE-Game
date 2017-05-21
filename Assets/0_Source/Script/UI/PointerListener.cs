using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private bool _pressed = false;
    private UnityEngine.Events.UnityAction _callOnPointerDown;
    private UnityEngine.Events.UnityAction _callOnPointerUp;

    public void AddOnDownDelegate(UnityEngine.Events.UnityAction call)
    {
        _callOnPointerDown = call;
    }
    public void AddOnUpDelegate(UnityEngine.Events.UnityAction call)
    {
        _callOnPointerUp = call;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _callOnPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        _callOnPointerUp();
    }
}
