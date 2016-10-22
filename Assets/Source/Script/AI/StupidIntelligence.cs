using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StupidIntelligence : Intelligence {

	public float ActionInterval;

	private float _timer;
	private bool _waitForAnswer;

	private List<string> _actions;

	// Use this for initialization
	void Start () {
		_actions = new List<string> () {
			"It clacks two coconuts together and pretends to be a horse.",
			"It pulls out some fireworks and lights them.",
			"It stick its finger up its nose.",
			"I better not say what its doing now...",
			"It dances to some tune playing inside its head.",
			"It tries to smell you."
		};

		_timer = 0;
		UIManager.Instance.ReceiveMessage ("You encounter a strange creature.");
		UIManager.Instance.ReceiveMessage ("It seems really stupid and needs your help.");
	}
	
	// Update is called once per frame
	void Update () {
		if(!_waitForAnswer) {
			_timer += Time.deltaTime;

			if(_timer > ActionInterval) {
				int randomID = (int)(Random.value * (float)_actions.Count);
				UIManager.Instance.ReceiveMessage (_actions [randomID]);
				_waitForAnswer = true;
				_timer = 0;
			}
		}
	}

	public override void ReceiveFeedback(int feedback)
	{
		if (_waitForAnswer) 
		{
			switch(feedback)
			{
			case -1:
				UIManager.Instance.ReceiveMessage ("It seems to feel bad about it.");
				_waitForAnswer = false;
				break;
			case 0:
				UIManager.Instance.ReceiveMessage ("It stares blanky at you.");
				_waitForAnswer = false;
				break;
			case 1:
				UIManager.Instance.ReceiveMessage ("It claps its hands in glee.");
				_waitForAnswer = false;
				break;
			}
		}
	}
}
