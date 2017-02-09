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
            new Vector2(0, -850), new Vector2(920, 420),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1)),
            GraphicsHelper.Instance.UIFont, 80, TextAnchor.UpperCenter);
    }

    public void DisplayMessage(string msg)
    {
        //OutputText.text = msg;
        ApplicationManager.Instance.StartCoroutine(AnimateText(msg));
    }

    public void ShowFeedback(int feedback)
    {
        if(feedback != 0)
        {
            if(feedback > 0)
            {
                OutputText.color = new Color32(0, 250, 50, 155);
            }
            else
            {
                OutputText.color = new Color32(200, 0, 0, 155);
            }
        }
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
        
    }
}
