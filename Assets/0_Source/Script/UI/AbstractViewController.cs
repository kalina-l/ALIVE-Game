using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbstractViewController {
    

    public GameObject View { get; set; }
    public RectTransform Rect { get; set; }

    public RectTransform CreateContainer(string name, Transform parent, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 minAnchor, Vector2 maxAnchor, Vector2 pivot)
    {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();

        rect.name = name;
        rect.SetParent(parent);
        rect.localScale = Vector3.one;

        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.anchorMin = minAnchor;
        rect.anchorMax = maxAnchor;
        rect.pivot = pivot;

        return rect;
    }

    public RawImage AddImage(RectTransform container, Texture2D texture, Color color)
    {
        RawImage image = container.gameObject.AddComponent<RawImage>();

        image.texture = texture;
        image.color = color;

        return image;
    }

    public Image AddSprite(RectTransform container, Sprite sprite, Color color)
    {
        Image image = container.gameObject.AddComponent<Image>();

        image.sprite = sprite;
        image.color = color;

        return image;
    }

    public Text AddText(RectTransform container, Font font, int fontSize, TextAnchor anchor)
    {
        Text text = container.gameObject.AddComponent<Text>();
        text.fontSize = fontSize;
        text.alignment = anchor;

        text.font = font;

        return text;
    }

    public Button CreateButton(RectTransform container, UnityEngine.Events.UnityAction call)
    {
        Button button = container.gameObject.AddComponent<Button>();

        button.onClick.AddListener(call);

        return button;
    }

    public Button CreateStandardButton(RectTransform container, UnityEngine.Events.UnityAction call)
    {
        Button button = container.gameObject.AddComponent<Button>();

        button.onClick.AddListener(call);

        Image img = container.gameObject.AddComponent<Image>();
        img.sprite = GraphicsHelper.Instance.UIButton;
        img.type = Image.Type.Sliced;

        button.transition = Selectable.Transition.SpriteSwap;

        SpriteState spriteState = new SpriteState();

        spriteState.highlightedSprite = GraphicsHelper.Instance.UIButton;
        spriteState.pressedSprite = GraphicsHelper.Instance.UIButton_pressed;
        spriteState.disabledSprite = GraphicsHelper.Instance.UIButton;

        button.spriteState = spriteState;

        button.targetGraphic = img;

        return button;
    }

    public Color LerpColor(Color c1, Color c2, float t)
    {
        return new Color(Mathf.Lerp(c1.r, c2.r, t),
                        Mathf.Lerp(c1.g, c2.g, t),
                        Mathf.Lerp(c1.b, c2.b, t),
                        Mathf.Lerp(c1.a, c2.a, t));
    }
}
