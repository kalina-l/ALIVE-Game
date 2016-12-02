using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Personality {

    public Dictionary<NeedType, Need> Conditions;
	public Dictionary<int, Activity> BaseActivities;
    public Dictionary<int, Item> Items;

    private Dictionary<AttributeType, Attribute> Attributes;
    
	public Personality parent;
	public List<Personality> children = new List<Personality>();
	public int deepnessInParent = 0;
	public int parentActionID;

    public float storedEvaluation;

    public Personality()
    {
        Conditions = new Dictionary<NeedType, Need>();
        Attributes = new Dictionary<AttributeType, Attribute>();
        BaseActivities = new Dictionary<int, Activity>();
        Items = new Dictionary<int, Item>();
        deepnessInParent = 0;
    }

	public Personality(Personality parent, int parentActionID){

        Conditions = new Dictionary<NeedType, Need>();

        foreach(KeyValuePair<NeedType, Need> need in parent.Conditions)
        {
            Conditions[need.Key] = need.Value.Copy();
        }
        
		BaseActivities = parent.BaseActivities;
		Items = parent.Items;
		this.parent = parent;
		this.parentActionID = parentActionID;
		deepnessInParent = parent.deepnessInParent + 1;
	}

    //Attributes
    public Personality AddCondition(NeedType value, Need condition)
    {
        Conditions[value] = condition;

        return this;
    }

    public Personality AddAttribute(AttributeType value, Attribute attribute)
    {
        Attributes[value] = attribute;

        return this;
    }

    public Personality AddBaseActivity(int id, string name, Activity activity)
    {
        BaseActivities[id] = activity;

        activity.ID = id;
        activity.Name = name;

        return this;
    }

    //Getter
    public Need GetCondition(NeedType value)
    {
        if (Conditions.ContainsKey(value))
        {
            return Conditions[value];
        }

        Debug.LogError("Condition " + value + " doesn't exist!");

        return null;
    }


    public Attribute GetAttribute(AttributeType value)
    {
        if (Attributes.ContainsKey(value))
        {
            return Attributes[value];
        }

        Debug.LogError("Attribute " + value + " doesn't exist!");

        return null;
    }

    public Activity GetActivity(int id)
    {
        if (BaseActivities.ContainsKey(id))
        {
            return BaseActivities[id];
        }

        foreach(KeyValuePair<int, Item> item in Items)
        {
            if (item.Value.GetActivity(id) != null)
                return item.Value.GetActivity(id);
        }

        Debug.LogError("Activity " + id + " doesn't exist!");

        return null;
    }

    public List<Activity> GetAllActivities()
    {
        List<Activity> activities = new List<Activity>();

        foreach (KeyValuePair<int, Activity> activity in BaseActivities)
        {
            activities.Add(activity.Value);
        }

        foreach (KeyValuePair<int, Item> item in Items)
        {
            activities.AddRange(item.Value.GetAllActivities());
        }

        return activities;
    }

	public Dictionary<int, Item> GetItems() {
		return Items;
	}

    public Item GetItem(int id)
    {
        if (Items.ContainsKey(id))
        {
            return Items[id];
        }

        Debug.LogError("Item " + id + " doesn't exist!");

        return null;
    }

    //Actions
    public void AddItem(int id, Item item)
    {
        Items[id] = item;
    }

    public void RemoveItem(int id)
    {
        if (Items.ContainsKey(id))
        {
            Items.Remove(id);
        }
        else
        {
            Debug.LogWarning("Item " + id + " couldn't be removed, because it's not in the dictionairy.");
        }
    }

	public float Evaluation(){
        float value = 0;

        foreach(KeyValuePair<NeedType, Need> need in Conditions)
        {
            switch(need.Value.getEvaluation())
            {
                case global::Evaluation.SUICIDAL:
                    value -= 300;
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
        if (parent != null)
        {
            value = value * Mathf.Pow(ApplicationManager.DISCOUNT_FACTOR, deepnessInParent - 1);
            value += parent.storedEvaluation;
        }

        return value;
	}

    public void printConditions() {
        string conditions = "";
        foreach (KeyValuePair<NeedType, Need> condition in Conditions) {
            conditions += condition.Key + ": " + condition.Value.Value + ", ";
        }
        Debug.Log(conditions);
    }
}
