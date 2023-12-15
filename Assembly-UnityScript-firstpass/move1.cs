using System;
using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class move1 : MonoBehaviour
{
	public float maxSpeed;

	public float speedStep;

	public float currentStep;

	public float stepCounter;

	public float stepTime;

	public float turnSpeed;

	private float lastVelocity;

	private bool canTurnAround;

	public int groundSlopeLimit;

	public int wallRideSlopeLimit;

	public bool onMoon;

	private bool groundedLastFrame;

	[HideInInspector]
	public bool grounded;

	private float groundRayLength;

	private float groundTime;

	private float groundCounter;

	private float changeInY;

	private Vector3 lastPosition;

	public bool isGrinding;

	public bool freezeControls;

	public bool jumpPressed;

	public float skateForce;

	public float decaySpeed;

	private bool skateTriggered;

	private bool skateLeft;

	private bool skatePushed;

	private bool isSkating;

	private string lastAnim;

	private float transitionCount;

	private float transitionNeeded;

	private object soundManager;

	private UnityScript.Lang.Array decayingVelocities;

	private Vector3 jumpVelocity;

	public int jumpPower;

	public float jumpDecay;

	private float gravMove;

	private Vector3 lastJump;

	private float distanceFromGround;

	public float gravityBase;

	public float gravityGround;

	public float gravityIncrease;

	public float gravity;

	private float baseGravity;

	private float maxAirSpeed;

	private Vector3 airVelocity;

	public float airMovement;

	private Vector3 face;

	public bool wallRiding;

	public Vector3 wallNormal;

	private float maxY;

	private float minY;

	private float wallRideGracePeriod;

	private float wallRideGraceCounter;

	private float wallJumpGracePeriod;

	private float wallJumpGraceCounter;

	private float wallRideSpeed;

	private bool wallJumpTriggered;

	private bool jumpTriggered;

	private float jumpGraceCounter;

	public bool justJumped;

	private bool thisInputIsForJumps;

	private bool thisInputIsForRides;

	private Vector3 overVelocity;

	private float overSpeed;

	private float speedAtOverStep;

	private Transform mathIsHard;

	private float amountToRotate;

	private Quaternion lastRotation;

	public bool zeroGravity;

	private Transform holder;

	private Transform model;

	public bool wallRideRight;

	private bool rightNext;

	private bool rightSkate;

	private bool leftSkate;

	private int lastSpeed;

	private bool airControl;

	private bool airRelease;

	public bool wallRideV2;

	private bool overTurnLimit;

	private bool overTurnRelease;

	private bool atOneTimeOver;

	private bool allFine;

	private float turnLimit;

	private int cannotWallJump;

	private float turnAngle;

	public GameObject xinputObj;

	private float jumpShit;

	public float jumpDecaySpeed;

	public float baseMoonPull;

	private float moonPull;

	private float deadZone;

	private float inputPower;

	public move1()
	{
		maxSpeed = 30f;
		speedStep = 5f;
		currentStep = speedStep * 2f;
		stepTime = 0.2f;
		turnSpeed = 1f;
		canTurnAround = true;
		groundSlopeLimit = 45;
		wallRideSlopeLimit = 90;
		onMoon = true;
		groundTime = 0.02f;
		lastPosition = new Vector3(0f, 0f, 0f);
		skateForce = 20f;
		decaySpeed = 2f;
		skateLeft = true;
		lastAnim = "idle";
		transitionNeeded = 60f;
		decayingVelocities = new UnityScript.Lang.Array();
		jumpVelocity = new Vector3(0f, 0f, 0f);
		jumpPower = 20;
		jumpDecay = 0.5f;
		gravMove = 0.1f;
		lastJump = new Vector3(0f, 0f, 0f);
		gravityBase = 2.8f;
		gravityGround = 9.6f;
		gravityIncrease = 0.8f;
		gravity = 9.8f;
		airVelocity = Vector3.zero;
		airMovement = 20f;
		face = Vector3.zero;
		wallNormal = new Vector3(0f, 0f, 0f);
		wallRideGracePeriod = 0.2f;
		wallJumpGracePeriod = 0.4f;
		overVelocity = Vector3.zero;
		wallRideV2 = true;
		turnLimit = speedStep * 2f;
		jumpDecaySpeed = 10f;
		baseMoonPull = 2f;
		deadZone = 0.19f;
		inputPower = 2f;
	}

	public virtual Vector2 GetInputVec()
	{
		Vector2 vector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		Vector2 result;
		if (!(vector.magnitude > deadZone))
		{
			result = Vector2.zero;
		}
		else
		{
			vector = Vector2.ClampMagnitude(vector, 1f);
			float t = (vector.magnitude - deadZone) / (1f - deadZone);
			vector = Vector2.Lerp(Vector2.zero, vector.normalized, t);
			vector = vector.normalized * Mathf.Pow(vector.magnitude, inputPower);
			result = vector;
		}
		return result;
	}

	public virtual float GetAxis(object axis)
	{
		Vector2 inputVec = GetInputVec();
		return RuntimeServices.EqualityOperator(axis, "Horizontal") ? inputVec.x : ((!RuntimeServices.EqualityOperator(axis, "Vertical")) ? 0f : inputVec.y);
	}

	public virtual void Start()
	{
	}

	public virtual void DebugBehavoirs()
	{
		if (Input.GetButtonDown("Debug"))
		{
			Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
			direction.z += 200f;
			rigidbody.velocity = transform.TransformDirection(direction);
		}
	}

	public virtual void Update()
	{
		if (!freezeControls)
		{
			jumpGraceCounter += Time.deltaTime;
			wallJumpGraceCounter += Time.deltaTime;
			if (Input.GetButtonDown("Jump"))
			{
				if (grounded)
				{
					jumpTriggered = true;
				}
				else if (wallRiding && cannotWallJump > 2)
				{
					wallJumpTriggered = true;
				}
				cannotWallJump = 0;
			}
			if (wallRiding)
			{
				cannotWallJump++;
			}
			if (Input.GetButtonDown("Skate"))
			{
				Skate();
			}
			gravity = baseGravity * (Mathf.Abs(Input.GetAxis("Dive") * 2.5f) + 1f);
		}
		if (!freezeControls || !freezeControls || isGrinding)
		{
		}
		DebugBehavoirs();
		if (wallRiding)
		{
			if (wallRideRight)
			{
				xinputObj.SendMessage("WallRideR");
			}
			else
			{
				xinputObj.SendMessage("WallRideL");
			}
		}
	}

	public virtual void FixedUpdate()
	{
		if (freezeControls)
		{
			return;
		}
		CheckStep();
		if (!wallRiding)
		{
			if (jumpTriggered)
			{
				Jump();
			}
			StandardMovement();
			rigidbody.velocity += Vector3.down * gravity * Time.fixedDeltaTime;
		}
		else if (wallJumpTriggered)
		{
			WallJump();
		}
		else
		{
			WallRide();
		}
		jumpTriggered = false;
		wallJumpTriggered = false;
		skateTriggered = false;
		lastRotation = transform.rotation;
		lastPosition = transform.position;
	}

	public virtual void StandardMovement()
	{
		Gravity();
		if (!grounded && !airRelease && !Input.GetButton("Jump") && !airControl)
		{
			airRelease = true;
		}
		if (!grounded && airRelease && Input.GetButton("Jump") && !airControl)
		{
			airControl = true;
		}
		if (!grounded && airRelease && !Input.GetButton("Jump") && airControl)
		{
			airControl = false;
			airRelease = false;
		}
		if (grounded || (wallRideV2 && !airControl) || (!wallRideV2 && !Input.GetButton("Jump")))
		{
			float num = GetAxis("Horizontal") * 3f;
			transform.Rotate(new Vector3(0f, num, 0f));
			if (!(rigidbody.velocity.magnitude <= maxSpeed))
			{
				Vector3 vector = transform.InverseTransformDirection(rigidbody.velocity);
				vector = Quaternion.AngleAxis(num, transform.up) * vector;
				if (!grounded)
				{
					vector.x /= 1.01f;
				}
				else
				{
					vector.x /= 2f;
				}
				if (!(rigidbody.velocity.magnitude >= lastVelocity) && !(Input.GetAxis("Vertical") <= -0.5f))
				{
					rigidbody.velocity = transform.forward * lastVelocity;
				}
				else
				{
					rigidbody.velocity = transform.forward * rigidbody.velocity.magnitude;
				}
				rigidbody.velocity -= transform.TransformDirection(new Vector3(0f, 0f, overSpeed * 0.2f));
				overSpeed *= 0.8f;
				if (!(overSpeed >= skateForce / 10f))
				{
					overSpeed = 0f;
				}
			}
			else if (!(rigidbody.velocity.magnitude >= lastVelocity) && !(Input.GetAxis("Vertical") <= -0.5f))
			{
				rigidbody.velocity = transform.forward * lastVelocity;
			}
			else
			{
				rigidbody.velocity = transform.forward * rigidbody.velocity.magnitude;
			}
			lastVelocity = rigidbody.velocity.magnitude;
			if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= turnLimit))
			{
				overTurnLimit = true;
				atOneTimeOver = true;
				overTurnRelease = false;
				allFine = false;
			}
			else if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= turnLimit) && overTurnLimit && !(GetAxis("Vertical") <= -0.3f))
			{
				overTurnLimit = false;
				overTurnRelease = true;
			}
			else if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= turnLimit) && overTurnRelease && GetAxis("Vertical") == 0f)
			{
				allFine = true;
			}
			float num2 = 0f;
			Vector3 vector2 = default(Vector3);
			if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= speedStep * 2f))
			{
				if ((transform.InverseTransformDirection(rigidbody.velocity).z > 0.02f || !(GetAxis("Vertical") < -0.8f)) && grounded)
				{
					num2 = GetAxis("Vertical") * 2f;
					vector2 = transform.InverseTransformDirection(rigidbody.velocity);
					vector2.z += num2;
					vector2.z -= 1f;
					vector2.z = Mathf.Clamp(vector2.z, 0f, speedStep * 2f);
					rigidbody.velocity = transform.TransformDirection(vector2);
					canTurnAround = true;
					if (atOneTimeOver && !overTurnRelease)
					{
						canTurnAround = false;
					}
				}
				else if (canTurnAround && !(GetAxis("Vertical") >= -0.8f))
				{
					transform.Rotate(new Vector3(0f, 180f, 0f));
					canTurnAround = false;
					overTurnLimit = false;
					atOneTimeOver = false;
				}
			}
			else
			{
				Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
				float z = direction.z;
				if (!(GetAxis("Vertical") <= -0.01f) && (grounded || !(transform.InverseTransformDirection(rigidbody.velocity).z <= speedStep * 2f + 2f)))
				{
					direction.z = Mathf.Clamp(direction.z * 0.99f + direction.z * 0.01f * (GetAxis("Vertical") + 0.4f), 0f - z, z);
				}
				else if (!(GetAxis("Vertical") >= -0.01f))
				{
					direction.z = Mathf.Clamp(direction.z * 0.95f + direction.z * 0.05f * GetAxis("Vertical"), 0f - z, z);
				}
				rigidbody.velocity = transform.TransformDirection(direction);
				lastVelocity = rigidbody.velocity.magnitude;
			}
			if (skateTriggered)
			{
				Skate();
			}
			skateTriggered = false;
		}
		else
		{
			AirControl();
		}
	}

	public virtual void AirControl()
	{
		float num = GetAxis("Horizontal");
		if (!(wallJumpGraceCounter >= 0.2f))
		{
			Debug.DrawRay(transform.position, Vector3.up, Color.white, 100f);
			num = ((!wallRideRight) ? Mathf.Clamp(num, 0f, 1f) : Mathf.Clamp(num, -1f, 0f));
		}
		else
		{
			WallRideCheck();
		}
		rigidbody.velocity += transform.TransformDirection(new Vector3(num * airMovement, 0f, 0f));
		Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
		direction.x = Mathf.Clamp(direction.x, -20f - transform.InverseTransformDirection(rigidbody.velocity).z / 20f, 20f + transform.InverseTransformDirection(rigidbody.velocity).z / 20f);
		rigidbody.velocity = transform.TransformDirection(direction);
	}

	public virtual void JumpDecay()
	{
		transform.position += transform.up * jumpShit;
		jumpShit -= jumpDecaySpeed;
		if (!(jumpShit >= 0f))
		{
			jumpShit = 0f;
		}
	}

	public virtual void CheckStep()
	{
		if (!(stepCounter <= stepTime))
		{
			Vector3 vector = transform.InverseTransformDirection(rigidbody.velocity);
			vector.y = 0f;
			float magnitude = vector.magnitude;
			while (magnitude < currentStep - speedStep)
			{
				currentStep -= speedStep;
			}
			if (!(currentStep >= speedStep * 2f))
			{
				currentStep = speedStep * 2f;
			}
		}
		stepCounter += Time.fixedDeltaTime;
	}

	public virtual void WallRideCheck()
	{
		jumpTriggered = false;
		if (rigidbody.velocity.y <= 0f)
		{
			return;
		}
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(GetAxis("Horizontal"), 0f, 0f)), out hitInfo, Mathf.Abs(GetAxis("Horizontal") * 1.5f)))
		{
			wallNormal = hitInfo.normal * -1f;
			MonoBehaviour.print(hitInfo.collider.name);
			mathIsHard.position = hitInfo.point;
			mathIsHard.LookAt(mathIsHard.position + hitInfo.normal);
			Vector3 vector = Quaternion.AngleAxis(-90f, mathIsHard.up) * hitInfo.normal;
			Vector3 vector2 = Quaternion.AngleAxis(90f, mathIsHard.up) * hitInfo.normal;
			object obj = null;
			Vector3 vector3 = transform.InverseTransformDirection(rigidbody.velocity);
			if (!(Vector3.Angle(vector, transform.forward) >= Vector3.Angle(vector2, transform.forward)))
			{
				transform.localRotation = Quaternion.LookRotation(vector, mathIsHard.up);
				transform.Rotate(new Vector3(0f, 0f, -45f));
				wallRideRight = false;
			}
			else
			{
				transform.localRotation = Quaternion.LookRotation(vector2, mathIsHard.up);
				transform.Rotate(new Vector3(0f, 0f, 45f));
				wallRideRight = true;
			}
			if (Physics.Raycast(transform.position, wallNormal, out hitInfo, 2f))
			{
				transform.position = hitInfo.point;
				wallRiding = true;
			}
			rigidbody.velocity = transform.forward * vector3.z;
			wallRideSpeed = vector3.z + 10f;
			transform.position = hitInfo.point;
		}
	}

	public virtual void WallRide()
	{
		RaycastHit hitInfo = default(RaycastHit);
		float z = transform.InverseTransformDirection(rigidbody.velocity).z;
		Vector3 vector = wallNormal / -2f;
		float distance = 0.8f;
		if (!(z <= 10f))
		{
			if (!(z <= 600f))
			{
				distance = 10f;
				vector = wallNormal * -2f;
			}
			if (Physics.Raycast(transform.position + vector, wallNormal, out hitInfo, distance) && !Physics.Raycast(transform.position - wallNormal / 2f + transform.up / 2f, -Vector3.up, 0.7f) && !(Vector3.Distance(transform.position, lastPosition) <= 0.03f))
			{
				transform.position = hitInfo.point;
				wallNormal = hitInfo.normal * -1f;
				mathIsHard.position = hitInfo.point;
				mathIsHard.LookAt(mathIsHard.position + hitInfo.normal);
				Vector3 vector2 = Quaternion.AngleAxis(-90f, mathIsHard.up) * hitInfo.normal;
				Vector3 vector3 = Quaternion.AngleAxis(90f, mathIsHard.up) * hitInfo.normal;
				object obj = null;
				Vector3 vector4 = transform.InverseTransformDirection(rigidbody.velocity);
				if (!(Vector3.Angle(vector2, transform.forward) >= Vector3.Angle(vector3, transform.forward)))
				{
					transform.localRotation = Quaternion.LookRotation(vector2, mathIsHard.up);
					transform.Rotate(new Vector3(0f, 0f, -45f));
				}
				else
				{
					transform.localRotation = Quaternion.LookRotation(vector3, mathIsHard.up);
					transform.Rotate(new Vector3(0f, 0f, 45f));
				}
			}
			else
			{
				MonoBehaviour.print("ur dumb");
				wallRiding = false;
				airControl = false;
				airRelease = false;
			}
		}
		else
		{
			MonoBehaviour.print("this is weird");
			wallRiding = false;
			airControl = false;
			airRelease = false;
		}
		rigidbody.velocity = transform.forward * wallRideSpeed;
		wallRideSpeed += 0.6f;
	}

	public virtual void Skate()
	{
		skatePushed = true;
		Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
		direction.x = 0f;
		rigidbody.velocity = transform.TransformDirection(direction);
		if (!grounded || (!(overSpeed < 0.1f) && currentStep >= maxSpeed))
		{
			return;
		}
		float z = transform.InverseTransformDirection(rigidbody.velocity).z;
		rigidbody.velocity += transform.TransformDirection(new Vector3(0f, 0f, skateForce));
		stepCounter = 0f;
		if (!(currentStep + speedStep > maxSpeed))
		{
			currentStep += speedStep;
		}
		float num = 0f;
		if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= currentStep))
		{
			num = z - currentStep;
			if (!(num <= 0f))
			{
				overSpeed += skateForce;
			}
			else
			{
				overSpeed = 0f;
			}
		}
		speedAtOverStep = transform.InverseTransformDirection(rigidbody.velocity).z;
	}

	public virtual Vector3 decayVelocities()
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < decayingVelocities.length; i++)
		{
			if (RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_GreaterThan", UnityRuntimeServices.GetProperty(decayingVelocities[i], "magnitude"), 1)))
			{
				vector = (Vector3)RuntimeServices.InvokeBinaryOperator("op_Addition", vector, decayingVelocities[i]);
				object rhs = RuntimeServices.InvokeBinaryOperator("op_Subtraction", UnityRuntimeServices.GetProperty(decayingVelocities[i], "magnitude"), decaySpeed);
				decayingVelocities[i] = RuntimeServices.InvokeBinaryOperator("op_Multiply", UnityRuntimeServices.GetProperty(decayingVelocities[i], "normalized"), rhs);
			}
		}
		return vector;
	}

	public virtual void Jump()
	{
		jumpPressed = true;
		if (grounded || !(jumpGraceCounter >= 0.1f))
		{
			justJumped = true;
			jumpShit = jumpPower;
		}
		ExitZeroGravity();
	}

	public virtual void WallJump()
	{
		MonoBehaviour.print("fuck the police");
		justJumped = true;
		wallRiding = false;
		wallJumpGraceCounter = 0f;
		rigidbody.velocity += transform.up * jumpPower * 2f;
		airControl = false;
		airRelease = false;
		Vector3 vector = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		Quaternion rotation = transform.rotation;
		Vector3 vector3 = (rotation.eulerAngles = vector);
		Quaternion quaternion2 = (transform.rotation = rotation);
	}

	public virtual void Gravity()
	{
		JumpDecay();
		RaycastHit hitInfo = default(RaycastHit);
		if (!onMoon)
		{
			float distance = groundRayLength + 0.3f;
			if (Physics.Raycast(transform.position + transform.up, -transform.up, out hitInfo, distance) && !(Mathf.Abs(Vector3.Angle(Vector3.up, hitInfo.normal)) >= (float)groundSlopeLimit) && !(Mathf.Abs(Vector3.Angle(transform.up, hitInfo.normal)) >= (float)(groundSlopeLimit - 30)))
			{
				Vector3 forward = Vector3.Cross(transform.right, hitInfo.normal);
				Vector3 vector = transform.InverseTransformDirection(rigidbody.velocity);
				transform.localRotation = Quaternion.LookRotation(forward, hitInfo.normal);
				groundCounter = 0f;
				if (!grounded)
				{
					Landed();
				}
			}
			else
			{
				if (wallRiding)
				{
					return;
				}
				groundCounter += Time.fixedDeltaTime;
				if (!(groundCounter <= groundTime))
				{
					changeInY += Mathf.Abs(transform.position.y - lastPosition.y);
					if (!onMoon)
					{
						transform.localEulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
					}
					if (grounded)
					{
						LeftGround();
					}
				}
			}
		}
		else
		{
			if (!Physics.Raycast(transform.position + transform.up, -transform.up, out hitInfo, 150f))
			{
				return;
			}
			Vector3 forward2 = Vector3.Cross(transform.right, hitInfo.normal);
			transform.localRotation = Quaternion.LookRotation(forward2, hitInfo.normal);
			transform.rigidbody.velocity = transform.forward * transform.rigidbody.velocity.magnitude;
			if (!(Vector3.Distance(transform.position, hitInfo.point) >= 5f))
			{
				groundCounter = 0f;
				transform.position = hitInfo.point;
				Landed();
				return;
			}
			transform.position -= transform.up * moonPull;
			moonPull += baseMoonPull;
			if (!(moonPull <= 5f))
			{
				moonPull = 5f;
			}
		}
	}

	public virtual void Landed()
	{
		airControl = false;
		airRelease = false;
		grounded = true;
		changeInY = 0f;
		moonPull = baseMoonPull;
	}

	public virtual void LeftGround()
	{
		jumpGraceCounter = 0f;
		if (!onMoon)
		{
			transform.localEulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		}
		airControl = false;
		airRelease = false;
		face = transform.forward;
		maxAirSpeed = transform.InverseTransformDirection(rigidbody.velocity).z;
		grounded = false;
		if (zeroGravity)
		{
			ExitZeroGravity();
		}
	}

	public virtual void EnterZeroGravity()
	{
		zeroGravity = true;
		gravity = 0f;
	}

	public virtual void ExitZeroGravity()
	{
		zeroGravity = false;
		gravity = baseGravity;
	}

	public virtual void PlaySFX()
	{
		float z = transform.InverseTransformDirection(rigidbody.velocity).z;
		bool flag = false;
		if ((z > 0.3f || !(z >= -0.3f)) && grounded)
		{
			flag = true;
		}
		if (justJumped)
		{
			UnityRuntimeServices.Invoke(soundManager, "PlayJump", new object[0], typeof(MonoBehaviour));
			justJumped = false;
		}
		else if (wallRiding && !RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "wallRide"))
		{
			UnityRuntimeServices.Invoke(soundManager, "PlayWallRide", new object[0], typeof(MonoBehaviour));
		}
		else if (isGrinding && !RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "grind"))
		{
			UnityRuntimeServices.Invoke(soundManager, "PlayGrind", new object[0], typeof(MonoBehaviour));
		}
		else if (flag && !RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "skate"))
		{
			bool flag2 = false;
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Linecast(transform.position + new Vector3(0f, 5f, 0f), transform.position + new Vector3(0f, -10f, 0f), out hitInfo) && hitInfo.collider.name == "Terrain")
			{
				flag2 = true;
				if (!RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "skateSand"))
				{
					UnityRuntimeServices.Invoke(soundManager, "PlaySkateSand", new object[0], typeof(MonoBehaviour));
				}
			}
			if (!flag2)
			{
				UnityRuntimeServices.Invoke(soundManager, "PlaySkate", new object[0], typeof(MonoBehaviour));
			}
		}
		else if (!RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "none") && !isGrinding && !flag && !wallRiding)
		{
			UnityRuntimeServices.Invoke(soundManager, "StopSound", new object[0], typeof(MonoBehaviour));
		}
	}

	public virtual void Animate()
	{
		if (wallRiding)
		{
			isSkating = false;
			IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(model.animation);
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				if (!(obj is AnimationState))
				{
					obj = RuntimeServices.Coerce(obj, typeof(AnimationState));
				}
				AnimationState animationState = (AnimationState)obj;
				animationState.speed = 1f;
				UnityRuntimeServices.Update(enumerator, animationState);
			}
			if (wallRideRight)
			{
				lastAnim = "Right Grind";
				model.animation.CrossFade("Right Grind");
			}
			else
			{
				lastAnim = "Left Grind";
				model.animation.CrossFade("Left Grind");
			}
		}
		else if (isGrinding)
		{
			isSkating = false;
			IEnumerator enumerator2 = UnityRuntimeServices.GetEnumerator(model.animation);
			while (enumerator2.MoveNext())
			{
				object obj2 = enumerator2.Current;
				if (!(obj2 is AnimationState))
				{
					obj2 = RuntimeServices.Coerce(obj2, typeof(AnimationState));
				}
				AnimationState animationState2 = (AnimationState)obj2;
				animationState2.speed = 1f;
				UnityRuntimeServices.Update(enumerator2, animationState2);
			}
			lastAnim = "RailGrind";
			model.animation.CrossFade("RailGrind");
		}
		else if (!grounded)
		{
			float num = 150f;
			isSkating = false;
			if (jumpPressed)
			{
				IEnumerator enumerator3 = UnityRuntimeServices.GetEnumerator(model.animation);
				while (enumerator3.MoveNext())
				{
					object obj3 = enumerator3.Current;
					if (!(obj3 is AnimationState))
					{
						obj3 = RuntimeServices.Coerce(obj3, typeof(AnimationState));
					}
					AnimationState animationState3 = (AnimationState)obj3;
					animationState3.speed = 2f;
					UnityRuntimeServices.Update(enumerator3, animationState3);
				}
				jumpPressed = false;
				lastAnim = "JumpInit";
				if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= num))
				{
					model.animation.CrossFade("Flail");
				}
				else
				{
					model.animation.CrossFade("JumpInit");
				}
				return;
			}
			if (!(transform.InverseTransformDirection(rigidbody.velocity).y <= 0f))
			{
				IEnumerator enumerator4 = UnityRuntimeServices.GetEnumerator(model.animation);
				while (enumerator4.MoveNext())
				{
					object obj4 = enumerator4.Current;
					if (!(obj4 is AnimationState))
					{
						obj4 = RuntimeServices.Coerce(obj4, typeof(AnimationState));
					}
					AnimationState animationState4 = (AnimationState)obj4;
					animationState4.speed = 1f;
					UnityRuntimeServices.Update(enumerator4, animationState4);
				}
				lastAnim = "JumpAfter";
				if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= num))
				{
					model.animation.CrossFade("Flail");
				}
				else
				{
					model.animation.CrossFade("JumpAfter");
				}
				return;
			}
			IEnumerator enumerator5 = UnityRuntimeServices.GetEnumerator(model.animation);
			while (enumerator5.MoveNext())
			{
				object obj5 = enumerator5.Current;
				if (!(obj5 is AnimationState))
				{
					obj5 = RuntimeServices.Coerce(obj5, typeof(AnimationState));
				}
				AnimationState animationState5 = (AnimationState)obj5;
				animationState5.speed = 0.5f;
				UnityRuntimeServices.Update(enumerator5, animationState5);
			}
			lastAnim = "Fall";
			model.animation.CrossFade("Fall");
		}
		else if (lastAnim == "Fall" && grounded)
		{
			IEnumerator enumerator6 = UnityRuntimeServices.GetEnumerator(model.animation);
			while (enumerator6.MoveNext())
			{
				object obj6 = enumerator6.Current;
				if (!(obj6 is AnimationState))
				{
					obj6 = RuntimeServices.Coerce(obj6, typeof(AnimationState));
				}
				AnimationState animationState6 = (AnimationState)obj6;
				animationState6.speed = 1.5f;
				UnityRuntimeServices.Update(enumerator6, animationState6);
			}
			lastAnim = "Landing";
			model.animation.CrossFade("Landing");
		}
		else
		{
			if (!grounded)
			{
				return;
			}
			float axis = GetAxis("Horizontal");
			float min = 2.5f;
			float num2 = 2.5f;
			float num3 = 80f;
			int num4 = 40;
			float z = transform.InverseTransformDirection(rigidbody.velocity).z;
			float num5 = 0.5f;
			float num6 = 0.3f;
			if (!(z <= num3))
			{
				transitionCount += 1f;
			}
			else
			{
				transitionCount = 0f;
			}
			if (!(GetAxis("Vertical") >= 0f) && !(z <= num6))
			{
				lastAnim = "Stop";
				model.animation.CrossFade("Stop");
				return;
			}
			if (!(transitionCount <= (float)num4))
			{
				string text = "center";
				if (skatePushed && (lastAnim == "IdleR" || lastAnim == "IdleD" || lastAnim == "Skate" || lastAnim == "Landing"))
				{
					bool flag = false;
					IEnumerator enumerator7 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator7.MoveNext())
					{
						object obj7 = enumerator7.Current;
						if (!(obj7 is AnimationState))
						{
							obj7 = RuntimeServices.Coerce(obj7, typeof(AnimationState));
						}
						AnimationState animationState7 = (AnimationState)obj7;
						if ((animationState7.name == "DeepHoldRight" || animationState7.name == "DeepHoldLeft" || animationState7.name == "RightToIdleD" || animationState7.name == "LeftToIdleD" || animationState7.name == "Skate" || animationState7.name == "IdleR" || animationState7.name == "IdleD" || animationState7.name == "IdleD2" || animationState7.name == "Landing") && !(animationState7.time < animationState7.length))
						{
							flag = true;
						}
					}
					if (flag)
					{
						IEnumerator enumerator8 = UnityRuntimeServices.GetEnumerator(model.animation);
						while (enumerator8.MoveNext())
						{
							object obj8 = enumerator8.Current;
							if (!(obj8 is AnimationState))
							{
								obj8 = RuntimeServices.Coerce(obj8, typeof(AnimationState));
							}
							AnimationState animationState8 = (AnimationState)obj8;
							animationState8.speed = Mathf.Clamp(num2 / z, min, num2);
							UnityRuntimeServices.Update(enumerator8, animationState8);
						}
						isSkating = true;
						lastAnim = "SkateD";
						if (skateLeft)
						{
							model.animation.CrossFade("SkateLeftD");
							skateLeft = false;
						}
						else
						{
							model.animation.CrossFade("SkateRightD");
							skateLeft = true;
						}
					}
					skatePushed = false;
				}
				else if (isSkating && lastAnim != "IdleD" && lastAnim != "IdleR" && lastAnim != "Landing")
				{
					bool flag2 = false;
					IEnumerator enumerator9 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator9.MoveNext())
					{
						object obj9 = enumerator9.Current;
						if (!(obj9 is AnimationState))
						{
							obj9 = RuntimeServices.Coerce(obj9, typeof(AnimationState));
						}
						AnimationState animationState9 = (AnimationState)obj9;
						if ((animationState9.name == "SkateLeftD" || animationState9.name == "SkateRightD") && !(animationState9.time < animationState9.length))
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						lastAnim = "IdleD";
						isSkating = true;
						if (skateLeft)
						{
							model.animation.CrossFade("RightToIdleD");
						}
						else
						{
							model.animation.CrossFade("LeftToIdleD");
						}
					}
				}
				else if (isSkating)
				{
					bool flag3 = false;
					IEnumerator enumerator10 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator10.MoveNext())
					{
						object obj10 = enumerator10.Current;
						if (!(obj10 is AnimationState))
						{
							obj10 = RuntimeServices.Coerce(obj10, typeof(AnimationState));
						}
						AnimationState animationState10 = (AnimationState)obj10;
						if ((animationState10.name == "RightToIdleD" || animationState10.name == "LeftToIdleD" || animationState10.name == "DeepHoldLeft" || animationState10.name == "DeepHoldRight" || animationState10.name == "IdleD" || animationState10.name == "IdleD2") && !(animationState10.time < animationState10.length))
						{
							flag3 = true;
						}
					}
					if (flag3)
					{
						lastAnim = "IdleD";
						if (!(axis >= 0f - num5))
						{
							model.animation.CrossFade("DeepHoldLeft");
						}
						else if (!(axis <= num5))
						{
							model.animation.CrossFade("DeepHoldRight");
						}
						else
						{
							model.animation.CrossFade("IdleD");
						}
					}
				}
				else
				{
					if (isSkating)
					{
						return;
					}
					bool flag4 = false;
					if (lastAnim == "Landing")
					{
						IEnumerator enumerator11 = UnityRuntimeServices.GetEnumerator(model.animation);
						while (enumerator11.MoveNext())
						{
							object obj11 = enumerator11.Current;
							if (!(obj11 is AnimationState))
							{
								obj11 = RuntimeServices.Coerce(obj11, typeof(AnimationState));
							}
							AnimationState animationState11 = (AnimationState)obj11;
							if (animationState11.name == "Landing" && !(animationState11.time < animationState11.length))
							{
								flag4 = true;
							}
						}
					}
					else
					{
						flag4 = true;
					}
					if (flag4)
					{
						lastAnim = "IdleD";
						if (!(axis >= 0f - num5))
						{
							model.animation.CrossFade("DeepHoldLeft");
						}
						else if (!(axis <= num5))
						{
							model.animation.CrossFade("DeepHoldRight");
						}
						else
						{
							model.animation.CrossFade("IdleD");
						}
					}
				}
				return;
			}
			bool flag5;
			if (!(z <= 1f))
			{
				min = 2f;
				num2 = 5f;
				flag5 = false;
				bool flag6 = false;
				if (skatePushed || !(GetAxis("Vertical") <= 0f))
				{
					flag6 = false;
					if (lastAnim == "SkateD" || lastAnim == "Landing" || lastAnim == "Skate")
					{
						IEnumerator enumerator12 = UnityRuntimeServices.GetEnumerator(model.animation);
						while (enumerator12.MoveNext())
						{
							object obj12 = enumerator12.Current;
							if (!(obj12 is AnimationState))
							{
								obj12 = RuntimeServices.Coerce(obj12, typeof(AnimationState));
							}
							AnimationState animationState12 = (AnimationState)obj12;
							if (animationState12.name == "SkateLeftD" || animationState12.name == "SkateRightD" || animationState12.name == "RightToIdleD" || animationState12.name == "LeftToIdleD" || animationState12.name == "Landing")
							{
								if (!(animationState12.time < animationState12.length))
								{
									flag6 = true;
								}
							}
							else if (animationState12.name == "Skate")
							{
								if (!(animationState12.time < animationState12.length))
								{
									flag6 = true;
									animationState12.time -= animationState12.length;
									UnityRuntimeServices.Update(enumerator12, animationState12);
								}
								else if (!(animationState12.time >= animationState12.length * 0.75f))
								{
									skatePushed = false;
								}
							}
						}
					}
					else
					{
						flag6 = true;
					}
					if (!(GetAxis("Vertical") <= 0f))
					{
						flag6 = true;
					}
					if (!flag6)
					{
						return;
					}
					skatePushed = false;
					float speed = Mathf.Clamp(z / num2, min, num2);
					IEnumerator enumerator13 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator13.MoveNext())
					{
						object obj13 = enumerator13.Current;
						if (!(obj13 is AnimationState))
						{
							obj13 = RuntimeServices.Coerce(obj13, typeof(AnimationState));
						}
						AnimationState animationState13 = (AnimationState)obj13;
						animationState13.speed = speed;
						UnityRuntimeServices.Update(enumerator13, animationState13);
					}
					lastAnim = "Skate";
					model.animation.CrossFade("Skate");
					return;
				}
				flag5 = true;
				if (lastAnim == "Skate")
				{
					IEnumerator enumerator14 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator14.MoveNext())
					{
						object obj14 = enumerator14.Current;
						if (!(obj14 is AnimationState))
						{
							obj14 = RuntimeServices.Coerce(obj14, typeof(AnimationState));
						}
						AnimationState animationState14 = (AnimationState)obj14;
						if (animationState14.name == "Skate")
						{
							if (!(animationState14.time >= animationState14.length))
							{
								flag5 = false;
								continue;
							}
							animationState14.time -= animationState14.length;
							UnityRuntimeServices.Update(enumerator14, animationState14);
						}
					}
				}
				if (flag5)
				{
					lastAnim = "IdleD";
					model.animation.CrossFade("IdleD2");
				}
				return;
			}
			flag5 = false;
			if (lastAnim == "Landing")
			{
				IEnumerator enumerator15 = UnityRuntimeServices.GetEnumerator(model.animation);
				while (enumerator15.MoveNext())
				{
					object obj15 = enumerator15.Current;
					if (!(obj15 is AnimationState))
					{
						obj15 = RuntimeServices.Coerce(obj15, typeof(AnimationState));
					}
					AnimationState animationState15 = (AnimationState)obj15;
					if (animationState15.name == "Landing" && !(animationState15.time < animationState15.length))
					{
						flag5 = true;
					}
				}
			}
			else
			{
				flag5 = true;
			}
			if (!flag5)
			{
				return;
			}
			isSkating = false;
			IEnumerator enumerator16 = UnityRuntimeServices.GetEnumerator(model.animation);
			while (enumerator16.MoveNext())
			{
				object obj16 = enumerator16.Current;
				if (!(obj16 is AnimationState))
				{
					obj16 = RuntimeServices.Coerce(obj16, typeof(AnimationState));
				}
				AnimationState animationState16 = (AnimationState)obj16;
				animationState16.speed = 3f;
				UnityRuntimeServices.Update(enumerator16, animationState16);
			}
			lastAnim = "Idle";
			model.animation.CrossFade("Idle");
		}
	}

	public virtual void Main()
	{
	}
}
