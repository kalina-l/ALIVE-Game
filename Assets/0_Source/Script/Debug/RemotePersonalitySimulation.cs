using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePersonalitySimulation {

    private ApplicationManager _manager;
    private Personality _personality;
    private ArtificialIntelligence _intelligence;
    private List<Item> _items;
    private string personalityCSVPath = "Data\\";
    private MultiplayerController _multiplayer;

    private Experience _lastExperience;
    private Activity _lastActivity;

    private int _actionCounter;

    public Personality GetPersonality()
    {
        return _personality;
    }

    public MultiplayerController GetController()
    {
        return _multiplayer;
    }

    public RemotePersonalitySimulation(ApplicationManager manager, Personality localPersonality)
    {
        PersonalityCreator creatorCSV = new PersonalityCreator(personalityCSVPath);
        _personality = creatorCSV.Personality;

        //TODO: give the personality a multiplayer activity

        _items = creatorCSV.ItemList;
        _intelligence = new ArtificialIntelligence();

        _manager = manager;
        _manager.StartCoroutine(Simulate());

        _multiplayer = new MultiplayerController(_personality);
        _multiplayer.ConnectWithRemote(manager.Multiplayer);
    }


    private IEnumerator Simulate()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            yield return _manager.StartCoroutine(DoActivityRoutine());

            //random feedback
            GiveFeedback((int)(Random.value * 3) - 1);

            _actionCounter++;

            if(_actionCounter > Random.value * 20)
            {
                //give random item
                int randomItem = (int)(_items.Count * Random.value);

                _personality.AddItem(_items[randomItem].ID, _items[randomItem]);
            }
        }
    }

    private IEnumerator DoActivityRoutine()
    {
        bool removeExtraActivity = false;

        if (_multiplayer.IsRequestpending())
        {
            _personality.AddBaseActivity(_multiplayer.GetPendingActivity());
            removeExtraActivity = true;
        }

        _intelligence.GetNextActivity(_personality, _multiplayer.IsConnected);

        float timer = 0;

        while (!_intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _intelligence.GetResult();

        if (activityID != -1)
        {
            _lastActivity = _personality.GetActivity(activityID);

            if (_lastActivity.IsMultiplayer)
            {
                if (_lastActivity.IsRequest)
                {
                    _multiplayer.AcceptRequest();
                }
                else {
                    if (_multiplayer.IsRequestpending())
                    {
                        _multiplayer.DeclineRequest();
                    }

                    _multiplayer.SendActivityRequest(_lastActivity);

                    while (_multiplayer.IsWaitingForAnswer())
                    {
                        yield return 0;
                    }
                }
            }
            else if (_multiplayer.IsRequestpending())
            {
                _multiplayer.DeclineRequest();
            }

            _lastExperience = _lastActivity.DoActivity(_personality);
        }
        else
        {
            _lastActivity = null;
        }

        //remove multiplayer activity
        if (removeExtraActivity)
        {
            _personality.RemovePendingActivity();
        }
    }

    private void GiveFeedback(int feedback)
    {
        if (_lastActivity != null)
        {
            //Store Feedback in Activity
            _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);
        }
    }

}
