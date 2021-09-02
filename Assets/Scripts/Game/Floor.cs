using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : MonoBehaviour
{
    [Header("Variables")]
    public int index;
    public bool stepped;

    [Header("Channels")]
    [SerializeField] FloorCanReallocateChannel _floorCanReallocateChannel;
    [SerializeField] FloorFirstSteppedChannel _floorFirstSteppedChannel;


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        
    }

    public void Reallocate(int index, float posX)
    {
        gameObject.name = $"Floor {index}";
        this.index = index;
        stepped = false;
        transform.position = new Vector3(posX, index);

        int length = 2; // TODO floor length
        transform.localScale = new Vector3(length, transform.localScale.y, transform.localScale.z);
    }
    
    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(!stepped)
            {
                _floorFirstSteppedChannel.RaiseEvent(this);
                stepped = true;
            }
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("DamageObject"))
        {
            _floorCanReallocateChannel.RaiseEvent(this);
        }
    }
}