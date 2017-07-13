using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraitType { INTROVERT, EXTROVERT, GREEDY, FRUGAL, DISCIPLINED, WILD, ENERGETIC, LETHARGIC, ANXIOUS, BRAVE, TEMPORARY_TRAIT }

public class Trait {

    public TraitType Identifier { get; set; }
    public int TraitTag;
    public float FeedbackModifier;
    public int AskForItemModifier;
    public int SimilarExperienceDifferenceModifier;

    public Dictionary<NeedType, int[]> ThresholdModifiers;
    public Dictionary<ActivityTag, List<Reward>> ActivityModifiers;

    public Trait(TraitType identifier)
    {
        Identifier = identifier;
        ThresholdModifiers = new Dictionary<NeedType, int[]>();
        ActivityModifiers = new Dictionary<ActivityTag, List<Reward>>();
        FeedbackModifier = 0;
        AskForItemModifier = 0;
        SimilarExperienceDifferenceModifier = 0;
    }

    public bool AddThresholdModifier(NeedType needType, int[] thresholdModifier)
    {
        if (!ThresholdModifiers.ContainsKey(needType))
        {
            ThresholdModifiers[needType] = thresholdModifier;
            return true;
        }
        else
        {
            Debug.LogWarning(needType.ToString() + " is already added to this Trait " + Identifier.ToString());
            return false;
        }
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

    public bool AddFeedbackModifier(float feedbackModifier)
    {
        FeedbackModifier = feedbackModifier;
        return true;
    }

    public bool AddAskForItemModifier(int askForItemModifier)
    {
        AskForItemModifier = askForItemModifier;
        return true;
    }

    public bool AddSimilarExperienceDifferenceModifier(int similarExperienceModifier)
    {
        SimilarExperienceDifferenceModifier = similarExperienceModifier;
        return true;
    }
}
