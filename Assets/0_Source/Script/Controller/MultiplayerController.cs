using System.Collections;
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

    public bool IsConnected { get; private set; }

    private GameLoop _gameLoop;

    private Personality _localPersonality;
    private MultiplayerController _remoteController;

    private string _id;

    private Activity _currentMultiplayerActivity;
    private Activity _currentFeedbackActivity;

    private bool _gettingRequest;
    private bool _sendingRequest;

    private bool _gettingFeedbackRequest;

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

    public Activity GetPendingActivity()
    {
        return _currentMultiplayerActivity;
    }

    public Activity GetFeedbackActivity()
    {
        return _currentFeedbackActivity;
    }
    
    public MultiplayerController(Personality localPersonality, string id) {
        _localPersonality = localPersonality;
        _id = id;
        _localPersonality.Multiplayer = this;
    }

    public void setGameLoop(GameLoop gameLoop)
    {
        _gameLoop = gameLoop;
    }

    public void ConnectWithRemote(MultiplayerController remoteController) {
        _remoteController = remoteController;
        IsConnected = true;

        DebugController.Instance.Log("CONNECTED", DebugController.DebugType.Multiplayer);

    }
	
    public void Disconnect() {
        _remoteController = null;
        IsConnected = false;
    }

    public Personality GetPersonality()
    {
        return _localPersonality;
    }

    public Personality GetRemotePersonality() {
        return _remoteController.GetPersonality();
    }

    public void SendFeedbackRequest(Activity activity)
    {
        _remoteController.GetFeedbackRequest(activity);
    }

    public void GetFeedbackRequest(Activity activity)
    {
        _gettingFeedbackRequest = true;
        _currentFeedbackActivity = activity;
    }

    public void SendFeedback(int feedback)
    {
        _gettingFeedbackRequest = false;
        _remoteController.GetFeedback(feedback);
    }

    public void GetFeedback(int feedback)
    {
        // TODO: woanders auslagern
        ApplicationManager.Instance.getFeedbackController().setLastFeedbackType(FeedbackType.Multiplayer);
        _gameLoop.GiveFeedback(feedback);
    }

    public void SendActivityRequest(Activity activity)
    {
        DebugController.Instance.Log(_id + ": Send Request for " + activity.Name, DebugController.DebugType.Multiplayer);

        activity.IsDeclined = false;

        _sendingRequest = true;
        _currentMultiplayerActivity = activity;
        _remoteController.GetActivityRequest(activity.ID);
    }

    public void GetActivityRequest(int activityID)
    {
        _currentMultiplayerActivity = _localPersonality.GetActivity(activityID);

        if(_currentMultiplayerActivity == null)
        {
            //TODO getactivity from itembox
        }

        _currentMultiplayerActivity.IsRequest = true;

        DebugController.Instance.Log(_id + ": Get Request for " + _currentMultiplayerActivity.Name, DebugController.DebugType.Multiplayer);

        _gettingRequest = true;
    }

    public void AcceptRequest()
    {
        DebugController.Instance.Log(_id + ": AcceptRequest", DebugController.DebugType.Multiplayer);

        if (_gettingRequest)
        {
            _remoteController.AcceptRequest();
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
            _remoteController.DeclineRequest();
            _gettingRequest = false;
        }
        else
        {
            _sendingRequest = false;
            _currentMultiplayerActivity.IsDeclined = true;
        }
    }

    
}
