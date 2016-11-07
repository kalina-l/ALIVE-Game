using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StupidIntelligence : Intelligence {

	public float ActionInterval;

	private float _timer;
	private bool _waitForAnswer;

	private List<string> _actions;

    //Attributes: Strong, Intelligence, Charisma, Constitution, Wisdom - every value 1-20; alltogether max. 100
	//Valorous - Coward
    private int _strong;
    public int strong
    {
        get
        {
            return _strong;
        }
        set
        {
            if (strong > 20)
                _strong = 20;
            else if (strong < 1)
            {
                _strong = 1;
            }
        }
    }
	//Generous - Selfish
    private int _intelligence;
    public int intelligence
    {
        get
        {
            return _intelligence;
        }
        set
        {
            if (intelligence > 20)
                _intelligence = 20;
            else if (intelligence < 1)
            {
                _intelligence = 1;
            }
        }
    }
	//Introvert - Extrovert
    private int _charisma;
    public int charisma
    {
        get
        {
            return _charisma;
        }
        set
        {
            if (charisma > 20)
                _charisma = 20;
            else if (charisma < 1)
            {
                _charisma = 1;
            }
        }
    }
	//Disciplined - Wild
    private int _constituation;
    public int constituation
    {
        get
        {
            return _constituation;
        }
        set
        {
            if (constituation > 20)
                _constituation = 20;
            else if (constituation < 1)
            {
                _constituation = 1;
            }
        }
    }
	//Forgiving - Vengeful
    private int _wisdom;
    public int wisdom
    {
        get
        {
            return _wisdom;
        }
        set
        {
            if (wisdom > 20)
                _wisdom = 20;
            else if (wisdom < 1)
            {
                _wisdom = 1;
            }
        }
    }


	//states: healthiness, hunger, social, energy, general satisfaction - every value 0-100
	private int _healthiness = 100;
	public int healthiness
	{
		get
		{
			return _healthiness;
		}
		set
		{
			if (healthiness > 100)
				_healthiness = 100;
			else if (healthiness < 0)
			{
				_healthiness = 0;
			}
			_general_satisfaction = calcMeanState ();
		}
	}
    private int _hunger = 20;
    public int hunger
    {
        get
        {
			return _hunger;
        }
        set
        {
			if (hunger > 100)
				_hunger = 100;
			else if (hunger < 0)
            {
				_hunger = 0;
            }
			_general_satisfaction = calcMeanState ();
        }
    }
    private int _social = 0;
    public int social
    {
        get
        {
			return _social;
        }
        set
        {
			if (social > 100)
				_social = 100;
			else if (social < 0)
            {
				_social = 0;
            }
			_general_satisfaction = calcMeanState ();
        }
    }
	private int _energy = 0;
	public int energy {
		get
		{
			return _energy;
		}
		set
		{
			if (energy > 100)
				_energy = 100;
			else if (energy < 0)
			{
				_energy = 0;
			}
			_general_satisfaction = calcMeanState ();
		}
	}
	private int _general_satisfaction;
    public int general_satisfaction
    {
        get
        {
            return _general_satisfaction;
        }
        set
        {
           
        }
    }


    //Actions
    private Dictionary<string, int> receivedBall = new Dictionary<string, int> { {"Creature eats the ball...", -5}, {"Your pet is playing with it.", 0}, {"The ball gets destroyed.", 3} };


    //Feedback
    private Dictionary<string, int> lastActionSet;
    private string lastAction;

    // Use this for initialization
    void Start () {
		_general_satisfaction = calcMeanState ();
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
                actionsReward -= 3;
				_waitForAnswer = false;
				break;
			case 0:
				UIManager.Instance.ReceiveMessage ("It stares blanky at you.");
                _waitForAnswer = false;
                actionsReward -= 1;
                break;
			case 1:
				UIManager.Instance.ReceiveMessage ("It claps its hands in glee.");
				_waitForAnswer = false;
                actionsReward += 3;
                break;
			}

            lastActionSet[lastAction] = actionsReward;

            //Debug.Log("condition: general_satisfaction: " + general_satisfaction);
        }
	}

    public override void receiveBall()
    {

        if(hunger > 20)
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


        if (general_satisfaction < 30)
        {
            UIManager.Instance.ReceiveMessage("It doesn't want to play anymore...");

            general_satisfaction++;
        }
        else
        {
            lastActionSet = receivedBall;
            lastAction = receivedBall.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            UIManager.Instance.ReceiveMessage(lastAction);
            _waitForAnswer = true;
        }


        hunger += 1;
        if (lastAction.Contains("eat"))
        {
			hunger -= 20;
        }

    }

	public int calcMeanState(){
		return (healthiness + hunger + social + energy) / 4;
	}
}
