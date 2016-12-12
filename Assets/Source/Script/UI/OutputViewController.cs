using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutputViewController : AbstractViewController {

    private Text OutputText;

    private int messages;

    public OutputViewController(Transform parent)
    {
        Rect = CreateContainer("Output", parent,
            new Vector2(0, 0), new Vector2(1000, 500),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        //AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        OutputText = AddText(
            CreateContainer("OutputText", Rect,
            new Vector2(0, -20), new Vector2(920, 420),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1)),
            GraphicsHelper.Instance.UIFont, 80, TextAnchor.UpperCenter);
    }

    public void DisplayMessage(string msg)
    {
        //OutputText.text = msg;
        ApplicationManager.Instance.StartCoroutine(AnimateText(msg));
    }

    private IEnumerator AnimateText(string msg)
    {
        float timer = 0;

        OutputText.text = msg;

        while (timer < 1)
        {
            timer += Time.deltaTime * 2;

            OutputText.fontSize = (int)Mathf.Lerp(0, 80, timer);
            OutputText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.TextColorHidden, GraphicsHelper.Instance.TextColor, timer);

            yield return 0;
        }

        //Show Feedback

        //Wait for Feedback or TimeOut

            //Do Stuff when Feedback was given
        
        //Hide Feedback

        //Hide Text
    }
}
