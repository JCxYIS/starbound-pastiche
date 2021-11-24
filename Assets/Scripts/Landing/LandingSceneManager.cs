using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


    [Header("Variables")]
    [SerializeField] State state;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        ChangeState(State.Main);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }




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
}