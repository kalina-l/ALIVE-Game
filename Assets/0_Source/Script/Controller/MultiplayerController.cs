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

    public bool MultiplayerOn { get; private set; }
    private HappeningController _happeningController;

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

    public bool IsFeedbackRequestPending()
    {
        return _gettingFeedbackRequest && MultiplayerOn && !_gettingRequest;
    }

    public bool IsRequestPending()
    {
        return _gettingRequest && MultiplayerOn;
    }

    public bool IsWaitingForAnswer()
    {
        return _sendingRequest && MultiplayerOn;
    }

    public void ClearActivity()
    {
        _currentMultiplayerActivity = null;
    }

    public Activity GetFeedbackActivity()
    {
        return _currentFeedbackActivity;
    }
    
    public MultiplayerController(GameData gameData, HappeningController happeningController) {
        _localPersonality = gameData.Person;
        _gameData = gameData;
        _happeningController = happeningController;
        _localPersonality.Multiplayer = this;
    }

    public void setGameLoop(GameLoop gameLoop)
    {
        _gameLoop = gameLoop;
    }

    public void StartMultiplayer() {
        //_remoteController = remoteController;
        MultiplayerOn = true;

        DebugController.Instance.Log("MULTIPLAYER ON", DebugController.DebugType.Multiplayer);
    }
	
    public void EndMultiplayer() {
        //_remoteController = null;
        MultiplayerOn = false;
    }

    public Personality GetPersonality()
    {
        return _localPersonality;
    }

    public void SendFeedbackRequest(Activity activity, Dictionary<NeedType, Evaluation> needs)
    {
        _happeningController.sendMessage("needs", needs);
        _happeningController.sendMessage("feedbackRequest", activity);
       // _remoteController.GetFeedbackRequest(activity);
    }

    public void GetRemoteNeeds(Dictionary<NeedType, Evaluation> needs)
    {
        RemoteNeeds = needs;
    }

    public void GetFeedbackRequest(Activity activity)
    {
        _gettingFeedbackRequest = true;
        _currentFeedbackActivity = activity;
    }

    public void SendFeedback(int feedback)
    {
        _gettingFeedbackRequest = false;
        _happeningController.sendMessage("feedback", feedback);
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

        _sendingRequest = true;
        _currentMultiplayerActivity = activity;
        _happeningController.sendMessage("activityRequest", activity.ID);
        // _remoteController.GetActivityRequest(activity.ID);
    }

    public void GetActivityRequest(int activityID)
    {
        _currentMultiplayerActivity = _localPersonality.GetActivity(activityID);

        if(_currentMultiplayerActivity == null)
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

        _currentMultiplayerActivity.IsRequest = true;

        DebugController.Instance.Log(_id + ": Get Request for " + _currentMultiplayerActivity.Name, DebugController.DebugType.Multiplayer);

        _gettingRequest = true;
    }

    public void AcceptRequest()
    {
        DebugController.Instance.Log(_id + ": AcceptRequest", DebugController.DebugType.Multiplayer);

        if (_gettingRequest)
        {
            _happeningController.sendMessage("accept", true);
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
            _happeningController.sendMessage("decline", false);
            //_remoteController.DeclineRequest();
            _gettingRequest = false;
        }
        else
        {
            _sendingRequest = false;
            _currentMultiplayerActivity.IsDeclined = true;
        }
    }

    
}
