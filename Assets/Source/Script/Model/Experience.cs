using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

[Serializable]
public class Experience {

    public Dictionary<NeedType, Evaluation> BaseNeeds;
    public Dictionary<NeedType, int> Rewards;
    public int Feedback;

    public Experience()
    {
        BaseNeeds = new Dictionary<NeedType, Evaluation>();
        Rewards = new Dictionary<NeedType, int>();
    }

    public void AddBaseNeeds(Personality basePersonality)
    {
        BaseNeeds = new Dictionary<NeedType, Evaluation>();

        BaseNeeds[NeedType.HUNGER] = basePersonality.Conditions[NeedType.HUNGER].getEvaluation();
        BaseNeeds[NeedType.ENERGY] = basePersonality.Conditions[NeedType.ENERGY].getEvaluation();
        BaseNeeds[NeedType.HEALTH] = basePersonality.Conditions[NeedType.HEALTH].getEvaluation();
        BaseNeeds[NeedType.SATISFACTION] = basePersonality.Conditions[NeedType.SATISFACTION].getEvaluation();
        BaseNeeds[NeedType.SOCIAL] = basePersonality.Conditions[NeedType.SOCIAL].getEvaluation();
    }

    public void AddBaseNeeds(Dictionary<NeedType, Evaluation> basePersonality)
    {
        BaseNeeds = new Dictionary<NeedType, Evaluation>();

        BaseNeeds[NeedType.HUNGER] = basePersonality[NeedType.HUNGER];
        BaseNeeds[NeedType.ENERGY] = basePersonality[NeedType.ENERGY];
        BaseNeeds[NeedType.HEALTH] = basePersonality[NeedType.HEALTH];
        BaseNeeds[NeedType.SATISFACTION] = basePersonality[NeedType.SATISFACTION];
        BaseNeeds[NeedType.SOCIAL] = basePersonality[NeedType.SOCIAL];
    }

    public void AddRewards(Personality resultPersonality)
    {
        Rewards = new Dictionary<NeedType, int>();

        Rewards[NeedType.HUNGER] = (int)resultPersonality.Conditions[NeedType.HUNGER].getEvaluation() - (int)BaseNeeds[NeedType.HUNGER];
        Rewards[NeedType.ENERGY] = (int)resultPersonality.Conditions[NeedType.ENERGY].getEvaluation() - (int)BaseNeeds[NeedType.ENERGY];
        Rewards[NeedType.HEALTH] = (int)resultPersonality.Conditions[NeedType.HEALTH].getEvaluation() - (int)BaseNeeds[NeedType.HEALTH];
        Rewards[NeedType.SATISFACTION] = (int)resultPersonality.Conditions[NeedType.SATISFACTION].getEvaluation() - (int)BaseNeeds[NeedType.SATISFACTION];
        Rewards[NeedType.SOCIAL] = (int)resultPersonality.Conditions[NeedType.SOCIAL].getEvaluation() - (int)BaseNeeds[NeedType.SOCIAL];
    }

    public void AddRandomRewards()
    {
        System.Random r = new System.Random();

        Rewards[NeedType.HUNGER] = (int)((r.NextDouble() * 4f) - 2f);
        Rewards[NeedType.ENERGY] = (int)((r.NextDouble() * 4f) - 2f);
        Rewards[NeedType.HEALTH] = (int)((r.NextDouble() * 4f) - 2f);
        Rewards[NeedType.SATISFACTION] = (int)((r.NextDouble() * 4f) - 2f);
        Rewards[NeedType.SOCIAL] = (int)((r.NextDouble() * 4f) - 2f);
    }

    public Experience(Personality basePersonality, Personality resultPersonality)
    {
        AddBaseNeeds(basePersonality);
        AddRewards(resultPersonality);
    }
    
    public int CompareStatus(Dictionary<NeedType, Evaluation> compareWith)
    {
        int value = 0;

        foreach(KeyValuePair<NeedType, Evaluation> kvp in compareWith)
        {
            value -= Mathf.Abs((int)kvp.Value - (int)BaseNeeds[kvp.Key]);
        }

        return value;
    }

    public bool UpdateRewards(Dictionary<NeedType, int> newRewards)
    {
        bool changedRewards = false;

        foreach(KeyValuePair<NeedType, int> reward in newRewards)
        {
            if(Rewards[reward.Key] != reward.Value)
            {
                if(Rewards[reward.Key] == 0)
                {
                    Debug.Log("Change " + Rewards[reward.Key] + " to " + reward.Value);
                    Rewards[reward.Key] = reward.Value;
                    changedRewards = true;
                }
                else if(Rewards[reward.Key] < 0)
                {
                    if (reward.Value < Rewards[reward.Key])
                    {
                        Debug.Log("Change " + Rewards[reward.Key] + " to " + reward.Value);
                        Rewards[reward.Key] = reward.Value;
                        changedRewards = true;
                    }
                }
                else if(Rewards[reward.Key] > 0)
                {
                    if (reward.Value > Rewards[reward.Key])
                    {
                        Debug.Log("Change " + Rewards[reward.Key] + " to " + reward.Value);
                        Rewards[reward.Key] = reward.Value;
                        changedRewards = true;
                    }
                }
            }
        }

        return changedRewards;
    }

    public void PrintRewards()
    {
        string s = "";

        foreach(KeyValuePair<NeedType, int> reward in Rewards)
        {
            s += reward.Key.ToString() + ": " + reward.Value + " | ";
        }

        Debug.Log(s);
    }

    public override string ToString()
    {
        return JsonMapper.ToJson(this);
    }
}
