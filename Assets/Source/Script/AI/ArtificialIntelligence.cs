using UnityEngine;
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
		createPersonalityTree ();
		Personality bestFuturePersonality = chooseBestFuturePersonality ();
		int activityID = bestFuturePersonality.parentActionID;

		_personality.GetActivity(activityID).DoActivity (_personality, _textOutput);
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
					Personality changedPersonality = new Personality (leafToEvaluate, activity.ID); //TODO: change constr dont forget set children and parent
					activity.DoActivity (changedPersonality);
					leafToEvaluate.children.Add (changedPersonality);
				}

				leafsToEvaluate.Dequeue ();
			}
		}

		while (leafsToEvaluate.Count != 0) {
			Personality currPer = leafsToEvaluate.Dequeue ();
			if (treeConstruction) {
				foreach (Activity activity in currPer) {
					Personality changedPersonality = new Personality (currPer, activity.ID); //TODO: change constr dont forget set children and parent
					activity.DoActivity (changedPersonality);
					currPer.children.Add (changedPersonality);
					leafsToEvaluate.Enqueue (changedPersonality);
				}
			} else
				break;
		}
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
		Personality currParent;
		while (bestFuturePersonality.parent != _personality) {
			bestFuturePersonality = bestFuturePersonality.parent;
		}
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
