using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmotionType { BAD, NORMAL, GOOD };

public class Emotion {

    public List<Item> Items;

    public EmotionType EmotionType;
    public Trait TemporaryTrait;
    public int Trigger;

    public int SatisfactionRewardsModification;

    public Emotion(EmotionType emotionType, int trigger, int satisfactionRewardsModification)
    {
        EmotionType = emotionType;
        Trigger = trigger;
        SatisfactionRewardsModification = satisfactionRewardsModification;
    }

    public bool AddTemporaryTrait(Trait trait)
    {
        if(trait != null)
        {
            TemporaryTrait = trait;
            return true;
        }
        return false;
    }


    public void ActivateEmotion(Personality personality)
    {
        personality.AddTrait(TemporaryTrait, Items);
        //durch alle rewards durchgehen (Item->Aktivitäten|BaseActivitiers) -> Satisfaction Rewards mit 0.5 Value erhöhen/verringern (je nach SatisfactionRewardsModification)
        foreach (Item item in Items.ToArray())
        {
            foreach (Activity activity in item.GetAllActivities().ToArray())
            {
                //Debug.Log(activity.Name + ": ");
                foreach (Reward reward in activity.RewardList.ToArray())
                {
                    //Debug.Log(reward.ID + ": " + reward.RewardType + " " + reward.RewardValue);
                    if (reward.RewardType == NeedType.SATISFACTION)
                    {
                        Reward rew = reward.Copy();
                        rew.ID *= 100;
                        rew.RewardValue = Mathf.Abs(rew.RewardValue / 2) * SatisfactionRewardsModification;
                        int index = Items.IndexOf(item);
                        Items[index].GetActivity(activity.ID).AddReward(rew);
                    }
                }
            }
        }

        //Dictionary<int, Activity> allBaseActivityIterator = personality.BaseActivities;
        foreach (KeyValuePair<int, Activity> kvp in personality.BaseActivities)
        {
            //Debug.Log(kvp.Value.Name + ": ");
            foreach (Reward reward in kvp.Value.RewardList.ToArray())
            {
                //Debug.Log(reward.ID + ": " + reward.RewardType + " " + reward.RewardValue);
                if (reward.RewardType == NeedType.SATISFACTION)
                {
                    Reward rew = reward.Copy();
                    rew.ID *= 100;
                    rew.RewardValue = Mathf.Abs(rew.RewardValue / 2) * SatisfactionRewardsModification;
                    personality.BaseActivities[kvp.Key].AddReward(rew);
                }
            }
        }
    }


    public void DeactivateEmotion(Personality personality)
    {
        personality.RemoveTrait(TemporaryTrait, Items);

        foreach (Item item in Items.ToArray())
        {
            foreach (Activity activity in item.GetAllActivities().ToArray())
            {
                foreach (Reward reward in activity.RewardList.ToArray())
                {
                    if (reward.ID >= 10000 /*reward.RewardType == NeedType.SATISFACTION*/)
                    {
                        Items[Items.IndexOf(item)].GetActivity(activity.ID).RemoveReward(reward);
                    }
                }
            }
        }

        //foreach (KeyValuePair<int, Activity> kvp in personality.BaseActivities)
        //{
        //    DebugController.Instance.Log(kvp.Value.Name + ": ", DebugController.DebugType.Emotion);
        //    foreach (Reward reward in kvp.Value.RewardList)
        //    {
        //        DebugController.Instance.Log(reward.ID + ": " + reward.RewardType + " " + reward.RewardValue, DebugController.DebugType.Emotion);
        //    }
        //}

        foreach (KeyValuePair<int, Activity> kvp in personality.BaseActivities)
        {
            foreach (Reward reward in kvp.Value.RewardList.ToArray())
            {
                if (reward.ID >= 10000 /*reward.RewardType == NeedType.SATISFACTION*/)
                {
                    personality.BaseActivities[kvp.Key].RemoveReward(reward);
                }
            }
        }
    }
}


