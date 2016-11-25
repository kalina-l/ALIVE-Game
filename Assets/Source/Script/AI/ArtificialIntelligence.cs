﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtificialIntelligence
{

    private Personality _personality;

    public float ActionInterval = 1.5f;
    public float FeedbackInterval = 0.5f;

	private float _actionTimer;
    private bool _waitForAnswer;
    private float _feedbackTimer;

	public float treeConstructionDuration = 0.5f; // in sec
	public bool treeConstruction;

	List<Personality> lastCalculatedPersonalities = new List<Personality>(); 

    private OutputViewController _textOutput;

    public ArtificialIntelligence(Personality personality, OutputViewController textOutput)
    {
        _personality = personality;
        _textOutput = textOutput;
    }

    // Update is called once per frame
    public void TimeStep()
    {
        if (!_waitForAnswer)
        {
            _actionTimer += Time.deltaTime;

            if (_actionTimer > ActionInterval)
            {
                _personality.printConditions();
				decideOnAction ();
                _waitForAnswer = true;
                _actionTimer = 0;
            }
        }
        else
        {
            _feedbackTimer += Time.deltaTime;
            if (_feedbackTimer > FeedbackInterval)
            {
                _waitForAnswer = false;
                _feedbackTimer = 0;
            }
        }
    }

	public void decideOnAction()
	{
        ApplicationManager.Instance.StartCoroutine(createPersonalityTree());
    }

	private IEnumerator createPersonalityTree() {

		treeConstruction = true;

        float timer = 0;

		_personality.children.Clear ();
		Queue<Personality> leafsToEvaluate = new Queue<Personality>();
		leafsToEvaluate.Enqueue (_personality);

        int counter = 0;
        

		while (leafsToEvaluate.Count != 0 && treeConstruction) {
			Personality currPer = leafsToEvaluate.Dequeue ();

			foreach (Activity activity in currPer.GetAllActivities()) {
                
				Personality changedPersonality = new Personality (currPer, activity.ID); //TODO: change constr dont forget set children and parent
                counter++;
				activity.DoActivity (changedPersonality);
				currPer.children.Add (changedPersonality);
				leafsToEvaluate.Enqueue (changedPersonality);
			}

            timer += Time.deltaTime;

            if(timer > treeConstructionDuration)
            {
                treeConstruction = false;
            }

            yield return 0;
		}
        

        Personality bestFuturePersonality = chooseBestFuturePersonality();
        int activityID = bestFuturePersonality.parentActionID;

        for(int i=0; i<_personality.children.Count; i++)
        {
            Debug.Log(_personality.GetActivity(_personality.children[i].parentActionID).feedBackString + ": " + _personality.children[i].Evaluation());
        }

        Debug.Log("Do Activity: " + _personality.GetActivity(activityID).feedBackString + " - " + bestFuturePersonality.Evaluation());

        _personality.GetActivity(activityID).DoActivity(_personality, _textOutput);
    }

	public Personality chooseBestFuturePersonality () {
		lastCalculatedPersonalities.Clear ();
		calcLastFuturePersonalities (_personality);
		int counter = 0;
		int biggestReward = int.MinValue;
		for (int i = 0; i < lastCalculatedPersonalities.Count; i++) {
			if (biggestReward < lastCalculatedPersonalities[i].Evaluation()){
				biggestReward = lastCalculatedPersonalities [i].Evaluation ();
				counter = i;
			}
		}
        
		Personality bestFuturePersonality = lastCalculatedPersonalities [counter];
		while (bestFuturePersonality.parent != _personality) {
			bestFuturePersonality = bestFuturePersonality.parent;
		}

        Debug.Log("Best Value: " + bestFuturePersonality.Evaluation());

        return bestFuturePersonality;
	}

	public void calcLastFuturePersonalities (Personality personality) {
		if (personality.children.Count == 0) {
			lastCalculatedPersonalities.Add (personality);
		} else {
			foreach (Personality childPersonality in personality.children){
				calcLastFuturePersonalities (childPersonality);
			}
		}
	}

    public void ReceiveFeedback(int feedback)
    {
        if (_waitForAnswer)
        {
            switch (feedback)
            {
                case -1:
                    _textOutput.DisplayMessage("It seems to feel bad about it.");
                    _waitForAnswer = false;
                    break;
                case 0:
                    _textOutput.DisplayMessage("It stares blanky at you.");
                    _waitForAnswer = false;
                    break;
                case 1:
                    _textOutput.DisplayMessage("It claps its hands in glee.");
                    _waitForAnswer = false;
                    break;
            }
            
        }
    }
}
