using UnityEngine;

public class SplineGrinding : MonoBehaviour
{
	private static SplineGrinding _instance;

	public Spline spline;

	public Transform playerRef;

	public float offSet;

	public WrapMode wrapMode = WrapMode.Once;

	public float passedTime;

	private float lookahead = 1f;

	public float maxVelocity = 1.85f;

	public float currentVelocity;

	private float minVelocity = -0.85f;

	private float gravity = 0.0005f;

	private float gravityVelocity;

	private float maxGravityVelocity = 0.055f;

	public bool isGrinding;

	public float grindDistance = 4f;

	public float snapDistance = 1f;

	public float constantIncrease = 0.001f;

	private float grindY;

	private float speedOffset = 50f;

	private int itr = 4;

	private int slerp = 2;

	public bool forward = true;

	private float railSlope = 1f;

	public float grindTimer;

	public float maxGrindTimer = 10f;

	public float forwardBailSpeed = 3000f;

	public float jumpBuffer = 5f;

	private float currentBuffer;

	private float minBailBoost = 1000f;

	private float extraOff = 0.5f;

	public bool paused;

	private static int maxGracePeriod = 2;

	public int currentGracePeriod = maxGracePeriod;

	private GameObject[] railObjects;

	private HawkBehavior hawkBehavior;

	private GameObject grindPoint;

	public static SplineGrinding instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(SplineGrinding)) as SplineGrinding;
			}
			return _instance;
		}
	}

	private move move_ref
	{
		get
		{
			return PhoneInterface.player_move;
		}
	}

	private void Awake()
	{
		RefreshRails();
		hawkBehavior = PhoneInterface.hawk;
		grindPoint = GameObject.Find("GrindPoint");
	}

	public void RefreshRails()
	{
		railObjects = GameObject.FindGameObjectsWithTag("Rail");
	}

	private void CreateBoundingBox(GameObject obj)
	{
		Vector3 position = obj.GetComponent<Spline>().SplineNodeTransforms[0].position;
		float x = position.x;
		float x2 = position.x;
		float y = position.y;
		float y2 = position.y;
		float z = position.z;
		float z2 = position.z;
		Transform[] splineNodeTransforms = obj.GetComponent<Spline>().SplineNodeTransforms;
		Transform[] array = splineNodeTransforms;
		foreach (Transform transform in array)
		{
			Vector3 position2 = transform.position;
			if (position2.x < x)
			{
				x = position2.x;
			}
			else if (position2.x > x2)
			{
				x2 = position2.x;
			}
			if (position2.y < y)
			{
				y = position2.y;
			}
			else if (position2.y > y2)
			{
				y2 = position2.y;
			}
			if (position2.z < z)
			{
				z = position2.z;
			}
			else if (position2.z > z2)
			{
				z2 = position2.z;
			}
		}
		x -= grindDistance;
		x2 += grindDistance;
		y -= grindDistance;
		y2 += grindDistance;
		z -= grindDistance;
		z2 += grindDistance;
		Vector3 center = new Vector3(x + (x2 - x) / 2f, y + (y2 - y) / 2f, z + (z2 - z) / 2f);
		Vector3 size = new Vector3(x2 - x, y2 - y, z2 - z);
		Bounds bounds = new Bounds(center, size);
		BoundsHolder boundsHolder = obj.AddComponent<BoundsHolder>();
		boundsHolder.bounds = bounds;
		boundsHolder.pos = obj.transform.position;
	}

	private void FixedUpdate()
	{
		if (paused || hawkBehavior.targetHeld)
		{
			return;
		}
		if (isGrinding)
		{
			grind();
			checkRailEnd();
			XInput.GrindingVibrate();
		}
		else if (grindTimer <= 0f && !move_ref.wallRiding)
		{
			if (currentGracePeriod <= 0)
			{
				checkGrind();
			}
			else
			{
				currentGracePeriod--;
			}
		}
		else
		{
			grindTimer -= 1f;
		}
	}

	private void Update()
	{
		if (!paused && isGrinding)
		{
			if (currentBuffer > 0f)
			{
				currentBuffer -= 1f;
			}
			else if (currentBuffer <= 0f)
			{
				checkBail();
			}
		}
	}

	private void checkGrind()
	{
		Vector3 position = playerRef.transform.position;
		GameObject[] array = railObjects;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				continue;
			}
			BoundsHolder component = gameObject.GetComponent<BoundsHolder>();
			if (component == null || component.pos != gameObject.transform.position)
			{
				CreateBoundingBox(gameObject);
				component = gameObject.GetComponent<BoundsHolder>();
			}
			if (component.bounds.Contains(position))
			{
				Spline component2 = gameObject.GetComponent<Spline>();
				float pos = _GetClosestPoint(component2, position, itr);
				float num = Vector3.Distance(_GetPositionOnSpline(component2, pos), position);
				if (num < grindDistance)
				{
					spline = component2;
					checkGrindDirection();
					move_ref.isGrinding = true;
					move_ref.freezeControls = true;
					playerRef.rigidbody.isKinematic = true;
					isGrinding = true;
					float pos2 = (offSet = _GetClosestPoint(spline, position, itr));
					base.transform.position = _GetPositionOnSpline(spline, pos2);
					float num2 = 0f;
					float z = playerRef.transform.InverseTransformDirection(playerRef.rigidbody.velocity).z;
					float num3 = 0f - playerRef.transform.InverseTransformDirection(playerRef.rigidbody.velocity).y;
					num2 = ((!(z > num3)) ? num3 : z);
					currentVelocity = num2 / 50f;
					movePlayerToGrindPoint();
				}
			}
		}
	}

	private void checkGrindDirection()
	{
		float param = _GetClosestPoint(spline, playerRef.transform.position, itr);
		Vector3 tangentToSpline = spline.GetTangentToSpline(param);
		Vector3 from = tangentToSpline * -1f;
		Vector3 vector = playerRef.transform.forward;
		Vector3 to = vector;
		if (Vector3.Angle(tangentToSpline, to) > Vector3.Angle(from, to))
		{
			forward = false;
		}
		else
		{
			forward = true;
		}
	}

	private void checkRailEnd()
	{
		float num = 1f / spline.Length;
		float num2 = 1f - num;
		float num3 = 0f + num;
		Vector3 vector = playerRef.transform.forward;
		float num4 = currentVelocity * forwardBailSpeed;
		if (num4 < minBailBoost)
		{
			num4 = minBailBoost;
		}
		float num5 = (num5 = _GetClosestPoint(spline, base.transform.position, itr));
		if (forward)
		{
			if (num5 >= num2)
			{
				if (!checkSnap())
				{
					bail();
					playerRef.transform.position += vector * (grindDistance + extraOff);
					playerRef.rigidbody.AddForce(vector * num4);
					currentVelocity = 0f;
				}
			}
			else if (num5 <= num3 && currentVelocity <= minVelocity && !checkSnap())
			{
				bail();
				playerRef.transform.position -= vector * (grindDistance + extraOff);
				playerRef.rigidbody.AddForce(-vector * num4);
				currentVelocity = 0f;
			}
		}
		else if (num5 <= num3)
		{
			if (!checkSnap())
			{
				bail();
				playerRef.transform.position += vector * (grindDistance + extraOff);
				playerRef.rigidbody.AddForce(vector * num4);
				currentVelocity = 0f;
			}
		}
		else if (num5 >= num2 && currentVelocity <= minVelocity && !checkSnap())
		{
			bail();
			playerRef.transform.position -= vector * (grindDistance + extraOff);
			playerRef.rigidbody.AddForce(-vector * num4);
			currentVelocity = 0f;
		}
	}

	private bool checkSnap()
	{
		Vector3 position = playerRef.transform.position;
		GameObject[] array = railObjects;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != spline.gameObject))
			{
				continue;
			}
			float num;
			if (forward)
			{
				num = Vector3.Distance(gameObject.GetComponent<Spline>().transform.GetChild(0).position, spline.transform.GetChild(spline.transform.childCount - 1).position);
				float num2 = Vector3.Distance(gameObject.GetComponent<Spline>().transform.GetChild(gameObject.GetComponent<Spline>().transform.childCount - 1).position, spline.transform.GetChild(spline.transform.childCount - 1).position);
				if (num2 < num)
				{
					num = num2;
				}
			}
			else
			{
				num = Vector3.Distance(gameObject.GetComponent<Spline>().transform.GetChild(gameObject.GetComponent<Spline>().transform.childCount - 1).position, spline.transform.GetChild(0).position);
				float num2 = Vector3.Distance(gameObject.GetComponent<Spline>().transform.GetChild(0).position, spline.transform.GetChild(0).position);
				if (num2 < num)
				{
					num = num2;
				}
			}
			if (num < snapDistance)
			{
				passedTime = 0f;
				offSet = 0f;
				spline = gameObject.GetComponent<Spline>();
				checkGrindDirection();
				float pos = (offSet = _GetClosestPoint(spline, position, itr));
				base.transform.position = _GetPositionOnSpline(spline, pos);
				movePlayerToGrindPoint();
				return true;
			}
		}
		return false;
	}

	private void checkBail()
	{
		Vector3 up = playerRef.transform.up;
		Vector3 vector = playerRef.transform.forward;
		float num = currentVelocity * forwardBailSpeed;
		if (spline.gameObject.name == "Super")
		{
			num = currentVelocity * 2500f;
		}
		if (num < minBailBoost)
		{
			num = minBailBoost;
		}
		if (Input.GetButtonDown("Jump"))
		{
			bail();
			currentGracePeriod = maxGracePeriod;
			move_ref.justJumped = true;
			playerRef.transform.position += up * snapDistance * 1.1f;
			if ((double)railSlope > 0.15)
			{
				playerRef.rigidbody.velocity += up * move_ref.jumpPower / railSlope / 5f;
			}
			else
			{
				playerRef.rigidbody.velocity += up * move_ref.jumpPower;
			}
			move_ref.jumpPressed = true;
			playerRef.rigidbody.AddForce(vector * num);
			currentVelocity = 0f;
		}
	}

	public void bail()
	{
		grindTimer = maxGrindTimer;
		move_ref.isGrinding = false;
		move_ref.freezeControls = false;
		playerRef.rigidbody.isKinematic = false;
		isGrinding = false;
		passedTime = 0f;
		offSet = 0f;
		base.transform.rotation *= Quaternion.Euler(new Vector3(0f - base.transform.rotation.eulerAngles.x, 0f, 0f - base.transform.rotation.eulerAngles.z));
		playerRef.transform.parent = null;
	}

	private void grind()
	{
		float num = _GetClosestPoint(spline, playerRef.transform.position, itr);
		float num2 = lookahead / spline.Length;
		Vector3 vector = _GetPositionOnSpline(spline, num);
		if (num + num2 >= 1f)
		{
			num = 1f - num2;
			num -= 0.001f;
		}
		else if (num - num2 <= 0f)
		{
			num = num2 + 0.001f;
		}
		Vector3 vector2 = _GetPositionOnSpline(spline, num + num2);
		if (!forward)
		{
			vector2 = _GetPositionOnSpline(spline, num - num2);
		}
		Vector3 vector3 = new Vector3(1f, vector2.y - vector.y, 1f);
		railSlope = vector2.y - vector.y;
		currentVelocity += constantIncrease;
		if (spline.gameObject.name == "Super")
		{
			currentVelocity += constantIncrease * 5.7f;
		}
		if (currentVelocity >= maxVelocity)
		{
			currentVelocity = maxVelocity;
		}
		else if (currentVelocity <= minVelocity)
		{
			currentVelocity = minVelocity;
		}
		float num3 = Time.deltaTime * currentVelocity;
		if (forward)
		{
			passedTime += num3;
		}
		else
		{
			passedTime -= num3;
		}
		float num4 = passedTime;
		num4 /= spline.Length;
		num4 *= speedOffset;
		num4 += offSet;
		base.transform.position = _GetPositionOnSpline(spline, WrapValue(num4, 1E-05f, 0.99999f, wrapMode));
		if (forward)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, spline.GetOrientationOnSpline(WrapValue(num4, 1E-05f, 0.99999f, wrapMode)), Time.time * (float)slerp);
		}
		else
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, spline.GetOrientationOnSpline(WrapValue(num4, 1E-05f, 0.99999f, wrapMode)) * Quaternion.Euler(0f, 180f, 0f), Time.time * (float)slerp);
		}
	}

	public void movePlayerToGrindPoint()
	{
		currentBuffer = jumpBuffer;
		playerRef.transform.parent = grindPoint.transform;
		playerRef.transform.localPosition = new Vector3(0f, grindY, 0f);
		playerRef.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
		if (forward)
		{
			base.transform.rotation = spline.GetOrientationOnSpline(_GetClosestPoint(spline, playerRef.transform.position, itr));
		}
		else
		{
			base.transform.rotation = spline.GetOrientationOnSpline(_GetClosestPoint(spline, playerRef.transform.position, itr)) * Quaternion.Euler(0f, 180f, 0f);
		}
	}

	private float WrapValue(float v, float start, float end, WrapMode wMode)
	{
		switch (wMode)
		{
		case WrapMode.Once:
		case WrapMode.ClampForever:
			return Mathf.Clamp(v, start, end);
		case WrapMode.Default:
		case WrapMode.Loop:
			return Mathf.Repeat(v, end - start) + start;
		case WrapMode.PingPong:
			return Mathf.PingPong(v, end - start) + start;
		default:
			return v;
		}
	}

	private Vector3 _GetPositionOnSpline(Spline target, float pos)
	{
		if (pos <= 0f)
		{
			pos = 1E-05f;
		}
		else if (pos >= 1f)
		{
			pos = 0.99999f;
		}
		return target.GetPositionOnSpline(pos);
	}

	private float _GetClosestPoint(Spline target, Vector3 pos, int itr)
	{
		float num = target.GetClosestPoint(pos, itr);
		if (num <= 0f)
		{
			num = 1E-05f;
		}
		else if (num >= 1f)
		{
			num = 0.99999f;
		}
		return num;
	}
}
