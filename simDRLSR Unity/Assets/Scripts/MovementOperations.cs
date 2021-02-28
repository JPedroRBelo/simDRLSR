using UnityEngine;


	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]


    //Baseado no script ThirdPersonCharacter, disponível no pacote Standard Assets disponibilizado pela Unity Technologies através do Asset Store.
	public class MovementOperations : MonoBehaviour
	{
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float jumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float gravityMultiplier = 2f;
		[SerializeField] float runCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animSpeedMultiplier = 1f;
		[SerializeField] float groundCheckDistance = 0.1f;

		private Rigidbody rigidBody;
		Animator animator;
		bool isGrounded;
		float origGroundCheckDistance;
		const float k_Half = 0.5f;
		float turnAmount;
		float forwardAmount;
		Vector3 groundNormal;
		//float capsuleHeight;
		//Vector3 capsuleCenter;
		CapsuleCollider capsule;
		bool crouching;


		void Start()
		{
			animator = GetComponent<Animator>();
			rigidBody = GetComponent<Rigidbody>();
			capsule = GetComponent<CapsuleCollider>();
		//capsuleHeight = capsule.height;;
		//capsuleCenter = capsule.center;
		//animator.applyRootMotion = true;

		rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			origGroundCheckDistance = groundCheckDistance;
		}


		public void Move(Vector3 move, bool crouch, bool jump)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, groundNormal);
			turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (isGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			//ScaleCapsuleForCrouching(crouch);
			//PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}


		/*void ScaleCapsuleForCrouching(bool crouch)
		{
			if (isGrounded && crouch)
			{
				if (crouching) return;
				capsule.height = capsule.height / 2f;
				capsule.center = capsule.center / 2f;
				crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(rigidbody.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
				{
					crouching = true;
					return;
				}
				capsule.height = capsuleHeight;
				capsule.center = capsuleCenter;
				crouching = false;
			}
		}*/

		/*void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!crouching)
			{
				Ray crouchRay = new Ray(rigidbody.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
				{
					crouching = true;
				}
			}
		}*/


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.SetBool("Crouch", crouching);
			animator.SetBool("OnGround", isGrounded);
			if (!isGrounded)
			{
				animator.SetFloat("Jump", rigidBody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
			if (isGrounded)
			{
				animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (isGrounded && move.magnitude > 0)
			{
				animator.speed = animSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				animator.speed = 1;
			}
		}

        


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rigidBody.AddForce(extraGravityForce);

			groundCheckDistance = rigidBody.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
            // jump!
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpPower, rigidBody.velocity.z);
				isGrounded = false;
				animator.applyRootMotion = false;
				groundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (isGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = rigidBody.velocity.y;
            rigidBody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
			{
				groundNormal = hitInfo.normal;
				isGrounded = true;
				animator.applyRootMotion = true;
			}
			else
			{
				isGrounded = false;
				groundNormal = Vector3.up;
				animator.applyRootMotion = false;
			}
		}
	}

