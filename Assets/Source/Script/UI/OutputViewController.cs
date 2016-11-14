using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutputViewController : AbstractViewController {

    private Text OutputText;

    private int messages;

    public OutputViewController(Transform parent)
    {
        Rect = CreateContainer("Output", parent,
            new Vector2(0, -40), new Vector2(1000, 500),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
        View = Rect.gameObject;

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        Mask mask = View.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        OutputText = AddText(
            CreateContainer("OutputText", Rect,
            new Vector2(0, -210), new Vector2(920, 420),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1)),
            GraphicsHelper.Instance.UIFont, 40, TextAnchor.UpperCenter);
    }

    public void DisplayMessage(string msg)
    {
        OutputText.text += msg + "\n" + "\n";

        if (messages > 2)
        {
            OutputText.rectTransform.anchoredPosition += Vector2.up * 90;
            OutputText.rectTransform.sizeDelta += Vector2.up * 90;
        }

        messages++;
    }
}
