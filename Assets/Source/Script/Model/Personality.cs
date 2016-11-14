using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Personality {

    private Dictionary<string, Condition> Conditions;
    private Dictionary<string, Attribute> Attributes;

    private Dictionary<string, Activity> BaseActivities;
    private Dictionary<string, Item> Items;

    public Personality()
    {
        Conditions = new Dictionary<string, Condition>();
        Attributes = new Dictionary<string, Attribute>();
        BaseActivities = new Dictionary<string, Activity>();
        Items = new Dictionary<string, Item>();
    }

    //Attributes
    public Personality AddCondition(string name, Condition condition)
    {
        Conditions[name] = condition;

        return this;
    }

    public Personality AddAttribute(string name, Attribute attribute)
    {
        Attributes[name] = attribute;

        return this;
    }

    public Personality AddBaseActivity(string name, Activity activity)
    {
        BaseActivities[name] = activity;

        return this;
    }

    //Getter
    public Condition GetCondition(string name)
    {
        if (Conditions.ContainsKey(name))
        {
            return Conditions[name];
        }

        Debug.LogError("Condition " + name + " doesn't exist!");

        return null;
    }

    public Dictionary<string, Condition> GetConditions() {
        return Conditions;
    }

    public Attribute GetAttribute(string name)
    {
        if (Attributes.ContainsKey(name))
        {
            return Attributes[name];
        }

        Debug.LogError("Attribute " + name + " doesn't exist!");

        return null;
    }

    public Activity GetActivity(string name)
    {
        if (BaseActivities.ContainsKey(name))
        {
            return BaseActivities[name];
        }

        foreach(KeyValuePair<string, Item> item in Items)
        {
            if (item.Value.GetActivity(name) != null)
                return item.Value.GetActivity(name);
        }

        Debug.LogError("Activity " + name + " doesn't exist!");

        return null;
    }

    public List<Activity> GetAllActivities()
    {
        List<Activity> activities = new List<Activity>();

        foreach (KeyValuePair<string, Activity> activity in BaseActivities)
        {
            activities.Add(activity.Value);
        }

        foreach (KeyValuePair<string, Item> item in Items)
        {
            activities.AddRange(item.Value.GetAllActivities());
        }

        return activities;
    }

    public Item GetItem(string name)
    {
        if (Items.ContainsKey(name))
        {
            return Items[name];
        }

        Debug.LogError("Item " + name + " doesn't exist!");

        return null;
    }

    //Actions
    public void AddItem(string name, Item item)
    {
        Items[name] = item;
    }

    public void RemoveItem(string name)
    {
        if (Items.ContainsKey(name))
        {
            Items.Remove(name);
        }
        else
        {
            Debug.LogWarning("Item " + name + " couldn't be removed, because it's not in the dictionairy.");
        }
    }

    public void naturalStateReduction() {
        Conditions["HUNGER"].value -= 3;
        Conditions["ENERGY"].value -= 2;
        Conditions["SOCIAL"].value -= 1;
    }

    public void printConditions() {
        string conditions = "";
        foreach (KeyValuePair<string, Condition> condition in Conditions) {
            conditions += condition.Key + ": " + condition.Value.value + ", ";
        }
        Debug.Log(conditions);
    }
}
