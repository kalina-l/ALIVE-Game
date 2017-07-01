using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using FullSerializer;

public interface MultiplayerConnection
{
    MultiplayerController Lemo { get; set; }
    bool Connected { get; set; }
    bool WaitForReconnect { get; set; }
    void sendMessage(String messageType, System.Object content);
}

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

public class HappeningController : MonoBehaviour, MultiplayerConnection {

	HappeningPlugin Plugin;
    public MultiplayerController Lemo { get; set; }

    public bool Connected { get; set; }
    private HappeningClients.HappeningClient _remote;

    public bool WaitForReconnect { get; set; }
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

        if(Connected)
        {
            Plugin.sendData(_remote, s);
        }
        else if (WaitForReconnect)
        {
            StartCoroutine(waitWithSendingMsg(s));
        }
    }

    IEnumerator waitWithSendingMsg (String data)
    {
        while(WaitForReconnect)
        {
            yield return null;
        }
        if(Connected)
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
                WaitForReconnect = false;
                yield break;
            }
            secondsLeft -= Time.deltaTime;
            yield return null;
        }

        WaitForReconnect = false;
        Lemo.SetRemoteTexture("no_texture");
    }

    // Callbacks

    void onClientAdded(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
        if(!Connected)
        {
            _remote = client;
            Connected = true;
            sendMessage("texture", GraphicsHelper.Instance.lemo.GetComponentInChildren<Renderer>().material.name);
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
            WaitForReconnect = true;
            Connected = false;
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
                Lemo.GetFeedbackRequest(activity);
                break;
            case "feedback":
                int feedback = (int)dataContainer.content;
                Lemo.GetFeedback(feedback);
                break;
            case "activityRequest":
                int activityId = (int)dataContainer.content;
                Lemo.GetActivityRequest(activityId);
                break;
            case "accept":
                Lemo.AcceptRequest();
                break;
            case "decline":
                Lemo.DeclineRequest();
                break;
            case "needs":
                Dictionary<NeedType, Evaluation> needs = (Dictionary<NeedType, Evaluation>)dataContainer.content;
                Lemo.GetRemoteNeeds(needs);
                break;
            case "texture":
                String texture = (String)dataContainer.content;
                Lemo.SetRemoteTexture(texture);
                break;
        }

		Package pkg = JsonUtility.FromJson<Package>(json);
		Plugin.Toast(pkg.data + " from " + pkg.source.uuid);
	}

}