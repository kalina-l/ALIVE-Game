using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialSliderViewController : AbstractViewController {
    
    private Image _fillImage;
    private RectTransform _backgroundRect;

    public RadialSliderViewController(RectTransform container, Sprite icon)
    {
        Rect = container;
        View = Rect.gameObject;

        AddSprite(container, GraphicsHelper.Instance.radialSliderSprite, new Color(1, 1, 1, 0.3f));

        _fillImage = AddSprite(CreateContainer("Fill", container,
            Vector2.zero, container.sizeDelta,
            Vector2.zero, Vector2.zero, Vector2.zero),
            GraphicsHelper.Instance.radialSliderSprite, GraphicsHelper.Instance.SpriteColorWhite);

        _fillImage.type = Image.Type.Filled;
        _fillImage.fillOrigin = 2;

        Image iconImage = AddSprite(CreateContainer("Icon", _fillImage.rectTransform,
            Vector2.zero, _fillImage.rectTransform.sizeDelta,
            Vector2.zero, Vector2.zero, Vector2.zero),
            icon, GraphicsHelper.Instance.SpriteColorWhite);
    }

    public void UpdateSlider(float amount)
    {
        _fillImage.fillAmount = amount;

    }

    public void SetColor(Color c)
    {
        _fillImage.color = c;
    }
}
