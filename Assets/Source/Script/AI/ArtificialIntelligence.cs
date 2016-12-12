using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtificialIntelligence
{

    private Personality _personality;

    private float _actionDelay = 2f;
    private float _feedbackDuration = 2f;
    private float _treeConstructionDuration = 10f;

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
    private Activity _lastActivity;

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
        //TODO: Start seperate thread for this
		createPartialLearningTree ();
    }

	private void createPartialLearningTree(){
		int maxDepth = 5;
		PersonalityNode root = new PersonalityNode(_personality);

		dfs (root, maxDepth);

        if (root.Children.Count != 0)
        {
            int activityID = root.Children[0].ParentActionID;
            _lastActivity = _personality.GetActivity(activityID);
            _lastExperience = _lastActivity.DoActivity(_personality, _textOutput);

            ApplicationManager.Instance.PersonalityUpdate();
        }
        else
        {
            Debug.LogError("No Child generated. why?");
        }
	}

	private void dfs(PersonalityNode pn, int maxDepth){

		if (pn.Depth < maxDepth && !pn.visited) {
			for (int i = 0; i < pn.ActivityIDs.Count; i++) {
				PersonalityNode newPerson = new PersonalityNode (pn, _personality.GetActivity (pn.ActivityIDs [i]).GetExperience (pn), pn.ActivityIDs [i], _personality.GetActivity(pn.ActivityIDs[i]).Feedback.GetFeedback(pn.Needs));
				pn.Children.Add (newPerson);
				dfs (newPerson, maxDepth);
			}
			pn.visited = true;
		}

		if ((pn.Depth >= maxDepth || pn.visited) && pn.Parent!=null) {
			if (pn.Parent.BestChildsEvaluation < (pn.SelfEvaluation + pn.BestChildsEvaluation)) {
				pn.Parent.removeAllChildrenExceptOne (pn);
				pn.Parent.BestChildsEvaluation = (pn.SelfEvaluation + pn.BestChildsEvaluation);
			} else {
				pn.Parent.removeChildReference (pn);
			}
		}
	}

    public void ReceiveFeedback(int feedback)
    {
        if (_waitForAnswer)
        {
            if (_lastActivity != null)
                _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);

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
