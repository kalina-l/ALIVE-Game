using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemBoxViewController : AbstractViewController
{
    private Image _boxImage;
    private Image _background;
    private bool _isOpen;

    private List<DragItemContainer> _itemList;

    private bool _animating;
    private DropItemHandler _dropZone;
    
    private DragItemContainer _itemInSlot;

    private GameObject _itemObject;
    private GameObject _remoteItemObject;


    public ItemBoxViewController(Transform parent, List<Item> itemList, Personality personality)
    {
        Dictionary<int, Item> items = new Dictionary<int, Item>();

        foreach (Item item in itemList)
        {
            items[item.ID] = item;
        }

        Rect = CreateContainer("ItemBox", parent,
            new Vector2(-190, 190), new Vector2(340, 340),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f));

        View = Rect.gameObject;
        

        _dropZone = CreateContainer("ItemDrop", parent,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f)).gameObject.AddComponent<DropItemHandler>();

        _boxImage = AddSprite(Rect, GraphicsHelper.Instance.itemboxClosedSprite, GraphicsHelper.Instance.SpriteColorWhite);

        CreateButton(Rect, delegate { ToggleBox(); }).transition = Selectable.Transition.None;
        
        _background = AddSprite(
                            CreateContainer("ItemBackground", parent,
                                new Vector2(-720, 190), new Vector2(640, 170),
                                new Vector2(1, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.itemBackgroundSprite,
                            GraphicsHelper.Instance.SpriteColorWhiteHidden);


        _itemList = new List<DragItemContainer>();

        int startRow = 4;
        
        int rowIndex = 0;
        int colIndex = 0;
        
        foreach(KeyValuePair<int, Item> kvp in items)
        {

            Vector2 radPoint = Vector2.zero;

            radPoint.x = 120 - (240 * colIndex);

            RectTransform radRect = CreateContainer("Item_" + kvp.Value.Name, _background.GetComponent<RectTransform>(),
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
            
            ApplicationManager.Instance.StartCoroutine(AnimateBox(_isOpen));
        }
    }

    public Sprite GetItemIcon(Item item)
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].ItemID == item.ID)
            {
                return _itemList[i].Icon;
            }
        }

        return null;
    }

    public void AddItemFromPersonality(Item item)
    {
        for(int i=0; i<_itemList.Count; i++)
        {
            if(_itemList[i].ItemID == item.ID)
            {
                Debug.Log("Add Item");
                AddItemToSlot(_itemList[i].Icon, _itemList[i]);
                return;
            }
        }
    }

    public void AddItemToSlot(Sprite icon, DragItemContainer itemInSlot)
    {
        if(_itemInSlot != null)
        {
            GameObject.Destroy(_itemObject);

            if(_itemInSlot.ItemID != itemInSlot.ItemID)
            {
                _itemInSlot.RemoveItem();
            }
        }

        _itemInSlot = itemInSlot;

        ShowItemObject();
        
    }

    public void RemoveItemFromSlot()
    {
        if(_itemInSlot != null)
        {
            _itemInSlot.RemoveItem();

            GameObject.Destroy(_itemObject);
            
        }
    }

    public void UpdateBox(Personality personality)
    {
        if (_itemInSlot != null)
        {
            Item item = personality.GetItem(_itemInSlot.ItemID);

            if (item == null)
            {
                _itemInSlot.RemoveItem();
                _itemInSlot = null;
            }
            else
            {
                if (item.uses >= item.maxUses)
                {
                    RemoveItemFromSlot();
                }
            }
        }
    }

    private void ShowItemObject()
    {
        DebugController.Instance.Log("Add item to slot", DebugController.DebugType.UI);

        Transform parent = GraphicsHelper.Instance.itemAnchor;

        _itemObject = GameObject.Instantiate(GraphicsHelper.Instance.GetItemObject(_itemInSlot.ItemName), parent);

        _itemObject.GetComponent<ItemBoxObject>().Setup(this);
    }

    private IEnumerator AnimateBox(bool show)
    {
        _animating = true;

        float timer = 0;

        Vector2 fullSize = new Vector2(340, 340);
        Vector2 smallSize = new Vector2(310, 310);

        Vector2 bgPositionIn = new Vector2(-200, 190);
        Vector2 bgPositionOut = new Vector2(-710, 190);

        Vector2 bgFullSize = new Vector2(640, 170);

        RectTransform bgRect = _background.GetComponent<RectTransform>();

        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

        while (timer < 1)
        {
            timer += Time.deltaTime * 8;
            yield return 0;

            Rect.sizeDelta = Vector2.Lerp(fullSize, smallSize, curve.Evaluate(timer));

            if (!show)
            {
                bgRect.anchoredPosition = Vector2.Lerp(bgPositionOut, bgPositionIn, curve.Evaluate(timer));
                bgRect.sizeDelta = Vector2.Lerp(bgFullSize, Vector2.zero, curve.Evaluate(timer));

                _background.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, Mathf.Clamp(timer, 0, 1));


                for (int i = _itemList.Count - 1; i >= 0; i--)
                {
                    _itemList[i].ShowItem(show, curve.Evaluate(timer));
                }
            }
        }

        if (show)
        {
            _boxImage.sprite = GraphicsHelper.Instance.itemboxOpenSprite;
        }
        else
        {
            _boxImage.sprite = GraphicsHelper.Instance.itemboxClosedSprite;
        }

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;
            yield return 0;

            Rect.sizeDelta = Vector2.Lerp(smallSize, fullSize, curve.Evaluate(timer));

            if (show)
            {
                bgRect.anchoredPosition = Vector2.Lerp(bgPositionIn, bgPositionOut, curve.Evaluate(timer));
                bgRect.sizeDelta = Vector2.Lerp(Vector2.zero, bgFullSize, curve.Evaluate(timer));

                _background.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, Mathf.Clamp(timer, 0, 1));


                for (int i = _itemList.Count - 1; i >= 0; i--)
                {
                    _itemList[i].ShowItem(show, curve.Evaluate(timer));
                }
            }
        }

        _animating = false;
    }
}
