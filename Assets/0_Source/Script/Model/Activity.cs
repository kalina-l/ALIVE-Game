using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActivityTag { OTHERSOCIAL, OWNSOCIAL, PHYSICAL, EATING, NATURAL, UNNATURAL, STANDARD, SLEEP, MULTIPLAYER }

public class Activity {
    public int ID { get; set; }
    public string Name { get; set; }
    public List<ActivityTag> Tags { get; set; }

    public bool IsMultiplayer { get; set; } //set at construction

    public bool IsRequest { get; set; } //set when received by the multiplayerController
    public bool IsDeclined { get; set; } //set in the multiplayerController when declined

    public List<Reward> RewardList;
    public List<Experience> LearnedExperiences;

    public Feedback Feedback;

    public Item item;
    public int useConsume;

    private static int _similar_experience_difference = -15;
    public static int SIMILAR_EXPERIENCE_DIFFERENCE
    {
        get
        {
            return _similar_experience_difference;
        }
        set
        {
            _similar_experience_difference = Mathf.Clamp(value, int.MinValue, 0);
        }
    }

    public bool IsKnown { get { return LearnedExperiences.Count > 0; } }

    private string _feedBackString;
    [SerializeField]
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

    public Activity() { }

	public Activity(int ID, string Name, Item item, int useConsume, string feedBackString)
    {
		this.ID = ID;
		this.Name = Name;
		this.item = item;
		this.useConsume = useConsume;
        this.feedBackString = feedBackString;
        RewardList = new List<Reward>();
        LearnedExperiences = new List<Experience>();
        Tags = new List<ActivityTag>();
        IsMultiplayer = false;

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

    public Activity RemoveReward(Reward reward)
    {
        if (RewardList.Contains(reward))
        {
            RewardList.Remove(reward);
        }
        else
        {
            Debug.LogWarning(reward.ID + " is not in the RewardList -> can't be removed");
        }
        return this;
    }

	public Experience DoActivity(Personality parentPersonality)
    {
        Experience xp = null;
        if (IsMultiplayer)
            xp = new MultiplayerExperience();
        else
            xp = new Experience();

        xp.AddBaseNeeds(parentPersonality);

        if (IsMultiplayer)
        {
            Personality remotePersonality = parentPersonality.Multiplayer.GetRemotePersonality();
            ((MultiplayerExperience)xp).AddRemoteNeeds(remotePersonality);
            ((MultiplayerExperience)xp).IsRequest = IsRequest;
        }

        Dictionary<NeedType, Need> need = new Dictionary<NeedType, Need>();

        foreach(KeyValuePair<NeedType, Need> kvp in parentPersonality.Conditions)
        {
            need[kvp.Key] = kvp.Value.Copy();
        }
        
        if (IsMultiplayer)
        {
            if (IsRequest)
            {
                if (parentPersonality.executedEmotion == EmotionType.NORMAL)
                {
                    parentPersonality.emotionCounter += 1;
                }
                GetAcceptReward().DoReward(parentPersonality, need);

                foreach (Reward reward in RewardList)
                {
                    reward.DoReward(parentPersonality, need);
                }
            }
            else if (IsDeclined)
            {
                if (parentPersonality.executedEmotion == EmotionType.NORMAL)
                {
                    parentPersonality.emotionCounter -= 1;
                }

                GetRejectionReward().DoReward(parentPersonality, need);
            }
        }
        else {
            foreach (Reward reward in RewardList)
            {
                reward.DoReward(parentPersonality, need);
            }
        }
        
        xp.AddRewards(parentPersonality);

        bool newXP = true;

        for(int i=0; i<LearnedExperiences.Count; i++)
        {
            if(LearnedExperiences[i].CompareStatus(xp.BaseNeeds) == 0)
            {
                if(LearnedExperiences[i].UpdateRewards(xp.Rewards))
                {
                    DebugController.Instance.Log(feedBackString + ": Updated Rewards!", DebugController.DebugType.Activity);
                }
                else
                {
                    DebugController.Instance.Log(feedBackString + " (nothing learned...)", DebugController.DebugType.Activity);
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
				ApplicationManager.Instance.RemoveItem ();
			}
		}

        IsRequest = false;
        IsDeclined = false;

        return xp;
    }

    public Experience GetExperience(PersonalityNode personality)
    {
        //if value is 0, only perfectly fitting experiences will be returned. when there are none return a random experience
        //if value is int.MinValue, return the closest experience that was gathered, no matter how close it is
        int bestValue = SIMILAR_EXPERIENCE_DIFFERENCE;
        int bestExperienceID = -1;

        if (LearnedExperiences.Count == 0)
        {
            return GetRandomExperience(personality);
        }
        else
        {
            for (int i = 0; i < LearnedExperiences.Count; i++)
            {
                if (IsMultiplayer)
                {
                    MultiplayerExperience mxp = (MultiplayerExperience)LearnedExperiences[i];

                    if (mxp.IsRequest == IsRequest) {
                        if (mxp.CompareStatus(personality.Needs) + mxp.CompareRemoteStatus(personality.Needs) > bestValue)
                        {
                            PersonalityNode remotePersonality = new PersonalityNode(personality.Multiplayer.GetRemotePersonality());
                            bestValue = mxp.CompareStatus(personality.Needs) + mxp.CompareRemoteStatus(remotePersonality.Needs);
                            bestExperienceID = i;
                        }
                    }
                }
                else
                {
                    if (LearnedExperiences[i].CompareStatus(personality.Needs) > bestValue)
                    {
                        bestValue = LearnedExperiences[i].CompareStatus(personality.Needs);
                        bestExperienceID = i;
                    }
                }
            }
        }

        if(bestExperienceID == -1)
        {
            return GetRandomExperience(personality);
        }
        
        return LearnedExperiences[bestExperienceID];
    }

    public void PrintExperience(Personality personality)
    {
        DebugController.Instance.Log(Name + " REWARDS:", DebugController.DebugType.Activity);
        Experience xp = GetExperience(new PersonalityNode(personality));
        xp.PrintRewards();
    }

    private Experience GetRandomExperience(PersonalityNode personality)
    {
        Experience xp = null;

        if (IsMultiplayer)
        {
            xp = new MultiplayerExperience();
            
            PersonalityNode remotePersonality = new PersonalityNode(personality.Multiplayer.GetRemotePersonality());
            ((MultiplayerExperience)xp).AddRemoteNeeds(remotePersonality.Needs);
            ((MultiplayerExperience)xp).IsRequest = IsRequest;
        }
        else
        {
            xp = new Experience();
        }

        xp.AddBaseNeeds(personality.Needs);

        if (IsMultiplayer && IsRequest)
        {
            //MP: always accept MP Activities you don't know
            xp.AddFavorableRewards();
        }
        else
        {
            xp.AddRandomRewards();
        }

        return xp;
    }

    public Reward GetAcceptReward()
    {
        Reward r = new Reward();

        r.ID = 1010101010;
        r.RewardType = NeedType.SOCIAL;
        r.RewardValue = 5;

        return r;
    }

    public Reward GetRejectionReward()
    {
        Reward r = new Reward();

        r.ID = 1010101010;
        r.RewardType = NeedType.SOCIAL;
        r.RewardValue = -20;

        return r;
    }

    public void DebugWholeFeedbackValue ()
    {
        DebugController.Instance.Log("Activity " + Name + " has a total feedback value: " + Feedback.GetWholeFeedbackValue(), DebugController.DebugType.Activity);
    }
}
