using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides access to a remote personality. 
/// It allows for two personalities to perform actions together, 
/// give feedback on each others actions 
/// and view each others status.
/// </summary>
public class MultiplayerController {

    public bool IsConnected { get; private set; }

    private Personality _localPersonality;
    private MultiplayerController _remoteController;

    private Activity _currentMultiplayerActivity;
    private bool _gettingRequest;
    private bool _sendingRequest;

    public bool IsRequestpending()
    {
        return _gettingRequest && IsConnected;
    }

    public bool IsWaitingForAnswer()
    {
        return _sendingRequest && IsConnected;
    }

    public Activity GetPendingActivity()
    {
        _currentMultiplayerActivity.ID = Personality.PENDING_ACTIVITY_ID;
        return _currentMultiplayerActivity;
    }
    
    public MultiplayerController(Personality localPersonality) {
        _localPersonality = localPersonality;
    }

    public void ConnectWithRemote(MultiplayerController remoteController) {
        _remoteController = remoteController;
        IsConnected = true;
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

    public void SendActivityRequest(Activity activity)
    {
        activity.IsDeclined = false;

        _sendingRequest = true;
        _currentMultiplayerActivity = activity;
        _remoteController.GetActivityRequest(activity);
    }

    public void GetActivityRequest(Activity activity)
    {
        activity.IsRequest = true;
        _currentMultiplayerActivity = activity;
        _gettingRequest = true;
    }

    public void AcceptRequest()
    {
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
