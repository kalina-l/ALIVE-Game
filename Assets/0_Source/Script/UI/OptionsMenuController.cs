using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuController : AbstractViewController {

    private ApplicationManager _manager;

    private RectTransform _menu;

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

        AddImage(_menu, null, new Color(0, 0, 0, 0.2f));

        _menu.gameObject.SetActive(false);
    }

    public void ShowMenu(bool show)
    {
        //
    }

    public void StartMultiplayer()
    {
        _manager.StartMultiplayer();
    }

    public void StopMultiplayer()
    {
        _manager.StopMultiplayer();
    }
}
