using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public Canvas UICanvas;
    public static ApplicationManager Instance;
    
    private string personalityCSVPath = "\\Resources\\Data\\personality.csv";
    private string itemsCSVpath = "\\Resources\\Data\\items.csv";

    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<string, Item> _items;

    private OutputViewController _output;
    private ConditionViewController _conditionMonitor;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        _output = new OutputViewController(UICanvas.transform);

        _personality = new PersonalityCreator(personalityCSVPath).personality;

        _intelligence = new ArtificialIntelligence(_personality, _output);

        _items = new Dictionary<string, Item>();
        List<Item> itemList = new ItemCreator(itemsCSVpath).items;
        foreach(Item item in itemList)
        {
            _items[item.name] = item;
            _personality.AddItem(item.name, item);
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
