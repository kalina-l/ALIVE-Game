using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item  {

    public int ID { get; set; }
    public string Name { get; set; }

    public Dictionary<int, Activity> Activities;

	public Item()
    {
        Activities = new Dictionary<int, Activity>();
    }

    public Item AddActivity(int id, string name, Activity activity)
    {
        if (!Activities.ContainsKey(id))
        {
            Activities[id] = activity;
            activity.Name = name;
            activity.ID = id;
        }
        else
        {
            Debug.LogWarning(activity.Name + " is already added as a Activity");
        }

        return this;
    }

    public Activity GetActivity(int id)
    {
        if (Activities.ContainsKey(id))
        {
            return Activities[id];
        }

        return null;
    }

    public List<Activity> GetAllActivities()
    {
        List<Activity> activities = new List<Activity>();

        foreach (KeyValuePair<int, Activity> activity in Activities)
        {
            activities.Add(activity.Value);
        }

        return activities;
    }
}
