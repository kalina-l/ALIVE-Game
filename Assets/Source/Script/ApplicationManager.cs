using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public Canvas UICanvas;
    public static ApplicationManager Instance;

    private string ConditionThresholdsCSVpath = "\\Resources\\Data\\conditionThresholds.csv";
    private string ActionsNaturalRewardsCSVpath = "\\Resources\\Data\\actionsNaturalRewards.csv";

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

        _personality = new PersonalityCreator(ConditionThresholdsCSVpath, ActionsNaturalRewardsCSVpath).getPersonality();

        _intelligence = new ArtificialIntelligence(_personality, _output);

        _items = new Dictionary<string, Item>();

        //TODO: new CreateItem().getItem... -> auslesen von Daten aus CSV? oder gleich readCSV(Action...) in Klasse Item einfügen...?
        _items["Ball"] = new Item().AddActivity("PLAYWITHBALL", new Activity("It plays with the ball")
                                                                    .AddReward("ENERGY", -2)
                                                                    .AddReward("SATISFACTION", 5))
                                   .AddActivity("EATBALL", new Activity("It eats the ball")
                                                                     .AddReward("HUNGER", 5));

        //UI
        new FeedbackViewController(UICanvas.transform, _intelligence);

        _conditionMonitor = new ConditionViewController(UICanvas.transform, _personality);
        
	}

    void Update()
    {
        _intelligence.TimeStep();
        _conditionMonitor.UpdateSlider(_personality);
    }
}
