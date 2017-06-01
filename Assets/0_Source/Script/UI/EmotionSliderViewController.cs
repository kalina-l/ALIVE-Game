using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmotionSliderViewController : AbstractViewController
{

    private Image _pointerImg;
    private RectTransform _backgroundRect;

    public EmotionSliderViewController(RectTransform container, string sliderName, string objectName)
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
            new Vector2(0, 450), new Vector2(600, 80),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.emotionSliderBGSpirte, GraphicsHelper.Instance.SpriteColorWhite);

        background.type = Image.Type.Sliced;
        _backgroundRect = background.rectTransform;

        _pointerImg = AddSprite(CreateContainer("Pointer", background.GetComponent<RectTransform>(),
            Vector2.zero, new Vector2(48, 134),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.emotionSliderPointerSprite, GraphicsHelper.Instance.SpriteColorWhite);

        SetActive(false);
    }

    public void UpdateSlider(float amount)
    {

        amount /= 20;
        float newXPosition = _pointerImg.rectTransform.localPosition.x + amount;
        float halfSliderSize = _backgroundRect.sizeDelta.x / 2;
        if (newXPosition > halfSliderSize)
        {
            newXPosition = halfSliderSize;
        }
        else if (newXPosition < -halfSliderSize)
        {
            newXPosition = -halfSliderSize;
        }
        _pointerImg.rectTransform.localPosition = new Vector3(newXPosition, 0);

    }

    public void ResetSlider()
    {
        _pointerImg.rectTransform.localPosition = Vector3.zero;
    }

    public void SetActive(bool active)
    {
        _backgroundRect.gameObject.SetActive(active);
    }
}