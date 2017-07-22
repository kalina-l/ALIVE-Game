using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutputViewController : AbstractViewController {

    private Text OutputText;

    private int messages;

    private bool _waitForFeedback;
    private int _feedback;
    private Image _background;
    private Image _fillImage;

    public OutputViewController(Transform parent)
    {
        Rect = CreateContainer("Output", parent,
            new Vector2(540, -155), new Vector2(320, 100),
            new Vector2(0, 1f), new Vector2(0, 1f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        RectTransform backgroundImage = CreateContainer("Background", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        _background = AddSprite(backgroundImage, GraphicsHelper.Instance.outputFrameSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        RectTransform fillImage = CreateContainer("Fill", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        _fillImage = AddSprite(fillImage, GraphicsHelper.Instance.outputFillSprite, new Color(0, 0, 0, 0));

        OutputText = AddText(
            CreateContainer("OutputText", Rect,
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f)),
            GraphicsHelper.Instance.UIFont, 80, TextAnchor.MiddleCenter);
    }

    public void DisplayMessage(string msg)
    {
        //OutputText.text = msg;
        ApplicationManager.Instance.StartCoroutine(AnimateText(msg));
    }

    public void ShowFeedback(int feedback)
    {
        _feedback = feedback;
        _waitForFeedback = false;
    }

    private IEnumerator AnimateText(string msg)
    {
        float timer = 0;
        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

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
