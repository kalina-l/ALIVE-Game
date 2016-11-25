using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class PersonalityCreator
{
    public static readonly string ConditionIdentifier = "ConditionThresholds";
    public static readonly int ConditionStart = 100;
    public static readonly string AttributeIdentifier = "Attributes";
    public static readonly int MinAttribute = 1;
    public static readonly int MaxAttribute = 20;
    public static readonly string BaseActivityIdentifier = "BaseActivities";

    private Personality _personality;
    public Personality personality
    {
        get
        {
            return _personality;
        }
    }

    private string[][] _personalityCSV;

    private Dictionary<string, int> _attributes;
    private Dictionary<string, int[]> _conditionThresholds;
    private Dictionary<int, Activity> _baseActivities;

    public PersonalityCreator(string personalityCSVPath)
    {
        _personalityCSV = CSV.read(Application.dataPath + personalityCSVPath);

        getAllPersonalityData(_personalityCSV);

        _personality = new Personality();

        foreach(KeyValuePair<string, int> attribute in _attributes)
        {
            _personality.AddAttribute((AttributeType)Enum.Parse(typeof(AttributeType), attribute.Key), new Attribute(attribute.Value, MinAttribute, MaxAttribute));
        }

        foreach(KeyValuePair<string, int[]> conditionThreshold in _conditionThresholds)
        {
            _personality.AddCondition((NeedType)Enum.Parse(typeof(NeedType), conditionThreshold.Key), new Need(ConditionStart, conditionThreshold.Value));
        }

        foreach (KeyValuePair<int, Activity> baseActivity in _baseActivities)
        {
            _personality.AddBaseActivity(baseActivity.Key, "", baseActivity.Value);
        }

    } 

    private void getAllPersonalityData(string[][] personalityCSV)
    {
        _attributes = new Dictionary<string, int>();
        _conditionThresholds = new Dictionary<string, int[]>();
        int[] thresholds;
        _baseActivities = new Dictionary<int, Activity>();
        Activity activity;

        int start = -1;
        string identifier = null;

        for (int i = 0; (i < personalityCSV.GetLength(0)) && (start == -1); i++)
        {
            if(Array.IndexOf(personalityCSV[i], AttributeIdentifier) != -1)
            {
                start = i;
                identifier = "attribute";
            }
            else if(Array.IndexOf(personalityCSV[i], ConditionIdentifier) != -1){
                start = i;
                identifier = "condition";
            }
            else if(Array.IndexOf(personalityCSV[i], BaseActivityIdentifier) != -1)
            {
                start = i;
                identifier = "baseActivity";
            }

            if (start != -1)
            {
                for (int k = start + 1; k < personalityCSV.GetLength(0) && !String.IsNullOrEmpty(personalityCSV[k][0]); k++)
                {
                    switch (identifier)
                    {
                        case "attribute":
                            _attributes.Add(personalityCSV[k][0], Int32.Parse(personalityCSV[k][1]));
                            break;
                        case "condition":
                            thresholds = new int[personalityCSV[k].Length - 1];
                            for (int j = 1; j < personalityCSV[k].Length; j++)
                            {
                                thresholds[j - 1] = Int32.Parse(personalityCSV[k][j]);
                            }
                            _conditionThresholds.Add(personalityCSV[k][0], thresholds);
                            break;
                        case "baseActivity":
                            activity = new Activity("");
                            for (int j = 1; j < personalityCSV[k].Length; j++)
                            {
                                if (personalityCSV[start][j] != "feedBackString")
                                {
                                    activity.AddReward(new Reward());
                                }
                                else
                                {
                                    activity.feedBackString = personalityCSV[k][j];
                                }
                            }
                            _baseActivities.Add(int.Parse(personalityCSV[k][0]), activity);
                            break;
                        default:
                            break;
                    }
                    i = k;
                }
            }
            start = -1;
        }
    }
}
