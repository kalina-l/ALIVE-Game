using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController {

    private HappeningController _happeningController;

    public HappeningClients.HappeningClient Remote;

    public bool IsConnected;
    private bool _isWaitingForClient;

    public bool IsConnectionRestored;

    public ConnectionController (HappeningController happeningController)
    {
        _happeningController = happeningController;
        Remote = new HappeningClients.HappeningClient();
        Remote.uuid = "";
    }

    public void StartConnection ()
    {
        HappeningClients clients = _happeningController.Plugin.getClients();
        foreach (HappeningClients.HappeningClient client in clients.clients)
        {
            _happeningController.Plugin.sendData(client, "HI, I AM LEMO");
        }
        IsConnected = false;
        _isWaitingForClient = true;
    }

    public void GetConnectionRequest (HappeningClients.HappeningClient remote)
    {
        if (Remote.uuid == "" || remote.uuid == Remote.uuid)
        {
            _happeningController.Plugin.sendData(remote, "request accepted");
            _isWaitingForClient = false;
            Remote = remote;
        }
    }

    public void GetConnectionAccept(HappeningClients.HappeningClient remote)
    {
        if (Remote.uuid == "" || remote.uuid == Remote.uuid)
        {
            if(Remote.uuid == remote.uuid)
            {
                IsConnectionRestored = true;
            }
            if(_isWaitingForClient)
            {
                _happeningController.Plugin.sendData(remote, "request accepted");
            }
            _isWaitingForClient = false;
            IsConnected = true;
            Remote = remote;
            _happeningController.sendMessage("texture", GraphicsHelper.Instance.lemo.GetComponentInChildren<Renderer>().material.name);
        } else
        {
            _happeningController.Plugin.sendData(remote, "request denied");
        }
    }

    public void GetConnectionDecline ()
    {
        StartConnection();
    }

    public void LostConnection ()
    {
        ApplicationManager.Instance.StartCoroutine(SaveIdForTime(30));
        IsConnectionRestored = false;
        _happeningController.Lemo.SetRemoteTexture("no_texture");

        StartConnection();
    }

    IEnumerator SaveIdForTime(float timeToSave)
    {
        yield return new WaitForSeconds(timeToSave);
        if(!IsConnected)
        {
            Remote.uuid = "";
        }
    }
}
