using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderViewController : AbstractViewController {

    private Image _fillImage;
    private RectTransform _backgroundRect;

	public SliderViewController(RectTransform container, string name)
    {

        Text text = AddText(CreateContainer("Fill", container,
            new Vector2(0, 54), new Vector2(container.sizeDelta.x, 64),
            Vector2.zero, Vector2.zero, Vector2.zero), GraphicsHelper.Instance.UIFont, 32, TextAnchor.MiddleCenter);

        text.text = name;

        Image background = AddSprite(CreateContainer("Fill", container,
            Vector2.zero, new Vector2(container.sizeDelta.x, 64),
            Vector2.zero, Vector2.zero, Vector2.zero), 
            GraphicsHelper.Instance.sliderBackgroundSpirte, GraphicsHelper.Instance.SpriteColorWhite);

        background.type = Image.Type.Sliced;
        _backgroundRect = background.rectTransform;

        _fillImage = AddSprite(CreateContainer("Fill", background.GetComponent<RectTransform>(),
            Vector2.zero, background.rectTransform.sizeDelta,
            Vector2.zero, Vector2.zero, Vector2.zero),
            GraphicsHelper.Instance.sliderFillSprite, GraphicsHelper.Instance.SpriteColorWhite);

        _fillImage.type = Image.Type.Sliced;
    }

    public void UpdateSlider(float amount)
    {
        _fillImage.rectTransform.sizeDelta = new Vector2(_backgroundRect.sizeDelta.x * amount, _backgroundRect.sizeDelta.y);
    }
}
