using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : AbstractViewController {

    private ApplicationManager _manager;

    private RectTransform _menu;
    private Button _multiplayerButton;

	public OptionsMenuController(Transform parent)
    {
        _manager = ApplicationManager.Instance;

        Rect = CreateContainer("Options", parent,
            Vector2.zero, new Vector2(1080, 1920),
            new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(0.5f, 0.5f));

        Rect.offsetMax = Vector2.zero;
        Rect.offsetMin = Vector2.zero;
        View = Rect.gameObject;

        //Create a Button

        //Create a Menu
        _menu = CreateContainer("Menu", Rect, 
            new Vector2(0, -20), new Vector2(1040, 500),
            new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f), new Vector2(0.5f, 1.0f));

        GridLayoutGroup grid = _menu.gameObject.AddComponent<GridLayoutGroup>();
        grid.padding = new RectOffset(40, 40, 40, 40);
        grid.cellSize = new Vector2(600, 128);
        grid.spacing = new Vector2(0, 20);
        grid.childAlignment = TextAnchor.UpperCenter;

        AddSprite(_menu, GraphicsHelper.Instance.UIContainer, GraphicsHelper.Instance.SpriteColorWhite).type = Image.Type.Sliced;

        _multiplayerButton = CreateStandardButton(CreateContainer("MultiplayerButton", _menu,
                                            Vector2.zero, Vector2.zero,
                                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                        delegate { _manager.ToggleMultiplayer(); });
        

        _menu.gameObject.SetActive(false);
    }

    public void ShowMenu(bool show)
    {
        //
    }
    
}
