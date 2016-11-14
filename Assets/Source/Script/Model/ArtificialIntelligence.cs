using UnityEngine;
using System.Collections;

public class ArtificialIntelligence {

    private Personality _personality;

    public float ActionInterval = 5;
    public float FeedbackInterval = 5;

    private float _timer;
    private bool _waitForAnswer;
    private float _feedbackTimer;

    public ArtificialIntelligence(Personality personality)
    {
        _personality = personality;
    }
	
	// Update is called once per frame
	public void TimeStep () {
        if (!_waitForAnswer)
        {
            _timer += Time.deltaTime;

            if (_timer > ActionInterval)
            {
                _personality.printConditions();
                if(!decideOnAction())
                    _personality.naturalStateReduction();
                _waitForAnswer = true;
                _timer = 0;
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

        foreach(Activity activity in _personality.GetAllActivities())
        {
            int weightedReward = activity.GetWeightedReward(_personality);
            if (weightedReward > biggestValue)
            {
                chosenActivity = activity;
                biggestValue = weightedReward;
            }
        }

        if (chosenActivity != null) {
            chosenActivity.DoActivity(_personality);
            return true;
        } else return false;
    }
}
