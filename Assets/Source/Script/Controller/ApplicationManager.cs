﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LoadStates { UseDummy, UseCSV, UseSavedState};

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public Canvas UICanvas;
    public LoadStates Load;

    public float WaitTime = 2;
    public float FeedBackTime = 2;
    public int AutomaticSaveAfterActions = 10;
    private int saveCounter;
    private int activityCounter;

    private string personalityCSVPath = "Data\\";
    public string SaveFile = "please_specify_the_filename_for_saving";
    public string LoadFile = "please_specify_the_filename_for_Loading";

    //AI
    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;
    private List<Item> _itemList;
    private List<Reward> rewardList;
    

    private Experience _lastExperience;
    private Activity _lastActivity;

    //UI
    private OutputViewController _output;
    private FeedbackViewController _feedback;
    private ItemBoxViewController _itemBox;
    private ConditionViewController _conditionMonitor;

    private bool waitForFeedback;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        _itemList = new List<Item>();
        _items = new Dictionary<int, Item>();

        switch(Load)
        {
            case LoadStates.UseDummy:
                DummyCreator creator = new DummyCreator();
                _personality = creator.CreateDummyPerson();
                _itemList = creator.CreateDummyItems();
                break;

            case LoadStates.UseCSV:
                PersonalityCreator creatorCSV = new PersonalityCreator(personalityCSVPath);
                _personality = creatorCSV.personality;
                _itemList = creatorCSV.ItemList;
                rewardList = creatorCSV.Rewards;
                break;

            case LoadStates.UseSavedState:
                JSON readJSON = new JSON(_personality, rewardList, _itemList);
                readJSON.readJSON(readJSON, LoadFile);
                _personality = readJSON.personality;
                rewardList = readJSON.rewardList;
                _itemList = readJSON.itemList;
                break;
            default:
                Debug.LogError("No Load State");
                break;
        }

        foreach (Item item in _itemList)
        {
            _items[item.ID] = item;
            //_personality.AddItem(item.ID, item);
        }
        
        _intelligence = new ArtificialIntelligence();

        //UI
        _output = new OutputViewController(UICanvas.transform);
        _feedback = new FeedbackViewController(UICanvas.transform, _intelligence);
        _itemBox = new ItemBoxViewController(UICanvas.transform, _items, _personality);
        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);

        if (_personality.GetItems().Count >= 1)
        {
            foreach (KeyValuePair<int, Item> kvp in _personality.GetItems())
            {
                _itemBox.AddItemFromPersonality(kvp.Value);
            }
        }

        saveCounter = 0;
        StartCoroutine(Run());
    }

	public void RemoveItem(Item item){
		_itemBox.RemoveItemFromSlot ();
	}

    private IEnumerator DoActivityRoutine()
    {
        //GetActivity

        bool askForItem = activityCounter >= 2;

        Debug.Log(activityCounter);
        
        if (askForItem)
        {
            Debug.Log("Ask for Item!!!");
            _intelligence.AskForItem(_personality, _itemList);
        }
        else
        {
            activityCounter++;
            _intelligence.GetNextActivity(_personality);
        }

        float timer = 0;

        while(!_intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _intelligence.GetResult();

        Debug.Log("Calculation took " + timer + " seconds " + askForItem.ToString());
        

        if (activityID != -1)
        {
            if (askForItem)
            {
                Item askItem = null;

                for (int i = 0; i < _itemList.Count; i++)
                {
                    foreach (Activity activity in _itemList[i].GetAllActivities())
                    {
                        if (activity.ID == activityID)
                        {
                            askItem = _itemList[i];
                        }
                    }
                }

                //TODO Create an Activity that asks for the Item
                if (askItem != null)
                {
                    _output.DisplayMessage("Give me " + askItem.Name);
                }
                else
                {
                    Debug.Log("NO ITEM NEEDED - I want to " + _personality.GetActivity(activityID).feedBackString);
                }

                activityCounter = 0;
            }
            else
            {
                _lastActivity = _personality.GetActivity(activityID);
                _lastExperience = _lastActivity.DoActivity(_personality);

                //Show Activity
                _conditionMonitor.UpdateSlider(_personality);
                _output.DisplayMessage(_lastActivity.feedBackString);

                //Ask for Feedback
                _feedback.ShowFeedback(true);
                waitForFeedback = true;
            }
        }
        else
        {
            _lastActivity = null;
        }
    }

    public void GiveFeedback(int feedback)
    {
        if (waitForFeedback)
        {
            if (_lastActivity != null)
            {
                //Store Feedback in Activity
                _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);

                //Show Feedback
                _output.ShowFeedback(feedback);
            }

            _feedback.ShowFeedback(false);

            waitForFeedback = false;
        }
    }

    private IEnumerator Run()
    {
        Debug.Log("Start!");

        while(true)
        {
            Debug.Log("Wait");
            yield return new WaitForSeconds(WaitTime);

            Debug.Log("Do Activity");
            yield return StartCoroutine(DoActivityRoutine());

            Debug.Log("Feedback");

            float timer = 0;

            while(timer < FeedBackTime && waitForFeedback)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            if(waitForFeedback)
            {
                GiveFeedback(0);
            }

            if (saveCounter >= AutomaticSaveAfterActions)
            {
                JSON writeJSON = new JSON(_personality, rewardList, _itemList);
                writeJSON.writeJSON(writeJSON, SaveFile);
                Debug.Log("Status saved!");
                saveCounter = 0;
            }
            else
            {
                saveCounter++;
            }
        }
    }

}
