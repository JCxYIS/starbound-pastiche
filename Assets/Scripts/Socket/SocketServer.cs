using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;

/// <summary>
/// Socket Server.
/// Will run in another thread
/// </summary>
public class SocketServer : MonoSingleton<SocketServer>, IDisposable
{
    /// <summary>
    /// Run in this thread
    /// </summary>
    private Thread updateThread;

    /// <summary>
    /// Should continue listening to socket?
    /// </summary>
    private volatile bool shouldStop;

    /// <summary>
    /// Server socket endpoint
    /// </summary>
    private IPEndPoint ipEndPoint;

    /// <summary>
    /// Server socket for accepting connection
    /// </summary>
    private Socket serverSocket;

    /// <summary>
    /// Socket with client we established
    /// </summary>
    private List<Socket> clientSockets = new List<Socket>();

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

    public SocketServer()
    {
        
    }

    /// <summary>
    /// Start Server Socket 
    /// </summary>
    /// <returns>local ip / port</returns>
    public IPEndPoint StartServer()
    {
        // Get host
        IPAddress localIp = GetLocalIp();
        Debug.Log("Ip=" + localIp.ToString());
        ipEndPoint = new IPEndPoint(localIp, 42069);

        // Create server socket (SOCKET)
        serverSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // Create Update thread
        updateThread = new Thread(ServerUpdate);
        updateThread.IsBackground = true;
        updateThread.Start();
        
        return ipEndPoint;
    }

    /// <summary>
    /// The major code that server runs (on another thread)
    /// </summary>
    private void ServerUpdate()
    {
        // BIND
        serverSocket.Bind(ipEndPoint);

        // LISTEN
        serverSocket.Listen(16);

        // ACCEPT

        // TODO.
    }

    /// <summary>
    /// Try closing sockets
    /// </summary>
    public void Dispose()
    {
        shouldStop = true;

        updateThread?.Abort();
        serverSocket?.Dispose();
        clientSockets.ForEach(s => s?.Dispose());
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Send message to all clients
    /// </summary>
    public void BroadcastMessage()
    {
        // TODO
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Get local ip address
    /// </summary>
    /// <returns></returns>
    private IPAddress GetLocalIp()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }

        }
        return null;
    }
}