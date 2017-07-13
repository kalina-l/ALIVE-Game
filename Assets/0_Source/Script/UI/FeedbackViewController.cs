using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FeedbackType { Audio, Video, Touch, Button, Multiplayer }

public class FeedbackViewController : AbstractViewController {

    private Button _startRecording; // audio
    private bool _isRecording; // attached to audio AND video

    private Vector2 _buttonSize = new Vector2(256, 256);
    
    private AudioFeedbackController _audioFeedbackController;
    private TouchController _touchController;
    private VideoFeedbackController _videoFeedbackController;

    private FeedbackType lastFeedbackType;
    private ParticleSystem positiveFX;
    private ParticleSystem negativeFX;

    private ParticleSystem remotePositiveFX;
    private ParticleSystem remoteNegativeFX;

    private RectTransform _feedbackMenu;
    private bool _showMenu;
    private bool _animating;

    public FeedbackViewController(Transform parent, ArtificialIntelligence intelligence)
    {
        Rect = CreateContainer("Feedback", parent,
            Vector2.zero, new Vector2(1080, 1920),
            new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(0.5f, 0.5f));

        Rect.offsetMax = Vector2.zero;
        Rect.offsetMin = Vector2.zero;
        View = Rect.gameObject;

        _audioFeedbackController = new AudioFeedbackController(this);
        _touchController = new TouchController(this, Rect);
        _videoFeedbackController = new VideoFeedbackController(this, Rect);

        positiveFX = GraphicsHelper.Instance.positiveFX;
        negativeFX = GraphicsHelper.Instance.negativeFX;

        remotePositiveFX = GraphicsHelper.Instance.remotePositiveFX;
        remoteNegativeFX = GraphicsHelper.Instance.remoteNegativeFX;

        _feedbackMenu = CreateContainer("FeedbackMenu", Rect,
                                    new Vector2(140, 0), new Vector2(186, 298),
                                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));

        _showMenu = false;

        Image menuImage = AddSprite(_feedbackMenu, GraphicsHelper.Instance.feedbackMenu, GraphicsHelper.Instance.SpriteColorWhite);
        menuImage.raycastTarget = false;

        _startRecording = CreateButton(
                                    CreateContainer("StartRecording", _feedbackMenu,
                                    new Vector2(0, 77), new Vector2(150, 150),
                                    new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f)),
                                    delegate { _audioFeedbackController.StartRecording(); }
                                    );
        AddSprite(_startRecording.GetComponent<RectTransform>(), null, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        RectTransform startStreamingRect =
                                    CreateContainer("StartStreaming", _feedbackMenu,
                                    new Vector2(0, -77), new Vector2(150, 150),
                                    new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));                                 
        PointerListener pl = startStreamingRect.gameObject.AddComponent<PointerListener>();
        pl.AddOnDownDelegate(_videoFeedbackController.StartRecording);
        pl.AddOnUpDelegate(_videoFeedbackController.StopRecording);
        AddSprite(startStreamingRect.GetComponent<RectTransform>(), null, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        RectTransform buttonRect = CreateContainer("MenuButton", _feedbackMenu,
                            new Vector2(-140, 0), new Vector2(60, 100),
                            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));

        AddSprite(buttonRect, null, GraphicsHelper.Instance.SpriteColorWhiteHidden);

        CreateButton(buttonRect, delegate { ToggleMenu(); });
    }

    public void ToggleMenu()
    {
        if (!_animating)
        {
            ApplicationManager.Instance.StartCoroutine(AnimateMenuRoutine());
        }
    }

    private IEnumerator AnimateMenuRoutine()
    {
        _animating = true;

        _showMenu = !_showMenu;

        float timer = 0;

        Vector2 bgPositionIn = new Vector2(140, 0);
        Vector2 bgPositionOut = Vector2.zero;

        Vector2 bgFullSize = new Vector2(320, 90);

        AnimationCurve curve = GraphicsHelper.Instance.AlertAnimation;

        if (_showMenu)
        {
            while (timer < 1)
            {
                timer += Time.deltaTime * 8;
                yield return 0;

                _feedbackMenu.anchoredPosition = Vector2.Lerp(bgPositionIn, bgPositionOut, curve.Evaluate(timer));
            }

        }
        else
        {
            while (timer < 1)
            {
                timer += Time.deltaTime * 4;
                yield return 0;

                _feedbackMenu.anchoredPosition = Vector2.Lerp(bgPositionOut, bgPositionIn, curve.Evaluate(timer));
            }
        }

        _animating = false;
    }

    public void ShowFeedback(int feedback)
    {
        if (feedback == 1)
        {
            positiveFX.Play();
        }
        else if (feedback == -1)
        {
            negativeFX.Play();
        }
    }

    public void ShowRemoteFeedback(int feedback)
    {
        if (feedback == 1)
        {
            remotePositiveFX.Play();
        }
        else if (feedback == -1)
        {
            remoteNegativeFX.Play();
        }
    }

    public void SetIsRecording(bool isRecording)
    {
        this._isRecording = isRecording;
    }
    public bool IsRecording()
    {
        return _isRecording;
    }

    public void SendFeedBack(int feedback, FeedbackType feedbackType)
    {
        SetIsRecording(false);
        lastFeedbackType = feedbackType;
        ApplicationManager.Instance.GiveFeedback(feedback);

    }

    public FeedbackType getLastFeedbackType()
    {
        return lastFeedbackType;
    }

    public void setLastFeedbackType(FeedbackType lastFeedbackType)
    {
        this.lastFeedbackType = lastFeedbackType;
    }

}
