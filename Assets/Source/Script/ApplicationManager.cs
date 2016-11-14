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

        //ConditionThresholdsCSVpath = Application.dataPath + @"\Source\Script\Data\conditionThresholds.csv";
        //ActionsNaturalRewardsCSVpath = Application.dataPath + @"\Source\Script\Data\actionsNaturalRewards.csv";

        _personality = new PersonalityCreator(ConditionThresholdsCSVpath, ActionsNaturalRewardsCSVpath).getPersonality();

        _intelligence = new ArtificialIntelligence(_personality, _output);

        _items = new Dictionary<string, Item>();

        //TODO: new CreateItem().getItem... -> auslesen von Daten aus CSV? oder gleich readCSV(Action...) in Klasse Item einfügen...?
        _items["Ball"] = new Item().AddActivity("PLAY", new Activity("It plays with the ball")
                                                                    .AddReward("HEALTHINESS", 0)
                                                                    .AddReward("HUNGER", -10)
                                                                    .AddReward("SOCIAL", 20)
                                                                    .AddReward("ENERGY", -10)
                                                                    .AddReward("SATISFACTION", 25))
                                   .AddActivity("EAT", new Activity("It eats the ball")
                                                                    .AddReward("HEALTHINESS", -5)
                                                                    .AddReward("HUNGER", 3)
                                                                    .AddReward("SOCIAL", -3)
                                                                    .AddReward("ENERGY", -3)
                                                                    .AddReward("SATISFACTION", 5));
        _items["Doll"] = new Item().AddActivity("PLAY", new Activity("It plays with the doll")
                                                                    .AddReward("HEALTHINESS", 0)
                                                                    .AddReward("HUNGER", -5)
                                                                    .AddReward("SOCIAL", 20)
                                                                    .AddReward("ENERGY", -5)
                                                                    .AddReward("SATISFACTION", 20))
                                   .AddActivity("EAT", new Activity("It eats the doll")
                                                                    .AddReward("HEALTHINESS", -7)
                                                                    .AddReward("HUNGER", 1)
                                                                    .AddReward("SOCIAL", -20)
                                                                    .AddReward("ENERGY", -5)
                                                                    .AddReward("SATISFACTION", -5));
        _items["Cake"] = new Item().AddActivity("PLAY", new Activity("It plays with the cake")
                                                                    .AddReward("HEALTHINESS", 0)
                                                                    .AddReward("HUNGER", -3)
                                                                    .AddReward("SOCIAL", 10)
                                                                    .AddReward("ENERGY", -2)
                                                                    .AddReward("SATISFACTION", 10))
                                   .AddActivity("EAT", new Activity("It eats the cake")
                                                                    .AddReward("HEALTHINESS", 5)
                                                                    .AddReward("HUNGER", 25)
                                                                    .AddReward("SOCIAL", 0)
                                                                    .AddReward("ENERGY", 2)
                                                                    .AddReward("SATISFACTION", 10));
        _personality.AddItem("Ball", _items["Ball"]);
        _personality.AddItem("Doll", _items["Doll"]);
        _personality.AddItem("Cake", _items["Cake"]);

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
