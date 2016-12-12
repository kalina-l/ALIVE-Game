using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeedbackViewController : AbstractViewController {


    private RawImage _block;

    public FeedbackViewController(Transform parent, ArtificialIntelligence intelligence)
    {
        Rect = CreateContainer("Feedback", parent,
            new Vector2(296, 0), new Vector2(296, 572),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));
        View = Rect.gameObject;

		

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        //Buttons
        Button positiveButton = CreateButton(
                                    CreateContainer("PositiveFeedback", Rect,
                                    new Vector2(-20, -20), Vector2.one * 256,
                                    Vector2.one, Vector2.one, Vector2.one),
                                    delegate { intelligence.ReceiveFeedback(1); }
                                    );

        AddSprite(positiveButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackPositiveSprite, GraphicsHelper.Instance.SpriteColorWhite);

        Button negativeButton = CreateButton(
                                    CreateContainer("NegativeFeedback", Rect,
                                    new Vector2(-20, 20), Vector2.one * 256,
                                    Vector2.right, Vector2.right, Vector2.right),
                                    delegate { intelligence.ReceiveFeedback(-1); }
                                    );

        AddSprite(negativeButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackNegativeSprite, GraphicsHelper.Instance.SpriteColorWhite);
    }

    public void ShowFeedback(bool show)
    {
        //
    }
    

	private IEnumerator updateFeedbackButtons(ArtificialIntelligence intelligence){
		while (true) {
            if(intelligence.NeedFeedback)
            {
                Rect.anchoredPosition = new Vector2(296, 0);
            }
            else
            {
                Rect.anchoredPosition = Vector2.zero;
            }
			yield return 0;
		}
	}
}
