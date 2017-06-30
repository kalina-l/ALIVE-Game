using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerExperience : Experience {

    public Dictionary<NeedType, Evaluation> RemoteNeeds;
    public bool IsRequest;

    public MultiplayerExperience()
    {
        BaseNeeds = new Dictionary<NeedType, Evaluation>();
        RemoteNeeds = new Dictionary<NeedType, Evaluation>();
        Rewards = new Dictionary<NeedType, int>();
    }

    public MultiplayerExperience(Personality basePersonality, Personality resultPersonality, Dictionary<NeedType, Evaluation> needs)
    {
        AddBaseNeeds(basePersonality);
        AddRewards(resultPersonality);
        AddRemoteNeeds(needs);
    }

    public void AddRemoteNeeds(Dictionary<NeedType, Evaluation> needs)
    {
        RemoteNeeds = new Dictionary<NeedType, Evaluation>();

        foreach (var item in RemoteNeeds)
        {
            RemoteNeeds[item.Key] = needs[item.Key];
        }

        /*RemoteNeeds[NeedType.HUNGER] = basePersonality[NeedType.HUNGER];
        RemoteNeeds[NeedType.ENERGY] = basePersonality[NeedType.ENERGY];
        RemoteNeeds[NeedType.HEALTH] = basePersonality[NeedType.HEALTH];
        RemoteNeeds[NeedType.SATISFACTION] = basePersonality[NeedType.SATISFACTION];
        RemoteNeeds[NeedType.SOCIAL] = basePersonality[NeedType.SOCIAL];
        */
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
