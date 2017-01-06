using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class PersonalityCreator
{
    public List<Reward> Rewards;
    public List<Item> ItemList;

    public static readonly string AttributesAndNeedsCSV = "personality";
    public static readonly string RewardsCSV = "rewards";
    public static readonly string ActivitiesRewardsCSV = "activities_rewards";

    public static readonly string ConditionIdentifier = "ConditionThresholds";
    public static readonly int ConditionStart = 100;
    public static readonly string AttributeIdentifier = "Attributes";
    public static readonly int MinAttribute = 1;
    public static readonly int MaxAttribute = 20;

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
        Rewards = new List<Reward>();
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
        int ID;
        for (int i = 1; (i < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[i][0])); i++)
        {
            if (!personalityCSV[i][1].Equals("Body"))
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
                    item.maxUses = 100;
                    itemCounter++;
                }
                ID = Int32.Parse(personalityCSV[i][0]);
                act = new Activity(ID, item.Name + "." + personalityCSV[i][2], item, 1, personalityCSV[i][2] + " " + item.Name);
                string[] actRewards = personalityCSV[i][3].Split(new[] { ',' });
                int[] activityRewards = new int[actRewards.Length];
                for (int p = 0; p < activityRewards.Length; p++)
                {
                    activityRewards[p] = Int32.Parse(actRewards[p]);
                }
                foreach (Reward rewa in Rewards)
                {
                    for (int q = 0; q < activityRewards.Length; q++)
                    {
                        int rewaID = activityRewards[q];
                        if (rewa.ID == rewaID)
                        {
                            act.AddReward(rewa);
                        }
                    }
                }
                item.AddActivity(act);
                if (!itemExists)
                {
                    ItemList.Add(item);
                }
                itemExists = false;
            }
        }
        return ItemList;
    }

    private void getBaseActivites(string[][] personalityCSV)
    {
        Activity act;
        int ID;

        for(int i = 1; (i < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[i][0])); i++)
        {
            if (personalityCSV[i][1].Equals("Body"))
            {
                ID = Int32.Parse(personalityCSV[i][0]);
                act = new Activity(ID, personalityCSV[i][2], null, 0, personalityCSV[i][2]);
                string[] actRewards = personalityCSV[i][3].Split(new[] { ',' });
                int[] activityRewards = new int[actRewards.Length];
                for(int p = 0; p < activityRewards.Length; p++)
                {
                    activityRewards[p] = Int32.Parse(actRewards[p]);
                }
                foreach (Reward rewa in Rewards)
                {
                    for (int q = 0; q < activityRewards.Length; q++)
                    {
                        int rewaID = activityRewards[q];
                        if(rewa.ID == rewaID)
                        {
                            act.AddReward(rewa);
                        } 
                    }
                }

                _personality.AddBaseActivity(act);
            }
        }
    }

    private void getRewards(string[][] personalityCSV)
    {
        Reward rew;

        for (int i = 1; (i < personalityCSV.GetLength(0)) && (!String.IsNullOrEmpty(personalityCSV[i][0])); i++)
        {
            int ID = Int32.Parse(personalityCSV[i][0]);

            rew = new Reward();
            rew.ID = ID;        

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
            Rewards.Add(rew);
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
                            AttributeType attributeType = (AttributeType)Enum.Parse(typeof(AttributeType), personalityCSV[k][0]);
                            Attribute attribute = new Attribute(Int32.Parse(personalityCSV[k][1]), MinAttribute, MaxAttribute);
                            attribute.Identifier = attributeType;
                            _personality.AddAttribute(attributeType, attribute);
                            break;
                        case "condition":
                            thresholds = new int[personalityCSV[k].Length - 1];
                            for (int j = 1; j < personalityCSV[k].Length; j++)
                            {
                                thresholds[j - 1] = Int32.Parse(personalityCSV[k][j]);
                            }
                            NeedType needType = (NeedType)Enum.Parse(typeof(NeedType), (personalityCSV[k][0]));
                            Need need = new Need(ConditionStart, thresholds);
                            need.Type = needType;
                            _personality.AddCondition(needType, need);
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
