using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class PlayerScript_rigidBody : MonoBehaviour
{
	private float currentAcceleration;

	public bool impulseMode;

	public float maxAcceleration;

	public float turnSpeed;

	private float lastVelocity;

	private bool jumpTriggered;

	private bool skateTriggered;

	public float jumpHeight;

	public float gravity;

	public PhysicMaterial groundMaterial;

	public PhysicMaterial airMaterial;

	public int groundSlopeLimit;

	public int wallRideSlopeLimit;

	private bool grounded;

	private float groundRayLength;

	private float maxSpeed;

	private float speedStep;

	private float currentStep;

	private float stepCounter;

	public bool v1;

	public bool v2;

	private Quaternion lastRotation;

	public PlayerScript_rigidBody()
	{
		maxAcceleration = 1f;
		turnSpeed = 1f;
		jumpHeight = 10f;
		gravity = 9.8f;
		groundSlopeLimit = 45;
		wallRideSlopeLimit = 90;
		maxSpeed = 30f;
		speedStep = 5f;
		currentStep = speedStep * 2f;
		lastRotation = Quaternion.identity;
	}

	public virtual void Start()
	{
		groundRayLength = RuntimeServices.UnboxSingle(RuntimeServices.InvokeBinaryOperator("op_Division", RuntimeServices.InvokeBinaryOperator("op_Addition", UnityRuntimeServices.GetProperty(collider, "height"), UnityRuntimeServices.GetProperty(UnityRuntimeServices.GetProperty(collider, "center"), "y")), 2));
	}

	public virtual void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			rigidbody.AddForce(transform.up * CalculateJumpVerticalSpeed(), ForceMode.Impulse);
		}
		if (Input.GetButtonDown("Skate"))
		{
			Skate();
		}
		if (Input.GetButtonDown("Right Boost"))
		{
			rigidbody.AddForce(transform.right * 25f, ForceMode.Impulse);
		}
		if (Input.GetButtonDown("Left Boost"))
		{
			rigidbody.AddForce(-transform.right * 25f, ForceMode.Impulse);
		}
		GroundCheck();
	}

	public virtual void FixedUpdate()
	{
		stepCounter += Time.deltaTime;
		if (!(stepCounter <= 0.09f))
		{
			Vector3 vector = transform.InverseTransformDirection(rigidbody.velocity);
			while (currentStep > vector.z + 1f)
			{
				currentStep -= speedStep;
			}
			if (!(currentStep > speedStep * 2f))
			{
				currentStep = speedStep * 2f;
			}
		}
		if (v1)
		{
			transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * turnSpeed, 0f));
			Vector3 velocity = rigidbody.velocity;
			float z = transform.InverseTransformDirection(velocity).z;
			float num = Input.GetAxis("Vertical") * currentStep * 1.2f;
			Vector3 direction = new Vector3(0f, 0f, num);
			direction = transform.TransformDirection(direction);
			if (!impulseMode)
			{
				rigidbody.AddForce(direction);
			}
			else
			{
				rigidbody.AddForce(direction, ForceMode.Impulse);
			}
			if (!(Mathf.Abs(z) + num <= currentStep) && impulseMode)
			{
				Vector3 direction2 = new Vector3(0f, 0f, -1f * (z + num - currentStep));
				MonoBehaviour.print(-1f * (z + num - currentStep));
				direction2 = transform.TransformDirection(direction2);
				rigidbody.AddForce(direction2, ForceMode.Impulse);
			}
			rigidbody.AddForce(-Vector3.up * gravity);
		}
		if (!v2)
		{
			return;
		}
		Vector3 velocity2 = rigidbody.velocity;
		velocity2.y = 0f;
		transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * turnSpeed, 0f));
		transform.Find("Velocity Offset").rotation = Quaternion.Slerp(lastRotation, transform.rotation, 0.5f);
		Vector3 vector2 = transform.TransformDirection(new Vector3(0f, 0f, Input.GetAxis("Vertical") * currentStep));
		float z2 = vector2.z;
		Vector3 velocity3 = rigidbody.velocity;
		float num2 = (velocity3.z = z2);
		Vector3 vector4 = (rigidbody.velocity = velocity3);
		float x = vector2.x;
		Vector3 velocity4 = rigidbody.velocity;
		float num3 = (velocity4.x = x);
		Vector3 vector6 = (rigidbody.velocity = velocity4);
		rigidbody.AddForce(-Vector3.up * gravity);
		if (skateTriggered)
		{
			skateTriggered = false;
			stepCounter = 0f;
			if (!(currentStep >= maxSpeed))
			{
				currentStep += speedStep;
				Vector3 force = new Vector3(0f, 0f, speedStep * 200f);
				rigidbody.AddRelativeForce(force, ForceMode.Impulse);
			}
		}
	}

	public virtual float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * jumpHeight * 10f);
	}

	public virtual void Skate()
	{
		if (v2)
		{
			skateTriggered = true;
		}
		if (v1)
		{
			stepCounter = 0f;
			if (!(currentStep >= maxSpeed))
			{
				currentStep += speedStep;
				Vector3 force = new Vector3(0f, 0f, speedStep * 2f);
				rigidbody.AddRelativeForce(force, ForceMode.Impulse);
			}
		}
	}

	public virtual void GroundCheck()
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(transform.position, -transform.up, out hitInfo, groundRayLength) && !(Vector3.Angle(transform.up, hitInfo.normal) >= (float)groundSlopeLimit))
		{
			if (!grounded)
			{
				Landed();
			}
		}
		else if (grounded)
		{
			LeftGround();
		}
	}

	public virtual void Landed()
	{
		grounded = true;
		collider.material = groundMaterial;
	}

	public virtual void LeftGround()
	{
		grounded = false;
		collider.material = airMaterial;
	}
}
