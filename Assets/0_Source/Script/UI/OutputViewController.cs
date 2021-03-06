﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutputViewController : AbstractViewController {

    private Text OutputText;

    private int messages;

    private bool _waitForFeedback;
    private int _feedback;
    private Image _background;
    private Image _fillImage;

    private Vector2 _singlePlayerPos;
    private Vector2 _singlePlayerAnchor;
    private Vector2 _singlePlayerSize;

    private Vector2 _multiPlayerPos;
    private Vector2 _multiPlayerAnchor;
    private Vector2 _multiPlayerSize;

    public OutputViewController(Transform parent)
    {
        _singlePlayerPos = new Vector2(540, -155);
        _singlePlayerAnchor = new Vector2(0, 1f);
        _singlePlayerSize = new Vector2(320, 100);

        _multiPlayerPos = new Vector2(0, -155);
        _multiPlayerAnchor = new Vector2(0.5f, 1f);
        _multiPlayerSize = new Vector2(640, 100);

        Rect = CreateContainer("Output", parent,
            _singlePlayerPos, new Vector2(320, 100),
            new Vector2(0, 1f), new Vector2(0, 1f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        RectTransform fillImage = CreateContainer("Fill", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        _fillImage = AddSprite(fillImage, GraphicsHelper.Instance.outputFillSprite, new Color(0, 0, 0, 0));
        _fillImage.type = Image.Type.Sliced;

        RectTransform backgroundImage = CreateContainer("Background", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        _background = AddSprite(backgroundImage, GraphicsHelper.Instance.outputFrameSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _background.type = Image.Type.Sliced;

        OutputText = AddText(
            CreateContainer("OutputText", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.UIFont, 80, TextAnchor.MiddleCenter);
    }

    public void DisplayMessage(string msg, bool isMultiplayer)
    {
        //OutputText.text = msg;
        ApplicationManager.Instance.StartCoroutine(AnimateText(msg, isMultiplayer));
    }

    public void ShowFeedback(int feedback)
    {
        _feedback = feedback;
        _waitForFeedback = false;
    }

    private IEnumerator AnimateText(string msg, bool isMultiplayer)
    {
        if (isMultiplayer)
        {
            Rect.anchoredPosition = _multiPlayerPos;
            Rect.anchorMin = _multiPlayerAnchor;
            Rect.anchorMax = _multiPlayerAnchor;
            Rect.sizeDelta = _multiPlayerSize;
        }
        else
        {
            Rect.anchoredPosition = _singlePlayerPos;
            Rect.anchorMin = _singlePlayerAnchor;
            Rect.anchorMax = _singlePlayerAnchor;
            Rect.sizeDelta = _singlePlayerSize;
        }

        float timer = 0;
        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;
        AnimationCurve blink = GraphicsHelper.Instance.BlinkAnimation;

        OutputText.text = msg;

        _waitForFeedback = true;

        Color fillNeutralColor = new Color(0, 0, 0, 0.3f);
        Color fillHiddenColor = new Color(0, 0, 0, 0);

        Color finalColor = fillNeutralColor;

        while (timer < 1)
        {
            timer += Time.deltaTime * 8;

            Rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curve.Evaluate(timer));

            OutputText.fontSize = (int)Mathf.Lerp(0, 45, curve.Evaluate(timer));
            OutputText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.TextColorHidden, GraphicsHelper.Instance.TextColor, curve.Evaluate(timer));
            _background.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, curve.Evaluate(timer));
            _fillImage.color = GraphicsHelper.Instance.LerpColor(fillHiddenColor, fillNeutralColor, curve.Evaluate(timer));

            yield return 0;
        }

        //Animating Feedback
        
        while (_waitForFeedback)
        {
            yield return 0;
        }

        if(_feedback != 0)
        {
            timer = 0;

            if (_feedback == -1)
            {
                finalColor =  new Color(1, 0, 0, 1);
            }
            else
            {
                finalColor = new Color(0, 1, 0, 1);
            }

            while (timer < 1)
            {
                timer += Time.deltaTime * 2;

                _fillImage.color = GraphicsHelper.Instance.LerpColor(fillNeutralColor, finalColor, curve.Evaluate(timer));

                yield return 0;
            }

            yield return new WaitForSeconds(0.5f);
        }

        Color finalHidden = finalColor;
        finalHidden.a = 0;

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;

            Rect.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, curve.Evaluate(timer));

            OutputText.fontSize = (int)Mathf.Lerp(50, 0, curve.Evaluate(timer));
            OutputText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.TextColor, GraphicsHelper.Instance.TextColorHidden, curve.Evaluate(timer));
            _background.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, curve.Evaluate(timer));
            _fillImage.color = GraphicsHelper.Instance.LerpColor(finalColor, finalHidden, curve.Evaluate(timer));

            yield return 0;
        }

    }
}
