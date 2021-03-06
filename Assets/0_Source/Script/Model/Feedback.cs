﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feedback {

    private static int globalFeedbackCounter;

    private static int GLOBAL_MAX_FEEDBACK_VALUE = 3;                       // the feedback value lemo gets no actions resulted with any feedback for a long time
    private static int GLOBAL_MIN_FEEDBACK_VALUE = 0;
    private static int GLOBAL_ACTIONS_BEFORE_LOWERING = 1;                  // after how many with feedback evaluated actions the feedback value sinks
    [SerializeField]
    private static int globalFeedbackValue = GLOBAL_MAX_FEEDBACK_VALUE;     // depends on the conditioning schedule, it lowers with continuous positive reinforcement and grows every time no feedback to an action is given

    [SerializeField]
    private float[] EnergyFeedback;
    [SerializeField]
    private float[] HealthFeedback;
    [SerializeField]
    private float[] HungerFeedback;
    [SerializeField]
    private float[] SatisfactionFeedback;
    [SerializeField]
    private float[] SocialFeedback;

    [SerializeField]
    private int feedbackCounter;                         // numbers of evaluated actions in row (set to 0 if action did not got a feedback)

    private static int MAX_FEEDBACK_VALUE = 5;           // the feedback value lemo gets for an action which didn't result with any feedback for a long time
    private static int MIN_FEEDBACK_VALUE = 2;
    private static int FORGETTING_VALUE = 2;             // how much lemo forgets every not evaluated action (used same way as feedback variable, but feedback exp goes towards zero)
    private static int ACTIONS_BEFORE_LOWERING = 2;      // after how many with feedback evaluated actions the feedback value sinks
    [SerializeField]
    private int feedbackValue = MAX_FEEDBACK_VALUE;      // depends on the conditioning schedule, it lowers with continuous positive reinforcement and grows every time no feedback to an action is given
    [SerializeField]
    private bool lastTimePositive;                       // if the feedback varies in value from the last time, feedbackValue is set to MAX_FEEDBACK_VALUE and ACTIONS_BEFORE_LOWERING is resetted

    [SerializeField]
    private Queue<FeedbackType> lastFeedbackTypes = new Queue<FeedbackType>();       // types of last used feedback

    // HOW FEEDBACK WORKS (P - positive, N - negative, O - none):
    //                      P  P  P  P  P  P  P  P  P  P  P  O  O  P  P  P  N  N  N ...
    // FEEDBACK VALUE       5  5  4  4  3  3  2  2  2  2  2  3  4  4  4  3  5  5  4 ...
    // GIVEN FEEDBACK      +5 +5 +4 +4 +3 +3 +2 +2 +2 +2 +2  -  - +4 +4 +3 -5 -5 -4 ...
    // FORGETTING FACTOR    -  -  -  -  -  -  -  -  -  -  -  2  2  -  -  -  -  -  -

    public Feedback()
    {
        EnergyFeedback = new float[8];
        HealthFeedback = new float[8];
        HungerFeedback = new float[8];
        SatisfactionFeedback = new float[8];
        SocialFeedback = new float[8];

        for(int i=0; i<8; i++)
        {
            EnergyFeedback[i] = 0f;
            HealthFeedback[i] = 0f;
            HungerFeedback[i] = 0f;
            SatisfactionFeedback[i] = 0f;
            SocialFeedback[i] = 0f;
        }

        feedbackCounter = 0;
        globalFeedbackCounter = 0;
    }

    public void AddFeedback(Dictionary<NeedType, Evaluation> Needs, int feedBack)
    {
        bool rareFeedback = isFeedbackRare(ApplicationManager.Instance.getFeedbackController().getLastFeedbackType());
        bool isPositive = feedBack > 0;

        if(rareFeedback || lastTimePositive != isPositive)
        {
            feedbackValue = MAX_FEEDBACK_VALUE;
            feedbackCounter = 0;
            globalFeedbackValue = GLOBAL_MAX_FEEDBACK_VALUE;
            globalFeedbackCounter = 0;
        }
        
        feedBack *= feedbackValue;
        if (isPositive)
            feedBack += globalFeedbackValue;
        else
            feedBack -= globalFeedbackValue;

        DebugController.Instance.Log("Global feedback value: " + globalFeedbackValue, DebugController.DebugType.Feedback);

        DebugController.Instance.Log("Given feedback (distributed): " + feedBack, DebugController.DebugType.Feedback);

        int valueIndex = 0;

        foreach(KeyValuePair<NeedType, Evaluation> kvp in Needs)
        {
            valueIndex = (int)kvp.Value;

            switch (kvp.Key)
            {
                case NeedType.ENERGY:
                    EnergyFeedback[valueIndex] += feedBack;
                    if (valueIndex > 0)
                        EnergyFeedback[valueIndex - 1] += feedBack * 0.6f;
                    if (valueIndex < 7)
                        EnergyFeedback[valueIndex + 1] += feedBack * 0.6f;
                    if (valueIndex > 1)
                        EnergyFeedback[valueIndex - 2] += feedBack * 0.3f;
                    if (valueIndex < 6)
                        EnergyFeedback[valueIndex + 2] += feedBack * 0.3f;
                    break;
                case NeedType.HEALTH:
                    HealthFeedback[valueIndex] += feedBack;
                    if (valueIndex > 0)
                        HealthFeedback[valueIndex - 1] += feedBack * 0.6f;
                    if (valueIndex < 7)
                        HealthFeedback[valueIndex + 1] += feedBack * 0.6f;
                    if (valueIndex > 1)
                        HealthFeedback[valueIndex - 2] += feedBack * 0.3f;
                    if (valueIndex < 6)
                        HealthFeedback[valueIndex + 2] += feedBack * 0.3f;
                    break;
                case NeedType.HUNGER:
                    HungerFeedback[valueIndex] += feedBack;
                    if (valueIndex > 0)
                        HungerFeedback[valueIndex - 1] += feedBack * 0.6f;
                    if (valueIndex < 7)
                        HungerFeedback[valueIndex + 1] += feedBack * 0.6f;
                    if (valueIndex > 1)
                        HungerFeedback[valueIndex - 2] += feedBack * 0.3f;
                    if (valueIndex < 6)
                        HungerFeedback[valueIndex + 2] += feedBack * 0.3f;
                    break;
                case NeedType.SATISFACTION:
                    SatisfactionFeedback[valueIndex] += feedBack;
                    if (valueIndex > 0)
                        SatisfactionFeedback[valueIndex - 1] += feedBack * 0.6f;
                    if (valueIndex < 7)
                        SatisfactionFeedback[valueIndex + 1] += feedBack * 0.6f;
                    if (valueIndex > 1)
                        SatisfactionFeedback[valueIndex - 2] += feedBack * 0.3f;
                    if (valueIndex < 6)
                        SatisfactionFeedback[valueIndex + 2] += feedBack * 0.3f;
                    break;
                case NeedType.SOCIAL:
                    SocialFeedback[valueIndex] += feedBack;
                    if (valueIndex > 0)
                        SocialFeedback[valueIndex - 1] += feedBack * 0.6f;
                    if (valueIndex < 7)
                        SocialFeedback[valueIndex + 1] += feedBack * 0.6f;
                    if (valueIndex > 1)
                        SocialFeedback[valueIndex - 2] += feedBack * 0.3f;
                    if (valueIndex < 6)
                        SocialFeedback[valueIndex + 2] += feedBack * 0.3f;
                    break;
            }
        }

        feedbackCounter++;
        globalFeedbackCounter++;
        if (feedbackCounter >= ACTIONS_BEFORE_LOWERING && feedbackValue > MIN_FEEDBACK_VALUE)
        {
            feedbackValue--;
            feedbackCounter = 0;
        }
        if (globalFeedbackCounter >= GLOBAL_ACTIONS_BEFORE_LOWERING && globalFeedbackValue > GLOBAL_MIN_FEEDBACK_VALUE)
        {
            globalFeedbackValue--;
            globalFeedbackCounter = 0;
        }
        lastTimePositive = isPositive;
    }

    private bool isFeedbackRare (FeedbackType feedbackType)
    {
        if(lastFeedbackTypes.Count >= 20)
        {
            lastFeedbackTypes.Dequeue();
        }
        lastFeedbackTypes.Enqueue(feedbackType);

        int counter = 0;
        foreach(FeedbackType ft in lastFeedbackTypes)
        {
            if(ft.Equals(feedbackType))
            {
                counter++;
            }
        }
        if (counter/lastFeedbackTypes.Count * 100 <= 15) return true; // only if the given feedback type was max. 15% times used in the last 20 feedbacks
        else return false;
    }

    public void AddNoFeedbackGiven (Dictionary<NeedType, Evaluation> Needs)
    {
        feedbackCounter = 0;
        if(feedbackValue < MAX_FEEDBACK_VALUE)
        {
            feedbackValue++;
        }

        globalFeedbackCounter = 0;
        if(globalFeedbackValue < GLOBAL_MAX_FEEDBACK_VALUE)
        {
            globalFeedbackValue++;
        }

        int valueIndex;

        foreach (KeyValuePair<NeedType, Evaluation> kvp in Needs)
        {
            valueIndex = (int)kvp.Value;

            switch (kvp.Key)
            {
                case NeedType.ENERGY:
                    EnergyFeedback[valueIndex] = bringTowardsZero(EnergyFeedback[valueIndex], FORGETTING_VALUE);
                    if (valueIndex > 0)
                        EnergyFeedback[valueIndex - 1] = bringTowardsZero(EnergyFeedback[valueIndex - 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex < 7)
                        EnergyFeedback[valueIndex + 1] = bringTowardsZero(EnergyFeedback[valueIndex + 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex > 1)
                        EnergyFeedback[valueIndex - 2] = bringTowardsZero(EnergyFeedback[valueIndex - 2], FORGETTING_VALUE * 0.3f);
                    if (valueIndex < 6)
                        EnergyFeedback[valueIndex + 2] = bringTowardsZero(EnergyFeedback[valueIndex + 2], FORGETTING_VALUE  * 0.3f);
                    break;
                case NeedType.HEALTH:
                    HealthFeedback[valueIndex] = bringTowardsZero(HealthFeedback[valueIndex], FORGETTING_VALUE);
                    if (valueIndex > 0)
                        HealthFeedback[valueIndex - 1] = bringTowardsZero(HealthFeedback[valueIndex - 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex < 7)
                        HealthFeedback[valueIndex + 1] = bringTowardsZero(HealthFeedback[valueIndex + 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex > 1)
                        HealthFeedback[valueIndex - 2] = bringTowardsZero(HealthFeedback[valueIndex - 2], FORGETTING_VALUE * 0.3f);
                    if (valueIndex < 6)
                        HealthFeedback[valueIndex + 2] = bringTowardsZero(HealthFeedback[valueIndex + 2], FORGETTING_VALUE * 0.3f);
                    break;
                case NeedType.HUNGER:
                    HungerFeedback[valueIndex] = bringTowardsZero(HungerFeedback[valueIndex], FORGETTING_VALUE);
                    if (valueIndex > 0)
                        HungerFeedback[valueIndex - 1] = bringTowardsZero(HungerFeedback[valueIndex - 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex < 7)
                        HungerFeedback[valueIndex + 1] = bringTowardsZero(HungerFeedback[valueIndex + 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex > 1)
                        HungerFeedback[valueIndex - 2] = bringTowardsZero(HungerFeedback[valueIndex - 2], FORGETTING_VALUE * 0.3f);
                    if (valueIndex < 6)
                        HungerFeedback[valueIndex + 2] = bringTowardsZero(HungerFeedback[valueIndex + 2], FORGETTING_VALUE * 0.3f);
                    break;
                case NeedType.SATISFACTION:
                    SatisfactionFeedback[valueIndex] = bringTowardsZero(SatisfactionFeedback[valueIndex], FORGETTING_VALUE);
                    if (valueIndex > 0)
                        SatisfactionFeedback[valueIndex - 1] = bringTowardsZero(SatisfactionFeedback[valueIndex - 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex < 7)
                        SatisfactionFeedback[valueIndex + 1] = bringTowardsZero(SatisfactionFeedback[valueIndex + 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex > 1)
                        SatisfactionFeedback[valueIndex - 2] = bringTowardsZero(SatisfactionFeedback[valueIndex - 2], FORGETTING_VALUE * 0.3f);
                    if (valueIndex < 6)
                        SatisfactionFeedback[valueIndex + 2] = bringTowardsZero(SatisfactionFeedback[valueIndex + 2], FORGETTING_VALUE * 0.3f);
                    break;
                case NeedType.SOCIAL:
                    SocialFeedback[valueIndex] = bringTowardsZero(SocialFeedback[valueIndex], FORGETTING_VALUE);
                    if (valueIndex > 0)
                        SocialFeedback[valueIndex - 1] = bringTowardsZero(SocialFeedback[valueIndex - 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex < 7)
                        SocialFeedback[valueIndex + 1] = bringTowardsZero(SocialFeedback[valueIndex + 1], FORGETTING_VALUE * 0.6f);
                    if (valueIndex > 1)
                        SocialFeedback[valueIndex - 2] = bringTowardsZero(SocialFeedback[valueIndex - 2], FORGETTING_VALUE * 0.3f);
                    if (valueIndex < 6)
                        SocialFeedback[valueIndex + 2] = bringTowardsZero(SocialFeedback[valueIndex + 2], FORGETTING_VALUE * 0.3f);
                    break;
            }
        }
        DebugController.Instance.Log("Global feedback value: " + globalFeedbackValue, DebugController.DebugType.Feedback);
    }

    public float bringTowardsZero(float num, float value)
    {
        bool isPositive = true;
        float result = 0;
        if (num < -0.1f)
        {
            isPositive = false;
        }
        else if (num < 0.1f)
        {
            return 0;
        }
        else result = num > 0 ? num - value : num + value;
        if (isPositive && result < 0) return 0;
        else if (!isPositive && result > 0) return 0;
        else return result;

    }

    public float GetFeedback(Dictionary<NeedType, Evaluation> Needs)
    {
        float retValue = 0;

        int valueIndex = 0;

        foreach (KeyValuePair<NeedType, Evaluation> kvp in Needs)
        {
            valueIndex = (int)kvp.Value;
            
            switch (kvp.Key)
            {
                case NeedType.ENERGY:
                    retValue += EnergyFeedback[valueIndex];
                    break;
                case NeedType.HEALTH:
                    retValue += HealthFeedback[valueIndex];
                    break;
                case NeedType.HUNGER:
                    retValue += HungerFeedback[valueIndex];
                    break;
                case NeedType.SATISFACTION:
                    retValue += SatisfactionFeedback[valueIndex];
                    break;
                case NeedType.SOCIAL:
                    retValue += SocialFeedback[valueIndex];
                    break;
            }
        }

        return retValue;
    }

    public float GetWholeFeedbackValue()
    {
        float result = 0;
        for (int i = 0; i < 8; i++)
        {
            result += EnergyFeedback[i];
            result += HealthFeedback[i];
            result += HungerFeedback[i];
            result += SatisfactionFeedback[i];
            result += SocialFeedback[i];
        }
        return result / 14;
    }

}
