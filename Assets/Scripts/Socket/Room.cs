using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoSingleton<Room>, IRoom
{
    /// <summary>
    /// room data
    /// </summary>
    [ReadOnly]
    [SerializeField]
    RoomData roomData;

    /// <summary>
    /// (Get) Room data
    /// </summary>
    public RoomData RoomData => roomData;

    /// <summary>
    /// my name
    /// </summary>
    public string MyName;

    /// <summary>
    /// socket used to send / receive data
    /// </summary>
    private ISocketBase socket;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MyName =  "Player" + Random.Range(0, short.MaxValue); // FIXME should be unique id
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Create room and socket connection.
    /// </summary>
    /// <param name="isHost"></param>
    /// <param name="ip">if isHost = true, this param is not required</param>
    /// <returns>ip</returns>
    public string CreateRoom(bool isHost, string ip)
    {
        if(isHost)
        {
            var ipEndpoint = SocketServer.Instance.StartServer();
            socket = SocketServer.Instance;
            return ipEndpoint.Address.ToString();
        }
        else
        {
            SocketClient.Instance.TryConnect(ip, 42069);
            socket = SocketClient.Instance;
            return ip;
        }
    }
    
    /// <summary>
    /// Send Message upwards
    /// </summary>
    /// <param name="msgtype"></param>
    /// <param name="content"></param>
    public void SendMessage(string msgtype, string content)
    {

    }

    /* -------------------------------------------------------------------------- */

    public void OnRoomUpdate(RoomData roomData)
    {
        print("[ROOM] Update");
        this.roomData = roomData;
    }

    public void OnReceiveMessage(SocketMessage message)
    {
        print($"[ROOM MSG] {message.Author } : ({message.Type}) {message.Content}");
        // TODO receive msg then ?
    }

    /* -------------------------------------------------------------------------- */

    public void Destroy()
    {
        socket.Dispose();
        Destroy(gameObject);
        print("[ROOM] Destroyed");
    }
}