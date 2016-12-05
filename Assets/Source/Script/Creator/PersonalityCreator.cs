using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class PersonalityCreator
{
    private List<Reward> _rewards;
    public List<Item> ItemList;

    public static readonly string AttributesAndNeedsCSV = "personality";
    public static readonly string RewardsCSV = "rewards";
    public static readonly string ActivitiesRewardsCSV = "activities_rewards";

    public static readonly string ConditionIdentifier = "ConditionThresholds";
    public static readonly int ConditionStart = 100;
    public static readonly string AttributeIdentifier = "Attributes";
    public static readonly int MinAttribute = 1;
    public static readonly int MaxAttribute = 20;

    private static int activityCounter = 0;

    private Personality _personality;
    public Personality personality
    {
        get
        {
            return _personality;
        }
    }

    private string[][] _personalityCSV;

    public PersonalityCreator(string personalityCSVPath)
    {
        _rewards = new List<Reward>();
        _personalityCSV = CSV.read(personalityCSVPath + RewardsCSV);
        getRewards(_personalityCSV);

        _personality = new Personality();
        _personalityCSV = CSV.read(personalityCSVPath + AttributesAndNeedsCSV);
        getAttributesAndNeeds(_personalityCSV);

        _personalityCSV = CSV.read(personalityCSVPath + ActivitiesRewardsCSV);
        getBaseActivites(_personalityCSV);

        _personalityCSV = CSV.read(personalityCSVPath + ActivitiesRewardsCSV);
        getItems(_personalityCSV);


    }

    private List<Item> getItems(string[][] personalityCSV)
    {
        ItemList = new List<Item>();
        Activity act;
        Item item = null;
        Boolean itemExists = false;
        int itemCounter = 0;
        for (int i = 1; (i < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[i][0])); i++)
        {
            if (!personalityCSV[i][1].Equals("non"))
            {
                foreach(Item ite in ItemList)
                {
                    if (ite.Name.Equals(personalityCSV[i][1]))
                    {
                        item = ite;
                        itemExists = true;
                    }
                }
                if (!itemExists)
                {
                    item = new Item();
                    item.Name = personalityCSV[i][1];
                    item.ID = itemCounter;
                    itemCounter++;
                }
                act = new Activity(0, personalityCSV[i][2], null, 0, null);
                foreach (Reward rewa in _rewards)
                {
                    if (rewa.ID == Int32.Parse(personalityCSV[i][0]))
                    {
                        act.AddReward(rewa);
                    }
                }
                //item.AddActivity(activityCounter, personalityCSV[i][2], act);
                activityCounter++;
                ItemList.Add(item);
                itemExists = false;
            }
        }
        return ItemList;
    }

    private void getBaseActivites(string[][] personalityCSV)
    {
        Activity act;

        for(int i = 1; (i < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[i][0])); i++)
        {
            if (personalityCSV[i][1].Equals("non"))
            {
                act = new Activity(0, personalityCSV[i][2], null, 0, null);
                foreach(Reward rewa in _rewards)
                {
                    if(rewa.ID == Int32.Parse(personalityCSV[i][0]))
                    {
                        act.AddReward(rewa);
                    }
                }
                //_personality.AddBaseActivity(activityCounter, act.feedBackString, act);
                activityCounter++;
            }
        }
    }

    private void getRewards(string[][] personalityCSV)
    {
        Reward rew;
        
        for (int i = 1; i < personalityCSV.GetLength(0); i++)
        {
            int ID = Int32.Parse(personalityCSV[i][0]);

            rew = new Reward();
            rew.ID = ID;        //need to be changed in CSV -> Reward.ID instead of Activity.ID

            int value = Int32.Parse(personalityCSV[i][1]);
            rew.RewardValue = value;
            rew.RewardType = (NeedType)Enum.Parse(typeof(NeedType), (personalityCSV[i][2]), true);

            for(int j = 3; (j < personalityCSV[i].Length); j++)
            {
                if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                {
                    switch (personalityCSV[0][j])
                    {
                        case "MinHealth":
                            rew.MinHealth = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MaxHealth":
                            rew.MaxHealth = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MinHunger":
                            rew.MinHunger = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MaxHunger":
                            rew.MaxHunger = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MinSatisfaction":
                            rew.MinSatisfaction = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MaxSatisfaction":
                            rew.MaxSatisfaction = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MinSocial":
                            rew.MinSocial = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MaxSocial":
                            rew.MaxSocial = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MinEnergy":
                            rew.MinEnergy = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                        case "MaxEnergy":
                            rew.MaxEnergy = (Evaluation)Enum.Parse(typeof(Evaluation), personalityCSV[i][j], true);
                            break;
                    }
                }
            }
            _rewards.Add(rew);
        }
    }

    private void getAttributesAndNeeds(string[][] personalityCSV)
    {
        int[] thresholds;

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

            if (start != -1)
            {
                for (int k = start + 1; (k < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[k][0])); k++)
                {
                    switch (identifier)
                    {
                        case "attribute":
                            _personality.AddAttribute((AttributeType)Enum.Parse(typeof(AttributeType), personalityCSV[k][0]), new Attribute(Int32.Parse(personalityCSV[k][1]), MinAttribute, MaxAttribute));
                            break;
                        case "condition":
                            thresholds = new int[personalityCSV[k].Length - 1];
                            for (int j = 1; j < personalityCSV[k].Length; j++)
                            {
                                thresholds[j - 1] = Int32.Parse(personalityCSV[k][j]);
                            }
                            _personality.AddCondition((NeedType)Enum.Parse(typeof(NeedType), (personalityCSV[k][0])), new Need(ConditionStart, thresholds));
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
