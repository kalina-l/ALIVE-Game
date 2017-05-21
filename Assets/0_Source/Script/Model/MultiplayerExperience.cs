using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerExperience : Experience {

    public Dictionary<NeedType, Evaluation> RemoteNeeds;

    public MultiplayerExperience()
    {
        BaseNeeds = new Dictionary<NeedType, Evaluation>();
        RemoteNeeds = new Dictionary<NeedType, Evaluation>();
        Rewards = new Dictionary<NeedType, int>();
    }

    public MultiplayerExperience(Personality basePersonality, Personality resultPersonality, Personality remotePersonality)
    {
        AddBaseNeeds(basePersonality);
        AddRewards(resultPersonality);
        AddRemoteNeeds(remotePersonality);
    }

    public void AddRemoteNeeds(Personality basePersonality)
    {
        RemoteNeeds = new Dictionary<NeedType, Evaluation>();

        RemoteNeeds[NeedType.HUNGER] = basePersonality.Conditions[NeedType.HUNGER].getEvaluation();
        RemoteNeeds[NeedType.ENERGY] = basePersonality.Conditions[NeedType.ENERGY].getEvaluation();
        RemoteNeeds[NeedType.HEALTH] = basePersonality.Conditions[NeedType.HEALTH].getEvaluation();
        RemoteNeeds[NeedType.SATISFACTION] = basePersonality.Conditions[NeedType.SATISFACTION].getEvaluation();
        RemoteNeeds[NeedType.SOCIAL] = basePersonality.Conditions[NeedType.SOCIAL].getEvaluation();
    }

    public void AddRemoteNeeds(Dictionary<NeedType, Evaluation> basePersonality)
    {
        RemoteNeeds = new Dictionary<NeedType, Evaluation>();

        RemoteNeeds[NeedType.HUNGER] = basePersonality[NeedType.HUNGER];
        RemoteNeeds[NeedType.ENERGY] = basePersonality[NeedType.ENERGY];
        RemoteNeeds[NeedType.HEALTH] = basePersonality[NeedType.HEALTH];
        RemoteNeeds[NeedType.SATISFACTION] = basePersonality[NeedType.SATISFACTION];
        RemoteNeeds[NeedType.SOCIAL] = basePersonality[NeedType.SOCIAL];
    }

    public int CompareRemoteStatus(Dictionary<NeedType, Evaluation> compareWith)
    {
        int value = 0;

        foreach (KeyValuePair<NeedType, Evaluation> kvp in compareWith)
        {
            value -= Mathf.Abs((int)kvp.Value - (int)RemoteNeeds[kvp.Key]);
        }

        return value;
    }
}
