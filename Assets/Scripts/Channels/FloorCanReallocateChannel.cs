using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloorCanReallocateChannel", menuName = "Channels/FloorCanReallocateChannel")]
public class FloorCanReallocateChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<Floor> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(Floor floor)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(floor);
        }
        else
        {
            Debug.Log("成功接通 FloorCanReallocateChannel，但是沒有人收聽...");
        }
    }
}