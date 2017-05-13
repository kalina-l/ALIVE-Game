using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using KKSpeech;

public enum LoadStates { Dummy, CSV, SavedState};

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public Text debugText;

    public Canvas UICanvas;
    public LoadStates LoadFrom;

    public float WaitTime = 2;
    public int AutomaticSaveAfterActions = 5;
    private int saveCounter;
    private int activityCounter;

    private string personalityCSVPath = "Data\\";

    public string SaveFile = "please_specify_the_filename_for_saving";
    public string LoadFile = "please_specify_the_filename_for_Loading";

    public bool resetButton;

    //AI
    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;
    private List<Item> _itemList;
    private List<Reward> _rewardList;
    private Dictionary<TraitType, Trait> _traitList;

    private Experience _lastExperience;
    private Activity _lastActivity;

    //Animation
    private AnimationController _animation;

    //UI
    private OutputViewController _output;
    private FeedbackViewController _feedback;
    private ItemBoxViewController _itemBox;
    private ConditionViewController _conditionMonitor;
    private AlertViewController _alert;

    private bool waitForFeedback;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        _itemList = new List<Item>();
        _items = new Dictionary<int, Item>();
        _traitList = new Dictionary<TraitType, Trait>();

        loadPersonality(LoadFrom);
        
        foreach (Item item in _itemList)
        {
            _items[item.ID] = item;
            //_personality.AddItem(item.ID, item);
        }
        
        _intelligence = new ArtificialIntelligence();

        //UI
        _alert = new AlertViewController(UICanvas.transform);
        _output = new OutputViewController(UICanvas.transform);
        _feedback = new FeedbackViewController(UICanvas.transform, _intelligence);
        _itemBox = new ItemBoxViewController(UICanvas.transform, _items, _personality);
        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);

        _animation = new AnimationController();
        
        if (resetButton)
        {
            new ResetViewController(UICanvas.transform);
        }


        if (_personality.GetItems().Count >= 1)
        {
            foreach (KeyValuePair<int, Item> kvp in _personality.GetItems())
            {
                _itemBox.AddItemFromPersonality(kvp.Value);
            }
        }

        saveCounter = 1;
        StartCoroutine(Run());
    }

    public void loadPersonality(LoadStates LoadFrom)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            SaveFile = "savefile";
            LoadFile = "savefile";
            AutomaticSaveAfterActions = 1;

            string path = Path.Combine(Application.persistentDataPath, "savestates");
            if (!Directory.Exists(path))
            {
                LoadFrom = LoadStates.CSV;
            }
            path = Path.Combine(path, SaveFile);
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
                LoadFrom = LoadStates.CSV;
            }
            else
            {
                LoadFrom = LoadStates.SavedState;
            }
        }

        switch (LoadFrom)
        {
            case LoadStates.Dummy:
                DummyCreator creator = new DummyCreator();
                _personality = creator.CreateDummyPerson();
                _itemList = creator.CreateDummyItems();
                break;

            case LoadStates.CSV:
                PersonalityCreator creatorCSV = new PersonalityCreator(personalityCSVPath);
                _personality = creatorCSV.Personality;
                _itemList = creatorCSV.ItemList;
                _rewardList = creatorCSV.Rewards;
                _traitList = creatorCSV.TraitList;
                AddTraits();  
                break;

            case LoadStates.SavedState:
                JSON readJSON = new JSON(_personality, _rewardList, _itemList);
                readJSON.readJSON(readJSON, LoadFile);
                _personality = readJSON.personality;
                _rewardList = readJSON.rewardList;
                _itemList = readJSON.itemList;
                break;

            default:
                Debug.LogError("No Load State");
                break;
        }
        string traits = "Traits in Personality: ";
        foreach (Trait trait in _personality.Traits)
        {
            traits += trait.Identifier + " | ";
        }
        Debug.Log(traits);

    }

    public void reset()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Path.Combine(Application.persistentDataPath, "savestates");
            Directory.Delete(path, true);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void RemoveItem(){
		_itemBox.RemoveItemFromSlot ();
	}

    public void AddTraits()
    {
        //_personality.AddTrait(_traitList[TraitType.INTROVERT], _itemList);
        foreach(KeyValuePair<TraitType, Trait> kvp in _traitList)
        {
            if (Random.value <= 0.33)
            {
                _personality.AddTrait(kvp.Value, _itemList);
            }
        }
    }

    private IEnumerator DoActivityRoutine()
    {
        _intelligence.GetNextActivity(_personality);

        float timer = 0;

        while(!_intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _intelligence.GetResult();
        int askActivityID = -1;

        Debug.Log("Calculation took " + timer + " seconds " + _intelligence.GetValue());

        bool askForItem = _intelligence.GetValue() <= 0;

        if (askForItem)
        {
            //Debug.Log("Ask for Item!!!");
            _intelligence.AskForItem(_personality, _itemList);

            timer = 0;

            while (!_intelligence.IsDone)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            askActivityID = _intelligence.GetResult();
        }


        if (activityID != -1)
        {
            if (askForItem)
            {
                Item askItem = null;

                for (int i = 0; i < _itemList.Count; i++)
                {
                    foreach (Activity activity in _itemList[i].GetAllActivities())
                    {
                        if (activity.ID == askActivityID)
                        {
                            askItem = _itemList[i];
                        }
                    }
                }
                
                if (askItem != null)
                {
                    if (_personality.GetItem(askItem.ID, false) == null)
                    {
                        //_output.DisplayMessage("Give me " + askItem.Name);
                        _alert.ShowAlert(_itemBox.GetItemIcon(askItem));

                        yield return new WaitForSeconds(2);
                    }
                }
                else
                {
                    Debug.Log("NO EXTRA ITEM NEEDED - I want to " + _personality.GetActivity(activityID).feedBackString);
                }

                activityCounter = 0;
            }
            
            _lastActivity = _personality.GetActivity(activityID);
            _lastExperience = _lastActivity.DoActivity(_personality);

            //Show Activity
            _output.DisplayMessage(_lastActivity.feedBackString);

            //Ask for Feedback
            waitForFeedback = true;

            _animation.PlayActivityAnimation(_lastActivity, _personality);

            while (_animation.IsAnimating) {
                yield return 0;
            }

            _conditionMonitor.UpdateSlider(_personality);
            
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
            
            if(feedback == -1)
            {
                _feedback.ShowFeedback();
            } else if(feedback == 1)
            {
                _feedback.ShowFeedback();
            }

            waitForFeedback = false;
        }
    }

    private IEnumerator Run()
    {
        while(true)
        {
            //yield return new WaitForSeconds(WaitTime);
            
            yield return StartCoroutine(DoActivityRoutine());

            float timer = 0;
            while(timer < WaitTime || SpeechRecognizer.IsRecording())
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
                JSON writeJSON = new JSON(_personality, _rewardList, _itemList);
                writeJSON.writeJSON(writeJSON, SaveFile);
                //Debug.Log("Status saved!");
                saveCounter = 1;
            }
            else
            {
                saveCounter++;
            }
        }
    }

    public bool getWaitForFeedback()
    {
        return waitForFeedback;
    }
}
