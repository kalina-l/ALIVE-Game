using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonalityNode {

    public Dictionary<NeedType, Evaluation> Needs;
    public List<int> ActivityIDs;

    public PersonalityNode Parent;
    public List<PersonalityNode> Children;

    public int ParentActionID;
    public int Depth;

    public float StoredEvaluation;

    public int FeedBack;

    public PersonalityNode(Personality basePerson)
    {
        Needs = new Dictionary<NeedType, Evaluation>();

        foreach(KeyValuePair<NeedType, Need> need in basePerson.Conditions)
        {
            Needs[need.Key] = need.Value.getEvaluation();
        }

        ActivityIDs = new List<int>();
        foreach(Activity activity in basePerson.GetAllActivities())
        {
            ActivityIDs.Add(activity.ID);
        }

        Parent = null;
        Depth = 0;
        StoredEvaluation = 0;

        Children = new List<PersonalityNode>();
    }

    public PersonalityNode(PersonalityNode parent, Experience xp, int activityID)
    {
        Parent = parent;
        Depth = parent.Depth + 1;
        FeedBack = xp.Feedback;

        Needs = new Dictionary<NeedType, Evaluation>();
        foreach (KeyValuePair<NeedType, Evaluation> need in parent.Needs)
        {
            Needs[need.Key] = (Evaluation)((int)need.Value + xp.Rewards[need.Key]);
        }

        ParentActionID = activityID;

        ActivityIDs = new List<int>();
        ActivityIDs.AddRange(parent.ActivityIDs);

        Children = new List<PersonalityNode>();

        StoredEvaluation = Evaluation();
    }

    public float Evaluation()
    {
        float value = FeedBack * 100;

        foreach (KeyValuePair<NeedType, Evaluation> need in Needs)
        {
            switch (need.Value)
            {
                case global::Evaluation.SUICIDAL:
                    value -= 500;
                    break;
                case global::Evaluation.SUPER_BAD:
                    value -= 150;
                    break;
                case global::Evaluation.VERY_BAD:
                    value -= 100;
                    break;
                case global::Evaluation.BAD:
                    value -= 50;
                    break;
                case global::Evaluation.NEUTRAL:
                    value -= 0;
                    break;
                case global::Evaluation.GOOD:
                    value += 40;
                    break;
                case global::Evaluation.VERY_GOOD:
                    value += 80;
                    break;
                case global::Evaluation.SUPER_GOOD:
                    value += 120;
                    break;
            }
        }

        //Discounting
        if (Parent != null)
        {
            value = value * Mathf.Pow(ApplicationManager.DISCOUNT_FACTOR, Depth - 1);
            value += Parent.StoredEvaluation;
        }

        return value;
    }
}
