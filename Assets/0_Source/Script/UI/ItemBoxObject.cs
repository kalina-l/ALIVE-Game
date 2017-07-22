using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxObject : MonoBehaviour {

    private ItemBoxViewController _controller;

	public void Setup(ItemBoxViewController controller)
    {
        _controller = controller;
    }

    void OnMouseUpAsButton()
    {
        if (!_controller.IsAnimating)
        {
            _controller.RemoveItemFromSlot();
        }
    }
}
