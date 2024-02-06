using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

 /*
  * move.cs - player movement code:
  * this code seems to be responsible for managing the logic for the movement of the character
  *
  * notes:
  * - camera movement can't keep up at over 4700 units of speed, snaps to player, maybe in here?
  * 
  * TODO: clean up variable names in here as much as possible (i hate unityscript)
  */

[Serializable]
public class move : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Decent_002488 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024tv_002489;

			internal float _0024counter_002490;

			internal Vector3 _0024tempVelocity_002491;

			internal move _0024self__002492;

			public _0024(move self_)
			{
				_0024self__002492 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					result = (Yield(2, new WaitForSeconds(0.2f)) ? 1 : 0);
					break;
				case 2:
					_0024tv_002489 = _0024self__002492.rigidbody.velocity.magnitude;
					_0024counter_002490 = 0.01f;
					goto case 3;
				case 3:
					if (_0024counter_002490 < 0.95f)
					{
						_0024counter_002490 += 0.01f;
						_0024tempVelocity_002491 = _0024self__002492.transform.InverseTransformDirection(_0024self__002492.rigidbody.velocity);
						_0024tempVelocity_002491.z -= 100f;
						_0024tempVelocity_002491.y -= 50f;
						if (!(_0024tempVelocity_002491.z >= 20f))
						{
							_0024tempVelocity_002491.z = 20f;
						}
						_0024self__002492.rigidbody.velocity = _0024self__002492.transform.TransformDirection(_0024tempVelocity_002491);
						result = (Yield(3, new WaitForSeconds(0.03f)) ? 1 : 0);
						break;
					}
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal move _0024self__002493;

		public _0024Decent_002488(move self_)
		{
			_0024self__002493 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002493);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024LowerDamping_002494 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Component _0024cam_002495;

			internal move _0024self__002496;

			public _0024(move self_)
			{
				_0024self__002496 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024cam_002495 = _0024self__002496.mainCam.GetComponent<NewCamera>();
					goto case 2;
				case 2:
					if (RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_GreaterThan", UnityRuntimeServices.GetProperty(_0024cam_002495, "damping"), 8)))
					{
						RuntimeServices.SetProperty(_0024cam_002495, "damping", RuntimeServices.InvokeBinaryOperator("op_Subtraction", UnityRuntimeServices.GetProperty(_0024cam_002495, "damping"), 10));
						if (RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_LessThan", UnityRuntimeServices.GetProperty(_0024cam_002495, "damping"), 8)))
						{
							RuntimeServices.SetProperty(_0024cam_002495, "damping", 8);
						}
						result = (Yield(2, new WaitForSeconds(0.1f)) ? 1 : 0);
						break;
					}
					RuntimeServices.SetProperty(_0024cam_002495, "damping", 8);
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal move _0024self__002497;

		public _0024LowerDamping_002494(move self_)
		{
			_0024self__002497 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002497);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024IncreaseDamping_002498 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Component _0024cam_002499;

			internal move _0024self__0024100;

			public _0024(move self_)
			{
				_0024self__0024100 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024cam_002499 = _0024self__0024100.mainCam.GetComponent<NewCamera>();
					goto case 2;
				case 2:
					if (RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_LessThan", UnityRuntimeServices.GetProperty(_0024cam_002499, "damping"), 500)))
					{
						RuntimeServices.SetProperty(_0024cam_002499, "damping", RuntimeServices.InvokeBinaryOperator("op_Addition", UnityRuntimeServices.GetProperty(_0024cam_002499, "damping"), 10));
						if (RuntimeServices.ToBool(RuntimeServices.InvokeBinaryOperator("op_GreaterThan", UnityRuntimeServices.GetProperty(_0024cam_002499, "damping"), 500)))
						{
							RuntimeServices.SetProperty(_0024cam_002499, "damping", 500);
						}
						result = (Yield(2, new WaitForSeconds(0.1f)) ? 1 : 0);
						break;
					}
					RuntimeServices.SetProperty(_0024cam_002499, "damping", 500);
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal move _0024self__0024101;

		public _0024IncreaseDamping_002498(move self_)
		{
			_0024self__0024101 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__0024101);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Reminder_0024102 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal move _0024self__0024103;

			public _0024(move self_)
			{
				_0024self__0024103 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (_0024self__0024103.fuckYou != null)
					{
						_0024self__0024103.fuckYou.active = true;
						result = (Yield(2, new WaitForSeconds(3f)) ? 1 : 0);
						break;
					}
					goto IL_0062;
				case 2:
					_0024self__0024103.fuckYou.active = false;
					goto IL_0062;
				case 1:
					{
						result = 0;
						break;
					}
					IL_0062:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal move _0024self__0024104;

		public _0024Reminder_0024102(move self_)
		{
			_0024self__0024104 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__0024104);
		}
	}

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

	private bool groundedLastFrame;

	public bool onMoon;

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

	public string lastAnim;

	public string animName;

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

	private Transform mainCam;

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

	public float overSpeed;

	private float speedAtOverStep;

	private Transform mathIsHard;

	private float amountToRotate;

	private Quaternion lastRotation;

	public bool zeroGravity;

	private Transform holder;

	public Transform model;

	public bool wallRideRight;

	private bool rightNext;

	private bool rightSkate;

	private bool leftSkate;

	private PlayerGraphic playerGraphic;

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

	private bool jumpeded;

	public float airTime;

	private float deadZone;

	private float inputPower;

	private int moonGroundSlope;

	private int groundGroundSlope;

	private float moonJumpPower;

	private int groundJumpPower;

	private bool locked;

	private float moonTime;

	public bool canDebugBoost;

	private bool canHover;

	public Transform fuckYou;

	public bool canRewind;

	public bool canWallRide;

	public bool V1;

	public move()
	{
		maxSpeed = 30f;
		speedStep = 5f;
		currentStep = speedStep * 2f;
		stepTime = 0.2f;
		turnSpeed = 1f;
		canTurnAround = true;
		groundSlopeLimit = 45;
		wallRideSlopeLimit = 90;
		groundTime = 0.02f;
		lastPosition = new Vector3(0f, 0f, 0f);
		skateForce = 20f;
		decaySpeed = 2f;
		skateLeft = true;
		lastAnim = "idle";
		animName = "IdleD";
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
		moonGroundSlope = 400;
		groundGroundSlope = 72;
		moonJumpPower = 2f;
		groundJumpPower = 31;
		canRewind = true;
		canWallRide = true;
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

	public virtual void Stop()
	{
		if (rigidbody.isKinematic)
		{
			Debug.LogWarning("trying to alter kinematic rigidbody...");
			return;
		}
		int num = 0;
		Vector3 velocity = rigidbody.velocity;
		float num2 = (velocity.x = num);
		Vector3 vector2 = (rigidbody.velocity = velocity);
		int num3 = 0;
		Vector3 velocity2 = rigidbody.velocity;
		float num4 = (velocity2.z = num3);
		Vector3 vector4 = (rigidbody.velocity = velocity2);
		int num5 = -20;
		Vector3 velocity3 = rigidbody.velocity;
		float num6 = (velocity3.y = num5);
		Vector3 vector6 = (rigidbody.velocity = velocity3);
	}

	public virtual void CheckUnlock()
	{
		if (Input.GetAxis("Vertical") == 0f && Input.GetAxis("Horizontal") == 0f)
		{
			MonoBehaviour.print("yeah unlocking");
			locked = false;
		}
	}

	public virtual void Start()
	{
		if (Application.isEditor)
		{
			canDebugBoost = true;
		}
		if (PlayerPrefs.HasKey("debug_boost"))
		{
			canDebugBoost = true;
		}
		mainCam = GameObject.Find("Camera Holder").transform;
		soundManager = GameObject.Find("Camera Holder").GetComponent<SoundManager>();
		groundRayLength = RuntimeServices.UnboxSingle(RuntimeServices.InvokeBinaryOperator("op_Addition", RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(collider, "height"), 2), 0.4f));
		mathIsHard = transform.Find("Math Is Hard");
		mathIsHard.parent = null;
		lastRotation = transform.rotation;
		baseGravity = gravity;
		model = GameObject.Find("main_character").transform;
		holder = transform.Find("Holder");
		holder.parent = null;
		playerGraphic = (PlayerGraphic)GameObject.Find("Holder").GetComponent<PlayerGraphic>();
		if (xinputObj == null)
		{
			xinputObj = GameObject.Find("XInputContainer");
		}
		if (model.animation["PhoneArm"] != null)
		{
			AnimationState animationState = model.animation["PhoneArm"];
			animationState.AddMixingTransform(model.Find("Joints_GRP/root/hips_upper/chest/R_h_shoulder"));
			animationState.layer = 2;
			animationState.blendMode = AnimationBlendMode.Blend;
			animationState.wrapMode = WrapMode.Once;
			animationState.enabled = true;
			animationState.weight = 1f;
			animationState.speed = 3f;
			model.animation.Stop();
		}
	}

	public virtual void DebugBehavoirs()
	{
		if (Input.GetButtonDown("Debug"))
		{
			DebugBoost(200f);
		}
	}

	public virtual void DebugBoost(float amount)
	{
		if (canDebugBoost)
		{
			Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
			direction.z += amount;
			if (!rigidbody.isKinematic)
			{
				rigidbody.velocity = transform.TransformDirection(direction);
				UnityRuntimeServices.Invoke(soundManager, "PlayBoost", new object[0], typeof(MonoBehaviour));
			}
		}
	}

	public virtual void ForceDebugBoost(float amount)
	{
		Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
		direction.z += amount;
		if (!rigidbody.isKinematic)
		{
			rigidbody.velocity = transform.TransformDirection(direction);
			UnityRuntimeServices.Invoke(soundManager, "PlayBoost", new object[0], typeof(MonoBehaviour));
		}
	}

	public virtual void LandOnMoon()
	{
		canDebugBoost = true;
		PlayerPrefs.SetInt("debug_boost", 1);
		onMoon = true;
		jumpPower = (int)moonJumpPower;
		groundSlopeLimit = moonGroundSlope;
		Transform transform = GameObject.Find("Moon").transform;
		RaycastHit raycastHit = default(RaycastHit);
		RuntimeServices.SetProperty(GameObject.Find("Holder").GetComponent<PlayerGraphic>(), "onMoon", true);
		Vector3 vector = this.transform.position - transform.position;
		Vector3 forward = Vector3.Cross(this.transform.right, vector);
		this.transform.localRotation = Quaternion.LookRotation(forward, vector);
		Vector3 direction = this.transform.InverseTransformDirection(rigidbody.velocity);
		direction.y = 0f;
		rigidbody.velocity = this.transform.TransformDirection(direction);
		StartCoroutine("IncreaseDamping");
	}

	public virtual void LeaveMoon()
	{
		onMoon = false;
		moonTime = 0f;
		jumpPower = groundJumpPower;
		groundSlopeLimit = groundGroundSlope;
		RuntimeServices.SetProperty(GameObject.Find("Holder").GetComponent<PlayerGraphic>(), "onMoon", false);
		RuntimeServices.SetProperty(GameObject.Find("Moon").GetComponent<Moon_Script>(), "onMoon", false);
		StartCoroutine("LowerDamping");
		StartCoroutine("Decent");
	}

	public virtual IEnumerator Decent()
	{
		return new _0024Decent_002488(this).GetEnumerator();
	}

	public virtual IEnumerator LowerDamping()
	{
		return new _0024LowerDamping_002494(this).GetEnumerator();
	}

	public virtual IEnumerator IncreaseDamping()
	{
		return new _0024IncreaseDamping_002498(this).GetEnumerator();
	}

	public virtual void Update()
	{
		if (onMoon)
		{
			moonTime += Time.deltaTime;
		}
		if (locked)
		{
			CheckUnlock();
		}
		if (!freezeControls && !locked)
		{
			jumpGraceCounter += Time.deltaTime;
			wallJumpGraceCounter += Time.deltaTime;
			if (Input.GetButtonDown("Jump"))
			{
				if (!wallRiding)
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
			float num = 0f;
			num = ((Application.platform != RuntimePlatform.OSXPlayer) ? Mathf.Abs(Input.GetAxis("Dive_PC")) : Input.GetAxis("Dive_OSX"));
			if (!(num > 0.001f))
			{
				num = Input.GetAxis("Dive");
			}
			gravity = baseGravity * (num * 2.5f + 1f);
		}
		if (!freezeControls || (freezeControls && isGrinding))
		{
			Animate();
			PlaySFX();
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
		playerGraphic.DoFixedUpdate();
	}

	public virtual IEnumerator Reminder()
	{
		return new _0024Reminder_0024102(this).GetEnumerator();
	}

	public virtual void FixedUpdate()
	{
		if (!freezeControls && !locked)
		{
			if (!onMoon)
			{
				if (!(lastVelocity / 1.2f <= rigidbody.velocity.magnitude) && !(lastVelocity <= 80f) && canRewind)
				{
					StartCoroutine("Reminder");
				}
				rigidbody.velocity -= transform.TransformDirection(new Vector3(0f, 0f, overSpeed * 0.2f));
				overSpeed *= 0.8f;
				if (!(overSpeed >= skateForce / 10f))
				{
					overSpeed = 0f;
				}
				CheckStep();
				if (!wallRiding)
				{
					if (jumpTriggered)
					{
						Jump();
					}
					StandardMovement();
					if (!onMoon)
					{
						rigidbody.velocity += Vector3.down * gravity * Time.fixedDeltaTime;
					}
				}
				else if (wallJumpTriggered)
				{
					WallJump();
				}
				else
				{
					WallRide();
				}
			}
			else
			{
				CheckStep();
				if (!wallRiding)
				{
					if (jumpTriggered)
					{
						Jump();
					}
					StandardMovement();
				}
				else if (wallJumpTriggered)
				{
					WallJump();
				}
				else
				{
					WallRide();
				}
			}
			jumpTriggered = false;
			wallJumpTriggered = false;
			skateTriggered = false;
			lastRotation = transform.rotation;
			lastPosition = transform.position;
			if (grounded)
			{
				airTime = 0f;
			}
			else
			{
				airTime += Time.fixedDeltaTime;
			}
		}
		lastVelocity = rigidbody.velocity.magnitude;
		playerGraphic.DoFixedUpdate();
	}

	public virtual void StandardMovement()
	{
		object obj = null;
		object obj2 = null;
		float num = 0f;
		object obj3 = null;
		object obj4 = null;
		Vector3 vector = default(Vector3);
		if (!onMoon)
		{
			Gravity();
			bool flag = false;
			if (!V1 && Input.GetButton("Jump") && !(GetAxis("Horizontal") <= 0.1f) && Physics.Raycast(transform.position, mainCam.TransformDirection(new Vector3(GetAxis("Horizontal"), 0f, 0f)), 23f))
			{
				rigidbody.velocity += transform.TransformDirection(new Vector3(GetAxis("Horizontal") * airMovement, 0f, 0f));
				flag = true;
			}
			if (flag)
			{
				obj3 = transform.InverseTransformDirection(rigidbody.velocity);
				RuntimeServices.SetProperty(obj3, "x", Mathf.Clamp(RuntimeServices.UnboxSingle(UnityRuntimeServices.GetProperty(obj3, "x")), -20f - transform.InverseTransformDirection(rigidbody.velocity).z / 20f, 20f + transform.InverseTransformDirection(rigidbody.velocity).z / 20f));
				rigidbody.velocity = transform.TransformDirection((Vector3)obj3);
				AirControl();
			}
			else if ((V1 && (grounded || !Input.GetButton("Jump"))) || !V1)
			{
				obj = GetAxis("Horizontal") * 3f;
				transform.Rotate(Quaternion.AngleAxis(RuntimeServices.UnboxSingle(obj), transform.up).eulerAngles);
				obj2 = transform.InverseTransformDirection(rigidbody.velocity);
				obj2 = RuntimeServices.InvokeBinaryOperator("op_Multiply", Quaternion.AngleAxis(RuntimeServices.UnboxSingle(obj), transform.up), obj2);
				if (!grounded)
				{
					RuntimeServices.SetProperty(obj2, "x", RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(obj2, "x"), 1.01f));
				}
				else
				{
					RuntimeServices.SetProperty(obj2, "x", RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(obj2, "x"), 2));
				}
				rigidbody.velocity = transform.TransformDirection((Vector3)obj2);
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
				if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= speedStep * 2f))
				{
					if ((transform.InverseTransformDirection(rigidbody.velocity).z > 0.02f || !(GetAxis("Vertical") < -0.8f)) && (grounded || !Physics.Raycast(transform.position - transform.forward / 20f, transform.forward, 3f)))
					{
						num = GetAxis("Vertical") * 2f;
						vector = transform.InverseTransformDirection(rigidbody.velocity);
						vector.z += num;
						vector.z -= 1f;
						vector.z = Mathf.Clamp(vector.z, 0f, speedStep * 2f);
						rigidbody.velocity = transform.TransformDirection(vector);
					}
					if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= 1f) && overTurnRelease)
					{
						if (canTurnAround && !(GetAxis("Vertical") >= -0.8f))
						{
							transform.Rotate(new Vector3(0f, 180f, 0f));
							canTurnAround = false;
							overTurnLimit = false;
							atOneTimeOver = false;
						}
						else if (!(GetAxis("Vertical") < 0f))
						{
							canTurnAround = true;
						}
					}
				}
				else if (!(GetAxis("Vertical") >= -0.01f))
				{
					obj3 = transform.InverseTransformDirection(rigidbody.velocity);
					obj4 = UnityRuntimeServices.GetProperty(obj3, "z");
					RuntimeServices.SetProperty(obj3, "z", UnityRuntimeServices.Invoke(typeof(Mathf), "Clamp", new object[3]
					{
						RuntimeServices.InvokeBinaryOperator("op_Addition", RuntimeServices.InvokeBinaryOperator("op_Multiply", UnityRuntimeServices.GetProperty(obj3, "z"), 0.97f), RuntimeServices.InvokeBinaryOperator("op_Multiply", RuntimeServices.InvokeBinaryOperator("op_Multiply", UnityRuntimeServices.GetProperty(obj3, "z"), 0.03f), GetAxis("Vertical"))),
						RuntimeServices.InvokeUnaryOperator("op_UnaryNegation", obj4),
						obj4
					}, typeof(MonoBehaviour)));
					rigidbody.velocity = transform.TransformDirection((Vector3)obj3);
				}
				rigidbody.velocity += decayVelocities();
				if (skateTriggered)
				{
					Skate();
				}
				skateTriggered = false;
				if (!V1)
				{
					AirControl();
				}
			}
			else if (V1 && !grounded && Input.GetButton("Jump"))
			{
				AirControl();
			}
			return;
		}
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
			obj = GetAxis("Horizontal") * 3f;
			transform.Rotate(new Vector3(0f, RuntimeServices.UnboxSingle(obj), 0f));
			if (!(rigidbody.velocity.magnitude <= maxSpeed))
			{
				obj2 = transform.InverseTransformDirection(rigidbody.velocity);
				obj2 = RuntimeServices.InvokeBinaryOperator("op_Multiply", Quaternion.AngleAxis(RuntimeServices.UnboxSingle(obj), transform.up), obj2);
				if (!grounded)
				{
					RuntimeServices.SetProperty(obj2, "x", RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(obj2, "x"), 1.01f));
				}
				else
				{
					RuntimeServices.SetProperty(obj2, "x", RuntimeServices.InvokeBinaryOperator("op_Division", UnityRuntimeServices.GetProperty(obj2, "x"), 2));
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
			if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= speedStep * 2f))
			{
				if ((transform.InverseTransformDirection(rigidbody.velocity).z > 0.02f || !(GetAxis("Vertical") < -0.8f)) && grounded)
				{
					num = GetAxis("Vertical") * 2f;
					vector = transform.InverseTransformDirection(rigidbody.velocity);
					vector.z += num;
					vector.z -= 1f;
					vector.z = Mathf.Clamp(vector.z, 0f, speedStep * 2f);
					rigidbody.velocity = transform.TransformDirection(vector);
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
			else if (!(GetAxis("Vertical") >= -0.01f))
			{
				obj3 = transform.InverseTransformDirection(rigidbody.velocity);
				obj4 = UnityRuntimeServices.GetProperty(obj3, "z");
				RuntimeServices.SetProperty(obj3, "z", UnityRuntimeServices.Invoke(typeof(Mathf), "Clamp", new object[3]
				{
					RuntimeServices.InvokeBinaryOperator("op_Addition", RuntimeServices.InvokeBinaryOperator("op_Multiply", UnityRuntimeServices.GetProperty(obj3, "z"), 0.97f), RuntimeServices.InvokeBinaryOperator("op_Multiply", RuntimeServices.InvokeBinaryOperator("op_Multiply", UnityRuntimeServices.GetProperty(obj3, "z"), 0.03f), GetAxis("Vertical"))),
					RuntimeServices.InvokeUnaryOperator("op_UnaryNegation", obj4),
					obj4
				}, typeof(MonoBehaviour)));
				rigidbody.velocity = transform.TransformDirection((Vector3)obj3);
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
			num = ((!wallRideRight) ? Mathf.Clamp(num, 0f, 1f) : Mathf.Clamp(num, -1f, 0f));
		}
		else
		{
			WallRideCheck();
		}
		if (!V1)
		{
			return;
		}
		rigidbody.velocity += transform.TransformDirection(new Vector3(num * airMovement, 0f, 0f));
		Vector3 direction = transform.InverseTransformDirection(rigidbody.velocity);
		direction.x = Mathf.Clamp(direction.x, -20f - transform.InverseTransformDirection(rigidbody.velocity).z / 20f, 20f + transform.InverseTransformDirection(rigidbody.velocity).z / 20f);
		rigidbody.velocity = transform.TransformDirection(direction);
		if (!(transform.InverseTransformDirection(rigidbody.velocity).z >= speedStep * 2f))
		{
			if ((transform.InverseTransformDirection(rigidbody.velocity).z > 0.02f || !(GetAxis("Vertical") < -0.8f)) && (grounded || !Physics.Raycast(transform.position - transform.forward / 20f, transform.forward, 3f)))
			{
				float num2 = GetAxis("Vertical") * 2f;
				Vector3 direction2 = transform.InverseTransformDirection(rigidbody.velocity);
				direction2.z += num2;
				direction2.z -= 1f;
				direction2.z = Mathf.Clamp(direction2.z, 0f, speedStep * 2f);
				rigidbody.velocity = transform.TransformDirection(direction2);
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
		if (!(GetAxis("Vertical") >= -0.01f))
		{
			direction = transform.InverseTransformDirection(rigidbody.velocity);
			float z = direction.z;
			direction.z = Mathf.Clamp(direction.z * 0.97f + direction.z * 0.03f * GetAxis("Vertical"), 0f - z, z);
			rigidbody.velocity = transform.TransformDirection(direction);
		}
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
		if (!canWallRide || rigidbody.velocity.y <= -4.5f || (!V1 && (!Input.GetButton("Jump") || grounded || jumpGraceCounter <= 0.1f || transform.InverseTransformDirection(rigidbody.velocity).z <= 19f)))
		{
			return;
		}
		RaycastHit hitInfo = default(RaycastHit);
		RaycastHit hitInfo2 = default(RaycastHit);
		bool flag = false;
		if (!V1)
		{
			if (Physics.Raycast(transform.position + new Vector3(0f, 1.8f, 0f), mainCam.transform.TransformDirection(1f, 0f, 0f), out hitInfo2, 1.5f))
			{
				hitInfo = hitInfo2;
				flag = true;
			}
			if (Physics.Raycast(transform.position + new Vector3(0f, 1.8f, 0f), mainCam.transform.TransformDirection(-1f, 0f, 0f), out hitInfo2, 1.5f))
			{
				hitInfo = hitInfo2;
				flag = true;
			}
		}
		else if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(GetAxis("Horizontal"), 0f, 0f)), out hitInfo, Mathf.Abs(GetAxis("Horizontal") * 1.5f)))
		{
			flag = true;
		}
		if (flag && hitInfo.transform.name != "Terrain")
		{
			wallNormal = hitInfo.normal * -1f;
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
				if (hitInfo.transform.name != "Terrain")
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
					wallRiding = false;
					airControl = false;
					airRelease = false;
					jumpGraceCounter = 0f;
				}
			}
			else
			{
				wallRiding = false;
				airControl = false;
				airRelease = false;
				jumpGraceCounter = 0f;
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
			else if (!onMoon)
			{
				overSpeed += skateForce - (currentStep - z);
			}
			else
			{
				overSpeed = 0f;
			}
		}
		speedAtOverStep = transform.InverseTransformDirection(rigidbody.velocity).z;
		xinputObj.SendMessage("Skate");
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
		if (grounded || (!(jumpGraceCounter >= 0.13f) && !jumpeded))
		{
			justJumped = true;
			jumpeded = true;
			jumpGraceCounter += 2f;
			if (!onMoon)
			{
				rigidbody.velocity += Vector3.up * jumpPower;
			}
			else
			{
				jumpShit = jumpPower;
				grounded = false;
			}
		}
		ExitZeroGravity();
	}

	public virtual void WallJump()
	{
		justJumped = true;
		wallRiding = false;
		wallJumpGraceCounter = 0f;
		jumpGraceCounter += 2f;
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
		if (onMoon)
		{
			JumpDecay();
		}
		RaycastHit hitInfo = default(RaycastHit);
		if (!onMoon)
		{
			float num = groundRayLength + 0.3f;
			if (Physics.Raycast(transform.position + transform.up * 2f, -transform.up, out hitInfo, 2f + num) && !(Mathf.Abs(Vector3.Angle(Vector3.up, hitInfo.normal)) >= (float)groundSlopeLimit) && (Mathf.Abs(Vector3.Angle(transform.up, hitInfo.normal)) < (float)(groundSlopeLimit - 30) || hitInfo.collider.tag == "Terrain"))
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
						transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
					}
					if (grounded)
					{
						LeftGround();
					}
				}
			}
		}
		else if (Physics.Raycast(transform.position + transform.up, -transform.up, out hitInfo, 150f))
		{
			Vector3 forward2 = Vector3.Cross(transform.right, hitInfo.normal);
			transform.localRotation = Quaternion.LookRotation(forward2, hitInfo.normal);
			transform.rigidbody.velocity = transform.forward * transform.rigidbody.velocity.magnitude;
			if (!(Vector3.Distance(transform.position, hitInfo.point) >= 1f))
			{
				groundCounter = 0f;
				transform.position = hitInfo.point;
				Landed();
			}
			else if (jumpeded)
			{
				transform.position -= transform.up * moonPull;
				moonPull += baseMoonPull;
			}
			else
			{
				transform.position = hitInfo.point;
			}
		}
		else if (!(moonTime <= 1f))
		{
			LeaveMoon();
		}
	}

	public virtual void Landed()
	{
		airControl = false;
		airRelease = false;
		jumpeded = false;
		if (changeInY > 4f || (onMoon && !grounded))
		{
			mainCam.Find("Main Camera").animation.Play();
			xinputObj.SendMessage("LandVibrate");
		}
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
		else if (flag)
		{
			bool flag2 = false;
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Linecast(transform.position + new Vector3(0f, 5f, 0f), transform.position + new Vector3(0f, -10f, 0f), out hitInfo) && (hitInfo.collider.name == "Terrain" || hitInfo.collider.tag == "Terrain"))
			{
				flag2 = true;
				if (!RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "skateSand"))
				{
					UnityRuntimeServices.Invoke(soundManager, "PlaySkateSand", new object[0], typeof(MonoBehaviour));
				}
			}
			if (!flag2 && !RuntimeServices.EqualityOperator(UnityRuntimeServices.GetProperty(soundManager, "currentSound"), "skate"))
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
			model.animation["Right Grind"].speed = 1f;
			model.animation["Left Grind"].speed = 1f;
			if (wallRideRight)
			{
				lastAnim = "Right Grind";
				animName = lastAnim;
				model.animation.CrossFade("Right Grind");
			}
			else
			{
				lastAnim = "Left Grind";
				animName = lastAnim;
				model.animation.CrossFade("Left Grind");
			}
		}
		else if (isGrinding)
		{
			isSkating = false;
			model.animation["RailGrind"].speed = 1f;
			lastAnim = "RailGrind";
			animName = lastAnim;
			model.animation.CrossFade("RailGrind");
		}
		else if (!grounded)
		{
			float num = 150f;
			isSkating = false;
			if (jumpPressed)
			{
				model.animation["Flail"].speed = 2f;
				model.animation["JumpInit"].speed = 2f;
				jumpPressed = false;
				lastAnim = "JumpInit";
				if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= num))
				{
					model.animation.CrossFade("Flail");
					animName = "Flail";
				}
				else
				{
					model.animation.CrossFade("JumpInit");
					animName = "JumpInit";
				}
			}
			else if (!(transform.InverseTransformDirection(rigidbody.velocity).y <= 0f))
			{
				model.animation["Flail"].speed = 1f;
				model.animation["JumpAfter"].speed = 1f;
				lastAnim = "JumpAfter";
				if (!(transform.InverseTransformDirection(rigidbody.velocity).z <= num))
				{
					model.animation.CrossFade("Flail");
					animName = "Flail";
				}
				else
				{
					model.animation.CrossFade("JumpAfter");
					animName = "JumpAfter";
				}
			}
			else
			{
				model.animation["Fall"].speed = 0.5f;
				lastAnim = "Fall";
				animName = lastAnim;
				model.animation.CrossFade("Fall");
			}
		}
		else if (lastAnim == "Fall" && grounded)
		{
			model.animation["Landing"].speed = 1.5f;
			lastAnim = "Landing";
			animName = lastAnim;
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
			if (!(GetAxis("Vertical") >= -0.25f) && !(z <= num6))
			{
				lastAnim = "Stop";
				animName = lastAnim;
				model.animation["Stop"].speed = 1f;
				model.animation.CrossFade("Stop");
				return;
			}
			if (!(transitionCount <= (float)num4))
			{
				string text = "center";
				if (skatePushed && (lastAnim == "IdleR" || lastAnim == "IdleD" || lastAnim == "Skate" || lastAnim == "Landing"))
				{
					bool flag = false;
					IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						if (!(obj is AnimationState))
						{
							obj = RuntimeServices.Coerce(obj, typeof(AnimationState));
						}
						AnimationState animationState = (AnimationState)obj;
						if ((animationState.name == "DeepHoldRight" || animationState.name == "DeepHoldLeft" || animationState.name == "RightToIdleD" || animationState.name == "LeftToIdleD" || animationState.name == "Skate" || animationState.name == "IdleR" || animationState.name == "IdleD" || animationState.name == "IdleD2" || animationState.name == "Landing") && !(animationState.time < animationState.length))
						{
							flag = true;
						}
					}
					if (flag)
					{
						float speed = Mathf.Clamp(num2 / z, min, num2);
						model.animation["SkateLeftD"].speed = speed;
						model.animation["SkateRightD"].speed = speed;
						model.animation["RightToIdleD"].speed = speed;
						model.animation["LeftToIdleD"].speed = speed;
						model.animation["DeepHoldLeft"].speed = speed;
						model.animation["DeepHoldRight"].speed = speed;
						isSkating = true;
						lastAnim = "SkateD";
						if (skateLeft)
						{
							model.animation.CrossFade("SkateLeftD");
							animName = "SkateLeftD";
							skateLeft = false;
						}
						else
						{
							model.animation.CrossFade("SkateRightD");
							animName = "SkateRightD";
							skateLeft = true;
						}
					}
					skatePushed = false;
				}
				else if (isSkating && lastAnim != "IdleD" && lastAnim != "IdleR" && lastAnim != "Landing")
				{
					bool flag2 = false;
					model.animation["IdleD"].speed = 1f;
					IEnumerator enumerator2 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						if (!(obj2 is AnimationState))
						{
							obj2 = RuntimeServices.Coerce(obj2, typeof(AnimationState));
						}
						AnimationState animationState2 = (AnimationState)obj2;
						if ((animationState2.name == "SkateLeftD" || animationState2.name == "SkateRightD") && !(animationState2.time < animationState2.length))
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
							animName = "RightToIdleD";
						}
						else
						{
							model.animation.CrossFade("LeftToIdleD");
							animName = "LeftToIdleD";
						}
					}
				}
				else if (isSkating)
				{
					bool flag3 = false;
					IEnumerator enumerator3 = UnityRuntimeServices.GetEnumerator(model.animation);
					while (enumerator3.MoveNext())
					{
						object obj3 = enumerator3.Current;
						if (!(obj3 is AnimationState))
						{
							obj3 = RuntimeServices.Coerce(obj3, typeof(AnimationState));
						}
						AnimationState animationState3 = (AnimationState)obj3;
						if ((animationState3.name == "RightToIdleD" || animationState3.name == "LeftToIdleD" || animationState3.name == "DeepHoldLeft" || animationState3.name == "DeepHoldRight" || animationState3.name == "IdleD" || animationState3.name == "IdleD2") && !(animationState3.time < animationState3.length))
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
							animName = "DeepHoldLeft";
						}
						else if (!(axis <= num5))
						{
							model.animation.CrossFade("DeepHoldRight");
							animName = "DeepHoldRight";
						}
						else
						{
							model.animation.CrossFade("IdleD");
							animName = "IdleD";
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
						IEnumerator enumerator4 = UnityRuntimeServices.GetEnumerator(model.animation);
						while (enumerator4.MoveNext())
						{
							object obj4 = enumerator4.Current;
							if (!(obj4 is AnimationState))
							{
								obj4 = RuntimeServices.Coerce(obj4, typeof(AnimationState));
							}
							AnimationState animationState4 = (AnimationState)obj4;
							if (animationState4.name == "Landing" && !(animationState4.time < animationState4.length))
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
							animName = "DeepHoldLeft";
						}
						else if (!(axis <= num5))
						{
							model.animation.CrossFade("DeepHoldRight");
							animName = "DeepHoldRight";
						}
						else
						{
							model.animation.CrossFade("IdleD");
							animName = "IdleD";
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
						IEnumerator enumerator5 = UnityRuntimeServices.GetEnumerator(model.animation);
						while (enumerator5.MoveNext())
						{
							object obj5 = enumerator5.Current;
							if (!(obj5 is AnimationState))
							{
								obj5 = RuntimeServices.Coerce(obj5, typeof(AnimationState));
							}
							AnimationState animationState5 = (AnimationState)obj5;
							if (animationState5.name == "SkateLeftD" || animationState5.name == "SkateRightD" || animationState5.name == "RightToIdleD" || animationState5.name == "LeftToIdleD" || animationState5.name == "Landing")
							{
								if (!(animationState5.time < animationState5.length))
								{
									flag6 = true;
								}
							}
							else if (animationState5.name == "Skate")
							{
								if (!(animationState5.time < animationState5.length))
								{
									flag6 = true;
									animationState5.time -= animationState5.length;
									UnityRuntimeServices.Update(enumerator5, animationState5);
								}
								else if (!(animationState5.time >= animationState5.length * 0.75f))
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
					if (flag6)
					{
						skatePushed = false;
						float speed2 = Mathf.Clamp(z / num2, min, num2);
						model.animation["Skate"].speed = speed2;
						lastAnim = "Skate";
						animName = lastAnim;
						model.animation.CrossFade("Skate");
					}
					return;
				}
				flag5 = true;
				if (lastAnim == "Skate")
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
						if (animationState6.name == "Skate")
						{
							if (!(animationState6.time >= animationState6.length))
							{
								flag5 = false;
								continue;
							}
							animationState6.time -= animationState6.length;
							UnityRuntimeServices.Update(enumerator6, animationState6);
						}
					}
				}
				if (flag5)
				{
					lastAnim = "IdleD";
					model.animation.CrossFade("IdleD2");
					animName = "IdleD2";
				}
				return;
			}
			flag5 = false;
			if (lastAnim == "Landing")
			{
				AnimationState animationState7 = model.animation["Landing"];
				if (!(animationState7.time < animationState7.length))
				{
					flag5 = true;
				}
			}
			else
			{
				flag5 = true;
			}
			if (flag5)
			{
				isSkating = false;
				model.animation["Idle"].speed = 3f;
				lastAnim = "Idle";
				animName = "Idle";
				model.animation.CrossFade("Idle");
			}
		}
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Terrain" || collision.gameObject.name == "Terrain")
		{
			airTime = 0f;
		}
		else if (collision.gameObject.name.StartsWith("BOOST"))
		{
			float z = transform.InverseTransformDirection(rigidbody.velocity).z;
			z = Mathf.Max(100f - z, 30f);
			ForceDebugBoost(z);
		}
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (other.name.StartsWith("BOOST"))
		{
			float z = transform.InverseTransformDirection(rigidbody.velocity).z;
			z = Mathf.Max(100f - z, 30f);
			ForceDebugBoost(z);
		}
	}

	public virtual void Main()
	{
	}
}
