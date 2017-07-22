using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : AbstractViewController {

    private ApplicationManager _manager;

    private Image _inputBlock;
    private Image _background;
    private Button _multiplayerButton;

    private bool _showMenu;

    private bool _animate;

	public OptionsMenuController(Transform parent, Personality personality)
    {
        _manager = ApplicationManager.Instance;

        Rect = CreateContainer("Options", parent,
            Vector2.zero, new Vector2(1080, 1920),
            new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(0.5f, 0.5f));

        _inputBlock = AddSprite(Rect, null, GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _inputBlock.raycastTarget = false;

        Rect.offsetMin = Vector2.zero;
        Rect.offsetMax = Vector2.zero;
        
        View = Rect.gameObject;

        //Create a Button
        RectTransform menuButton = CreateContainer("MenuButton", Rect,
            new Vector2(-40, -40), new Vector2(150, 150),
            Vector2.one, Vector2.one, Vector2.one);

        AddSprite(menuButton, GraphicsHelper.Instance.optionsButtonSprite, GraphicsHelper.Instance.SpriteColorWhite);
        CreateButton(menuButton, delegate { ToggleMenu(); });

        //Create a Menu

        _background = AddSprite(CreateContainer("Menu", Rect,
            new Vector2(0, 0), new Vector2(740, 1040),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.menuBackgroundSprite, GraphicsHelper.Instance.SpriteColorWhite);
        _background.type = Image.Type.Sliced;

        //Traits
        Text traitText = AddText(
                            CreateContainer("TraitText", _background.rectTransform,
                            new Vector2(0, 140), new Vector2(550, 600),
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.UIFont,
                            50,
                            TextAnchor.UpperLeft);

        string traitString = "Traits: " + "\n";

        for(int i=0; i<personality.Traits.Count; i++)
        {
            traitString += personality.Traits[i].Identifier.ToString();
            if(i < (personality.Traits.Count - 1))
            {
                traitString += ", ";
            }
        }

        traitText.text = traitString;
        traitText.color = GraphicsHelper.Instance.SpriteColorWhite;
        traitText.raycastTarget = false;

        AddSprite(CreateContainer("Divider", _background.rectTransform, 
            new Vector2(0, -240), new Vector2(740, 11), 
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)), 
            null, GraphicsHelper.Instance.SpriteColorWhite);

        //RestartButton
        Image restartImage = AddSprite(
                            CreateContainer("RestartButton", _background.rectTransform,
                            new Vector2(0, -380), new Vector2(320, 100),
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.outputFrameSprite,
                            GraphicsHelper.Instance.SpriteColorWhite);

        Text restartText = AddText(
                            CreateContainer("RestartText", restartImage.rectTransform,
                            Vector2.zero, Vector2.zero,
                            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.UIFont,
                            50,
                            TextAnchor.MiddleCenter);
        restartText.text = "Quit";
        restartText.color = GraphicsHelper.Instance.SpriteColorWhite;
        restartText.raycastTarget = false;

        CreateButton(restartImage.rectTransform, delegate { Restart(); });

        _background.rectTransform.localScale = Vector2.zero;
        _showMenu = false;
    }

    public void ToggleMenu()
    {
        if (!_animate)
        {
            _showMenu = !_showMenu;
            ApplicationManager.Instance.StartCoroutine(ShowMenu(_showMenu));
        }
    }

    private IEnumerator ShowMenu(bool show)
    {
        _animate = true;
        _inputBlock.raycastTarget = true;

        float timer = 0;
        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

        while(timer < 1)
        {
            if (show)
            {
                timer += Time.deltaTime * 8;
                _background.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curve.Evaluate(timer));
            }
            else
            {
                timer += Time.deltaTime * 4;
                _background.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, curve.Evaluate(timer));
            }

            yield return 0;
        }

        _inputBlock.raycastTarget = show;
        _animate = false;
    }

    public void Restart()
    {
        ToggleMenu();
        ApplicationManager.Instance.EndGame();
    }
    
}
