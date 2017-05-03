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

}
