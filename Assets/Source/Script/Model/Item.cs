using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item  {

    public int ID { get; set; }
    public string Name { get; set; }

	public int uses;
	public int maxUses;

    public Dictionary<int, Activity> Activities;

	public Item()
    {
        Activities = new Dictionary<int, Activity>();
    }

	public Item(int ID, string Name, int uses, int maxUses){
		this.ID = ID;
		this.Name = Name;
		this.uses = uses;
		this.maxUses = maxUses;
        Activities = new Dictionary<int, Activity>();
    }

    public Item AddActivity(Activity activity)
    {
        if (!Activities.ContainsKey(activity.ID))
        {
			Activities[activity.ID] = activity;
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

	public Item deepCopy(){
		Item item = new Item (ID, Name, uses, maxUses);

        foreach(Activity activity in GetAllActivities())
        {
            item.AddActivity(activity);
        }

        return item;
	}
}
