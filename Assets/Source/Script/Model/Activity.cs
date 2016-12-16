using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Activity {
    public int ID { get; set; }
    public string Name { get; set; }

    public List<Reward> RewardList;
    public List<Experience> LearnedExperiences;

    public Feedback Feedback;

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
        LearnedExperiences = new List<Experience>();
		if(item!=null) item.AddActivity (this);

        Feedback = new Feedback();
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

	public Experience DoActivity(Personality parentPersonality)
    {
		if (item != null) {
			item.uses += useConsume;
			if (item.uses >= item.maxUses) {
				ApplicationManager.Instance.RemoveItem (item);
			}
		}

        Experience xp = new Experience();
        xp.AddBaseNeeds(parentPersonality);

        Dictionary<NeedType, Need> need = new Dictionary<NeedType, Need>();

        foreach(KeyValuePair<NeedType, Need> kvp in parentPersonality.Conditions)
        {
            need[kvp.Key] = kvp.Value.Copy();
        }

        foreach(Reward reward in RewardList)
        {
            reward.DoReward(parentPersonality, need);
        }
        
        xp.AddRewards(parentPersonality);

        bool newXP = true;

        for(int i=0; i<LearnedExperiences.Count; i++)
        {
            if(LearnedExperiences[i].CompareStatus(xp.BaseNeeds) == 0)
            {
                if(LearnedExperiences[i].UpdateRewards(xp.Rewards))
                {
                    Debug.Log(feedBackString + ": Updated Rewards!");
                }
                else
                {
                    Debug.Log(feedBackString + " (nothing learned...)");
                }
                xp.PrintRewards();

                newXP = false;
                break;
            }
        }

        if (newXP)
        {
            LearnedExperiences.Add(xp);
        }

		if (item != null) {
			item.uses += useConsume;
			if (item.uses >= item.maxUses) {
				parentPersonality.RemoveItem (item.ID);
			}
		}

        return xp;
    }

    public Experience GetExperience(PersonalityNode personality)
    {
        int bestValue = int.MinValue;
        int bestExperienceID = 0;

        if (LearnedExperiences.Count == 0)
        {
            //Return Random Experience
            Experience xp = new Experience();
            xp.AddBaseNeeds(personality.Needs);
            xp.AddRandomRewards();

            return xp;
        }
        else
        {
            for (int i = 0; i < LearnedExperiences.Count; i++)
            {
                if (LearnedExperiences[i].CompareStatus(personality.Needs) > bestValue)
                {
                    bestValue = LearnedExperiences[i].CompareStatus(personality.Needs);
                    bestExperienceID = i;
                }
            }
        }
        
        return LearnedExperiences[bestExperienceID];
    }
}
