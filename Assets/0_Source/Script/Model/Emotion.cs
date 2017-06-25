using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmotionType { BAD, NORMAL, GOOD };

public class Emotion {

    public List<Item> Items;

    public EmotionType EmotionType;
    public Trait TemporaryTrait;
    public int Trigger;

    public Emotion(EmotionType emotionType, int trigger)
    {
        EmotionType = emotionType;
        Trigger = trigger;
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

}
