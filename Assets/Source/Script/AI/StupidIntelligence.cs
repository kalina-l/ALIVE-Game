﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

enum Activities { Idle, Sleeping, Playing };

public class StupidIntelligence : Intelligence {

	public float ActionInterval;
    public float FeedbackInterval;

	private float _timer;
	private bool _waitForAnswer;
    private float _feedbackTimer;

	private List<string> _actions;
    private Activities activity;


	//states: healthiness, hunger, social, energy, general satisfaction - every value 0-100
	private Condition healthiness;
	private Condition hunger;
	private Condition social;
	private Condition energy;
	private Condition general_satisfaction;


    //Actions
    private Dictionary<string, int> receivedBall = new Dictionary<string, int> { {"Creature eats the ball...", -5}, {"Your pet is playing with it.", 0}, {"The ball gets destroyed.", 3} };


    //Feedback
    private Dictionary<string, int> lastActionSet;
    private string lastAction;

    // Use this for initialization
    void Start () {
		healthiness = new Condition (100, new int[]{ 0, 10, 20, 30, 40, 50, 60 });
		hunger = new Condition (100, new int[]{ 0, 10, 20, 30, 40, 50, 60 });
		social = new Condition (100, new int[]{ 0, 10, 20, 30, 40, 50, 60 });
		energy = new Condition (100, new int[]{ 0, 10, 20, 30, 40, 50, 60 });
		general_satisfaction = new Condition (100, new int[]{ 0, 10, 20, 30, 40, 50, 60 });


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
        if (!_waitForAnswer) {
            _timer += Time.deltaTime;

            if (_timer > ActionInterval) {
                //UIManager.Instance.ReceiveMessage(_actions[randomID]);
                naturalStateReduction();
                decideOnAction();
                _waitForAnswer = true;
                _timer = 0;
            }
        }
        else {
            _feedbackTimer += Time.deltaTime;
            if (_feedbackTimer > FeedbackInterval) {
                _waitForAnswer = false;
                _feedbackTimer = 0;
            }
        }
    }

    public void decideOnAction() {
        Action action = null;
        int biggestValue = -5;

        // Basic actions
        int sleepReward = shouldISleep();
        if(sleepReward > biggestValue) {
            action = () => sleep();
            biggestValue = sleepReward;
        }

        int wakeUpReward = shouldIWakeUp();
        if (wakeUpReward > biggestValue) {
            action = () => wakeUp();
            biggestValue = wakeUpReward;
        }

        int plainPlayReward = shouldIPlainPlay();
        if (plainPlayReward > biggestValue) {
            action = () => plainPlay();
            biggestValue = plainPlayReward;
        }

        // Interactions with objects
        List<Item> items = SceneManager.Instance.getItems();
        int eatFoodReward = 0;
        int playWithBallReward = 0;
        foreach(Item item in items) {
            if(item is Food) {
                eatFoodReward = shouldIEatFood();
                if (eatFoodReward > biggestValue) {
                    action = () => eatFood();
                    biggestValue = eatFoodReward;
                }
            }
            if (item is Ball) {
                playWithBallReward = shouldIPlayBall();
                if (playWithBallReward > biggestValue) {
                    action = () => playBall();
                    biggestValue = playWithBallReward;
                }
            }
        }
        if (action != null) action.Invoke();
    }

    private int shouldISleep() {
        // TODO: implement logic (exptected value for this activity)
		int value = 0;
		return value;
    }

    private int shouldIWakeUp() {
        // TODO: implement logic (exptected value for this activity)
        if (activity == Activities.Sleeping)
            return 7;
        else return 0;
    }

    private int shouldIPlainPlay() {
        // TODO: implement logic (exptected value for this activity)
        return 3;
    }

    private int shouldIEatFood() {
        // TODO: implement logic (exptected value for this activity)
        return 10;
    }

    private int shouldIPlayBall() {
        // TODO: implement logic (exptected value for this activity)
        return 10;
    }

    private void sleep() {
        Debug.Log("Monster falls asleep.");
    }
    private void wakeUp() {
        Debug.Log("Monster falls asleep.");
    }
    private void plainPlay() {
        Debug.Log("Monster plays with his tail.");
    }
    private void eatFood() {
        Debug.Log("Monster eats given food.");
    }
    private void playBall() {
        Debug.Log("Monster plays with the ball.");
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

        /* if(hunger > 20)
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
        } */

    }

    private void naturalStateReduction() {
		hunger.value -= 3;
		energy.value -= 2;
        social.value -= 1;
    }


}
