using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Room : MonoSingleton<Room>, IRoom
{
    /* -------------------------------------------------------------------------- */
    /*                                  Variables                                 */
    /* -------------------------------------------------------------------------- */

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

    private ChatPanel chatPanel;
    

    /* -------------------------------------------------------------------------- */
    /*                                   Events                                   */
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// On Destroy
    /// </summary>
    public event System.Action OnDispose;

    /// <summary>
    /// (Name, Content)
    /// </summary>
    public event System.Action<string, string> OnChat;


    /* -------------------------------------------------------------------------- */
    /*                             Monobehaviour Func                             */
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MyName =  "Player" + Random.Range(0, short.MaxValue); // FIXME should be unique id
        DontDestroyOnLoad(gameObject);

        chatPanel = JC.Utility.ResourcesUtil.InstantiateFromResources("Prefabs/ChatPanel").GetComponent<ChatPanel>();
        chatPanel.transform.parent = transform;
        chatPanel.gameObject.SetActive(false);

        OnDispose += ()=>{
            SceneManager.LoadScene("Landing");
            // PromptBox.CreateMessageBox("Disconnected from Room...");
        };
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F7))
        {
            chatPanel.gameObject.SetActive(!chatPanel.gameObject.activeInHierarchy);
        }
    }

    /* -------------------------------------------------------------------------- */
    /*                                 Public Func                                */
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
            socket.RegisterRoom(this);
            return ipEndpoint.Address.ToString();
        }
        else
        {
            SocketClient.Instance.TryConnect(ip, 42069);
            socket = SocketClient.Instance;
            socket.RegisterRoom(this);
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
        SocketMessage message = new SocketMessage();
        message.Author = MyName;
        message.Type = msgtype;
        message.Content = content;
        // message.Timestamp = System.DateTime.UtcNow;

        print($"[ROOM SENDMSG] {message.Author} : ({message.Type}) {message.Content}");
        socket.Send(message);
    }

    public void Dispose()
    {
        socket?.Dispose(); 
        OnDispose?.Invoke();  
        Destroy(gameObject);
        print("[ROOM] Destroyed");
    }

    /* -------------------------------------------------------------------------- */
    /*                             Signal from socket                             */
    /* -------------------------------------------------------------------------- */

    public void OnRoomUpdate(RoomData roomData)
    {
        print("[ROOM] Update");
        this.roomData = roomData;
    }

    public void OnReceiveMessage(SocketMessage message)
    {
        print($"[ROOM GETMSG] {message.Author} : ({message.Type}) {message.Content}");

        // handle msg
        switch(message.Type)
        {
            default:
                Debug.LogWarning("[ROOM GETMSG] Message type is undefined: " + message.Type);
                break;

            case "Chat":
                OnChat?.Invoke(message.Author, message.Content);
                break;
        }
    }

    public void OnSocketDispose()
    {
        print("Socket has disposed!");
        socket = null;
        Dispose();
    }

    /* -------------------------------------------------------------------------- */

    
}