using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Feedback {

    private float[] EnergyFeedback;
    private float[] HealthFeedback;
    private float[] HungerFeedback;
    private float[] SatisfactionFeedback;
    private float[] SocialFeedback;

    private int feedbackCounter;

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
    }

    public void AddFeedback(Dictionary<NeedType, Evaluation> Needs, int feedBack)
    {
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

}
