using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemCollectionViewController : AbstractViewController {

    private GridLayoutGroup _grid;
    private Personality _personality;
    private ArtificialIntelligence _intelligence;
    private RawImage _block;

    public ItemCollectionViewController(Transform parent, Dictionary<int, Item> items, Personality personality, ArtificialIntelligence intelligence)
    {
        _personality = personality;

        Rect = CreateContainer("ItemCollection", parent,
            new Vector2(0, 420), new Vector2(1000, 420),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        
        View = Rect.gameObject;

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        _grid = View.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new Vector2(490, 128);
        _grid.spacing = new Vector2(20, 20);

        foreach (KeyValuePair<int, Item> kvp in items)
        {

            int tempKey = kvp.Key;
            Item tempItem = kvp.Value;

            ToggleViewController toggle = new ToggleViewController(
                CreateContainer("Toggle_" + tempItem.Name, Rect,
                                Vector2.zero, _grid.cellSize,
                                Vector2.zero, Vector2.zero, Vector2.zero),
                tempItem.Name,
                delegate { _personality.RemoveItem(tempKey); Debug.Log("Take " + tempItem.Name); },
                delegate { _personality.AddItem(tempKey, tempItem); Debug.Log("Give " + tempItem.Name); });
            
        }

        _block = AddImage(CreateContainer("ItemCollectionBlock", parent,
            new Vector2(0, 420), new Vector2(1000, 420),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0)),
            null, new Color(0, 0, 0, 0.6f));

        ApplicationManager.Instance.StartCoroutine(updateShowItems(intelligence));
    }

    private IEnumerator updateShowItems(ArtificialIntelligence intelligence)
    {
        while (true)
        {
            _block.enabled = !intelligence.NeedItems;
            yield return 0;
        }
    }
}
