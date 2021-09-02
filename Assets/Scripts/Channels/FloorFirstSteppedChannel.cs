using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloorFirstSteppedChannel", menuName = "Channels/FloorFirstSteppedChannel")]
public class FloorFirstSteppedChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<Floor> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(Floor f)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(f);
        }
        else
        {
            Debug.Log("成功接通 FloorFirstSteppedChannel，但是沒有人收聽...");
        }
    }
}