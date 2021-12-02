using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Text;

public class SocketClient : MonoSingleton<SocketClient>, ISocketBase
{
    /// <summary>
    /// Should continue listening to socket?
    /// </summary>
    private volatile bool shouldStop = false;

    private Socket socket;

    private Thread thread;

    private IRoom room;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        Dispose();
    }

    /* -------------------------------------------------------------------------- */


    public void TryConnect(string ip, int port)
    {
        // thread is running
        if(thread != null)
        {
            throw new Exception("[SOCKET] Socket is already created");
        }

        // parse param
        IPEndPoint host = new IPEndPoint(IPAddress.Parse(ip), port);

        // create thread
        thread = new Thread(()=>ClientThread(host));
        thread.IsBackground = true;
        thread.Start();
    }

    private void ClientThread(IPEndPoint host)
    {
        // SOCKET
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // CONNECT
        socket.Connect(host);

        // read
        byte[] buffer = new byte[1024];
        string receive = "";
        while(true)
        {
            buffer = new byte[1024];

            // Read buffer
            int receiveCount = socket.Receive(buffer);  // frz here until buffer full lmao
            if(receiveCount == 0)
            {
                Debug.LogWarning("[SOCKETC EMPTY RECV] Try to reconnect");
                // TODO reconnect
                continue;
            }

            // Encode to string
            receive = Encoding.ASCII.GetString(buffer, 0, receiveCount);
            Debug.Log("[SOCKETC GET] "+receive);

            // Parse Message
            try
            {
                var msg = JsonUtility.FromJson<SocketMessage>(receive);
                room?.OnReceiveMessage(msg);
            }
            catch
            {
                Debug.LogError("Failed to parse SocketMessage string: " + receive);
            }

            System.Threading.Thread.Sleep(1);
        }
    }

    public void RegisterRoom(IRoom listener)
    {
        room = listener;
    }

    public void Send(SocketMessage message)
    {
        string str = JsonUtility.ToJson(message);
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(str);
        socket.Send(sendData,sendData.Length, SocketFlags.None);
    }

    public void Dispose()
    {
        thread.Abort();
        socket.Dispose();
        thread = null;
    }    
}