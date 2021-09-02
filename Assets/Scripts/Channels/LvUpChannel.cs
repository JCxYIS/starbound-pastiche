using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LvUpChannel", menuName = "Channels/LvUpChannel")]
public class LvUpChannel : ScriptableObject
{
    /// <summary>
    /// 收聽這個 Channel。
    /// 不要直接 Invoke! 請使用 <see langword="RaiseEvent()"/>
    /// </summary>
    public event UnityAction<uint> OnEventRaised;

    /// <summary>
    /// 呼叫這個 channel。
    /// </summary>
    public void RaiseEvent(uint lv)
    {
        if(OnEventRaised != null)
        {
            OnEventRaised?.Invoke(lv);
        }
        else
        {
            Debug.Log("成功接通 LvUpChannel，但是沒有人收聽...");
        }
    }
}