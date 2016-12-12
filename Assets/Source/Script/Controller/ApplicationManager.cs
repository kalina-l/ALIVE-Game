using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public Canvas UICanvas;
    public bool UseDummy;

    public float WaitTime = 2;
    public float FeedBackTime = 2;

    //AI
    private string personalityCSVPath = "Data\\";
    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;
    public static float DISCOUNT_FACTOR = 0.91f;

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
        List<Item> itemList = new List<Item>();
        _items = new Dictionary<int, Item>();

        if (UseDummy)
        {
            DummyCreator creator = new DummyCreator();
            _personality = creator.CreateDummyPerson();
            itemList = creator.CreateDummyItems();
        }
        else
        {
            PersonalityCreator creator = new PersonalityCreator(personalityCSVPath);
            _personality = creator.personality;
            itemList = creator.ItemList;
        }

        foreach (Item item in itemList)
        {
            _items[item.ID] = item;
        }
        
        _intelligence = new ArtificialIntelligence(_personality);

        //UI
        _output = new OutputViewController(UICanvas.transform);
        _feedback = new FeedbackViewController(UICanvas.transform, _intelligence);
        _itemBox = new ItemBoxViewController(UICanvas.transform, _items, _personality);
        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);

        StartCoroutine(Run());
    }
    

    public void DoActivity()
    {
        //GetActivity
        int activityID = _intelligence.GetNextActivity();

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

            DoActivity();

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
        }
    }

}
