using UnityEngine;
using System.Collections;

public class ArtificialIntelligence
{

    private Personality _personality;

    public float ActionInterval = 5;
    public float FeedbackInterval = 5;

    private float _timer;
    private bool _waitForAnswer;
    private float _feedbackTimer;

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
            _timer += Time.deltaTime;

            if (_timer > ActionInterval)
            {
                decideOnAction();
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

    public void decideOnAction()
    {
        Activity chosenActivity = null;
        int biggestValue = -5;

        foreach (Activity activity in _personality.GetAllActivities())
        {
            if (activity.GetTotalReward() > biggestValue)
            {
                chosenActivity = activity;
                biggestValue = activity.GetTotalReward();
            }
        }

        if (chosenActivity != null) chosenActivity.DoActivity(_personality, _textOutput);
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
