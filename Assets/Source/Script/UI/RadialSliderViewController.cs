﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialSliderViewController : AbstractViewController {
    
    private Image _fillImage;
    private RectTransform _backgroundRect;
    private bool stopRoutine;

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

    public void UpdateSlider(float amount, Color c)
    {
        ApplicationManager.Instance.StartCoroutine(UpdateSliderRoutine(amount, c));
    }
    

    private IEnumerator UpdateSliderRoutine(float amount, Color c)
    {
        float startValue = _fillImage.fillAmount;

        Color startColor = _fillImage.color;

        float timer = 0;

        stopRoutine = true;

        yield return 0;

        stopRoutine = false;


        while (timer < 1 && !stopRoutine)
        {
            timer += Time.deltaTime * 2;

            _fillImage.fillAmount = Mathf.Lerp(startValue, amount, GraphicsHelper.Instance.SliderAnimation.Evaluate(timer));
            _fillImage.color = GraphicsHelper.Instance.LerpColor(startColor, c, GraphicsHelper.Instance.SliderAnimation.Evaluate(timer));

            yield return 0;
        }
    }
}
