using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderViewController : AbstractViewController {

    private Image _fillImage;
    private RectTransform _backgroundRect;

	public SliderViewController(RectTransform container, string sliderName, string objectName)
    {
        Rect = container;
        View = Rect.gameObject;

        if (!sliderName.Equals(""))
        {
            Text text = AddText(CreateContainer(sliderName, container,
                new Vector2(0, 54), new Vector2(container.sizeDelta.x, 64),
                Vector2.zero, Vector2.zero, Vector2.zero), GraphicsHelper.Instance.UIFont, 32, TextAnchor.MiddleCenter);

            text.text = sliderName;
        }

        Image background = AddSprite(CreateContainer(objectName, container,
            new Vector2(0, 10), new Vector2(container.sizeDelta.x, 64),
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
        _fillImage.enabled = amount > 0.05f;
            
    }

    public void SetColor(Color c)
    {
        _fillImage.color = c;
    }
}
