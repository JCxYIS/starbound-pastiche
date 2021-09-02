using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameOverChannel", menuName = "Channels/GameOverChannel")]
public class GameOverChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<GameOverReason> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(GameOverReason gameOverReason)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(gameOverReason);
        }
        else
        {
            Debug.Log("成功接通 GameOverChannel，但是沒有人收聽...");
        }
    }
}

public enum GameOverReason { Dead, Finished }