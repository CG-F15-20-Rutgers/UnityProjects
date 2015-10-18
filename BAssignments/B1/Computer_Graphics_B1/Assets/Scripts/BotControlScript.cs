using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class BotControlScript : MonoBehaviour
{
	public float animSpeed = 1.5f;
	public float lookSmoother = 3f;
	
	private Animator anim; // a reference to the animator on the character
	private AnimatorStateInfo currentBaseState; // a reference to the current state of the animator, used for base layer

	private Rigidbody rb;

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int jumpDownanimState = Animator.StringToHash("Base Layer.JumpDown");
	static int fallState = Animator.StringToHash("Base Layer.Fall");
	// static int rollState = Animator.StringToHash("Base Layer.Roll");

	public float JumpSpeed;
	public float Speed;
	private bool isJumping;

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		if (anim.layerCount == 2)
		{
			anim.SetLayerWeight (1, 1);
		}
	}
	
	void FixedUpdate ()
	{
		// Get values from input
		float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		float jump = Input.GetAxis("Jump");

		// "Fire1" is the left control key. If it is held down, make the speed higher than the default. This causes the player to run. 
		if (Input.GetKey(KeyCode.LeftShift))
		{
			v = v * 1.0f;
		} 
		else
		{
			v = v * 0.2f;
		}

		// Set animation variables that affect Animation State Machine
		anim.SetFloat("Speed", v);
		anim.SetFloat("Direction", h);
		anim.speed = animSpeed;
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	
	
		if (currentBaseState.nameHash == locoState) {
			if (jump > 0) {
				anim.SetBool ("Jump", true);
				// Vector3 upVector = Vector3.up * jump * 25f * 0.3f * Time.deltaTime;
				// Vector3 movementVector = anim.velocity * Time.deltaTime;
				// transform.Translate (movementVector + upVector, Space.World);
				rb.AddForce(new Vector3(0.0f, 50.0f * 100f, 0.0f));
			}
		}
		else if (currentBaseState.nameHash == fallState)
		{
			if (!IsFreeFalling ())
			{
				// Once we're done free falling, we must set Jump to false. This stops the animation, and brings the character back to the Locomotion state. 
				anim.SetBool ("Jump", false);
				anim.CrossFade(idleState, 0.0f);
			}
		} else if (currentBaseState.nameHash == idleState) {
			// Jumping and idle states are mutually exclusive. Set Jump to false to ensure we maintain mutual exclusivity.
			anim.SetBool("Jump", false);
		}
	}

	// Note: hardcoded height for player, please be advised if player model is changed that this should be updated.
	private bool IsFreeFalling() {
		return !Physics.Raycast (transform.position, new Vector3(0.0f, -1.0f, 0.0f), 1f);
	}
}
