using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemBoxViewController : AbstractViewController
{
    private Image _boxImage;
    private bool _isOpen;

    private List<DragItemContainer> _itemList;

    private bool _animating;
    private DropItemHandler _dropZone;

    private Image _itemSlotImage;
    private DragItemContainer _itemInSlot;
    private bool _animateSlot;

	public ItemBoxViewController(Transform parent, Dictionary<int, Item> items, Personality personality)
    {
        Rect = CreateContainer("ItemBox", parent,
            new Vector2(-20, 20), new Vector2(256, 256),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));

        View = Rect.gameObject;

        //ItemSlot
        Image slot = AddSprite(
            CreateContainer("ItemSlot", parent,
                new Vector2(-20, -20), new Vector2(256, 256),
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1)),
            GraphicsHelper.Instance.itemSlotSprite,
            GraphicsHelper.Instance.SpriteColorWhite);

        CreateButton(slot.rectTransform, delegate { RemoveItemFromSlot(); });

        _itemSlotImage = AddSprite(
            CreateContainer("ItemInSlotIcon", slot.rectTransform,
                new Vector2(0, 0), new Vector2(128, 128),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.itemSlotSprite,
            GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _itemSlotImage.raycastTarget = false;

        _dropZone = CreateContainer("ItemDrop", parent,
            new Vector2(-20, -20), new Vector2(1040, 1300),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1)).gameObject.AddComponent<DropItemHandler>();

        _boxImage = AddSprite(Rect, GraphicsHelper.Instance.itemboxClosedSprite, GraphicsHelper.Instance.SpriteColorWhite);

        CreateButton(Rect, delegate { ToggleBox(); }).transition = Selectable.Transition.None;

        _itemList = new List<DragItemContainer>();

        int startRow = 4;
        float angleRange = 0.3f;
        
        int rowIndex = 0;
        int colIndex = 0;
        
        foreach(KeyValuePair<int, Item> kvp in items)
        {
            float angleDistance = angleRange / (startRow + rowIndex - 1);
            float distance = 168f + (120 * rowIndex);

            Vector2 radPoint = Vector2.zero;

            float angle = Mathf.Lerp(0, 2 * Mathf.PI, (angleDistance * colIndex) + 0.225f);

            radPoint.y = (int)Mathf.Round(distance * Mathf.Sin(angle));
            radPoint.x = (int)Mathf.Round(distance * Mathf.Cos(angle));

            RectTransform radRect = CreateContainer("Item_" + kvp.Value.Name, Rect,
            radPoint, new Vector2(0, 0),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            
            _itemList.Add(new DragItemContainer(radRect, kvp.Value, _dropZone, this, personality));
            
            colIndex++;

            if(colIndex >= startRow+rowIndex)
            {
                colIndex = 0;
                rowIndex++;
            }
        }

        

        _isOpen = false;
    }

    public void ToggleBox()
    {
        if (!_animating)
        {
            _isOpen = !_isOpen;


            ApplicationManager.Instance.StartCoroutine(ShowItems(_isOpen));

            if (_isOpen)
            {
                _boxImage.sprite = GraphicsHelper.Instance.itemboxOpenSprite;
            }
            else
            {
                _boxImage.sprite = GraphicsHelper.Instance.itemboxClosedSprite;
            }
        }
    }

    public void AddItemToSlot(Sprite icon, DragItemContainer itemInSlot)
    {
        _itemInSlot = itemInSlot;

        _itemSlotImage.sprite = icon;
        _itemSlotImage.color = GraphicsHelper.Instance.SpriteColorWhiteHidden;
        _itemSlotImage.rectTransform.sizeDelta = Vector2.zero;

        ApplicationManager.Instance.StartCoroutine(ShowSlotItem(true));
    }

    public void RemoveItemFromSlot()
    {
        _itemInSlot.RemoveItem();
        ApplicationManager.Instance.StartCoroutine(ShowSlotItem(false));
    }

    private IEnumerator ShowSlotItem(bool show)
    {
        float timer = 0;

        while (_animateSlot)
            yield return 0;

        _animateSlot = true;

        while(timer < 1)
        {
            timer += Time.deltaTime * 2;
            if (show)
            {
                _itemSlotImage.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, timer);
                _itemSlotImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, Vector2.one * 128, timer);
            }
            else
            {
                _itemSlotImage.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, timer);
                _itemSlotImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.one * 128, Vector2.zero, timer);
            }


            yield return 0;
        }

        if(!show)
        {
            if (_isOpen)
                _itemInSlot.ShowItem(true, 1);

            _itemInSlot = null;
        }

        _animateSlot = false;
    }

    private IEnumerator ShowItems(bool show)
    {
        float timer = 0;

        _animating = true;

        while(timer < (1 + (0.2f * _itemList.Count)))
        {
            timer += Time.deltaTime * 2;

            if(show)
            {
                for(int i=0; i<_itemList.Count; i++)
                {
                    float t = Mathf.Clamp(timer - (0.2f * i), 0, 1);

                    _itemList[i].ShowItem(show, t);
                }
            }
            else
            {
                for (int i = _itemList.Count-1; i >= 0; i--)
                {
                    float t = Mathf.Clamp(timer - (0.2f * ((_itemList.Count - 1) - i)), 0, 1);

                    _itemList[i].ShowItem(show, t);
                }
            }

            yield return 0;
        }

        _animating = false;
    }
}
