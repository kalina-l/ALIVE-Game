using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIManager : MonoBehaviour {

	public Button ApproveButton;
	public Button NeutralButton;
	public Button DisapproveButton;

	public Text OutputText;

	private int messages;

	public static UIManager Instance;

	public Intelligence AI;

	void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		ApproveButton.onClick.AddListener (delegate {
			SendFeedback(1);
		});

		NeutralButton.onClick.AddListener (delegate {
			SendFeedback(0);
		});

		DisapproveButton.onClick.AddListener (delegate {
			SendFeedback(-1);
		});
	
	}

	public void SendFeedback(int score)
	{
		AI.ReceiveFeedback (score);
	}

	public void ReceiveMessage(string msg)
	{
		OutputText.text += msg + "\n" + "\n";

		if(messages > 6)
		{
			OutputText.rectTransform.anchoredPosition += Vector2.up * 80;
			OutputText.rectTransform.sizeDelta += Vector2.up * 80;
		}

		messages++;
	}
}
