using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyCreator {

    private List<Reward> _rewards;

    public DummyCreator()
    {
        CreateRewards();
    }

    public Personality CreateDummyPerson()
    {
        Personality person = new Personality();

        person.AddCondition(NeedType.ENERGY, new Need(0, new int[] { -80, -40, -20, 0, 20, 40, 80 }))
                .AddCondition(NeedType.HEALTH, new Need(0, new int[] { -80, -40, -20, 0, 20, 40, 80 }))
                .AddCondition(NeedType.HUNGER, new Need(0, new int[] { -80, -40, -20, 0, 20, 40, 80 }))
                .AddCondition(NeedType.SATISFACTION, new Need(0, new int[] { -80, -40, -20, 0, 20, 40, 80 }))
                .AddCondition(NeedType.SOCIAL, new Need(0, new int[] { -80, -40, -20, 0, 20, 40, 80 }));

        person.AddBaseActivity(0, "Schlafen",
            new Activity("Schlafen")
            .AddReward(_rewards[0])
            .AddReward(_rewards[1])
            .AddReward(_rewards[2])
            .AddReward(_rewards[11]));

        person.AddBaseActivity(1, "Essen",
            new Activity("Essen")
            .AddReward(_rewards[3])
            .AddReward(_rewards[4])
            .AddReward(_rewards[7]));

        person.AddBaseActivity(2, "Idle",
            new Activity("Idle")
            .AddReward(_rewards[5])
            .AddReward(_rewards[7])
            .AddReward(_rewards[12]));

        return person;
    }

    public List<Item> CreateDummyItems()
    {
        List<Item> items = new List<Item>();

        items.Add(new Item() { ID = 0, Name = "Ball" }
            .AddActivity(3, "Spielen",
                new Activity("Spielen")
                .AddReward(_rewards[6])
                .AddReward(_rewards[8])
                .AddReward(_rewards[9])
                .AddReward(_rewards[10])));

        return items;
    }

    private void CreateRewards()
    {
        _rewards = new List<Reward>();

        //0
        _rewards.Add(new Reward() {
            RewardType = NeedType.ENERGY,
            RewardValue = 80,
            MaxEnergy = Evaluation.NEUTRAL});

        //1
        _rewards.Add(new Reward() {
            RewardType = NeedType.SOCIAL,
            RewardValue = -25,
            MinEnergy = Evaluation.GOOD
        });

        //2
        _rewards.Add(new Reward() {
            RewardType = NeedType.HUNGER,
            RewardValue = -60
        });

        //3
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.HUNGER,
            RewardValue = 70,
            MaxHunger = Evaluation.GOOD
        });

        //4
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.HEALTH,
            RewardValue = -80,
            MinHunger = Evaluation.VERY_GOOD
        });

        //5
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.ENERGY,
            RewardValue = -10
        });

        //6
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.SATISFACTION,
            RewardValue = 30
        });

        //7
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.SOCIAL,
            RewardValue = 10
        });

        //8
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.SOCIAL,
            RewardValue = 50
        });

        //9
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.SATISFACTION,
            RewardValue = 40
        });

        //10
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.ENERGY,
            RewardValue = -40
        });

        //11
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.HEALTH,
            RewardValue = 10
        });

        //12
        _rewards.Add(new Reward()
        {
            RewardType = NeedType.SATISFACTION,
            RewardValue = -10
        });
    }
}
