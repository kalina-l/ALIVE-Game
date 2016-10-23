using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StupidIntelligence : Intelligence {

	public float ActionInterval;

	private float _timer;
	private bool _waitForAnswer;

	private List<string> _actions;

    //Properties
    private int _disciplined;
    public int disciplined
    {
        get
        {
            return _disciplined;
        }
        set
        {
            if (disciplined > 100)
                _disciplined = 100;
            else if (disciplined < 0)
            {
                _disciplined = 0;
            }
        }
    }
    private int _forgiving;
    public int forgiving
    {
        get
        {
            return _forgiving;
        }
        set
        {
            if (forgiving > 100)
                _forgiving = 100;
            else if (forgiving < 0)
            {
                _forgiving = 0;
            }
        }
    }
    

    //Statuses
    private Dictionary<string, int> receivedBall = new Dictionary<string, int> { {"Creature eats the ball...", -5}, {"Your pet is playing with it.", 0}, {"The ball gets destroyed.", 3} };
    private Dictionary<string, int> conditions = new Dictionary<string, int> { {"hungry", 0}, { "general satisfaction", 80 } };

    //Feedback
    private Dictionary<string, int> lastActionSet;
    private string lastAction;

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
		//if(!_waitForAnswer) {
		//	_timer += Time.deltaTime;

		//	if(_timer > ActionInterval) {
		//		int randomID = (int)(Random.value * (float)_actions.Count);
		//		UIManager.Instance.ReceiveMessage (_actions [randomID]);
		//		_waitForAnswer = true;
		//		_timer = 0;
		//	}
		//}
	}

	public override void ReceiveFeedback(int feedback)
	{
		if (_waitForAnswer) 
		{
            int actionsReward;
            lastActionSet.TryGetValue(lastAction, out actionsReward);

            switch (feedback)
			{
			case -1:
				UIManager.Instance.ReceiveMessage ("It seems to feel bad about it.");
                conditions["general satisfaction"] -= 3;
                actionsReward -= 3;
				_waitForAnswer = false;
				break;
			case 0:
				UIManager.Instance.ReceiveMessage ("It stares blanky at you.");
                conditions["general satisfaction"] -= 1;
                _waitForAnswer = false;
                actionsReward -= 1;
                break;
			case 1:
				UIManager.Instance.ReceiveMessage ("It claps its hands in glee.");
                conditions["general satisfaction"] += 3;
				_waitForAnswer = false;
                actionsReward += 3;
                break;
			}

            lastActionSet[lastAction] = actionsReward;

            foreach(KeyValuePair<string, int> state in conditions)
            {
                Debug.Log("condition: " + state.Key + ": " + state.Value);
            }
		}
	}

    public override void receiveBall()
    {

        if(conditions["hungry"] > 20)
        {
            UIManager.Instance.ReceiveMessage("It is hungry and wants to eat something...");
            List<string> keys = new List<string>(receivedBall.Keys);
            foreach(string entry in keys)
            {
                if (entry.Contains("eat"))
                {
                    receivedBall[entry] += 10;
                }
            }
        }

        foreach (KeyValuePair<string, int> entry in receivedBall)
        {
            Debug.Log("Action: " + entry.Key + " gives " + entry.Value + " reward points.");
        }


        if (conditions["general satisfaction"] < 30)
        {
            UIManager.Instance.ReceiveMessage("It doesn't want to play anymore...");

            conditions["general satisfaction"]++;
        }
        else
        {
            lastActionSet = receivedBall;
            lastAction = receivedBall.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            UIManager.Instance.ReceiveMessage(lastAction);
            _waitForAnswer = true;
        }


        conditions["hungry"] += 1;
        if (lastAction.Contains("eat"))
        {
            conditions["hungry"] -= 20;
        }

    }
}
