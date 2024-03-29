using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameStartChannel", menuName = "Channels/GameStartChannel")]
public class GameStartChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<GameController> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(GameController gc)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(gc);
        }
        else
        {
            Debug.Log("成功接通 GameStartChannel，但是沒有人收聽...");
        }
    }
}