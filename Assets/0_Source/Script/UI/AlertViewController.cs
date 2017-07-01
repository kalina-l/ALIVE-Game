using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertViewController : AbstractViewController {

    private Vector2 _bubbleSize;
    private Vector2 _iconSize;

    private Vector2 _singleplayerPosition = new Vector2(0, 186);
    private Vector2 _multiplayerPosition = new Vector2(-750, 186);

    private Image _icon;

    private bool _animating;

	public AlertViewController(Transform parent) {

        _bubbleSize = new Vector2(350, 350);
        _iconSize = new Vector2(110, 110);

        Rect = CreateContainer("Alert", parent,
            _singleplayerPosition, _bubbleSize,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1, 0));
        View = Rect.gameObject;

        AddSprite(Rect, GraphicsHelper.Instance.alertBubbleSprite, GraphicsHelper.Instance.SpriteColorWhite);

        RectTransform iconRect = CreateContainer("Icon", Rect,
            new Vector2(0, 30), _iconSize,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

        _icon = AddSprite(iconRect, null, GraphicsHelper.Instance.SpriteColorBlack);

        HideAlert(true);
    }

    public void setSingleplayer()
    {
        Rect.anchoredPosition = _singleplayerPosition;
    }

    public void setMultiplayer ()
    {
        Rect.anchoredPosition = _multiplayerPosition;
    }

    public void ShowAlert(Sprite icon)
    {
        ApplicationManager.Instance.StartCoroutine(AnimateAlert(true, icon));
    }

    public void HideAlert(bool instant = false)
    {
        if (instant)
        {
            _icon.rectTransform.sizeDelta = Vector2.zero;
            Rect.sizeDelta = Vector2.zero;
        }
        else
        {
            ApplicationManager.Instance.StartCoroutine(AnimateAlert(false, null));
        }
    }

    private IEnumerator AnimateAlert(bool show, Sprite icon) {

        while (_animating)
        {
            yield return 0;
        }

        _animating = true;
        float timer = 0;

        if (!show)
        {
            //shrink icon
            while (timer < 1)
            {
                timer += Time.deltaTime * 2f;
                _icon.rectTransform.sizeDelta = Vector2.Lerp(_iconSize, Vector2.zero, GraphicsHelper.Instance.AlertAnimation.Evaluate(timer));
                yield return 0;
            }

            timer = 0;

            //shrink bubble
            while (timer < 1)
            {
                timer += Time.deltaTime * 4f;
                Rect.sizeDelta = Vector2.Lerp(_bubbleSize, Vector2.zero, GraphicsHelper.Instance.AlertAnimation.Evaluate(timer));
                yield return 0;
            }
        }
        else
        {
            _icon.sprite = icon;

            float bubbleTimer = 0;
            float iconTimer = 0;

            while (bubbleTimer < 1 || iconTimer < 1)
            {
                //enlarge bubble
                if (bubbleTimer < 1)
                {
                    bubbleTimer += Time.deltaTime * 4f;
                    Rect.sizeDelta = Vector2.Lerp(Vector2.zero, _bubbleSize, GraphicsHelper.Instance.AlertAnimation.Evaluate(bubbleTimer));
                    
                }

                timer = 0;

                //enlarge icon
                if (iconTimer < 1)
                {
                    iconTimer += Time.deltaTime * 2f;
                    _icon.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _iconSize, GraphicsHelper.Instance.AlertAnimation.Evaluate(iconTimer));
                }

                yield return 0;
            }

            yield return new WaitForSeconds(4);

            bubbleTimer = 0;
            iconTimer = 0;

            while (bubbleTimer < 1 || iconTimer < 1)
            {
                //shrink bubble
                if (bubbleTimer < 1)
                {
                    bubbleTimer += Time.deltaTime * 6f;
                    Rect.sizeDelta = Vector2.Lerp(_bubbleSize, Vector2.zero, GraphicsHelper.Instance.AlertAnimation.Evaluate(bubbleTimer));

                }

                timer = 0;

                //shrink icon
                if (iconTimer < 1)
                {
                    iconTimer += Time.deltaTime * 8f;
                    _icon.rectTransform.sizeDelta = Vector2.Lerp(_iconSize, Vector2.zero, GraphicsHelper.Instance.AlertAnimation.Evaluate(iconTimer));
                }

                yield return 0;
            }
        }

        _animating = false;


    }
}
