using UnityEngine;
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

    //AI
    private string personalityCSVPath = "Data\\";
    //private string JSONPath = "Data\\JSON\\";
    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;
    private List<Item> itemList;
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
        itemList = new List<Item>();
        _items = new Dictionary<int, Item>();

        switch(Load)
        {
            case LoadStates.UseDummy:
                DummyCreator creator = new DummyCreator();
                _personality = creator.CreateDummyPerson();
                itemList = creator.CreateDummyItems();
                break;

            case LoadStates.UseCSV:
                PersonalityCreator creatorCSV = new PersonalityCreator(personalityCSVPath);
                _personality = creatorCSV.personality;
                itemList = creatorCSV.ItemList;
                rewardList = creatorCSV.Rewards;
                break;

            case LoadStates.UseSavedState:
                JSON readJSON = new JSON(_personality, rewardList, itemList);
                readJSON.readJSON(readJSON);
                _personality = readJSON.personality;
                rewardList = readJSON.rewardList;
                itemList = readJSON.itemList;
                break;
            default:
                Debug.LogError("No Load State");
                break;
        }

        foreach (Item item in itemList)
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

        saveCounter = 0;
        StartCoroutine(Run());
    }

	public void RemoveItem(Item item){
		_itemBox.RemoveItemFromSlot ();
	}

    private IEnumerator DoActivityRoutine()
    {
        //GetActivity
        _intelligence.GetNextActivity(_personality);

        float timer = 0;

        while(!_intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _intelligence.GetResult();

        Debug.Log("Calculation took " + timer + " seconds");

        

        if (activityID != -1)
        {
            _lastActivity = _personality.GetActivity(activityID);

            //Apply Rewards
            _lastExperience = _lastActivity.DoActivity(_personality);

            //Show Activity
            _conditionMonitor.UpdateSlider(_personality);
            _output.DisplayMessage(_lastActivity.feedBackString);

            //Ask for Feedback
            _feedback.ShowFeedback(true);
            waitForFeedback = true;
        }
        else
        {
            _lastActivity = null;
        }


        if(saveCounter >= AutomaticSaveAfterActions)
        {
            JSON writeJSON = new JSON(_personality, rewardList, itemList);
            writeJSON.writeJSON(writeJSON);
            Debug.Log("Status saved!");
            saveCounter = 0;
        }
        else
        {
            saveCounter++;
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
        while(true)
        {
            yield return new WaitForSeconds(WaitTime);

            StartCoroutine(DoActivityRoutine());

            float timer = 0;
            while(timer < 0.5f)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            timer = 0;

            while(timer < FeedBackTime && waitForFeedback)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            if(waitForFeedback)
            {
                GiveFeedback(0);
            }
        }
    }

}
