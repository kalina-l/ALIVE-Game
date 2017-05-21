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
    private Personality _remotePersonality;
    
    public MultiplayerController(Personality localPersonality) {
        _localPersonality = localPersonality;
    }

    public void ConnectWithRemote(Personality remotePersonality) {
        _remotePersonality = remotePersonality;
        IsConnected = true;
    }
	
    public void Disconnect() {
        _remotePersonality = null;
        IsConnected = false;
    }
}
