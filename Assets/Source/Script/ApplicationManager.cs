using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public bool UseDummy;

    public Canvas UICanvas;
    public static ApplicationManager Instance;

    private string personalityCSVPath = "Data\\";

    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;

    private OutputViewController _output;
    private ConditionViewController _conditionMonitor;

    public static float DISCOUNT_FACTOR = 0.91f;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        _output = new OutputViewController(UICanvas.transform);

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

        _intelligence = new ArtificialIntelligence(_personality, _output);
        
        foreach (Item item in itemList)
        {
            _items[item.ID] = item;
            //_personality.AddItem(item.ID, item);
        }


        //UI
        new FeedbackViewController(UICanvas.transform, _intelligence);
        new ItemBoxViewController(UICanvas.transform, _items, _personality);

        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);

        PersonalityUpdate();
    }

    void Update()
    {
        _intelligence.TimeStep();
    }

    public void PersonalityUpdate()
    {
        _conditionMonitor.UpdateSlider(_personality);
    }

}
