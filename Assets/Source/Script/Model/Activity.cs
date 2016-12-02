using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Activity {
    public int ID { get; set; }
    public string Name { get; set; }

    public List<Reward> RewardList;

    public List<Reward> ExpectedRewardList;

	public Item item;
	public int useConsume;

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

	public Activity(int ID, string Name, Item item, int useConsume, string feedBackString)
    {
		this.ID = ID;
		this.Name = Name;
		this.item = item;
		this.useConsume = useConsume;
        this.feedBackString = feedBackString;
        RewardList = new List<Reward>();
		item.AddActivity (this);
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
        Dictionary<NeedType, Need> need = new Dictionary<NeedType, Need>();

        foreach(KeyValuePair<NeedType, Need> kvp in parentPersonality.Conditions)
        {
            need[kvp.Key] = kvp.Value.Copy();
        }

        foreach(Reward reward in RewardList)
        {
            reward.DoReward(parentPersonality, need);
        }

        textOutput.DisplayMessage(feedBackString);
    }

    public void DoActivity(Personality parentPersonality)
    {
        Dictionary<NeedType, Need> need = new Dictionary<NeedType, Need>();

        foreach (KeyValuePair<NeedType, Need> kvp in parentPersonality.Conditions)
        {
            need[kvp.Key] = kvp.Value.Copy();
        }

        foreach (Reward reward in RewardList)
        {
            reward.DoReward(parentPersonality, need);
        }

        parentPersonality.storedEvaluation = parentPersonality.Evaluation();
    }
}
