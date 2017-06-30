using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using FullSerializer;

[Serializable]
public struct DataContainer
{
    public String messageType;
    public System.Object content;

    public DataContainer(String messageType, System.Object content)
    {
        this.messageType = messageType;
        this.content = content;
    }
}

public class HappeningController : MonoBehaviour {

	HappeningPlugin Plugin;
    MultiplayerController _multiplayerController;

    private bool _connected;
    private HappeningClients.HappeningClient _remote;

    private bool _waitForReconnect;
    private float _timeToWait = 2;

    private fsSerializer _serializer;

	void Start() {
        _serializer = new fsSerializer();
		Plugin = new HappeningPlugin();
		sendBroadcastMessage();
	}

	void Update() {
	}

	public void sendBroadcastMessage() {
		print("Broadcasting...");
		HappeningClients clients = Plugin.getClients();
		foreach (HappeningClients.HappeningClient client in clients.clients) {
			print(client.uuid);
			Plugin.sendData(client, "HALLO!");
		}
	}

    public void sendMessage(String messageType, System.Object content)
    {
        DataContainer data = new DataContainer(messageType, content);
        fsData fsData;
        _serializer.TrySerialize(data, out fsData);
        String s = fsJsonPrinter.CompressedJson(fsData);

        if(_connected)
        {
            Plugin.sendData(_remote, s);
        }
        else if (_waitForReconnect)
        {
            StartCoroutine(waitWithSendingMsg(s));
        }
    }

    IEnumerator waitWithSendingMsg (String data)
    {
        while(_waitForReconnect)
        {
            yield return null;
        }
        if(_connected)
        {
            Plugin.sendData(_remote, data);
        }
    }

    IEnumerator waitForReconnect()
    {
        float secondsLeft = _timeToWait;

        while (secondsLeft > 0f)
        {
            if(_remote.uuid != "")
            {
                _waitForReconnect = false;
                yield break;
            }
            secondsLeft -= Time.deltaTime;
            yield return null;
        }

        _waitForReconnect = false;
    }

    // Callbacks

    void onClientAdded(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
        if(!_connected && _multiplayerController.MultiplayerOn)
        {
            _remote = client;
            _connected = true;
        }
		print("onClientAdded: " + client.uuid);
	}

	void onClientUpdated(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
		print("onClientUpdated: " + client.uuid);
	}

	void onClientRemoved(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
        if(_remote.uuid == client.uuid)
        {
            _remote.uuid = "";
            _waitForReconnect = true;
            _connected = false;
            StartCoroutine(waitForReconnect());
        }

		print("onClientRemoved: " + client.uuid);
	}

	void onMessageReceived(String json) {

        DataContainer dataContainer = new DataContainer();
        fsData fsData = fsJsonParser.Parse(json);
        _serializer.TryDeserialize(fsData, ref dataContainer);

        switch(dataContainer.messageType)
        {
            case "feedbackRequest":
                Activity activity = (Activity)dataContainer.content;
                _multiplayerController.GetFeedbackRequest(activity);
                break;
            case "feedback":
                int feedback = (int)dataContainer.content;
                _multiplayerController.GetFeedback(feedback);
                break;
            case "activityRequest":
                int activityId = (int)dataContainer.content;
                _multiplayerController.GetActivityRequest(activityId);
                break;
            case "accept":
                _multiplayerController.AcceptRequest();
                break;
            case "decline":
                _multiplayerController.DeclineRequest();
                break;
            case "needs":
                Dictionary<NeedType, Evaluation> needs = (Dictionary<NeedType, Evaluation>)dataContainer.content;
                _multiplayerController.GetRemoteNeeds(needs);
                break;

        }

		Package pkg = JsonUtility.FromJson<Package>(json);
		Plugin.Toast(pkg.data + " from " + pkg.source.uuid);
	}

}