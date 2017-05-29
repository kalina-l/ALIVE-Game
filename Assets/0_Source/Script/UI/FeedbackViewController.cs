using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FeedbackType { Audio, Video, Touch, Button }

public class FeedbackViewController : AbstractViewController {

    private Button _startRecording; // audio
    private bool _isRecording; // attached to audio AND video

    private Vector2 _buttonSize = new Vector2(256, 256);
    
    private AudioFeedbackController _audioFeedbackController;
    private TouchController _touchController;
    private VideoFeedbackController _videoFeedbackController;

    private FeedbackType lastFeedbackType;
    private int lastFeedback;
    private ParticleSystem positiveFX;
    private ParticleSystem negativeFX;

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

       _startRecording = CreateButton(
                                    CreateContainer("StartRecording", Rect,
                                    new Vector2(-60, 40), _buttonSize*0.65f,
                                    new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f)),
                                    delegate { _audioFeedbackController.StartRecording(); }
                                    );
        AddSprite(_startRecording.GetComponent<RectTransform>(), GraphicsHelper.Instance.speakerSprite, GraphicsHelper.Instance.SpriteColorWhite);

        RectTransform startStreamingRect =
                                    CreateContainer("StartStreaming", Rect,
                                    new Vector2(-60, -150), _buttonSize * 0.65f,
                                    new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));                                 
        PointerListener pl = startStreamingRect.gameObject.AddComponent<PointerListener>();
        pl.AddOnDownDelegate(_videoFeedbackController.StartRecording);
        pl.AddOnUpDelegate(_videoFeedbackController.StopRecording);
        AddSprite(startStreamingRect.GetComponent<RectTransform>(), GraphicsHelper.Instance.cameraSprite, GraphicsHelper.Instance.SpriteColorWhite);

    }

    public void ShowFeedback()
    {
        if (lastFeedback > 0)
        {
            positiveFX.Play();
        }
        else
        {
            negativeFX.Play();
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
        lastFeedback = feedback;
        ApplicationManager.Instance.GiveFeedback(feedback);

    }

}
