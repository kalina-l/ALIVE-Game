using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitType { INTROVERT, EXTROVERT, GREEDY, FRUGAL, DISCIPLINED, WILD, ENERGETIC, LETHARGIC }

public class Trait {

    public TraitType Identifier { get; set; }
    public int TraitTag;

    public Dictionary<NeedType, int[]> ThresholdModifiers;
    public Dictionary<ActivityTag, List<Reward>> ActivityModifiers;

    public Trait(TraitType identifier)
    {
        Identifier = identifier;
        ThresholdModifiers = new Dictionary<NeedType, int[]>();
        ActivityModifiers = new Dictionary<ActivityTag, List<Reward>>();
    }

    public Trait AddThresholdModifier(NeedType needType, int[] thresholdModifier)
    {
        if (!ThresholdModifiers.ContainsKey(needType))
        {
            ThresholdModifiers[needType] = thresholdModifier;
        }
        else
        {
            Debug.LogWarning(needType.ToString() + " is already added to this Trait " + Identifier.ToString());
        }

        return this;
    }

    public bool AddActivityModifier(ActivityTag actTag, List<Reward> actModifiers)
    {
        if (!ActivityModifiers.ContainsKey(actTag))
        {
            ActivityModifiers[actTag] = actModifiers;
            return true;
        }
        else
        {
            Debug.LogWarning(actTag.ToString() + " is already added to this Trait " + Identifier.ToString());
            return false;
        }
    }

    public void printThresholds()
    {
        string threshold = "";
        foreach(KeyValuePair<NeedType, int[]> kvp in ThresholdModifiers)
        {
            threshold = "";
            threshold += "|" + kvp.Key + ": ";
            for(int i = 0; i < kvp.Value.Length; i++)
            {
                threshold += "|" + kvp.Value[i];
            }
        }      
        Debug.Log(threshold);
    }
}
