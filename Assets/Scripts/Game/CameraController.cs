using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] GameStartChannel _gameStartChannel;

    [Header("Variables")]
    [SerializeField] bool isStarted = false;
    [SerializeField] float scrollSpeed = 1;

    
    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        if(isStarted)
        {
            transform.position += Vector3.up * scrollSpeed;
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        _gameStartChannel.OnEventRaised += StartScroll;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        _gameStartChannel.OnEventRaised -= StartScroll;        
    }

    public void StartScroll(GameController gc)
    {
        isStarted = true;
    }
}