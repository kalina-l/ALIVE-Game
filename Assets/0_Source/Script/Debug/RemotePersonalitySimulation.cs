﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class SimulationController : MultiplayerConnection
{
    public MultiplayerController SecondLemo { get; set; }
    public bool Connected { get; set; }
    public bool WaitForReconnect { get; set; }

    private Personality secondLemo;

    public SimulationController ()
    {
        Connected = true;
    }

    public void SetMultiplayerController(MultiplayerController multiplayerController)
    {
        SecondLemo = multiplayerController;
    }

    public void sendMessage(string messageType, object content)
    {
        switch (messageType)
        {
            case "feedbackRequest":
                Activity activity = (Activity)content;
                SecondLemo.GetFeedbackRequest(activity);
                break;
            case "feedback":
                int feedback = (int)content;
                SecondLemo.GetFeedback(feedback);
                break;
            case "activityRequest":
                int activityId = (int)content;
                SecondLemo.GetActivityRequest(activityId);
                break;
            case "accept":
                SecondLemo.AcceptRequest();
                break;
            case "decline":
                SecondLemo.DeclineRequest();
                break;
            case "needs":
                Dictionary<NeedType, Evaluation> needs = (Dictionary<NeedType, Evaluation>)content;
                SecondLemo.GetRemoteNeeds(needs);
                break;

        }
    }
}

public class RemotePersonalitySimulation : GameLoop {

    private ApplicationManager _manager;

    private GameData _data;
    private MultiplayerController _multiplayer;

    private Experience _lastExperience;
    private Activity _lastActivity;

    private int _actionCounter;

    private bool isRunning;

    public Personality GetPersonality()
    {
        return _data.Person;
    }

    public MultiplayerController GetController()
    {
        return _multiplayer;
    }

    public RemotePersonalitySimulation(ApplicationManager manager, MultiplayerController mc)
    {
        _data = new GameData(LoadStates.CSV);

        isRunning = true;

        _manager = manager;
        _manager.StartCoroutine(Simulate());

        _multiplayer = mc;
        _multiplayer.setGameLoop(this);
    }


    private IEnumerator Simulate()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(2);

            yield return _manager.StartCoroutine(DoActivityRoutine());

            System.Random rand = new System.Random();
            if (_multiplayer.IsConnected)
            {
                //TODO: randomize this
                bool receivingFeedback = rand.NextDouble() < 0.25 ? true : false;
                if (receivingFeedback)
                {
                    _multiplayer.SendFeedbackRequest(_lastActivity);
                }
            }

            if (_multiplayer.IsFeedbackRequestPending())
            {
                //TODO: Calculate Feedback
                int feedback = rand.NextDouble() > 0.5 ? 1 : -1;
                DebugController.Instance.Log("Send feedback to the local LEMO: " + feedback, DebugController.DebugType.Multiplayer);
                _multiplayer.SendFeedback(feedback);
            }

            _actionCounter++;

            if(_actionCounter > UnityEngine.Random.value * 20)
            {
                //give random item
                int randomItem = (int)(_data.Items.Count * UnityEngine.Random.value);

                _data.Person.AddItem(_data.Items[randomItem].ID, _data.Items[randomItem]);
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
            DebugController.Instance.Log("remote: add activity " + _data.Person.GetAllActivities().Count, DebugController.DebugType.Multiplayer);
        }

        List<Activity> activities = _data.Person.GetAllActivities();
        string d = "";

        for(int i=0; i<activities.Count; i++)
        {
            d += activities[i].Name + ", ";
        }

        DebugController.Instance.Log("remote: calculate (" + d + ")", DebugController.DebugType.Multiplayer);

        //_data.Intelligence.GetNextActivity(_data.Person, _multiplayer.MultiplayerOn, _multiplayer.GetPendingActivity());

        float timer = 0;

        while (!_data.Intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        DebugController.Instance.Log("remote: calculation took " + timer, DebugController.DebugType.Multiplayer);

        int activityID = _data.Intelligence.GetResult();

        if (activityID != -1)
        {
            _lastActivity = _data.Person.GetActivity(activityID);

            if (_lastActivity == null)
            {
                _lastActivity = _multiplayer.GetPendingActivity();
            }

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
            _lastExperience = _lastActivity.DoActivity(_data.Person);

            _multiplayer.ClearActivity();

            _manager.MultiplayerViewController.RemoteCharacterAnimation.PlayActivityAnimation(_lastActivity, _data.Person);
            while (_manager.MultiplayerViewController.RemoteCharacterAnimation.IsAnimating)
            {
                yield return 0;
            }
        }
        else
        {
            _lastActivity = null;
        }
    }

    public void GiveFeedback(int feedback)
    {
        if (_lastActivity != null)
        {
            //Store Feedback in Activity
            _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);
        }
    }
}
