using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Reward {
    public int ID;

    public NeedType RewardType;
    public int RewardValue;

    public Evaluation MinHealth;
    public Evaluation MaxHealth;

    public Evaluation MinHunger;
    public Evaluation MaxHunger;

    public Evaluation MinSocial;
    public Evaluation MaxSocial;

    public Evaluation MinEnergy;
    public Evaluation MaxEnergy;

    public Evaluation MinSatisfaction;
    public Evaluation MaxSatisfaction;

    public Reward()
    {
        MinHealth = Evaluation.SUICIDAL;
        MaxHealth = Evaluation.SUPER_GOOD;

        MinHunger = Evaluation.SUICIDAL;
        MaxHunger = Evaluation.SUPER_GOOD;

        MinSocial = Evaluation.SUICIDAL;
        MaxSocial = Evaluation.SUPER_GOOD;

        MinEnergy = Evaluation.SUICIDAL;
        MaxEnergy = Evaluation.SUPER_GOOD;

        MinSatisfaction = Evaluation.SUICIDAL;
        MaxSatisfaction = Evaluation.SUPER_GOOD;
    }

    public void DoReward(Personality personality, Dictionary<NeedType, Need> need)
    {
        if ((int)need[NeedType.HEALTH].getEvaluation() >= (int)MinHealth && (int)need[NeedType.HEALTH].getEvaluation() <= (int)MaxHealth)
        {
            if ((int)need[NeedType.HUNGER].getEvaluation() >= (int)MinHunger && (int)need[NeedType.HUNGER].getEvaluation() <= (int)MaxHunger)
            {
                if ((int)need[NeedType.SOCIAL].getEvaluation() >= (int)MinSocial && (int)need[NeedType.SOCIAL].getEvaluation() <= (int)MaxSocial)
                {
                    if ((int)need[NeedType.ENERGY].getEvaluation() >= (int)MinEnergy && (int)need[NeedType.ENERGY].getEvaluation() <= (int)MaxEnergy)
                    {
                        if ((int)need[NeedType.SATISFACTION].getEvaluation() >= (int)MinSatisfaction && (int)need[NeedType.SATISFACTION].getEvaluation() <= (int)MaxSatisfaction)
                        {
                            personality.GetCondition(RewardType).Value += RewardValue;
                        }
                    }
                }
            }
        }
    }

    public Reward Copy()
    {
        Reward reward = new Reward();
        reward.ID = ID;
        reward.RewardType = RewardType;
        reward.RewardValue = RewardValue;
        reward.MinHealth = MinHealth;
        reward.MaxHealth = MaxHealth;

        reward.MinHunger = MinHunger;
        reward.MaxHunger = MaxHunger;

        reward.MinSocial = MinSocial;
        reward.MaxSocial = MaxSocial;

        reward.MinEnergy = MinEnergy;
        reward.MaxEnergy = MaxEnergy;

        reward.MinSatisfaction = MinSatisfaction;
        reward.MaxSatisfaction = MaxSatisfaction;

        return reward;
    }
}
