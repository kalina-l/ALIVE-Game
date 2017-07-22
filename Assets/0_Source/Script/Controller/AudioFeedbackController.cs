using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioFeedbackController {

    public Text recordDebugger;

    private FeedbackViewController _feedbackViewController;

    private AudioFeedbackRecognizer _audioFeedbackRecognizer;

    public AudioFeedbackController(FeedbackViewController feedbackViewConroller, Transform parent)
    {
        _feedbackViewController = feedbackViewConroller;

        _audioFeedbackRecognizer = new GameObject("AudioRecorder").AddComponent<AudioFeedbackRecognizer>();
        //_audioFeedbackRecognizer.recordDebugger = ApplicationManager.Instance.debugText;
        recordDebugger = GraphicsHelper.Instance.audioDebugText;
        _audioFeedbackRecognizer.Setup(this);
    }

    public void SendFeedback(int feedback)
    {
        _feedbackViewController.SendFeedBack(feedback, FeedbackType.Audio);
    }

    public void StartRecording()
    {
        _feedbackViewController.SetIsRecording(true);
        _feedbackViewController.SetRecordingPoint(true);
        _audioFeedbackRecognizer.StartRecording();
    }

    public void RecordingStopped()
    {
        _feedbackViewController.SetIsRecording(false);
        _feedbackViewController.SetRecordingPoint(false);
        ApplicationManager.Instance.StartCoroutine(clearAudioDebugText());
    }

    public void StopRecording()
    {
        _audioFeedbackRecognizer.StopRecording();
    }

    public void setAudioDebugText(string text)
    {
        recordDebugger.text = "You say: " + text;
    }

    private IEnumerator clearAudioDebugText()
    {
        yield return new WaitForSeconds(2);
        recordDebugger.text = "";
    }
}
