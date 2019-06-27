using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 200f;							
	[Range(1, 3)] [SerializeField] private float rollSpeed = 1.5f;			
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	
	[SerializeField] private bool airControl = false;							
	[SerializeField] private LayerMask groundMask;							
	[SerializeField] private Transform bottomPos;							
	[SerializeField] private Transform topPos;							
	[SerializeField] private Collider2D rollDisableCollider;				

	const float groundedRadius = .15f; // Radius of the overlap circle to determine if grounded
	private bool grounded;            // Whether or not the player is grounded.
	const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    [SerializeField] private Rigidbody2D rigidbody2D;
	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;

    public bool land;
    public bool isRolling;

/*	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnRollEvent;*/
	private bool wasRolling = false;

	private void Start()
	{
		rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        rollDisableCollider = gameObject.GetComponent<BoxCollider2D>();
	}
    

    private void FixedUpdate()
	{
		bool wasGrounded = grounded;
        if (rigidbody2D.velocity.y < 0.01)
        {
            grounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(bottomPos.position, groundedRadius, groundMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    grounded = true;
                    if (!wasGrounded)
                    {
                        land = true;
                    }
                }
            }
        }
        
	}

    public bool CheckCeiling()
    {
        if (Physics2D.OverlapCircle(topPos.position, ceilingRadius, groundMask))
        {
            return true;
        }
        return false;
    }




	public void Move(float move, bool roll, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (wasRolling && !roll)
		{
            // If the character has a ceiling preventing them from standing up, keep them crouching
            roll = CheckCeiling();
        }
        airControl = !roll;
		//only control the player if grounded or airControl is turned on
		if (grounded || airControl)
		{
			if (roll)
			{
                isRolling = true;
				if (!wasRolling)
				{
					wasRolling = true;
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= rollSpeed;

                // Disable one of the colliders when crouching
                if (rollDisableCollider != null)
                {
                    rollDisableCollider.enabled = false;
                }
			}
            else
			{
                isRolling = false;
                // Enable the collider when not crouching
                if (rollDisableCollider != null)
                {
                    rollDisableCollider.enabled = true;
                }

				if (wasRolling)
				{
                    wasRolling = false;
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);
            
			if (move > 0 && !facingRight)
			{
				Flip();
			}
			else if (move < 0 && facingRight)
			{
				Flip();
			}
		}
		if (grounded && jump)
		{
			grounded = false;
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
		}
	}


	private void Flip()
	{
		facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
	}
}
