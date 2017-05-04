using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitType { INTROVERT, EXTROVERT, GREEDY, FRUGAL }

public class Trait {

    public TraitType Identifier { get; set; }
    public int Tag;

    public Dictionary<NeedType, int[]> ThresholdModifiers;

    //[SerializeField]
    //private int _rewardModifier;

    public Trait(TraitType identifier)
    {
        Identifier = identifier;
        ThresholdModifiers = new Dictionary<NeedType, int[]>();
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
