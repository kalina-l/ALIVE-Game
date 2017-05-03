using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DigitalRubyShared;
using KKSpeech;

public class FeedbackViewController : AbstractViewController {

    private bool _animating;

    private Button _positiveButton;
    private Image _positiveButtonImage;

    private Button _negativeButton;
    private Image _negativeButtonImage;

    private Button _startRecording;

    private Vector2 _buttonSize = new Vector2(256, 256);

    private GestureController _gestures;
    private AudioFeedbackController _audioFeedbackController;
    private TouchController _touchController;

    private bool receiveFeedback;
    private bool _buttonsVisible;
    private IEnumerator showFeedbackCoroutine;

    public FeedbackViewController(Transform parent, ArtificialIntelligence intelligence)
    {
        Rect = CreateContainer("Feedback", parent,
            Vector2.zero, new Vector2(1080, 1920),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        //AddImage(Rect, null, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        //_gestures = new GestureController(Rect.transform, this);

        _audioFeedbackController = new AudioFeedbackController(this);
        _touchController = new TouchController(this);

        //Buttons
        _positiveButton = CreateButton(
                                    CreateContainer("PositiveFeedback", Rect,
                                    new Vector2(-256, -300), Vector2.zero,
                                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                    delegate { SendFeedBack(1); }
                                    );

        _positiveButtonImage = AddSprite(_positiveButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackPositiveSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        _negativeButton = CreateButton(
                                    CreateContainer("NegativeFeedback", Rect,
                                    new Vector2(256, -300), Vector2.zero,
                                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                    delegate { SendFeedBack(-1); }
                                    );

        _negativeButtonImage = AddSprite(_negativeButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackNegativeSprite, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        _startRecording = CreateButton(
                                    CreateContainer("StartRecording", Rect,
                                    new Vector2(460, 40), _buttonSize*0.65f,
                                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                                    delegate { _audioFeedbackController.StartRecording(); }
                                    );
        AddSprite(_startRecording.GetComponent<RectTransform>(), GraphicsHelper.Instance.speakerSprite, GraphicsHelper.Instance.SpriteColorWhite);

    }

    public void ShowFeedback(bool show)
    {
        if (show)
        {
            receiveFeedback = true;
        }

        /*if(_animating)
        {
            ApplicationManager.Instance.StopCoroutine(showFeedbackCoroutine);
            _animating = false;
            ShowImmediateFeedback();
        }
        else if(_buttonsVisible ^ show)
        {
            showFeedbackCoroutine = ShowFeedbackRoutine(show);
            ApplicationManager.Instance.StartCoroutine(showFeedbackCoroutine);
        }*/
    }

    private void ShowImmediateFeedback()
    {
        _positiveButtonImage.color = new Color(255f, 255f, 255f, 1);
        _negativeButtonImage.color = new Color(255f, 255f, 255f, 1);
        _positiveButtonImage.rectTransform.sizeDelta = _buttonSize;
        _negativeButtonImage.rectTransform.sizeDelta = _buttonSize;
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
                _buttonsVisible = true;
                _positiveButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhiteHidden, graphics.SpriteColorWhite, timer);
                _positiveButtonImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _buttonSize, timer);

                _negativeButtonImage.color = graphics.LerpColor(graphics.SpriteColorWhiteHidden, graphics.SpriteColorWhite, timer);
                _negativeButtonImage.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _buttonSize, timer);
            }
            else
            {
                _buttonsVisible = false;
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
