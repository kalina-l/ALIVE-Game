using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;

public class Personality {

    public static int PENDING_ACTIVITY_ID = 9999;

    public Dictionary<NeedType, Need> Conditions;
	public Dictionary<int, Activity> BaseActivities;
    public Dictionary<int, Item> Items;
    public List<Trait> Traits;

    [SerializeField]
    private Dictionary<AttributeType, Attribute> Attributes;
    
    //[fsIgnore]
	public Personality parent;
    //[fsIgnore]
	public List<Personality> children = new List<Personality>();
    //[fsIgnore]
	public int deepnessInParent = 0;
    //[fsIgnore]
	public int parentActionID;

    public float storedEvaluation;

    public Personality()
    {
        Conditions = new Dictionary<NeedType, Need>();
        Attributes = new Dictionary<AttributeType, Attribute>();
        BaseActivities = new Dictionary<int, Activity>();
        Items = new Dictionary<int, Item>();
        Traits = new List<Trait>();
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

    public Personality AddBaseActivity(Activity activity)
    {
		BaseActivities[activity.ID] = activity;

        return this;
    }

    public void RemovePendingActivity()
    {
        BaseActivities.Remove(PENDING_ACTIVITY_ID);
    }

    public Personality AddTrait(Trait trait, List<Item> itemList)
    {
        Need need;

        foreach(Trait traitThere in Traits)
        {
            if(traitThere.TraitTag == trait.TraitTag)
            {
                Debug.LogWarning(traitThere.Identifier + " is already added as Trait in this category! " + trait.Identifier + " won't be added!");
                return this;
            } 
        }
        Traits.Add(trait);
        foreach (KeyValuePair<NeedType, int[]> tr in trait.ThresholdModifiers)
        {
            if ((need = GetCondition(tr.Key)) != null)
            {
                need.Thresholds = tr.Value;
            }
            else
            {
                Debug.LogError("There is no " + tr.Key + " for modification!");
            }
        }

        /*Go through every activity in every item to add special-trait-rewards*/
        foreach (Item item in itemList)
        {
            foreach (Activity activity in item.GetAllActivities())
            {
                foreach (ActivityTag actTag in activity.Tags)
                {
                    foreach (KeyValuePair<ActivityTag, List<Reward>> kvp in trait.ActivityModifiers)
                    {
                        //Debug.Log("actTag of "+activity.Name+", " + actTag + ", Tag of "+trait.Identifier+": " + kvp.Key);
                        if (actTag.Equals(kvp.Key))
                        {
                            foreach (Reward reward in kvp.Value)
                            {
                                activity.AddReward(reward);
                            }
                        }
                    }
                }
                //string test = ""+activity.Name; 
                //foreach(Reward rewi in activity.RewardList)
                //{
                //    test += " "+rewi.ID+": "+rewi.MinSocial+" - "+rewi.MaxSocial+", "+rewi.RewardType+": "+rewi.RewardValue+"\n";
                //}
                //Debug.Log(test);
            }
        }

        /*Go through every BaseActivity of the personality to add special-trait-rewards*/
        foreach (KeyValuePair<int, Activity> kvp in BaseActivities)
        {
            foreach (ActivityTag actTag in kvp.Value.Tags)
            {
                foreach (KeyValuePair<ActivityTag, List<Reward>> kvpi in trait.ActivityModifiers)
                {
                    if (actTag.Equals(kvpi.Key))
                    {
                        foreach (Reward reward in kvpi.Value)
                        {
                            kvp.Value.AddReward(reward);
                        }
                    }
                }
            }
        }

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

    public Activity GetActivity(int id, bool showLog = true)
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

        if(showLog)
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

    public void PrintAllRewards()
    {
        DebugController.Instance.Log("-------------------------------------", DebugController.DebugType.Activity);

        List<Activity> activities = GetAllActivities();

        for (int i = 0; i < activities.Count; i++)
        {
            if (!activities[i].IsMultiplayer || ApplicationManager.Instance.Multiplayer.IsConnected)
            {
                activities[i].PrintExperience(this);
            }
        }

        DebugController.Instance.Log("-------------------------------------", DebugController.DebugType.Activity);
    }

	public Dictionary<int, Item> GetItems() {
		return Items;
	}

    public Item GetItem(int id, bool log = true)
    {
        if (Items.ContainsKey(id))
        {
            return Items[id];
        }

        if (log)
        {
            Debug.LogError("Item " + id + " doesn't exist!");
        }

        return null;
    }

    //Actions
    public void AddItem(int id, Item item)
    {
        Items[id] = item;
		item.uses = 0;
    }

    public void RemoveItem(int id)
    {
        if (Items.ContainsKey(id))
        {
            Items[id].uses = 0;
            Items.Remove(id);
        }
        else
        {
            Debug.LogWarning("Item " + id + " couldn't be removed, because it's not in the dictionairy.");
        }
    }

    public void printConditions() {
        string conditions = "";
        foreach (KeyValuePair<NeedType, Need> condition in Conditions) {
            conditions += condition.Key + ": " + condition.Value.Value + ", ";
        }
        Debug.Log(conditions);
    }
}
