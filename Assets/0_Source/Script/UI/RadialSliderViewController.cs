using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialSliderViewController : AbstractViewController {
    
    private Image _fillImage;
    private RectTransform _backgroundRect;
    private bool stopRoutine;

    private Image _toolTipBackground;
    private Text _toolTipText;

    private bool _animating;

    public RadialSliderViewController(RectTransform container, Sprite icon, string toolTip)
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

        AddSprite(CreateContainer("Icon", _fillImage.rectTransform,
            Vector2.zero, _fillImage.rectTransform.sizeDelta,
            Vector2.zero, Vector2.zero, Vector2.zero),
            icon, GraphicsHelper.Instance.SpriteColorWhite);

        _toolTipBackground = AddSprite(CreateContainer("Tooltip", _fillImage.rectTransform,
            new Vector2(250, 0), new Vector2(320, 90),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.tooltipBackgroundSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        _toolTipBackground.raycastTarget = false;

        RectTransform textRect = CreateContainer("Text", _toolTipBackground.transform,
                                                    Vector2.zero, Vector2.zero,
                                                    Vector2.zero, Vector2.one, Vector2.one * 0.5f);

        _toolTipText = AddText(textRect, GraphicsHelper.Instance.UIFont, 45, TextAnchor.MiddleCenter);
        _toolTipText.text = toolTip;
        _toolTipText.color = GraphicsHelper.Instance.SpriteColorWhiteHidden;
        _toolTipText.raycastTarget = false;

        CreateButton(Rect, delegate { ShowTooltip(); });
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

    public void ShowTooltip()
    {
        if (!_animating)
        {
            ApplicationManager.Instance.StartCoroutine(ShowTooltipRoutine());
        }
    }

    private IEnumerator ShowTooltipRoutine()
    {
        _animating = true;

        float timer = 0;
        

        Vector2 bgPositionIn = new Vector2(0, 0);
        Vector2 bgPositionOut = new Vector2(250, 0);

        Vector2 bgFullSize = new Vector2(320, 90);

        RectTransform bgRect = _toolTipBackground.GetComponent<RectTransform>();

        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;
            yield return 0;
            
            bgRect.anchoredPosition = Vector2.Lerp(bgPositionIn, bgPositionOut, curve.Evaluate(timer));
            bgRect.sizeDelta = Vector2.Lerp(Vector2.zero, bgFullSize, curve.Evaluate(timer));

            _toolTipBackground.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, Mathf.Clamp(timer, 0, 1));
            _toolTipText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, Mathf.Clamp(timer, 0, 1));

        }

        yield return new WaitForSeconds(2);

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 8;
            yield return 0;
            
            bgRect.anchoredPosition = Vector2.Lerp(bgPositionOut, bgPositionIn, curve.Evaluate(timer));
            bgRect.sizeDelta = Vector2.Lerp(bgFullSize, Vector2.zero, curve.Evaluate(timer));

            _toolTipBackground.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, Mathf.Clamp(timer, 0, 1));
            _toolTipText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, Mathf.Clamp(timer, 0, 1));

        }

        _animating = false;
    }
}
