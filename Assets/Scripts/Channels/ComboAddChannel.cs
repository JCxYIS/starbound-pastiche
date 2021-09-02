using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ComboAddChannel", menuName = "Channels/ComboAddChannel")]
public class ComboAddChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent()
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke();
        }
        else
        {
            Debug.Log("成功接通 ComboAddChannel，但是沒有人收聽...");
        }
    }
}