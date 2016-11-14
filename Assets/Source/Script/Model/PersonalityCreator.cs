using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PersonalityCreator
{
    private Personality _personality;

    private int[][] _conditionThresholds;       //[0]healthiness, [1]hunger, [2]satisfaction, [3]social, [4]energy
    private int[][] _actionsNaturalRewards;     //[i][0]eatBall, [i][1]eatFood, [i][2]sleep, [i][3]playBall, [i][4]playCake, [i][5]eatDoll, [i][6]playDoll

    public PersonalityCreator(string ConditionThresholdsCSVpath, string ActionsNaturalRewardsCSVpath)
    {
        _conditionThresholds = readCSV(Application.dataPath + ConditionThresholdsCSVpath);
        _actionsNaturalRewards = readCSV(Application.dataPath + ActionsNaturalRewardsCSVpath);



        _personality = new Personality()
            .AddAttribute("STRENGTH", new Attribute(10, 1, 20))
            .AddAttribute("CHARISMA", new Attribute(10, 1, 20))
            .AddAttribute("INTELLIGENCE", new Attribute(10, 1, 20))
            .AddAttribute("CONSTITUTION", new Attribute(10, 1, 20))
            .AddAttribute("WISDOM", new Attribute(10, 1, 20))

            .AddCondition("HEALTHINESS", new Condition(100, _conditionThresholds[0]))
            .AddCondition("HUNGER", new Condition(100, _conditionThresholds[1]))
            .AddCondition("SOCIAL", new Condition(100, _conditionThresholds[3]))
            .AddCondition("ENERGY", new Condition(100, _conditionThresholds[4]))
            .AddCondition("SATISFACTION", new Condition(100, _conditionThresholds[2]));

        _personality.AddBaseActivity("SLEEP", new Activity("It falls asleep")
            .AddReward("HEALTHINESS", _actionsNaturalRewards[0][2])
            .AddReward("HUNGER", _actionsNaturalRewards[1][2])
            .AddReward("SOCIAL", _actionsNaturalRewards[3][2])
            .AddReward("ENERGY", _actionsNaturalRewards[4][2])
            .AddReward("SATISFACTION", _actionsNaturalRewards[2][2]));
    }

    private int[][] readCSV(string pathCSV)
    {
        int[][] _data;
        string[] _lines;
        int[] _values;

        if (File.Exists(pathCSV))
        {
            _lines = File.ReadAllLines(pathCSV);
            _data = new int[_lines.Length][];

            for(int i = 0; i < _lines.Length; i++)
            {
                _values = Array.ConvertAll<string, int>(_lines[i].Split(';'), int.Parse);
                _data[i] = new int[_values.Length];

                for (int j = 0; j < _values.Length; j++)
                {
                    _data[i][j] = _values[i];
                }
            }

            return _data;
        }
        else
        {
            Debug.LogError("CSV-File doesn't exist!");
            throw new FileNotFoundException();
        }
    }

    public Personality getPersonality()
    {
        return _personality;
    }
}
