using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class AsyncConnection {

    public IPEndPoint _serverEndPoint;
    public TcpClient _result;

    private bool _isDone = false;
    private object _handle = new object();
    private Thread _thread = null;
    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (_handle)
            {
                tmp = _isDone;
            }
            return tmp;
        }
        set
        {
            lock (_handle)
            {
                _isDone = value;
            }
        }
    }

    public virtual void Start()
    {
        _thread = new Thread(Run);
        _thread.Start();
    }
    public virtual void Abort()
    {
        _thread.Abort();
    }

    private void ThreadFunction() {
        Debug.Log("Start Connection to Server");
        TcpClient tmp = new TcpClient();
        tmp.Connect(_serverEndPoint);
        _result = tmp;
    }

    public TcpClient OnFinished() {
        return _result;
    }

    public virtual bool Update()
    {
        if (IsDone)
        {
            return true;
        }
        return false;
    }
    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }
    private void Run()
    {
        ThreadFunction();
        IsDone = true;
    }

}
