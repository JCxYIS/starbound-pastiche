using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatSceneManager : MonoBehaviour
{
    [SerializeField] 
    Text _chatText;

    

    public void SendChat(InputField inputField)
    {
        Room.Instance.SendMessage("Chat", inputField.text);
    }
}