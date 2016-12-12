using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeedbackViewController : AbstractViewController {


    private RawImage _block;

    public FeedbackViewController(Transform parent, ArtificialIntelligence intelligence)
    {
        Rect = CreateContainer("Feedback", parent,
            new Vector2(0, 380), new Vector2(1080, 380),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        View = Rect.gameObject;

		

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        //Buttons
        Button positiveButton = CreateButton(
                                    CreateContainer("PositiveFeedback", Rect,
                                    Vector2.one * 40, Vector2.one * 300,
                                    Vector2.zero, Vector2.zero, Vector2.zero),
                                    delegate { intelligence.ReceiveFeedback(1); }
                                    );

        AddSprite(positiveButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackPositiveSprite, GraphicsHelper.Instance.SpriteColorWhite);

        Button negativeButton = CreateButton(
                                    CreateContainer("NegativeFeedback", Rect,
                                    new Vector2(-40, 40), Vector2.one * 300,
                                    Vector2.right, Vector2.right, Vector2.right),
                                    delegate { intelligence.ReceiveFeedback(-1); }
                                    );

        AddSprite(negativeButton.GetComponent<RectTransform>(), GraphicsHelper.Instance.feedbackNegativeSprite, GraphicsHelper.Instance.SpriteColorWhite);

        _block = AddImage(CreateContainer("Feedback", parent,
            Vector2.zero, new Vector2(1080, 380),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0)),
            null, new Color(0,0,0,0.6f));

        ApplicationManager.Instance.StartCoroutine(updateFeedbackButtons(intelligence));
    }

	private IEnumerator updateFeedbackButtons(ArtificialIntelligence intelligence){
		while (true) {
			_block.enabled = !intelligence.NeedFeedback;
			yield return 0;
		}
	}
}
