using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public string ConditionThresholdsCSVpath = Application.dataPath + @"\Source\Script\Data\conditionThresholds.csv";
    public string ActionsNaturalRewardsCSVpath = Application.dataPath + @"\Source\Script\Data\actionsNaturalRewards.csv";

    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<string, Item> _items;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {

        _personality = new PersonalityCreator(ConditionThresholdsCSVpath, ActionsNaturalRewardsCSVpath).getPersonality();

        _intelligence = new ArtificialIntelligence(_personality);

        _items = new Dictionary<string, Item>();

        //TODO: new CreateItem().getItem... -> auslesen von Daten aus CSV? oder gleich readCSV(Action...) in Klasse Item einfügen...?
        _items["Ball"] = new Item().AddActivity("PLAYWITHBALL", new Activity("It plays with the ball")
                                                                    .AddReward("ENERGY", -2)
                                                                    .AddReward("SATISFACTION", 5))
                                   .AddActivity("EATBALL", new Activity("It eats the ball")
                                                                     .AddReward("HUNGER", 5));

        
	}

    void Update()
    {
        _intelligence.TimeStep();
    }
}
