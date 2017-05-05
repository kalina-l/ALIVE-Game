using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController {

    private FeedbackViewController _feedbackViewController;

    private Lean.Touch.LeanTouchEvents _touchEvents;

    public TouchController(FeedbackViewController feedbackViewConroller)
    {
        _feedbackViewController = feedbackViewConroller;

        GameObject LeanTouchObject = new GameObject("LeanTouch");
        LeanTouchObject.AddComponent<Lean.Touch.LeanTouch>();
        _touchEvents = LeanTouchObject.AddComponent<Lean.Touch.LeanTouchEvents>();
        _touchEvents.Setup(this);
        _touchEvents.setLemo(GraphicsHelper.Instance.lemo.GetComponent<Collider>());
    }

    public void SendFeedback(int feedback)
    {
        _feedbackViewController.SendFeedBack(feedback);
    }

    public void ShowFistFeedback(Vector2 position)
    {
        _feedbackViewController.ShowFistFeedback(ScreenToCanvasPosition(position));
    }

    public void ShowPetFeedback(Vector2 position)
    {
        _feedbackViewController.ShowPetFeedback(ScreenToCanvasPosition(position));
    }

    public void EndPetFeedback()
    {
        _feedbackViewController.EndPetFeedback();
    }

    private Vector2 ScreenToCanvasPosition(Vector2 screenPosition)
    {
        float x = screenPosition.x / Screen.width;
        float y = screenPosition.y / Screen.height;

        return new Vector2(x * 1080, y * 1920);
    }

}
