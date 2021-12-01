using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoSingleton<Room>
{
    [ReadOnly]
    [SerializeField]
    RoomData RoomData;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }
}