using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : MonoBehaviour
{
    [Header("Param")]
    [SerializeField] Sprite[] _sprites;

    [Header("Variables")]
    public uint index;
    public bool stepped;

    [Header("Channels")]
    [SerializeField] FloorCanReallocateChannel _floorCanReallocateChannel;
    [SerializeField] FloorFirstSteppedChannel _floorFirstSteppedChannel;

    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }


    public void Reallocate(uint index, int spriteId, float posX, float floorWidth)
    {
        gameObject.name = $"Floor {index}";
        this.index = index;

        _spriteRenderer.sprite = _sprites[spriteId];
        _spriteRenderer.size = new Vector2(0.31f + floorWidth * 0.38f, _spriteRenderer.size.y);

        transform.position = new Vector3(posX, index);
        // transform.localScale = new Vector3(floorWidth, transform.localScale.y, transform.localScale.z);

        stepped = false;
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