using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public Canvas UICanvas;
    public static ApplicationManager Instance;

    private string personalityCSVPath = "Data\\";

    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<int, Item> _items;

    private OutputViewController _output;
    private ConditionViewController _conditionMonitor;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        _output = new OutputViewController(UICanvas.transform);

        //DummyCreator creator = new DummyCreator();
        //_personality = creator.CreateDummyPerson();
        PersonalityCreator creator = new PersonalityCreator(personalityCSVPath);
        _personality = creator.personality;

        _intelligence = new ArtificialIntelligence(_personality, _output);

        _items = new Dictionary<int, Item>();
        List<Item> itemList = creator.ItemList;
        foreach (Item item in itemList)
        {
            _items[item.ID] = item;
            _personality.AddItem(item.ID, item);
        }


        //UI
        new ItemCollectionViewController(UICanvas.transform, _items, _personality);
        new FeedbackViewController(UICanvas.transform, _intelligence);

        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);
    }

    void Update()
    {
        _intelligence.TimeStep();
        _conditionMonitor.UpdateSlider(_personality);
    }

}
