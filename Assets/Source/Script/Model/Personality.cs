using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Personality {

    public Dictionary<NeedType, Need> Conditions;
    private Dictionary<int, Activity> BaseActivities;
    private Dictionary<int, Item> Items;


    private Dictionary<AttributeType, Attribute> Attributes;
    

	public Personality parent;
	public List<Personality> children;

    public Personality()
    {
        Conditions = new Dictionary<NeedType, Need>();
        Attributes = new Dictionary<AttributeType, Attribute>();
        BaseActivities = new Dictionary<int, Activity>();
        Items = new Dictionary<int, Item>();
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

    public Personality AddBaseActivity(int id, Activity activity)
    {
        BaseActivities[id] = activity;

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

    public Dictionary<NeedType, Need> GetConditions() {
        return Conditions;
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

	public int Evaluation(){
		return 0;
	}

    public void naturalStateReduction() {
        Conditions[NeedType.HUNGER].Value -= 3;
        Conditions[NeedType.ENERGY].Value -= 2;
        Conditions[NeedType.SOCIAL].Value -= 1;
    }

    public void printConditions() {
        string conditions = "";
        foreach (KeyValuePair<NeedType, Need> condition in Conditions) {
            conditions += condition.Key + ": " + condition.Value.Value + ", ";
        }
        Debug.Log(conditions);
    }
}
