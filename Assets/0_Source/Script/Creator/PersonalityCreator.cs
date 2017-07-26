using UnityEngine;
using System;
using System.Collections.Generic;

public class PersonalityCreator
{
    public List<Reward> Rewards;
    public List<Item> ItemList;
    public Dictionary<TraitType, Trait> TraitList;

    public static readonly string AttributesNeedsTraitsCSV = "personality";
    public static readonly string RewardsCSV = "rewards";
    public static readonly string ActivitiesRewardsCSV = "activities_rewards";
    public static readonly string ItemsCSV = "items";

    public static readonly string ConditionIdentifier = "ConditionThresholds";
    public static readonly int ConditionStart = 0;
    public static readonly string AttributeIdentifier = "Attributes";
    public static readonly int MinAttribute = 1;
    public static readonly int MaxAttribute = 20;
    public static readonly string TraitIdentifier = "TraitModifier";

    private Personality _personality;
    public Personality Personality
    {
        get
        {
            return _personality;
        }
    }

    private string[][] _personalityCSV;
    private string[][] _itemCSV;

    public PersonalityCreator(string personalityCSVPath)
    {
        Rewards = new List<Reward>();
        _personalityCSV = CSV.read(personalityCSVPath + RewardsCSV);
        getRewards(_personalityCSV);

        _personality = new Personality();
        _personalityCSV = CSV.read(personalityCSVPath + AttributesNeedsTraitsCSV);
        getAttributesNeedsTraits(_personalityCSV);

        _personalityCSV = CSV.read(personalityCSVPath + ActivitiesRewardsCSV);
        _itemCSV = CSV.read(personalityCSVPath + ItemsCSV);
        getItemsAndAddBaseActivities(_itemCSV, _personalityCSV);
    }

    private List<Item> getItemsAndAddBaseActivities(string[][] ItemsCSV, string[][] personalityCSV)
    {
        ItemList = new List<Item>();
        Activity act = null;
        Item item = null;
        int ID = 0;
        string Name = null;
        int maxUses = 0;
        bool baseActivity = false;

        //first create Items from ItemsCSV
        for (int i = 1; (i < ItemsCSV.GetLength(0))/* && (!String.IsNullOrEmpty(ItemsCSV[i][0]))*/; i++)
        {

            for (int j = 0; (j < ItemsCSV[i].Length); j++)
            {

                switch (ItemsCSV[0][j])
                {
                    case "ID":
                        if (!int.TryParse(ItemsCSV[i][j], out ID) || ID < 0)
                        {
                            goto skip;
                        }
                        break;
                    case "Name":
                        if (ItemsCSV[i][j].Equals("Body"))
                        {
                            goto skip;
                        }
                        Name = ItemsCSV[i][j];
                        break;
                    case "maxUses":
                        if (!int.TryParse(ItemsCSV[i][j], out maxUses))
                        {
                            goto skip;
                        }
                        break;
                    default:
                        break;
                }
            }
            item = new Item(ID, Name, 0, maxUses);
            ItemList.Add(item);
            skip:;
        }

           
        //second add activities from activities_rewardsCSV & add BaseActivities
        for (int i = 1; (i < personalityCSV.GetLength(0)) /*&& (!String.IsNullOrEmpty(personalityCSV[i][0]))*/; i++)
        {
            item = null;
            for (int j = 0; (j < personalityCSV[i].Length); j++)
            {

                switch (personalityCSV[0][j])
                {
                    case "Activity.ID":
                        if (!int.TryParse(personalityCSV[i][j], out ID) || ID < 0)
                        {
                            goto skipping;
                        }
                        break;
                    case "Item":
                        if (personalityCSV[i][j].Equals("Body"))
                        {
                            baseActivity = true;
                            continue;
                        }
                        foreach (Item ite in ItemList)
                        {
                            if (personalityCSV[i][j].Equals(ite.Name))
                            {
                                item = ite;
                                break;
                            }
                        }
                        break;
                    case "Activity":
                        act = new Activity(ID, personalityCSV[i][j], item, 0, personalityCSV[i][j]);
                        if (!baseActivity)
                        {
                            act.Name = item.Name + "." + personalityCSV[i][j];
                            //act.feedBackString = personalityCSV[i][j] + " " + item.Name;
                        }
                        break;
                    case "Rewards":
                        int rewaID;
                        string[] actRewards = personalityCSV[i][j].Split(new[] { ',' });
                        foreach (Reward rewa in Rewards)
                        {
                            for (int q = 0; q < actRewards.Length; q++)
                            {
                                if (!int.TryParse(actRewards[q], out rewaID))
                                {
                                    Debug.Log("ERROR in READING REWARDS, " + actRewards[q] + " was not in correct format!");
                                }
                                if (rewa.ID == rewaID)
                                {
                                    act.AddReward(rewa);
                                }
                            }
                        }
                        break;
                    case "useConsum":
                        if (!int.TryParse(personalityCSV[i][j], out act.useConsume))
                        {
                            act.useConsume = item.maxUses;
                        }
                        break;
                    case "Tags for activities of same categories":
                        string[] activityTags = personalityCSV[i][j].Split(new[] { ',' });
                        for (int l = 0; l < activityTags.Length; l++)
                        {
                            ActivityTag tag = (ActivityTag)Enum.Parse(typeof(ActivityTag), activityTags[l], true);
                            act.Tags.Add(tag);
                            if (tag.Equals(ActivityTag.MULTIPLAYER))
                            {
                                act.IsMultiplayer = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (!act.IsMultiplayer && !baseActivity)
            {
                act.feedBackString = act.feedBackString + " " + act.item.Name;
            }

            if (baseActivity)
            {
                _personality.AddBaseActivity(act);
            }
            else
            {
                item.AddActivity(act);
            }
            baseActivity = false;
            skipping:;
        }
        return ItemList;
    }

    private void getRewards(string[][] personalityCSV)
    {
        Reward rew;

        for (int i = 1; (i < personalityCSV.GetLength(0))/* && (!String.IsNullOrEmpty(personalityCSV[i][0]))*/; i++)
        {
            rew = new Reward();
            for (int j = 0; (j < personalityCSV[i].Length); j++)
            {

                switch (personalityCSV[0][j])
                {
                    case "Reward ID":
                        int ID = Int32.Parse(personalityCSV[i][j]);
                        rew.ID = ID;
                        break;
                    case "Value":
                        int value = Int32.Parse(personalityCSV[i][j]);
                        rew.RewardValue = value;
                        break;
                    case "Type":
                        rew.RewardType = (NeedType)Enum.Parse(typeof(NeedType), (personalityCSV[i][j]), true);
                        break;
                    case "MinHealth":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MinHealth = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MaxHealth":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MaxHealth = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MinHunger":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MinHunger = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MaxHunger":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MaxHunger = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MinSatisfaction":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MinSatisfaction = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MaxSatisfaction":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MaxSatisfaction = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MinSocial":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MinSocial = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MaxSocial":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MaxSocial = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MinEnergy":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MinEnergy = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    case "MaxEnergy":
                        if (!String.IsNullOrEmpty(personalityCSV[i][j]))
                        {
                            string[] selection = personalityCSV[i][j].Split(new[] { '.' });
                            rew.MaxEnergy = (Evaluation)Enum.Parse(typeof(Evaluation), selection[1], true);
                        }
                        break;
                    default:
                        break;
                }
            }
            //Debug.Log(rew.ID + ": " + rew.RewardValue + " " + rew.RewardType);
            Rewards.Add(rew);
        }
    }

    private void getAttributesNeedsTraits(string[][] personalityCSV)
    {
        int[] thresholds;
        int[] thresholdModifier;
        List<Reward> activityModifier;
        TraitList = new Dictionary<TraitType, Trait>();

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
            else if(Array.IndexOf(personalityCSV[i], TraitIdentifier) != -1)
            {
                start = i;
                identifier = "trait";
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
                        case "trait":
                            Trait trait = new Trait((TraitType)Enum.Parse(typeof(TraitType), personalityCSV[k-1][1]));
                            trait.TraitTag = int.Parse(personalityCSV[k - 1][2]);
                            for(int l = k; (l < personalityCSV.GetLength(0)) && (!string.IsNullOrEmpty(personalityCSV[l][0])); l++)
                            {
                                NeedType needTypeMod;
                                if (StaticFunctions.ToEnum<NeedType>(personalityCSV[l][0], out needTypeMod))
                                {
                                    thresholdModifier = new int[personalityCSV[l].Length - 1];
                                    for (int j = 1; j < personalityCSV[l].Length; j++)
                                    {
                                        thresholdModifier[j - 1] = int.Parse(personalityCSV[l][j]) - _personality.Conditions[needTypeMod].Thresholds[j - 1];
                                    }
                                    trait.AddThresholdModifier(needTypeMod, thresholdModifier);
                                }
                                ActivityTag actTag;
                                if(StaticFunctions.ToEnum<ActivityTag>(personalityCSV[l][0], out actTag))
                                {
                                    activityModifier = new List<Reward>();
                                    string[] stringRewardsForActivityMod = personalityCSV[l][1].Split(new[] { ',' });
                                    int intReward;
                                    for (int p = 0; p < stringRewardsForActivityMod.Length; p++)
                                    {
                                        intReward = int.Parse(stringRewardsForActivityMod[p]);
                                        foreach (Reward reward in Rewards)
                                        {
                                            if(reward.ID == intReward)
                                            {
                                                activityModifier.Add(reward);
                                                break;
                                            }
                                        }
                                    }
                                    trait.AddActivityModifier(actTag, activityModifier);
                                }
                                if (personalityCSV[l][0].Equals("feedback"))
                                {
                                    float feedbackModifier = 0;
                                    foreach(char ch in personalityCSV[l][1])
                                    {
                                        switch(ch)
                                        {
                                            case '+':
                                                feedbackModifier += 25;
                                                break;
                                            case '-':
                                                feedbackModifier -= 25;
                                                break;
                                            default:
                                                break;
                                        };
                                    }
                                    trait.AddFeedbackModifier(feedbackModifier);
                                }
                                if (personalityCSV[l][0].Equals("askForItem"))
                                {
                                    int askForItemModifier = 0;
                                    foreach (char ch in personalityCSV[l][1])
                                    {
                                        switch (ch)
                                        {
                                            case '+':
                                                askForItemModifier += 50;
                                                break;
                                            case '-':
                                                askForItemModifier -= 50;
                                                break;
                                            default:
                                                break;
                                        };
                                    }
                                    trait.AddAskForItemModifier(askForItemModifier);
                                }
                                if (personalityCSV[l][0].Equals("similarExperienceDifference"))
                                {
                                    int similarExperienceModifier = 0;
                                    foreach (char ch in personalityCSV[l][1])
                                    {
                                        switch (ch)
                                        {
                                            //every + means that the similarExperienceDifference will increase so the Lemo will not try new Activities with "random learned" rewards (extreme case(value=int.MinValue): every closest experience will be taken -> see Activity)
                                            //->ANXIOUS
                                            case '+':
                                                similarExperienceModifier -= 5;
                                                break;
                                            //every - means that the similarExperienceDifference will decrease (so the value will increase - 3x+5 = 0) so the Lemo will try new Activities with "random learned" rewards (extreme case (value=0): no close experience will be taken -> see Activity)
                                            //->BRAVE
                                            case '-':
                                                similarExperienceModifier += 5;
                                                break;
                                            default:
                                                break;
                                        };
                                    }
                                    trait.AddSimilarExperienceDifferenceModifier(similarExperienceModifier);
                                }
                                k = l;
                            }
                            TraitList[trait.Identifier] = trait;
                            break;
                        default:
                            break;
                    }
                    i = k;
                }
            }
            start = -1;
        }


        //Emotions

        //good Emotion -> Aktivitäten mit Satisfaction: wenn fröhlich und das gemacht wird, was sowieso schon glücklich macht (-> positive Rewards werden ver-1.5-facht, negative Rewards werden halbiert)
        Emotion goodEmotion = new Emotion(EmotionType.GOOD, 4, 1);
        Trait temporaryTait = new Trait(TraitType.TEMPORARY_TRAIT);

        //Health-Thresholds all -15 (wird nicht so schnell “krank”)
        int[] healthThresholdModifier = new int[] { -15, -15, -15, -15, -15, -15, -15 };
        temporaryTait.AddThresholdModifier(NeedType.HEALTH, healthThresholdModifier);

        //Energy-Thresholds all -15 (ist energievoller)
        int[] energyThresholdModifier = new int[] { -15, -15, -15, -15, -15, -15, -15 };
        temporaryTait.AddThresholdModifier(NeedType.ENERGY, energyThresholdModifier);

        goodEmotion.AddTemporaryTrait(temporaryTait);
        _personality.AddEmotion(goodEmotion);



        //bad Emotion
        Emotion badEmotion = new Emotion(EmotionType.BAD, -4, -1);
        temporaryTait = new Trait(TraitType.TEMPORARY_TRAIT);

        //macht genau Gegenteil vom Feedback was er gelernt hat?!?
        temporaryTait.AddFeedbackModifier(-100);

        //Health-Thresholds all +15 (wird schneller “krank”)
        healthThresholdModifier = new int[] { 15, 15, 15, 15, 15, 15, 15 };
        temporaryTait.AddThresholdModifier(NeedType.HEALTH, healthThresholdModifier);

        //Social-Thresholds all +15 (fühlt sich unsozialer)
        int[] socialThresholdModifier = new int[] { 15, 15, 15, 15, 15, 15, 15 };
        temporaryTait.AddThresholdModifier(NeedType.SOCIAL, socialThresholdModifier);

        badEmotion.AddTemporaryTrait(temporaryTait);
        _personality.AddEmotion(badEmotion);
    }
}
