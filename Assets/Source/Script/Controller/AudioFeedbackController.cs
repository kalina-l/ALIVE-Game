using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFeedbackController {

    private FeedbackViewController _feedbackViewController;

    private AudioFeedbackRecognizer _audioFeedbackRecognizer;

    public AudioFeedbackController(FeedbackViewController feedbackViewConroller)
    {
        _feedbackViewController = feedbackViewConroller;

        _audioFeedbackRecognizer = new GameObject("AudioRecorder").AddComponent<AudioFeedbackRecognizer>();
        _audioFeedbackRecognizer.recordDebugger = ApplicationManager.Instance.debugText;
        _audioFeedbackRecognizer.Setup(this);
    }

    public void SendFeedback(int feedback)
    {
        _feedbackViewController.SendFeedBack(feedback);
    }
}
