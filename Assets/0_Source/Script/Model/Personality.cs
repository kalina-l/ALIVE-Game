using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;

public class Personality {

    public Dictionary<NeedType, Need> Conditions;
	public Dictionary<int, Activity> BaseActivities;
    public Dictionary<int, Item> Items;
    public List<Trait> Traits;
    public Dictionary<EmotionType, Emotion> Emotions;          //possible Emotions
    public int emotionCounter;
    [SerializeField]
    private int lastEmotionCounter;
    public EmotionType executedEmotion;

    public MultiplayerController Multiplayer { get; set; }

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
        Emotions = new Dictionary<EmotionType, Emotion>();
        emotionCounter = 0;
        executedEmotion = EmotionType.NORMAL;
        lastEmotionCounter = -1;
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
                for (int i = 0; i < tr.Value.Length; i++)
                {
                    need.Thresholds[i] += tr.Value[i];
                }
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

        PersonalityNode.FEEDBACK_FACTOR += trait.FeedbackModifier;
        GameLoopController.ASK_FOR_ITEM_FACTOR += trait.AskForItemModifier;
        Activity.SIMILAR_EXPERIENCE_DIFFERENCE += trait.SimilarExperienceDifferenceModifier;

        return this;
    }

    public Personality RemoveTrait(Trait trait, List<Item> itemList)
    {
        Need need;

        if (!Traits.Remove(trait))
        {
            Debug.LogWarning(trait.Identifier+" can't be removed because it was not added to the personality!");
        }

        foreach (KeyValuePair<NeedType, int[]> tr in trait.ThresholdModifiers)
        {
            if ((need = GetCondition(tr.Key)) != null)
            {
                for (int i = 0; i < tr.Value.Length; i++)
                {
                    need.Thresholds[i] -= tr.Value[i];
                }
            }
            else
            {
                Debug.LogError("There is no " + tr.Key + " for modification!");
            }
        }

        /*Go through every activity in every item to delete special-trait-rewards*/
        foreach (Item item in itemList)
        {
            foreach (Activity activity in item.GetAllActivities())
            {
                foreach (ActivityTag actTag in activity.Tags)
                {
                    foreach (KeyValuePair<ActivityTag, List<Reward>> kvp in trait.ActivityModifiers)
                    {
                        if (actTag.Equals(kvp.Key))
                        {
                            foreach (Reward reward in kvp.Value)
                            {
                                activity.RemoveReward(reward);
                            }
                        }
                    }
                }
            }
        }

        /*Go through every BaseActivity of the personality to delete special-trait-rewards*/
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
                            kvp.Value.RemoveReward(reward);
                        }
                    }
                }
            }
        }

        PersonalityNode.FEEDBACK_FACTOR -= trait.FeedbackModifier;
        GameLoopController.ASK_FOR_ITEM_FACTOR -= trait.AskForItemModifier;
        Activity.SIMILAR_EXPERIENCE_DIFFERENCE -= trait.SimilarExperienceDifferenceModifier;

        return this;
    }

    public Personality AddEmotion(Emotion emotion)
    {
        Emotions[emotion.EmotionType] = emotion;
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
            if (!activities[i].IsMultiplayer || Multiplayer.MultiplayerOn)
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
        DebugController.Instance.Log("---------- Add Item " + item.Name + " -------------", DebugController.DebugType.Activity);

        Items[id] = item;
		item.uses = 0;
    }

    public void RemoveItem(int id)
    {
        if (Items.ContainsKey(id))
        {
            DebugController.Instance.Log("---------- Remove Item " + Items[id].Name + " -------------", DebugController.DebugType.Activity);

            Items[id].uses = 0;
            Items.Remove(id);
        }
        else
        {
            Debug.LogWarning("Item " + id + " couldn't be removed, because it's not in the dictionairy.");
        }
    }

    public bool ActivateEmotion(Emotion emotion)
    {
        DebugController.Instance.Log("Emotion executed", DebugController.DebugType.Emotion);
        emotion.ActivateEmotion(this);
        emotionCounter = emotion.Trigger;
        executedEmotion = emotion.EmotionType;
        return true;
    }

    public bool DeactivateEmotion(Emotion emotion)
    {
        DebugController.Instance.Log("Emotion dissolved", DebugController.DebugType.Emotion);
        emotion.DeactivateEmotion(this);
        executedEmotion = EmotionType.NORMAL;
        return true;
    }

    public void checkEmotion()
    {
        DebugController.Instance.Log("EmotionCounter: " + emotionCounter, DebugController.DebugType.Emotion);

        if (executedEmotion == EmotionType.NORMAL)
        {
            foreach (KeyValuePair<EmotionType, Emotion> kvp in Emotions)
            {
                Emotion emotion = kvp.Value;
                if ((emotion.Trigger < 0) && (emotionCounter <= emotion.Trigger))
                {
                    ActivateEmotion(emotion);
                }
                if ((emotion.Trigger > 0) && (emotionCounter >= emotion.Trigger))
                {
                    ActivateEmotion(emotion);
                }
            }

            if(lastEmotionCounter == emotionCounter)
            {
                emotionCounter = emotionCounter < 0 ? emotionCounter + 1 : emotionCounter - 1;
            }
        }
        else
        {
            emotionCounter = emotionCounter < 0 ? emotionCounter+1 : emotionCounter-1;
            if(emotionCounter == 0)
            {
                DeactivateEmotion(Emotions[executedEmotion]);
            }
        }
        lastEmotionCounter = emotionCounter;
    }

    public void printConditions() {
        string conditions = "";
        foreach (KeyValuePair<NeedType, Need> condition in Conditions) {
            conditions += condition.Key + ": " + condition.Value.Value + ", ";
        }
        Debug.Log(conditions);
    }
}
