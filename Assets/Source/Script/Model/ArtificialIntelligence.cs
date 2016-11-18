﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtificialIntelligence
{

    private Personality _personality;

    public float ActionInterval = 5;
    public float FeedbackInterval = 5;

	private float _actionTimer;
    private bool _waitForAnswer;
    private float _feedbackTimer;

	public float treeConstructionDuration = 5; // in sec
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
                if(!decideOnAction())
                    _personality.naturalStateReduction();
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

	public bool decideOnAction()
    {
        Activity chosenActivity = null;
        int biggestValue = int.MinValue;

        foreach (Activity activity in _personality.GetAllActivities())
        {
            int weightedReward = activity.GetWeightedReward(_personality);
            if (weightedReward > biggestValue)
            {
                chosenActivity = activity;
                biggestValue = weightedReward;
            }
        }

        if (chosenActivity != null)
        {
            chosenActivity.DoActivity(_personality, _textOutput);
            return true;
        }
        else return false;
    }

	public void createPersonalityTree() {

		treeConstruction = true;
		ApplicationManager.Instance.StartCoroutine (treeConstructionCoroutine());

		_personality.children.Clear ();
		Queue<Personality> leafsToEvaluate = new Queue<Personality>();
		leafsToEvaluate.Enqueue (_personality);

		foreach (Personality leafToEvaluate in leafsToEvaluate) {
			if (treeConstruction) {
				foreach (Activity activity in leafToEvaluate.GetAllActivities()) {
					Personality changedPersonality = new Personality (); //TODO: change constr dont forget set children and parent
					activity.DoActivity (changedPersonality);
					leafToEvaluate.children.Add (changedPersonality);
				}

				leafsToEvaluate.Dequeue ();
			}
		}
	}

	public void chooseBestFuturePersonality () {
		lastCalculatedPersonalities.Clear ();
		calcLastFuturePersonalities ();
		int counter = 0;
		int biggestReward = int.MinValue;
		for (int i = 0; i < lastCalculatedPersonalities.Count; i++) {
			if (biggestReward < lastCalculatedPersonalities[i].Evaluation()){
				biggestReward = lastCalculatedPersonalities [i].Evaluation ();
				counter = i;
			}
		}
		Personality bestFuturePersonality = lastCalculatedPersonalities [counter];
		Personality currParent;
		while (bestFuturePersonality.parent != _personality) {
			bestFuturePersonality = bestFuturePersonality.parent;
		}
		// TODO chose correct action
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

	IEnumerator treeConstructionCoroutine(){
		yield return new WaitForSeconds (treeConstructionDuration);
		treeConstruction = false;
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
