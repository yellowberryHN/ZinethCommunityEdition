using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Player_Script__rigidBody_2 : MonoBehaviour
{
	private float currentAcceleration;

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

	public Player_Script__rigidBody_2()
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
		float num = Mathf.Sqrt(Mathf.Pow(rigidbody.velocity.x, 2f) + Mathf.Pow(rigidbody.velocity.z, 2f));
		float y = rigidbody.velocity.y;
		transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * turnSpeed, 0f));
		rigidbody.velocity = Vector3.zero;
		rigidbody.velocity = transform.forward * num;
		float y2 = y;
		Vector3 velocity = rigidbody.velocity;
		float num2 = (velocity.y = y2);
		Vector3 vector2 = (rigidbody.velocity = velocity);
		stepCounter += Time.deltaTime;
		if (!(stepCounter <= 0.09f))
		{
			Vector3 vector3 = transform.InverseTransformDirection(rigidbody.velocity);
			while (currentStep > vector3.z + 1f)
			{
				currentStep -= speedStep;
			}
			if (!(currentStep > speedStep * 2f))
			{
				currentStep = speedStep * 2f;
			}
		}
		Vector3 velocity2 = rigidbody.velocity;
		float z = transform.InverseTransformDirection(velocity2).z;
		float num3 = Input.GetAxis("Vertical") * currentStep * 1.2f;
		Vector3 vector4 = new Vector3(0f, 0f, num3);
		vector4 = transform.TransformDirection(vector4);
		rigidbody.velocity += vector4;
		Vector3 vector5 = transform.TransformDirection(rigidbody.velocity);
		int num4 = 0;
		Vector3 velocity3 = rigidbody.velocity;
		float num5 = (velocity3.y = num4);
		Vector3 vector7 = (rigidbody.velocity = velocity3);
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, currentStep);
		float y3 = y;
		Vector3 velocity4 = rigidbody.velocity;
		float num6 = (velocity4.y = y3);
		Vector3 vector9 = (rigidbody.velocity = velocity4);
		if (!(Mathf.Abs(z) + num3 <= currentStep))
		{
			MonoBehaviour.print(-1f * (z + num3 - currentStep));
		}
		rigidbody.velocity += -Vector3.up * gravity;
	}

	public virtual float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * jumpHeight * 10f);
	}

	public virtual void Skate()
	{
		stepCounter = 0f;
		if (!(currentStep >= maxSpeed))
		{
			currentStep += speedStep;
			Vector3 vector = new Vector3(0f, 0f, speedStep * 2f);
			vector = transform.TransformDirection(vector);
			rigidbody.AddForce(vector, ForceMode.Impulse);
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
