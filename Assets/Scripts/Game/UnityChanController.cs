using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script is edited from MoveCharacterAction.cs in UnityChan2D Demo
/// </summary>
public class UnityChanController : MonoBehaviour
{
    static int hashSpeed = Animator.StringToHash ("Speed");
	static int hashFallSpeed = Animator.StringToHash ("FallSpeed");
	static int hashGroundDistance = Animator.StringToHash ("GroundDistance");
	static int hashIsCrouch = Animator.StringToHash ("IsCrouch");
	static int hashDamage = Animator.StringToHash ("Damage");

    [Header("Boundings")]
    [SerializeField] private RuntimeAnimatorController _charaterAnimator;	
	[SerializeField] LayerMask _groundMask;

	Animator _animator;
	SpriteRenderer _spriteRenderer;
	Rigidbody2D _rig2d;

	[Header("Channels")]
	[SerializeField] GameOverChannel _gameOverChannel;
	[SerializeField] FloorFirstSteppedChannel _floorFirstSteppedChannel;
	[SerializeField] ComboAddChannel _comboAddChannel;
    

    [Header("Param")]
    [SerializeField] private float _characterHeightOffset = 0.245f;
	[SerializeField] private float _moveSpeed = 2f;
	[SerializeField] private float _jumpSpeed = 5f;

	[Header("Variable")]
	[SerializeField] private bool dontCountGroundedMoveTimes = false;
	[SerializeField] private float groundedMoveTimes;


	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	void OnEnable()
	{
		_floorFirstSteppedChannel.OnEventRaised += _ =>{
			 if(groundedMoveTimes <= 0.10f)
			 	_comboAddChannel.RaiseEvent();
			else
				print(groundedMoveTimes);
			 groundedMoveTimes = 0;
		};
	}

	void Awake ()
	{
		_animator = GetComponent<Animator> ();
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_rig2d = GetComponent<Rigidbody2D> ();

        if(_charaterAnimator)
            _animator.runtimeAnimatorController = _charaterAnimator;
        else
            Debug.LogWarning("No Character Animator set!");
	}

	void Update ()
	{
		float moveAxis = Input.GetAxis("Horizontal");
		float jumpAxis = Input.GetAxisRaw("Vertical");        
        Vector2 velocity = _rig2d.velocity;

        // get distance
        var raycast = Physics2D.Raycast(transform.position, Vector3.down, 1f, _groundMask);        
		float distanceFromGround = raycast.distance == 0 ? float.MaxValue : raycast.distance - _characterHeightOffset; // 0: no obejct
        bool grounded = distanceFromGround < 0.01f;
        // print(distanceFromGround);

        // controls
		if(grounded)
		{
			if (jumpAxis > 0 || Input.GetButtonDown("Jump")) 
			{
				velocity.y = _jumpSpeed;
			}
		}
			

		if (moveAxis != 0)
        {
			_spriteRenderer.flipX = moveAxis < 0;
            velocity.x = moveAxis * _moveSpeed;
			if(grounded && !dontCountGroundedMoveTimes)
				groundedMoveTimes += Time.deltaTime;
        }
        _rig2d.velocity = velocity;

		// update animator parameters
		_animator.SetBool (hashIsCrouch, jumpAxis < 0);
		_animator.SetFloat(hashGroundDistance, grounded ? 0 : 1/*distanceFromGround*/);
		_animator.SetFloat(hashFallSpeed, _rig2d.velocity.y);
		_animator.SetFloat(hashSpeed, Mathf.Abs (moveAxis));        
        // test: hit anim
        if(Input.GetKeyDown(KeyCode.Minus))
        {
            _animator.SetTrigger(hashDamage);
        }
	}
    

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		if(_rig2d.velocity.y > 0)
			dontCountGroundedMoveTimes = true;
		else
			dontCountGroundedMoveTimes = false;
	}


    void OnTriggerEnter2D(Collider2D other)
    {
		if(other.CompareTag("DamageObject"))
		{
        	_animator.SetTrigger(hashDamage);
			_gameOverChannel.RaiseEvent(GameOverReason.Dead);
			this.enabled = false;
		}
    }
}