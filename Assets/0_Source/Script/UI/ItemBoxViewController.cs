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
    
    private DragItemContainer _itemInSlot;

    private GameObject _itemObject;


    public ItemBoxViewController(Transform parent, List<Item> itemList, Personality personality)
    {
        Dictionary<int, Item> items = new Dictionary<int, Item>();

        foreach (Item item in itemList)
        {
            items[item.ID] = item;
        }

        Rect = CreateContainer("ItemBox", parent,
            new Vector2(-20, 20), new Vector2(256, 256),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));

        View = Rect.gameObject;
        

        _dropZone = CreateContainer("ItemDrop", parent,
            new Vector2(0, 0), new Vector2(1040, 1300),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)).gameObject.AddComponent<DropItemHandler>();

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

    private void ShowItemObject()
    {
        DebugController.Instance.Log("Add item to slot", DebugController.DebugType.UI);

        Transform parent = GraphicsHelper.Instance.itemAnchor;

        _itemObject = GameObject.Instantiate(GraphicsHelper.Instance.GetItemObject(_itemInSlot.ItemName), parent);

        _itemObject.GetComponent<ItemBoxObject>().Setup(this);
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
