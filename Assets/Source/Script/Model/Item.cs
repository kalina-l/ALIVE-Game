using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item  {

    private string _name;
    public string name { get; set; }

    public Dictionary<string, Activity> Activities;

	public Item()
    {
        Activities = new Dictionary<string, Activity>();
    }

    public Item AddActivity(string name, Activity activity)
    {
        if (!Activities.ContainsKey(name))
        {
            Activities[name] = activity;
        }
        else
        {
            Debug.LogWarning(name + " is already added as a Activity");
        }

        return this;
    }

    public Activity GetActivity(string name)
    {
        if (Activities.ContainsKey(name))
        {
            return Activities[name];
        }

        return null;
    }

    public List<Activity> GetAllActivities()
    {
        List<Activity> activities = new List<Activity>();

        foreach (KeyValuePair<string, Activity> activity in Activities)
        {
            activities.Add(activity.Value);
        }

        return activities;
    }
}
