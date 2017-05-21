using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;

public class VideoFeedbackController
{

    private FeedbackViewController _feedbackViewController;
    
    private VideoInput videoInput;
    private PlayerEmotions playerEmotions;

    private EmotionSliderViewController slider;

    public VideoFeedbackController(FeedbackViewController feedbackViewConroller, RectTransform rect)
    {
        _feedbackViewController = feedbackViewConroller;
        
        GameObject videoRecorder = GameObject.Find("VideoRecorder");
        videoInput = videoRecorder.AddComponent<VideoInput>();
        playerEmotions = videoRecorder.AddComponent<PlayerEmotions>();
        videoInput.setup(playerEmotions);
        playerEmotions.setup(this);

        slider = new EmotionSliderViewController(rect, "", "VideoFeedbackSlider");
       
    }

    /*private IEnumerator setupDetector ()
    {
        bool setupCompleted = false;
        while(!setupCompleted)
        {
            if(detector.IsRunning)
            {
                detector.SetEmotionState(Emotions.Joy, true);
                detector.SetEmotionState(Emotions.Anger, true);
                detector.SetEmotionState(Emotions.Sadness, true);
                detector.SetExpressionState(Expressions.Smile, true);
                detector.SetExpressionState(Expressions.BrowRaise, true);
                setupCompleted = true;
            }
            yield return 0;
        }
    }*/

    public void StartRecording()
    {
        slider.SetActive(true);
        _feedbackViewController.SetIsRecording(true);
        videoInput.startRecording();
    }

    public void StopRecording()
    {
        slider.ResetSlider();
        slider.SetActive(false);
        videoInput.stopRecording();
    }

    public void SendFeedback(int feedback)
    {
        _feedbackViewController.SendFeedBack(feedback, FeedbackType.Video);
    }

    public EmotionSliderViewController getSlider() {
        return slider;
    }
}
