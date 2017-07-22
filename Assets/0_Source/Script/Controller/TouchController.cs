using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchController : AbstractViewController {

    private FeedbackViewController _feedbackViewController;

    private Lean.Touch.LeanTouchEvents _touchEvents;

    private Image fistImage;
    private Image handImage;

    private bool handIsAnimating;
    private IEnumerator showHandRoutine;

    public TouchController(FeedbackViewController feedbackViewConroller, RectTransform parentRect)
    {
        _feedbackViewController = feedbackViewConroller;

        GameObject LeanTouchObject = new GameObject("LeanTouch");
        LeanTouchObject.AddComponent<Lean.Touch.LeanTouch>();
        _touchEvents = LeanTouchObject.AddComponent<Lean.Touch.LeanTouchEvents>();
        _touchEvents.Setup(this);
        _touchEvents.setLemo(GraphicsHelper.Instance.lemo.GetComponent<Collider>());


        fistImage = AddSprite(
            CreateContainer("FistFeedback", parentRect, new Vector2(0, 0), new Vector2(128, 128), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)),
            GraphicsHelper.Instance.fist, GraphicsHelper.Instance.SpriteColorWhite);
        fistImage.enabled = false;

        handImage = AddSprite(
            CreateContainer("HandFeedback", parentRect, new Vector2(0, 0), new Vector2(128, 128), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)),
            GraphicsHelper.Instance.hand, GraphicsHelper.Instance.SpriteColorWhite);
        handImage.enabled = false;
    }

    public void SendFeedback(int feedback)
    {
        _feedbackViewController.SendFeedBack(feedback, FeedbackType.Touch);
    }

    private Vector2 ScreenToCanvasPosition(Vector2 screenPosition)
    {
        float x = screenPosition.x / Screen.width;
        float y = screenPosition.y / Screen.height;

        return new Vector2(x * 1080, y * 1920);
    }

    public void ShowFistFeedback(Vector2 position)
    {
        position = ScreenToCanvasPosition(position);
        fistImage.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        fistImage.enabled = true;
        ApplicationManager.Instance.StartCoroutine(ShowFistRoutine());
    }

    private IEnumerator ShowFistRoutine()
    {
        float timer = 0;
        GraphicsHelper graphics = GraphicsHelper.Instance;
        Vector3 currentAngle = fistImage.rectTransform.eulerAngles;

        while (timer < 1)
        {
            timer += Time.deltaTime * 8;

            currentAngle = new Vector3(
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(0, 45, timer));
            fistImage.rectTransform.eulerAngles = currentAngle;

            yield return 0;
        }
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4;
            yield return 0;
        }

        fistImage.enabled = false;
    }

    public void StartPetting()
    {
        _feedbackViewController.SetIsRecording(true);
    }

    public void ShowPetFeedback(Vector2 position)
    {
        position = ScreenToCanvasPosition(position);
        handImage.rectTransform.anchoredPosition = position;
        if (!handImage.enabled) handImage.enabled = true;
        if (!handIsAnimating)
        {
            showHandRoutine = ShowPetRoutine();
            ApplicationManager.Instance.StartCoroutine(showHandRoutine);
        }
    }

    private IEnumerator ShowPetRoutine()
    {
        handIsAnimating = true;
        float timer = 0;
        GraphicsHelper graphics = GraphicsHelper.Instance;
        Vector3 currentAngle = handImage.rectTransform.eulerAngles;

        while (timer < 1)
        {
            timer += Time.deltaTime;

            currentAngle = new Vector3(
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(0, -30, timer));
            handImage.rectTransform.eulerAngles = currentAngle;

            yield return 0;
        }
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;

            currentAngle = new Vector3(
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(0, 0, timer),
             Mathf.LerpAngle(-30, 0, timer));
            handImage.rectTransform.eulerAngles = currentAngle;

            yield return 0;
        }
        handIsAnimating = false;
    }

    public void EndPetFeedback()
    {
        if (showHandRoutine != null)
        {
            ApplicationManager.Instance.StopCoroutine(showHandRoutine);
        }
        _feedbackViewController.SetIsRecording(false);
        handIsAnimating = false;
        handImage.enabled = false;
    }

}
