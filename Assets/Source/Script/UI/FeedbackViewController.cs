using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DigitalRubyShared;

public class FeedbackViewController : AbstractViewController {

    private bool _animating;

    private Button _positiveButton;
    private Image _positiveButtonImage;

    private Button _negativeButton;
    private Image _negativeButtonImage;

    private Vector2 _buttonSize = new Vector2(256, 256);

    private GestureController _gestures;

    private bool receiveFeedback;

    public FeedbackViewController(Transform parent, ArtificialIntelligence intelligence)
    {
        Rect = CreateContainer("Feedback", parent,
            Vector2.zero, new Vector2(296, 296),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        _gestures = new GestureController(Rect.transform, this);

        //Buttons
        _positiveButton = CreateButton(
                                    CreateContainer("PositiveFeedback", Rect,
                                    new Vector2(-256, 0), Vector2.zero,
                                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                    delegate { SendFeedBack(1); }
                                    );

        _positiveButtonImage = AddSprite(_positiveButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackPositiveSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        _negativeButton = CreateButton(
                                    CreateContainer("NegativeFeedback", Rect,
                                    new Vector2(256, 0), Vector2.zero,
                                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                    delegate { SendFeedBack(-1); }
                                    );

        _negativeButtonImage = AddSprite(_negativeButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackNegativeSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);
    }

    public void ShowFeedback(bool show)
    {
        if (show)
        {
            _gestures.AskForGesture();
            receiveFeedback = true;
        }
        else
        {
            _gestures.StopAsking();
        }

        //if(!_animating)
        //    ApplicationManager.Instance.StartCoroutine(ShowFeedbackRoutine(show));
    }

    private IEnumerator ShowFeedbackRoutine(bool show)
    {
        float timer = 0;
        _animating = true;

        GraphicsHelper graphics = GraphicsHelper.Instance;

        while(timer < 1)
        {
            timer += Time.deltaTime * 2;

            if(show)
            {
                _positiveButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhiteHidden, graphics.SpriteColorWhite, timer);
                _positiveButtonImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _buttonSize, timer);

                _negativeButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhiteHidden, graphics.SpriteColorWhite, timer);
                _negativeButtonImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _buttonSize, timer);
            }
            else
            {
                _positiveButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhite, graphics.SpriteColorWhiteHidden, timer);
                _positiveButtonImage.rectTransform.sizeDelta = Vector2.Lerp(_buttonSize, Vector2.zero, timer);

                _negativeButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhite, graphics.SpriteColorWhiteHidden, timer);
                _negativeButtonImage.rectTransform.sizeDelta = Vector2.Lerp(_buttonSize, Vector2.zero, timer);
            }

            yield return 0;
        }

        _animating = false;
    }

    public void SendFeedBack(int feedback)
    {
        if(receiveFeedback)
        {
            ApplicationManager.Instance.GiveFeedback(feedback);
            receiveFeedback = false;
        }
    }


}
