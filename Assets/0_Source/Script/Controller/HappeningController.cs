using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using FullSerializer;

public interface MultiplayerConnection
{
    MultiplayerController Lemo { get; set; }
    bool Connected { get; }
    void sendMessage(String messageType, System.Object content);
    void connect();
    void disconnect();
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

	public HappeningPlugin Plugin;
    public MultiplayerController Lemo { get; set; }

    //private HappeningClients clients;

    private ConnectionController connectionController;
    
    private float _timeToWait = 10;

    private fsSerializer _serializer;

    public bool Connected { get { if (connectionController == null) return false;  return connectionController.IsConnected; }  }

	void Awake() {
        _serializer = new fsSerializer();
		Plugin = new HappeningPlugin();
        connectionController = new ConnectionController(this);
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
            Plugin.sendData(connectionController.Remote, s);
        }
        else
        {
            StartCoroutine(waitWithSendingMsg(s));
        }
    }

    IEnumerator waitWithSendingMsg (String data)
    {
        while(!Connected)
        {
            yield return null;
        }
        if (connectionController.IsConnectionRestored)
        {
            Plugin.sendData(connectionController.Remote, data);
        }
    }

    public void connect()
    {
        connectionController.StartConnection();
    }

    public void connected()
    {
        sendMessage("texture", GraphicsHelper.Instance.lemo.GetComponentInChildren<Renderer>().material.name);
        foreach(KeyValuePair<int, Item> itemPair in Lemo.GetPersonality().Items)
        {
            sendMessage("itemAdded", itemPair.Key);
        }
    }

    public void disconnect ()
    {
        Lemo.SetRemoteTexture("no_texture");
        Plugin.sendData(connectionController.Remote, "closeConnection");
    }
    // Callbacks

    void onClientAdded(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
		print("onClientAdded: " + client.uuid);
        if(!connectionController.IsConnected && connectionController.Remote.uuid == client.uuid)
        {
            connectionController.GetConnectionRequest(client);
        }
	}

	void onClientUpdated(String json) {
        HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
        if(client.uuid == connectionController.Remote.uuid)
        {
            connectionController.Remote = client;
        }
            //print("onClientUpdated: " + client.uuid);
    }

	void onClientRemoved(String json) {
		HappeningClients.HappeningClient client = JsonUtility.FromJson<HappeningClients.HappeningClient>(json);
        if(connectionController.Remote.uuid == client.uuid)
        {
            connectionController.LostConnection();
        }

		print("onClientRemoved: " + client.uuid);
	}

	void onMessageReceived(String json) {

        if(!Lemo.MultiplayerOn)
        {
            return;
        }

        Package pkg = JsonUtility.FromJson<Package>(json);

        Plugin.Toast(pkg.data + " from " + pkg.source.uuid);

        print("msg received: " + pkg.data);

        bool isDataContainer = false;

        switch (pkg.data)
        {
            case "HI, I AM LEMO":
                connectionController.GetConnectionRequest(pkg.source);
                break;
            case "request accepted":
                connectionController.GetConnectionAccept(pkg.source);
                break;
            case "request denied":
                connectionController.GetConnectionDecline();
                break;
            case "closeConnection":
                connectionController.LostConnection();
                break; 
            default:
                isDataContainer = true;
                break;
        }

        if (isDataContainer && connectionController.IsConnected)
        {
           
                //DataContainer dataContainer = JsonUtility.FromJson<DataContainer>(pkg.data);
                DataContainer dataContainer = new DataContainer();
                fsData fsDataContainer = fsJsonParser.Parse(pkg.data);
                _serializer.TryDeserialize(fsDataContainer, ref dataContainer);
               
                switch (dataContainer.messageType)
                {
                    case "feedbackRequest":
                        int activity = (int)dataContainer.content;
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
                        Lemo.SetRemoteNeeds(needs);
                        break;
                    case "texture":
                        String texture = (String)dataContainer.content;
                        Lemo.SetRemoteTexture(texture);
                        break;
                    case "activity":
                        String activityName = (String)dataContainer.content;
                        Lemo.GetCurrentActivity(activityName);
                        break;
                    case "itemAdded":
                        int itemId = (int)dataContainer.content;
                        Lemo.GetItem(itemId, true);
                        break;
                    case "itemRemoved":
                        Lemo.GetItem(-1, false);
                        break;
            }
            

        }
	}

}