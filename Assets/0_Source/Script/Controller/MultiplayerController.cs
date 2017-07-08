﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameLoop
{
    void GiveFeedback(int feedback);
}

/// <summary>
/// This class provides access to a remote personality. 
/// It allows for two personalities to perform actions together, 
/// give feedback on each others actions 
/// and view each others status.
/// </summary>
public class MultiplayerController {

    public bool MultiplayerOn;
    public bool IsConnected {
        get { return MultiplayerOn && (_happeningController.Connected); }
        private set { IsConnected = value; } }
    private MultiplayerConnection _happeningController;

    private GameLoop _gameLoop;

    private GameData _gameData;
    private Personality _localPersonality;
    //private MultiplayerController _remoteController;

    private string _id;

    private Activity _currentMultiplayerActivity;
    private Activity _currentFeedbackActivity;

    public Dictionary<NeedType, Evaluation> RemoteNeeds { get; private set; }

    private bool _gettingRequest;
    private bool _sendingRequest;

    private bool _gettingFeedbackRequest;

    public void SetRemoteTexture(string textureName)
    {
        Material[] materials = GraphicsHelper.Instance.materials;
        Material remoteTexture = materials[0];
        for (int i = 0; i < materials.Length; i++)
        {
            if(textureName.Contains(materials[i].name))
            {
                remoteTexture = materials[i];
                break;
            }
        }
        ApplicationManager.Instance.SetupRemoteTexture(remoteTexture);
    }

    public Activity GetPendingActivity()
    {
        return _currentMultiplayerActivity;
    }

    public bool IsFeedbackRequestPending()
    {
        return _gettingFeedbackRequest && IsConnected && !_gettingRequest;
    }

    public bool IsRequestPending()
    {
        return _gettingRequest && IsConnected;
    }

    public bool IsWaitingForAnswer()
    {
        return _sendingRequest && IsConnected;
    }

    public void ClearActivity()
    {
        _currentMultiplayerActivity = null;
    }

    public Activity GetFeedbackActivity()
    {
        return _currentFeedbackActivity;
    }
    
    public MultiplayerController(GameData gameData, MultiplayerConnection happeningController, string id) {
        _localPersonality = gameData.Person;
        _gameData = gameData;
        _happeningController = happeningController;
        _localPersonality.Multiplayer = this;
        _id = id;

        RemoteNeeds = new Dictionary<NeedType, Evaluation>();
        RemoteNeeds[NeedType.HUNGER] = Evaluation.NEUTRAL;
        RemoteNeeds[NeedType.ENERGY] = Evaluation.NEUTRAL;
        RemoteNeeds[NeedType.HEALTH] = Evaluation.NEUTRAL;
        RemoteNeeds[NeedType.SATISFACTION] = Evaluation.NEUTRAL;
        RemoteNeeds[NeedType.SOCIAL] = Evaluation.NEUTRAL;
    }

    public void SetConnectionController (MultiplayerConnection connectionController)
    {
        _happeningController = connectionController;
    }

    public void setGameLoop(GameLoop gameLoop)
    {
        _gameLoop = gameLoop;
    }

    public void StartMultiplayer() {
        //_remoteController = remoteController;
        MultiplayerOn = true;
        _happeningController.connect();

        DebugController.Instance.Log("MULTIPLAYER ON", DebugController.DebugType.Multiplayer);
    }
	
    public void EndMultiplayer() {
        //_remoteController = null;
        _happeningController.disconnect();
        MultiplayerOn = false;
    }

    public Personality GetPersonality()
    {
        return _localPersonality;
    }

    public void SendCurrentActivity(string activityName)
    {
        _happeningController.sendMessage("activity", activityName);
    }

    public void GetCurrentActivity(string activityName)
    {
        ApplicationManager.Instance.MultiplayerViewController.RemoteCharacterAnimation.PlayActivityAnimation(activityName, RemoteNeeds);
    }

    public void SetRemoteNeeds(Dictionary<NeedType, Evaluation> needs)
    {
        RemoteNeeds = needs;
    }

    public void SendNeeds(Dictionary<NeedType, Evaluation> needs)
    {
        _happeningController.sendMessage("needs", needs);
    }

    public void SendFeedbackRequest(int activityID)
    {
        _happeningController.sendMessage("feedbackRequest", activityID);
       // _remoteController.GetFeedbackRequest(activity);
    }
    
    public void GetFeedbackRequest(int activityID)
    {
        _gettingFeedbackRequest = true;

        _currentFeedbackActivity = _localPersonality.GetActivity(activityID);


        if (_currentFeedbackActivity == null)
        {
            //TODO getactivity from itembox
            foreach (Item item in _gameData.Items)
            {
                if (item.GetActivity(activityID) != null)
                {
                    _currentFeedbackActivity = item.GetActivity(activityID);
                }
            }
        }
    }

    public void SendFeedback(int feedback)
    {
        _gettingFeedbackRequest = false;
        _happeningController.sendMessage("feedback", feedback);
        ApplicationManager.Instance.ShowRemoteFeedback(feedback);
        // _remoteController.GetFeedback(feedback);
    }

    public void GetFeedback(int feedback)
    {
        ApplicationManager.Instance.getFeedbackController().setLastFeedbackType(FeedbackType.Multiplayer);
        _gameLoop.GiveFeedback(feedback);
    }

    public void SendActivityRequest(Activity activity)
    {
        DebugController.Instance.Log(_id + ": Send Request for " + activity.Name, DebugController.DebugType.Multiplayer);

        activity.IsDeclined = false;
       // DebugController.Instance.Log(_id + " SEND ACTIVITY REQUEST: " + activity.Name + ", Object: " + activity.GetHashCode(), DebugController.DebugType.Multiplayer);


        _sendingRequest = true;
        _currentMultiplayerActivity = activity;
        _happeningController.sendMessage("activityRequest", activity.ID);
        // _remoteController.GetActivityRequest(activity.ID);
    }

    public void GetActivityRequest(int activityID)
    {
        _currentMultiplayerActivity = _localPersonality.GetActivity(activityID, false);


        if (_currentMultiplayerActivity == null)
        {
            //TODO getactivity from itembox
            foreach(Item item in _gameData.Items)
            {
                if(item.GetActivity(activityID) != null)
                {
                    _currentMultiplayerActivity = item.GetActivity(activityID);
                }
            }
        }
//        DebugController.Instance.Log(_id + " GET ACTIVITY REQUEST: " + _currentMultiplayerActivity.Name + ", Object: " + _currentMultiplayerActivity.GetHashCode(), DebugController.DebugType.Multiplayer);

        _currentMultiplayerActivity.IsRequest = true;

        DebugController.Instance.Log(_id + ": Get Request for " + _currentMultiplayerActivity.Name, DebugController.DebugType.Multiplayer);

        _gettingRequest = true;
    }

    public void AcceptRequest()
    {
        DebugController.Instance.Log(_id + ": AcceptRequest", DebugController.DebugType.Multiplayer);

        if (_gettingRequest)
        {
            _happeningController.sendMessage("accept", null);
            //_remoteController.AcceptRequest();
            _gettingRequest = false;
        }
        else
        {
            _sendingRequest = false;
        }
    }

    public void DeclineRequest()
    {
        DebugController.Instance.Log(_id + ": DeclineRequest", DebugController.DebugType.Multiplayer);

        if(_gettingRequest)
        {
            _happeningController.sendMessage("decline", null);
            //_remoteController.DeclineRequest();
            _gettingRequest = false;
            if(_currentMultiplayerActivity != null)
                _currentMultiplayerActivity.IsRequest = false;
        }
        else
        {
            _sendingRequest = false;
            _currentMultiplayerActivity.IsDeclined = true;
        }
    }

    
}
