using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] GameStartChannel _gameStartChannel;
    [SerializeField] GameOverChannel _gameOverChannel;
    [SerializeField] LvUpChannel _lvUpChannel;

    [Header("Predefine Params")]
    [SerializeField] float minSpeed = 0.014f;
    [SerializeField] float maxSpeed = 0.035f;
    [SerializeField] AnimationCurve speedCurve;


    [Header("Variables")]
    [SerializeField] bool isStarted = false;
    [SerializeField] float scrollSpeed = 0.025f;

    
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
        _gameStartChannel.OnEventRaised += _=> isStarted = true;
        _gameOverChannel.OnEventRaised += _=>isStarted = false;
        _lvUpChannel.OnEventRaised += OnLvUp;
    }

    private void OnLvUp(uint lv)
    {
        // 15: 基本起始
        // 30: 不跨跳極限
        // 75: 跨跳極限
        scrollSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedCurve.Evaluate(lv/30f));
    }
}