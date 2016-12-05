using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtificialIntelligence
{

    private Personality _personality;

    private float _actionDelay = 2f;
    private float _feedbackDuration = 2f;
    private float _treeConstructionDuration = 1f;

    private float _actionTimer;
    public bool _waitForAnswer;
    private float _feedbackTimer;

	private bool _treeConstruction;

    public bool NeedFeedback { get { return _waitForAnswer && !_treeConstruction; } }
    public bool NeedItems { get { return !_waitForAnswer; } }

    List<Personality> lastCalculatedPersonalities = new List<Personality>();
    List<PersonalityNode> lastCalculatedPersonalityNodes = new List<PersonalityNode>();

    private OutputViewController _textOutput;
    private Experience _lastExperience;

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

            if (_actionTimer > _actionDelay)
            {
                //_personality.printConditions();
				decideOnAction ();
                _waitForAnswer = true;
                _actionTimer = 0;
                _feedbackTimer = 0;
            }
        }
        else
        {
            if (!_treeConstruction)
            {
                _feedbackTimer += Time.deltaTime;
                if (_feedbackTimer > _feedbackDuration)
                {
                    _waitForAnswer = false;
                    _feedbackTimer = 0;
                }
            }
        }
    }

	public void decideOnAction()
	{
        ApplicationManager.Instance.StartCoroutine(createLearningTree());
        //ApplicationManager.Instance.StartCoroutine(createPersonalityTree());
    }

    private IEnumerator createLearningTree()
    {
        float timer = 0;
        _treeConstruction = true;

        PersonalityNode root = new PersonalityNode(_personality);
        Queue<PersonalityNode> leafsToEvaluate = new Queue<PersonalityNode>();
        leafsToEvaluate.Enqueue(root);

        while (leafsToEvaluate.Count != 0 && _treeConstruction)
        {
            PersonalityNode currPer = leafsToEvaluate.Dequeue();

            for(int i=0; i<currPer.ActivityIDs.Count; i++)
            {
                PersonalityNode newPerson = new PersonalityNode(currPer, _personality.GetActivity(currPer.ActivityIDs[i]).GetExperience(currPer), currPer.ActivityIDs[i]);
                currPer.Children.Add(newPerson);
                leafsToEvaluate.Enqueue(newPerson);
            }

            timer += Time.deltaTime;

            if (timer > _treeConstructionDuration)
            {
                _treeConstruction = false;
            }

            yield return 0;
        }

        PersonalityNode bestFuturePersonality = GetBestPersonality(root);
        int activityID = bestFuturePersonality.ParentActionID;

        _lastExperience = _personality.GetActivity(activityID).DoActivity(_personality, _textOutput);
    }

	private IEnumerator createPersonalityTree() {

		_treeConstruction = true;

        float timer = 0;

        _personality.parent = null;
        _personality.deepnessInParent = 0;
        _personality.storedEvaluation = 0;
		_personality.children.Clear ();

		Queue<Personality> leafsToEvaluate = new Queue<Personality>();
		leafsToEvaluate.Enqueue (_personality);

        int counter = 0;
        

		while (leafsToEvaluate.Count != 0 && _treeConstruction) {
			Personality currPer = leafsToEvaluate.Dequeue ();
			foreach (Activity activity in currPer.GetAllActivities()) {
                
				Personality changedPersonality = new Personality (currPer, activity.ID); //TODO: change constr dont forget set children and parent
                counter++;
				activity.DoActivity (changedPersonality);

				currPer.children.Add (changedPersonality);
				leafsToEvaluate.Enqueue (changedPersonality);
			}

            timer += Time.deltaTime;

            if(timer > _treeConstructionDuration)
            {
                _treeConstruction = false;
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

    public PersonalityNode GetBestPersonality(PersonalityNode root)
    {
        lastCalculatedPersonalityNodes.Clear();
        CalculatePersonalityNodes(root);

        int counter = 0;
        float biggestReward = float.MinValue;
        for (int i=0; i<lastCalculatedPersonalityNodes.Count; i++)
        {
            if(biggestReward < lastCalculatedPersonalityNodes[i].StoredEvaluation)
            {
                biggestReward = lastCalculatedPersonalityNodes[i].StoredEvaluation;
                counter = i;
            }
        }

        PersonalityNode bestFuturePersonality = lastCalculatedPersonalityNodes[counter];

        while (bestFuturePersonality.Parent != root)
        {
            bestFuturePersonality = bestFuturePersonality.Parent;
        }
        
        return bestFuturePersonality;
    }

	public Personality chooseBestFuturePersonality () {
		lastCalculatedPersonalities.Clear ();
		calcLastFuturePersonalities (_personality);
		int counter = 0;
		float biggestReward = float.MinValue;
		for (int i = 0; i < lastCalculatedPersonalities.Count; i++) {
			if (biggestReward < lastCalculatedPersonalities[i].Evaluation()){
				biggestReward = lastCalculatedPersonalities [i].Evaluation ();
				counter = i;
			}
		}
        
		Personality bestFuturePersonality = lastCalculatedPersonalities [counter];
        Debug.Log("Best Value: " + bestFuturePersonality.Evaluation());

        while (bestFuturePersonality.parent != _personality) {
			bestFuturePersonality = bestFuturePersonality.parent;
		}

        

        return bestFuturePersonality;
	}

    public void CalculatePersonalityNodes(PersonalityNode root)
    {
        if(root.Children.Count == 0)
        {
            lastCalculatedPersonalityNodes.Add(root);
        }
        else
        {
            foreach (PersonalityNode childPersonality in root.Children)
            {
                CalculatePersonalityNodes(childPersonality);
            }
        }
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
            if (_lastExperience != null)
                _lastExperience.Feedback += feedback;

            switch (feedback)
            {
                case -1:
                    _textOutput.DisplayMessage("Negative Feedback");
                    _waitForAnswer = false;
                    break;
                case 0:
                    _textOutput.DisplayMessage("Neutral Feedback");
                    _waitForAnswer = false;
                    break;
                case 1:
                    _textOutput.DisplayMessage("Positive Feedback");
                    _waitForAnswer = false;
                    break;
            }
            
        }
    }
}
