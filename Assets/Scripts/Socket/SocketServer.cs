using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// Socket Server.
/// Will run in another thread
/// </summary>
public class SocketServer : MonoSingleton<SocketServer>, ISocketBase
{
    /// <summary>
    /// Should continue listening to socket?
    /// </summary>
    private volatile bool shouldStop = false;

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

    /// <summary>
    /// Server socket thread
    /// </summary>
    private Thread serverSocketThread;

    /// <summary>
    /// Room
    /// </summary>
    private IRoom room;

    /// <summary>
    /// Client Socket threads
    /// </summary>
    /// <typeparam name="Socket"></typeparam>
    /// <typeparam name="Thread"></typeparam>
    /// <returns></returns>
    private Dictionary<Socket, Thread> clientSocketThread = new Dictionary<Socket, Thread>();

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
        // thread is running
        if(serverSocketThread != null)
        {
            throw new Exception("[Socket] Server Socket is already created");
        }

        // Get host
        IPAddress localIp = GetLocalIp();
        Debug.Log("Ip=" + localIp.ToString());
        ipEndPoint = new IPEndPoint(localIp, 42069);        

        // Create Update thread
        serverSocketThread = new Thread(ServerSocketThread);
        serverSocketThread.IsBackground = true;
        serverSocketThread.Start();
        
        return ipEndPoint;
    }

    /// <summary>
    /// The major code that server socket runs (on another thread)
    /// </summary>
    private void ServerSocketThread()
    {
        // SOCKET
        serverSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // BIND
        serverSocket.Bind(ipEndPoint);

        // LISTEN
        serverSocket.Listen(16);
        Debug.Log("[SOCKETS LISTENING] "+ipEndPoint.Address);


        while(!shouldStop)
        {
            // ACCEPT
            Socket newClient = serverSocket.Accept(); // thread will stuck here until connect
            
            Debug.Log("[SOCKETS ACCEPTED]");
            clientSockets.Add(newClient);
            Thread newClientThread = new Thread(()=>ClientSocketThread(newClient));
            clientSocketThread.Add(newClient, newClientThread);

            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// Thread of socket communicate with client (one thread per client)
    /// </summary>
    private void ClientSocketThread(Socket clientSocket)
    {
        byte[] buffer;
        string receive;

        while(!shouldStop)
        {
            buffer = new byte[1024];

            // Read buffer
            int receiveCount = clientSocket.Receive(buffer);
            if(receiveCount == 0)
            {
                Debug.LogWarning("[SOCKETS EMPTY RECV] Try to reconnect");
                // TODO reconnect
                continue;
            }

            // Encode to string
            receive = Encoding.ASCII.GetString(buffer, 0, receiveCount);
            Debug.Log("[SOCKETS GET] "+receive);

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

    /// <summary>
    /// Try closing sockets
    /// </summary>
    public void Dispose()
    {
        shouldStop = true;

        // close client
        clientSockets.ForEach(s => s?.Dispose());
        foreach(var key in clientSocketThread)
        {
            key.Value?.Abort();
        }
        clientSockets = new List<Socket>();
        clientSocketThread = new Dictionary<Socket, Thread>();

        // close server
        serverSocketThread?.Abort();
        serverSocket?.Dispose();
        serverSocketThread = null;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Send / Broadcast message to ALL clients
    /// </summary>
    public void Send(SocketMessage message)
    {
        string str = JsonUtility.ToJson(message);
        byte[] sendData = new byte[1024];
        sendData = Encoding.ASCII.GetBytes(str);
        foreach(var client in clientSockets)
        {
            client.Send(sendData,sendData.Length, SocketFlags.None);
        }
        Debug.Log("[SOCKETS SEND] (To ALL) "+sendData);
    }

    public void RegisterRoom(IRoom listener)
    {
        room = listener;
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