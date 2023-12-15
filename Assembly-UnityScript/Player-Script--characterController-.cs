using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(CharacterController))]
public class Player_Script__characterController_ : MonoBehaviour
{
	public bool parentToGround;

	public Vector3 groundRayOffset;

	public float groundRayCastLength;

	public float topSpeedForward;

	public float topSpeedReverse;

	public float accelerationRate;

	public float decelerationRate;

	public float brakingDecelerationRate;

	public float stoppedTurnRate;

	public float topSpeedForwardTurnRate;

	public float topSpeedReverseTurnRate;

	public float gravity;

	public bool stickyThrottle;

	public float stickyThrottleDelay;

	private float currentSpeed;

	private float currentTopSpeed;

	private direction currentDirection;

	private bool isBraking;

	private bool isAccelerating;

	private float stickyDelayCount;

	private CharacterController characterController;

	public Player_Script__characterController_()
	{
		groundRayOffset = Vector3.zero;
		groundRayCastLength = 1f;
		topSpeedForward = 3f;
		topSpeedReverse = 1f;
		accelerationRate = 3f;
		decelerationRate = 2f;
		brakingDecelerationRate = 4f;
		stoppedTurnRate = 2f;
		topSpeedForwardTurnRate = 1f;
		topSpeedReverseTurnRate = 2f;
		gravity = 10f;
		stickyThrottleDelay = 0.35f;
		currentTopSpeed = topSpeedForward;
		currentDirection = direction.stop;
		stickyDelayCount = 9999f;
	}

	public virtual void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	public virtual void FixedUpdate()
	{
		Vector3 motion = Vector3.zero;
		direction direction2 = direction.stop;
		if (characterController.isGrounded)
		{
			float num = Mathf.Lerp((currentDirection != 0) ? topSpeedReverseTurnRate : topSpeedForwardTurnRate, stoppedTurnRate, 1f - currentSpeed / currentTopSpeed);
			float y = transform.eulerAngles.y + Input.GetAxis("Horizontal") * num;
			Vector3 eulerAngles = transform.eulerAngles;
			float num2 = (eulerAngles.y = y);
			Vector3 vector2 = (transform.eulerAngles = eulerAngles);
			if (!(Input.GetAxis("Vertical") <= 0f))
			{
				direction2 = direction.forward;
				isAccelerating = true;
			}
			else if (!(Input.GetAxis("Vertical") >= 0f))
			{
				direction2 = direction.reverse;
				isAccelerating = true;
			}
			else
			{
				direction2 = currentDirection;
				isAccelerating = false;
			}
			isBraking = false;
			if (currentDirection == direction.stop)
			{
				stickyDelayCount += Time.deltaTime;
				if ((!stickyThrottle || !(stickyDelayCount <= stickyThrottleDelay)) && ((direction2 == direction.reverse && topSpeedReverse > 0f) || (direction2 == direction.forward && !(topSpeedForward <= 0f))))
				{
					currentDirection = direction2;
				}
			}
			else if (currentDirection != direction2)
			{
				isBraking = true;
				isAccelerating = false;
			}
			if (currentDirection == direction.forward)
			{
				motion = Vector3.forward;
				currentTopSpeed = topSpeedForward;
			}
			else if (currentDirection == direction.reverse)
			{
				motion = -1f * Vector3.forward;
				currentTopSpeed = topSpeedReverse;
			}
			else if (currentDirection == direction.stop)
			{
				motion = Vector3.zero;
			}
			if (isAccelerating)
			{
				if (!(currentSpeed >= currentTopSpeed))
				{
					currentSpeed += accelerationRate * Time.deltaTime;
				}
			}
			else if (!(currentSpeed <= 0f))
			{
				float num3 = (isBraking ? brakingDecelerationRate : (stickyThrottle ? 0f : decelerationRate));
				currentSpeed -= num3 * Time.deltaTime;
			}
			if (currentSpeed < 0f || (currentSpeed == 0f && currentDirection != direction.stop))
			{
				SetStopped();
			}
			else if (!(currentSpeed <= currentTopSpeed))
			{
				currentSpeed = currentTopSpeed;
			}
			motion = transform.TransformDirection(motion);
		}
		motion.y -= gravity * Time.deltaTime;
		motion.z *= Time.deltaTime * currentSpeed;
		motion.x *= Time.deltaTime * currentSpeed;
		characterController.Move(motion);
		if (parentToGround)
		{
			RaycastHit hitInfo = default(RaycastHit);
			Vector3 vector3 = transform.TransformDirection(-1f * Vector3.up);
			if (Physics.Raycast(transform.TransformPoint(groundRayOffset), vector3, out hitInfo, groundRayCastLength))
			{
				transform.parent = hitInfo.transform;
			}
			if (currentDirection == direction.stop)
			{
				characterController.SimpleMove(transform.TransformDirection(Vector3.forward) * 1E-12f);
			}
		}
	}

	public virtual float GetCurrentSpeed()
	{
		return currentSpeed;
	}

	public virtual float GetCurrentTopSpeed()
	{
		return currentTopSpeed;
	}

	public virtual direction GetCurrentDirection()
	{
		return currentDirection;
	}

	public virtual bool GetIsBraking()
	{
		return isBraking;
	}

	public virtual bool GetIsAccelerating()
	{
		return isAccelerating;
	}

	public virtual void SetStopped()
	{
		currentSpeed = 0f;
		currentDirection = direction.stop;
		isAccelerating = false;
		isBraking = false;
		stickyDelayCount = 0f;
	}
}

[Serializable]
public enum direction
{
	forward,
	reverse,
	stop
}

