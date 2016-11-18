using UnityEngine;
using System.Collections;

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

    public void DoReward(Personality personality)
    {
        if ((int)personality.GetCondition(NeedType.HEALTH).getEvaluation() >= (int)MinHealth && (int)personality.GetCondition(NeedType.HEALTH).getEvaluation() <= (int)MaxHealth)
        {
            if ((int)personality.GetCondition(NeedType.HUNGER).getEvaluation() >= (int)MinHunger && (int)personality.GetCondition(NeedType.HUNGER).getEvaluation() <= (int)MaxHunger)
            {
                if ((int)personality.GetCondition(NeedType.SOCIAL).getEvaluation() >= (int)MinSocial && (int)personality.GetCondition(NeedType.SOCIAL).getEvaluation() <= (int)MaxSocial)
                {
                    if ((int)personality.GetCondition(NeedType.ENERGY).getEvaluation() >= (int)MinEnergy && (int)personality.GetCondition(NeedType.ENERGY).getEvaluation() <= (int)MaxEnergy)
                    {
                        if ((int)personality.GetCondition(NeedType.SATISFACTION).getEvaluation() >= (int)MinSatisfaction && (int)personality.GetCondition(NeedType.SATISFACTION).getEvaluation() <= (int)MaxSatisfaction)
                        {
                            personality.GetCondition(RewardType).Value += RewardValue;
                        }
                    }
                }
            }
        }
    }
}
