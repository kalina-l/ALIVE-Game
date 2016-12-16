using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonalityNode {

    public Dictionary<NeedType, Evaluation> Needs;
    public List<int> ActivityIDs;
	public List<Item> Items;

    public PersonalityNode Parent;
    public List<PersonalityNode> Children;

    public int ParentActionID;
    public int Depth;

    public float StoredEvaluation;
	public float SelfEvaluation;
	public float BestChildsEvaluation;
	public bool visited = false;

    public float FeedBack;

    public float DISCOUNT_FACTOR = 0.91f;

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

		Items = new List<Item>();
		foreach (KeyValuePair<int,Item> item in basePerson.GetItems()) {
			Items.Add (item.Value.deepCopy ());
		}

        Parent = null;
        Depth = 0;
        StoredEvaluation = 0;
		SelfEvaluation = 0;
		BestChildsEvaluation = int.MinValue;

        Children = new List<PersonalityNode>();
    }

	public PersonalityNode(PersonalityNode parent, Experience xp, int activityID, float feedback, Item usedItem, int activityUseConsume)
    {
        Parent = parent;
        Depth = parent.Depth + 1;
        FeedBack = feedback;

		if (usedItem != null) {
			usedItem.uses += activityUseConsume;
			if (usedItem.uses >= usedItem.maxUses) {
				Items.Remove (usedItem);
				foreach (Activity activity in usedItem.GetAllActivities()) {
					ActivityIDs.Remove (activity.ID);
				}
			}
		}

        Needs = new Dictionary<NeedType, Evaluation>();
        foreach (KeyValuePair<NeedType, Evaluation> need in parent.Needs)
        {
            Needs[need.Key] = (Evaluation)Mathf.Clamp((int)need.Value + xp.Rewards[need.Key], 0, 7);
        }

        ParentActionID = activityID;

        ActivityIDs = new List<int>();
        ActivityIDs.AddRange(parent.ActivityIDs);

        Children = new List<PersonalityNode>();

		SelfEvaluation = Evaluation ();
		BestChildsEvaluation = 0;
        //StoredEvaluation = Evaluation();
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
            value = value * Mathf.Pow(DISCOUNT_FACTOR, Depth - 1);
            //value += Parent.StoredEvaluation;
        }

        return value;
    }

	public void removeChildReference(PersonalityNode child){
		Children.Remove (child);
		child.Parent = null;
	}

	public void removeAllChildrenExceptOne(PersonalityNode child){
		for(int i = Children.Count-1; i >= 0; i--){
			if (Children[i] != child) {
				removeChildReference (Children[i]);
			}
		}
	}
}
