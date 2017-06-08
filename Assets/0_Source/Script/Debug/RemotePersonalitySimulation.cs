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

    private bool isRunning;

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

        isRunning = true;

        _manager = manager;
        _manager.StartCoroutine(Simulate());

        _multiplayer = new MultiplayerController(_personality, "remote");
        _multiplayer.ConnectWithRemote(manager.Multiplayer);

        
    }


    private IEnumerator Simulate()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(2);

            yield return _manager.StartCoroutine(DoActivityRoutine());

            if (_multiplayer.IsConnected)
            {
                //TODO: randomize this
                _manager.Multiplayer.SendFeedbackRequest(_lastActivity);
            }

            //random feedback
            GiveFeedback((int)(Random.value * 3) - 1);

            if (_multiplayer.IsFeedbackRequestPending())
            {
                //TODO: Calculate Feedback
                _manager.Multiplayer.SendFeedback(1);
            }

            _actionCounter++;

            if(_actionCounter > Random.value * 20)
            {
                //give random item
                int randomItem = (int)(_items.Count * Random.value);

                _personality.AddItem(_items[randomItem].ID, _items[randomItem]);
            }
        }
    }

    public void StopSimulation()
    {
        isRunning = false;
    }

    private IEnumerator DoActivityRoutine()
    {
        DebugController.Instance.Log("remote: start activity loop", DebugController.DebugType.Multiplayer);

        if (_multiplayer.IsRequestPending())
        {
            DebugController.Instance.Log("remote: add activity " + _personality.GetAllActivities().Count, DebugController.DebugType.Multiplayer);
        }

        List<Activity> activities = _personality.GetAllActivities();
        string d = "";

        for(int i=0; i<activities.Count; i++)
        {
            d += activities[i].Name + ", ";
        }

        DebugController.Instance.Log("remote: calculate (" + d + ")", DebugController.DebugType.Multiplayer);
        _intelligence.GetNextActivity(_personality, _multiplayer.IsConnected);

        float timer = 0;

        while (!_intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        DebugController.Instance.Log("remote: calculation took " + timer, DebugController.DebugType.Multiplayer);

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
                    if (_multiplayer.IsRequestPending())
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
            else if (_multiplayer.IsRequestPending())
            {
                _multiplayer.DeclineRequest();
            }

            DebugController.Instance.Log("remote: " + _lastActivity.Name, DebugController.DebugType.Multiplayer);
            _lastExperience = _lastActivity.DoActivity(_personality);
        }
        else
        {
            _lastActivity = null;
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
