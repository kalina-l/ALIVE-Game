using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Activity {
    public int ID { get; set; }
    public string Name { get; set; }

    public List<Reward> RewardList;

    public List<Reward> ExpectedRewardList;

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
        RewardList = new List<Reward>();
    }

    public Activity AddReward(Reward reward)
    {
        if(!RewardList.Contains(reward))
        {
            RewardList.Add(reward);
        }
        else
        {
            Debug.LogWarning(reward.ID + " is already added as a reward");
        }

        return this;
    }

	public void DoActivity(Personality parentPersonality, OutputViewController textOutput)
    {
        foreach(Reward reward in RewardList)
        {
            reward.DoReward(parentPersonality);
        }

        textOutput.DisplayMessage(feedBackString);
    }

    public void DoActivity(Personality parentPersonality)
    {
        foreach (Reward reward in RewardList)
        {
            reward.DoReward(parentPersonality);
        }
    }
}
