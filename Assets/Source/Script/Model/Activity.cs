using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Activity {

    public Dictionary<string, int> Rewards;

    private string _feedBackString;
    public string feedBackString 
    {
            get
        {
                return _feedBackString;
            }
            set
        {
                _feedBackString = value;
            }
        }

        public Activity(string feedBackString)
    {
        this.feedBackString = feedBackString;
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

	public void DoActivity(Personality parentPersonality, OutputViewController textOutput)
    {
        //Change the Conditions of the Personality, depending on the Action
        foreach(KeyValuePair<string, int> reward in Rewards)
        {
            parentPersonality.GetCondition(reward.Key).value += reward.Value;
        }

        textOutput.DisplayMessage(feedBackString);
    }

	public void DoActivity(Personality parentPersonality)
	{
		//Change the Conditions of the Personality, depending on the Action
		foreach(KeyValuePair<string, int> reward in Rewards)
		{
			parentPersonality.GetCondition(reward.Key).value += reward.Value;
		}
	}


	// TODO: Rewards methods not needed anymore?
    /*
     * This method increases rewards for these conditions, which are currently low. 
     */
    public int GetWeightedReward(Personality personality) {
        int weightedReward = 0;
        string benefits = "Benefits of '" + feedBackString + "' are: ";
        foreach (KeyValuePair<string, Condition> condition in personality.GetConditions()) {
            // print rewards unequal 0
            int addedValue = (100 - condition.Value.value) * Rewards[condition.Key];
            if(addedValue != 0) benefits += (condition.Key + ": " + addedValue + ", ");
            weightedReward += addedValue;
        }
        Debug.Log(benefits + weightedReward);
        return weightedReward;
    }

	public int getExpectedReward(){
		//TODO: change total reward to expected reward
		return GetTotalReward ();
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
