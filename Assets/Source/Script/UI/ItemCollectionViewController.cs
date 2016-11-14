using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemCollectionViewController : AbstractViewController {

    private GridLayoutGroup _grid;
    private Personality _personality;

    public ItemCollectionViewController(Transform parent, Dictionary<string, Item> items, Personality personality)
    {
        _personality = personality;

        Rect = CreateContainer("ItemCollection", parent,
            new Vector2(0, -580), new Vector2(1000, 460),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
        View = Rect.gameObject;

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        _grid = View.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new Vector2(490, 128);
        _grid.spacing = new Vector2(20, 20);

        foreach (KeyValuePair<string, Item> kvp in items)
        {

            string tempString = kvp.Key;
            Item tempItem = kvp.Value;

            ToggleViewController toggle = new ToggleViewController(
                CreateContainer("Toggle_" + kvp.Key, Rect,
                                Vector2.zero, _grid.cellSize,
                                Vector2.zero, Vector2.zero, Vector2.zero),
                kvp.Key,
                delegate { _personality.RemoveItem(tempString); Debug.Log("Take " + tempString); },
                delegate { _personality.AddItem(tempString, tempItem); Debug.Log("Give " + tempString); });
            
        }
    }
}
