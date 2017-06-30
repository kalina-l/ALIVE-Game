using UnityEngine;
using System;
using System.Collections.Generic;

public class HappeningPlugin {

    private AndroidJavaClass Happening;

    public HappeningPlugin() {
        Happening = new AndroidJavaClass("blue.happening.unity.Main");
    }

	private string getClientsJSON() {
		return Happening.CallStatic<string>("getClients");
	}

	public HappeningClients getClients() {
		string json = getClientsJSON();
		return JsonUtility.FromJson<HappeningClients>(json);
	}

	public void sendData(HappeningClients.HappeningClient client, String data) {
		Package pkg = new Package(client, data);
		string json = JsonUtility.ToJson(pkg);
		Happening.CallStatic("sendData", json);
	}

    public void Toast(String msg) {
        Happening.CallStatic("makeToast", msg);
    }

}

[System.Serializable]
public struct HappeningClients {

	public List<HappeningClient> clients;

    [System.Serializable]
	public struct HappeningClient {
		public string uuid;
		public string name;
    }

}

[System.Serializable]
public struct Package {

	public HappeningClients.HappeningClient source;
	public string data;

	public Package(HappeningClients.HappeningClient source, string data) {
		this.source = source;
		this.data = data;
	}
}
