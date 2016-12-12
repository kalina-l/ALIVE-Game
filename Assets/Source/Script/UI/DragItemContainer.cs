using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DragItemContainer : AbstractViewController {

    private Item _item;
    private ItemBoxViewController _parentViewController;

    private Image _itemImage;

    public DropItemHandler Drop;

    private bool _given;

    private Personality _personality;

	public DragItemContainer(RectTransform container, Item item, DropItemHandler drop, ItemBoxViewController parentViewController, Personality personality)
    {
        Rect = container;
        View = container.gameObject;

        _item = item;
        _parentViewController = parentViewController;
        _personality = personality;

        Drop = drop;

        _itemImage = AddSprite(container, GraphicsHelper.Instance.radialSliderSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        View.AddComponent<CanvasGroup>();
        View.AddComponent<DragItemHandler>().Setup(this);
    }

    public void ShowItem(bool show, float t)
    {
        Color showColor = GraphicsHelper.Instance.SpriteColorWhite;
        _itemImage.raycastTarget = true;

        if(_personality.Items.ContainsKey(_item.ID))
        {
            showColor = new Color(1, 1, 1, 0.5f);
            _itemImage.raycastTarget = false;
        }

        if (show)
        {
            _given = false;
            _itemImage.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, showColor, t);
            _itemImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, Vector2.one * 64, t);
        }
        else
        {
            if (!_given)
            {
                _itemImage.color = GraphicsHelper.Instance.LerpColor(showColor, GraphicsHelper.Instance.SpriteColorWhiteHidden, t);
                _itemImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.one * 64, Vector2.zero, t);
            }
        }
    }

    public void GiveItem()
    {
        Debug.Log("Give Item!");

        _personality.AddItem(_item.ID, _item);

        _given = true;
        _itemImage.rectTransform.sizeDelta = Vector2.zero;
        _itemImage.color = GraphicsHelper.Instance.SpriteColorWhiteHidden;
        _itemImage.raycastTarget = false;

        _parentViewController.AddItemToSlot(_itemImage.sprite, this);
        _parentViewController.ToggleBox();
    }

    public void RemoveItem()
    {
        _personality.RemoveItem(_item.ID);
    }

    public void Dragging()
    {
        _itemImage.rectTransform.sizeDelta = Vector2.Lerp(_itemImage.rectTransform.sizeDelta, Vector2.one * 128, 0.3f);
    }

    public void DropItem()
    {
        _itemImage.rectTransform.sizeDelta = Vector2.one * 64;
    }
}
