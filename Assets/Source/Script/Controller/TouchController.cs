using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController {

    private FeedbackViewController _feedbackViewController;

    private Lean.Touch.LeanTouchEvents _touchEventsController;

    public TouchController(FeedbackViewController feedbackViewConroller)
    {
        _feedbackViewController = feedbackViewConroller;

        GameObject LeanTouchObject = new GameObject("LeanTouch");
        LeanTouchObject.AddComponent<Lean.Touch.LeanTouch>();
        _touchEventsController = LeanTouchObject.AddComponent<Lean.Touch.LeanTouchEvents>();

        _touchEventsController.setLemo(GraphicsHelper.Instance.lemo.GetComponent<Collider>());
    }

}
