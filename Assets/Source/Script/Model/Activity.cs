using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Activity {

    public Dictionary<string, int> Rewards;
    private Personality _parentPersonality;

    private string _feedBackString;

	public Activity(Personality parentPersonality, string feedBackString)
    {
        _parentPersonality = parentPersonality;
        _feedBackString = feedBackString;
        Rewards = new Dictionary<string, int>();
    }

    public Activity AddReward(string conditionIdentifier, int rewardValue)
    {
        if(!Rewards.ContainsKey(conditionIdentifier))
        {
            Rewards[conditionIdentifier] = rewardValue;
        }
        else
        {
            Debug.LogWarning(conditionIdentifier + " is already added as a reward");
        }

        return this;
    }

    public void DoActivity()
    {
        //Change the Conditions of the Personality, depending on the Action
        foreach(KeyValuePair<string, int> reward in Rewards)
        {
            _parentPersonality.GetCondition(reward.Key).value += reward.Value;
        }

        Debug.Log(_feedBackString);
    }

    public int GetTotalReward()
    {
        int returnValue = 0;

        foreach (KeyValuePair<string, int> reward in Rewards)
        {
            returnValue += reward.Value;
        }

        return returnValue;
    }
}
