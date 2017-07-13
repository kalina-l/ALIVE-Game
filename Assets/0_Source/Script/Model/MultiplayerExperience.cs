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

        /*foreach (var item in RemoteNeeds)
        {
            RemoteNeeds[item.Key] = needs[item.Key];
        }
        */
        RemoteNeeds[NeedType.HUNGER] = needs[NeedType.HUNGER];
        RemoteNeeds[NeedType.ENERGY] = needs[NeedType.ENERGY];
        RemoteNeeds[NeedType.HEALTH] = needs[NeedType.HEALTH];
        RemoteNeeds[NeedType.SATISFACTION] = needs[NeedType.SATISFACTION];
        RemoteNeeds[NeedType.SOCIAL] = needs[NeedType.SOCIAL];
        
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
