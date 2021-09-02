using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ComboChangeChannel", menuName = "Channels/ComboChangeChannel")]
public class ComboChangeChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<uint> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(uint combo)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(combo);
        }
        else
        {
            Debug.Log("成功接通 ComboChangeChannel，但是沒有人收聽...");
        }
    }
}