using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatSceneManager : MonoBehaviour
{
    [SerializeField] 
    Text _chatText;


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {        
        Room.Instance.OnChat += OnChat;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Room.Instance.OnChat -= OnChat;
    }


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _chatText.text = "";
    }    


    public void SendChat(InputField inputField)
    {
        Room.Instance.SendMessage("Chat", inputField.text);
    }

    void OnChat(string author, string msg)
    {
        _chatText.text += $"{author}: {msg}\n";
    }
}