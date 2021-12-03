using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandingSceneManager : MonoBehaviour
{
    public enum State 
    {
        Main = 0, 
        Multiplayer = 1,
        Room = 2,
    }

    [Header("Bindings")]
    [SerializeField] RectTransform[] Panels;
    [SerializeField] Text Room_IpText;
    [SerializeField] Text Room_PlayersText;


    [Header("Variables")]
    [SerializeField] State state;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        print(GameManager.Instance.Version);
        ChangeState(State.Main);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // DEBUG
        // if(Input.GetKeyDown(KeyCode.F7))
        // {
        //     SceneManager.LoadScene("Test_Chat");
        // }
    }


    /* -------------------------------------------------------------------------- */

    public void ChangeState(int stateId)
    {
        ChangeState((State)stateId);
    }

    public void ChangeState(State state)
    {
        this.state = state;

        for(int i = 0; i < 3; i++)
        {
            Panels[i].gameObject.SetActive(i == (int)state);
        }
    }

    /* -------------------------------------------------------------------------- */

    public void GoGame()
    {
        SceneManager.LoadScene("Game");
    }

    /* -------------------------------------------------------------------------- */
    /*                              Socket Stuff                                  */
    /* -------------------------------------------------------------------------- */
    
    public void CreateRoom()
    {
        Room_IpText.text = "IP: ???";
        Room_PlayersText.text = "Now Creating Room...";
        ChangeState(State.Room);

        var ip = Room.Instance.CreateRoom(isHost: true, "");
        Room_IpText.text = "IP: " + ip;
        Room_PlayersText.text = ""; // TODO
    }

    public void JoinRoom(InputField ipInput)
    {
        Room_PlayersText.text = "Now Connecting...";
        ChangeState(State.Room);
        
        var ip = Room.Instance.CreateRoom(isHost: false, ipInput.text);
        Room_IpText.text = "IP: " + ip;
        Room_PlayersText.text = ""; // TODO
    }

    public void ExitRoom()
    {
        Room.Instance.Dispose();
        ChangeState(State.Main);
    }

    /* -------------------------------------------------------------------------- */
}