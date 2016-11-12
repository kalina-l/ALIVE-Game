using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour {

    public static ApplicationManager Instance;

    public string PersonalityCSV;
    public string ActionCSV;

    private Personality _personality;
    private ArtificialIntelligence _intelligence;

    private Dictionary<string, Item> _items;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {
        //_personality = new PersonalityCreator().CreatePersonality(PersonalityCSV, ActionCSV);
        _personality = CreateDummyPersonality();
        _intelligence = new ArtificialIntelligence(_personality);

        _items = new Dictionary<string, Item>();

        _items["Ball"] = new Item().AddActivity("PLAYWITHBALL", new Activity("It plays with the ball")
                                                                    .AddReward("ENERGY", -2)
                                                                    .AddReward("SATISFACTION", 5))
                                   .AddActivity("EATBALL", new Activity("It eats the ball")
                                                                     .AddReward("HUNGER", 5));

        
	}

    //TODO: READ FROM CSV!!!
    private Personality CreateDummyPersonality()
    {
        //return new PersonalityCreator().CreatePersonality(PersonalityCSV, ActionCSV);

        Personality person = new Personality()
            .AddAttribute("STRENGTH", new Attribute(10, 1, 20))
            .AddAttribute("CHARISMA", new Attribute(10, 1, 20))
            .AddAttribute("INTELLIGENCE", new Attribute(10, 1, 20))
            .AddAttribute("CONSTITUTION", new Attribute(10, 1, 20))
            .AddAttribute("WISDOM", new Attribute(10, 1, 20))
            .AddCondition("HEALTHINESS", new Condition(100, new int[] { 0, 10, 20, 30, 40, 50, 60 }))
            .AddCondition("HUNGER", new Condition(100, new int[] { 0, 10, 20, 30, 40, 50, 60 }))
            .AddCondition("SOCIAL", new Condition(100, new int[] { 0, 10, 20, 30, 40, 50, 60 }))
            .AddCondition("ENERGY", new Condition(100, new int[] { 0, 10, 20, 30, 40, 50, 60 }))
            .AddCondition("SATISFACTION", new Condition(100, new int[] { 0, 10, 20, 30, 40, 50, 60 }));

            person.AddBaseActivity("SLEEP", new Activity("It falls asleep")
                                                .AddReward("ENERGY", 20)
                                                .AddReward("SOCIAL",-10));

        return person;
    }

    void Update()
    {
        _intelligence.TimeStep();
    }
}
