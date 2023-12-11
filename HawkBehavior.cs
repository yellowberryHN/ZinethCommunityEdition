using UnityEngine;

public class HawkBehavior : MonoBehaviour
{
	public Transform cameraRef;

	public Transform dropLocation;

	public Transform carryPoint;

	public AudioClip screech;

	public Transform fly;

	public Transform carry;

	public Projector shadow;

	public bool inBounds;

	private float inBound;

	public bool targetEngaged;

	public bool targetHeld;

	public bool flyAway;

	public bool facingNest;

	public bool midStruggle;

	public bool hasSwoopedIn;

	public bool canControl;

	public bool isHeld;

	public new bool active;

	public float timeFollowed;

	public float maxTimeFollowed = 5f;

	public float flightSpeed = 800f;

	public float controlSpeed = 50f;

	public float rotateAngle = 2f;

	public float hawkDamping = 25f;

	public float swoopDistance = 30f;

	public float startSwoopDistance = 150f;

	public float struggleAmount = 0.5f;

	public float offsetX;

	public float offsetY;

	public float offsetZ;

	private float takeOffTime = 1f;

	private float maxTakeOffTime = 1f;

	private float distanceAbove = 15f;

	private float maxStartSwoopDistance = 150f;

	private float swoopInc = 0.25f;

	private float startSwoopInc = 1f;

	private float dropDistance = 20f;

	private float flyAwayMax = 3f;

	private float flyAwayCurrent;

	private float originalDamping;

	private float nestAngle = 0.1f;

	private float lastUpDown;

	private float lastLeftRight;

	private float lastSpin;

	private float incValue = 0.2f;

	private Vector3 flightVector = new Vector3(0f, 0f, 0f);

	private SpawnPointScript spawnPoint;

	private float maxY = 44000f;

	public NPCTrainer myTrainer;

	public GameObject tempHawkRend;

	public float spd;

	public Vector3 velocity = Vector3.zero;

	public float saveSpd;

	public Transform playerRef
	{
		get
		{
			return PhoneInterface.player_trans;
		}
	}

	private void Awake()
	{
		spawnPoint = Object.FindObjectOfType(typeof(SpawnPointScript)) as SpawnPointScript;
		if (!myTrainer)
		{
			myTrainer = GetComponentInChildren<NPCTrainer>();
		}
	}

	private void Start()
	{
		if (myTrainer != null && myTrainer.defeated)
		{
			canControl = true;
		}
		else
		{
			Invoke("CheckNPC", 1f);
		}
	}

	private void CheckNPC()
	{
		if (myTrainer != null && myTrainer.defeated)
		{
			canControl = true;
		}
	}

	private void FixedUpdate()
	{
		if (!active)
		{
			return;
		}
		if (!isHeld)
		{
			if (spawnPoint.isRespawning)
			{
				return;
			}
			if (!flyAway)
			{
				if (!targetHeld)
				{
					if (InBounds() && !targetEngaged)
					{
						if (!hasSwoopedIn)
						{
							SwoopIn();
						}
						else
						{
							FollowClosely();
						}
					}
					else if (!targetEngaged)
					{
						timeFollowed = 0f;
						FlyAway();
					}
					else
					{
						SwoopDown();
					}
				}
				else
				{
					CarryToNest();
					Struggle();
				}
			}
			else
			{
				FlyAway();
			}
		}
		else
		{
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
		}
	}

	private bool InBounds()
	{
		if (!inBounds && (double)inBound < 0.1)
		{
			inBound += Time.deltaTime;
			return true;
		}
		if (!inBounds && (double)inBound >= 0.1)
		{
			return false;
		}
		inBound = 0f;
		return true;
	}

	private void SwoopIn()
	{
		base.transform.Find("interiorHawk").gameObject.SetActiveRecursively(true);
		flyAwayCurrent = flyAwayMax;
		Fly();
		base.transform.position = playerRef.position + new Vector3(0f, startSwoopDistance, 0f);
		base.transform.rotation = Quaternion.Euler(0f, playerRef.eulerAngles.y, 0f);
		startSwoopDistance -= startSwoopInc;
		if (startSwoopDistance <= distanceAbove)
		{
			startSwoopDistance = maxStartSwoopDistance;
			hasSwoopedIn = true;
			Screech();
		}
		if ((bool)myTrainer)
		{
			myTrainer.gameObject.active = true;
		}
	}

	private void FollowClosely()
	{
		base.transform.position = playerRef.position + new Vector3(0f, distanceAbove, 0f);
		base.transform.rotation = Quaternion.Euler(0f, playerRef.eulerAngles.y, 0f);
		timeFollowed += Time.fixedDeltaTime;
		if (timeFollowed >= maxTimeFollowed)
		{
			Carry();
			timeFollowed = 0f;
			targetEngaged = true;
			hasSwoopedIn = false;
			swoopDistance = distanceAbove;
		}
	}

	private void Disappear()
	{
		Fly();
		base.transform.position = new Vector3(0f, -10000f, 0f);
		targetEngaged = false;
		targetHeld = false;
		flyAway = false;
		facingNest = false;
		midStruggle = false;
		hasSwoopedIn = false;
		timeFollowed = 0f;
		swoopDistance = distanceAbove;
		startSwoopDistance = maxStartSwoopDistance;
		takeOffTime = maxTakeOffTime;
		offsetX = 0f;
		offsetY = 0f;
		offsetZ = 0f;
		base.transform.Find("interiorHawk").gameObject.SetActiveRecursively(false);
		active = false;
	}

	private void SwoopDown()
	{
		myTrainer.can_challenge = true;
		base.transform.position = playerRef.position + new Vector3(0f, swoopDistance, 0f);
		base.transform.rotation = Quaternion.Euler(0f, playerRef.eulerAngles.y, 0f);
		swoopDistance -= swoopInc;
		if (carryPoint.position.y - playerRef.position.y <= 0f)
		{
			shadow.gameObject.active = false;
			Fly();
			swoopDistance = distanceAbove;
			targetHeld = true;
			facingNest = false;
			playerRef.rotation = base.transform.rotation;
			cameraRef.GetComponent<SoundManager>().StopSound();
			GameObject.Find("main_character").transform.animation.CrossFade("Idle");
			playerRef.GetComponent<move>().freezeControls = true;
			playerRef.GetComponent<Rigidbody>().isKinematic = true;
			originalDamping = cameraRef.GetComponent<NewCamera>().damping;
			cameraRef.GetComponent<NewCamera>().damping = hawkDamping;
			GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = false;
			GameObject.Find("HawkBig").GetComponent<BigHawkBehavior>().enabled = true;
		}
	}

	private void CarryToNest()
	{
		if (canControl)
		{
			ControlHawk();
			return;
		}
		if (takeOffTime > 0f)
		{
			takeOffTime -= Time.fixedDeltaTime;
			base.transform.rotation *= Quaternion.Euler(-50f * Time.fixedDeltaTime, 0f, 0f);
			base.transform.position += base.transform.forward * flightSpeed * Time.fixedDeltaTime;
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
			return;
		}
		if (!facingNest)
		{
			carryPoint.transform.LookAt(dropLocation);
			Quaternion rotation = carryPoint.transform.rotation;
			float num = rotateAngle;
			float num2 = Vector3.Distance(base.transform.position, dropLocation.position);
			if (num2 < dropDistance * 5f)
			{
				num *= 3f;
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, num * Time.fixedDeltaTime);
			float num3 = flightSpeed;
			if (num2 <= dropDistance * 7f && num2 > 0f)
			{
				num3 *= Mathf.Pow(num2 / (dropDistance * 7f), 2f);
			}
			base.transform.position += base.transform.forward * num3 * Time.fixedDeltaTime;
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
			playerRef.position += new Vector3(offsetX, offsetY, offsetZ);
			if (Quaternion.Angle(base.transform.rotation, rotation) <= nestAngle)
			{
				base.transform.rotation = rotation;
				playerRef.rotation = base.transform.rotation;
				facingNest = true;
			}
		}
		else
		{
			Vector3 position = base.transform.position;
			base.transform.position = Vector3.MoveTowards(base.transform.position, dropLocation.position, flightSpeed * Time.fixedDeltaTime);
			flightVector = base.transform.position - position;
			playerRef.transform.position = carryPoint.position;
			playerRef.position += new Vector3(offsetX, offsetY, offsetZ);
		}
		if (Vector3.Distance(base.transform.position, dropLocation.position) <= dropDistance)
		{
			base.transform.position = dropLocation.position;
			Drop();
		}
	}

	private void Struggle()
	{
		if (!midStruggle && !canControl)
		{
			if (Input.anyKeyDown)
			{
				midStruggle = true;
				offsetX = Random.Range(0f - struggleAmount, struggleAmount);
				offsetY = Random.Range(0f - struggleAmount, struggleAmount);
				offsetZ = Random.Range(0f - struggleAmount, struggleAmount);
			}
		}
		else
		{
			midStruggle = false;
			offsetX = 0f;
			offsetY = 0f;
			offsetZ = 0f;
		}
	}

	private void ControlHawk()
	{
		if (takeOffTime > 0f)
		{
			spd = Mathf.Lerp(spd, flightSpeed, Time.fixedDeltaTime * 2f);
			takeOffTime -= Time.fixedDeltaTime;
			base.transform.rotation *= Quaternion.Euler(-50f * Time.fixedDeltaTime, 0f, 0f);
			base.transform.position += base.transform.forward * spd * Time.fixedDeltaTime;
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
			return;
		}
		Vector3 position = base.transform.position;
		float axis = Input.GetAxis("Horizontal");
		axis = Mathf.Abs(Mathf.Pow(axis, 3f)) * Mathf.Sign(axis);
		float axis2 = Input.GetAxis("Vertical");
		axis2 = Mathf.Abs(Mathf.Pow(axis2, 2f)) * Mathf.Sign(axis2);
		if (axis != 0f)
		{
			if (lastLeftRight != 0f && Mathf.Sign(axis) != Mathf.Sign(lastLeftRight))
			{
				lastLeftRight = 0f;
			}
			if (axis > lastLeftRight)
			{
				lastLeftRight += incValue;
				axis = lastLeftRight;
			}
			else if (axis < lastLeftRight)
			{
				lastLeftRight -= incValue;
				axis = lastLeftRight;
			}
		}
		else
		{
			lastLeftRight = Mathf.Lerp(lastLeftRight, 0f, Time.fixedDeltaTime * 10f);
		}
		if (axis2 != 0f)
		{
			if (lastUpDown != 0f && Mathf.Sign(axis2) != Mathf.Sign(lastUpDown))
			{
				lastUpDown = 0f;
			}
			if (axis2 > lastUpDown)
			{
				lastUpDown += incValue;
				axis2 = lastUpDown;
			}
			else if (axis2 < lastUpDown)
			{
				lastUpDown -= incValue;
				axis2 = lastUpDown;
			}
		}
		else
		{
			lastUpDown = Mathf.Lerp(lastUpDown, 0f, Time.fixedDeltaTime * 10f);
		}
		float num = base.transform.rotation.eulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		num /= 180f;
		num = num * num * Mathf.Sign(num);
		if (num > 0f)
		{
			num *= 1.5f;
		}
		spd = Mathf.Clamp(spd + num * 8f, 0f, 25000f);
		float num2 = 0f;
		num2 = ((Application.platform != RuntimePlatform.OSXPlayer) ? Mathf.Abs(Input.GetAxis("Dive_PC")) : Input.GetAxis("Dive_OSX"));
		if (num2 <= 0.001f)
		{
			num2 = Input.GetAxis("Dive");
		}
		if (num2 > 0f)
		{
			spd = Mathf.Max(Mathf.Lerp(300f, 150f, num2), spd * (1f - 0.08f * num2));
		}
		else if (spd < 300f)
		{
			spd = Mathf.Lerp(spd, 300f, 0.35f);
		}
		float num3 = Mathf.Clamp(600f / spd, 0.75f, 1f);
		num3 = 1f;
		float num4 = 0f;
		if (Input.GetButton("Rewind"))
		{
			num4 += 1f;
		}
		if (Input.GetButton("UnRewind"))
		{
			num4 -= 1f;
		}
		num4 *= 3f;
		num4 -= axis;
		num4 = Mathf.Lerp(lastSpin, num4, Time.fixedDeltaTime * 30f);
		if (Mathf.Abs(num4) < 0.01f)
		{
			num4 = 0f;
		}
		lastSpin = num4;
		base.transform.rotation *= Quaternion.Euler(axis2 * 1f * num3 * controlSpeed * Time.fixedDeltaTime, axis * 0.9f * num3 * controlSpeed * Time.fixedDeltaTime, num4 * 1.1f * controlSpeed * Time.fixedDeltaTime);
		velocity = base.transform.forward * spd;
		Vector3 vector = base.transform.position + velocity * Time.fixedDeltaTime;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, vector + base.transform.forward * 3f, out hitInfo) && hitInfo.collider.name != "Player")
		{
			base.transform.position = hitInfo.point - base.transform.forward * 10f;
			playerRef.position = carryPoint.position;
			Drop();
			return;
		}
		if (Input.GetButton("Jump") && Input.GetButton("Skate"))
		{
			Drop();
			playerRef.rigidbody.velocity = Vector3.ClampMagnitude(velocity / 3f, 1000f);
			return;
		}
		base.transform.position = vector;
		if (axis == 0f && axis2 == 0f && num4 == 0f)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.rotation.eulerAngles.x, base.transform.rotation.eulerAngles.y, 0f), rotateAngle * Time.fixedDeltaTime);
		}
		if (base.transform.position.y > maxY)
		{
			Vector3 position2 = base.transform.position;
			position2.y = Mathf.Lerp(position2.y, maxY, Time.fixedDeltaTime * 1f);
			base.transform.position = position2;
			base.transform.rotation *= Quaternion.Euler(Random.Range(-0.5f, 0.5f) * controlSpeed * Time.fixedDeltaTime, Random.Range(-0.5f, 0.5f) * controlSpeed * Time.fixedDeltaTime, Random.Range(-0.5f, 0.5f) * controlSpeed * Time.fixedDeltaTime);
		}
		playerRef.position = carryPoint.position;
		playerRef.rotation = Quaternion.Slerp(playerRef.rotation, base.transform.rotation, Time.fixedDeltaTime * 8f);
		flightVector = base.transform.position - position;
	}

	public void Drop()
	{
		targetHeld = false;
		targetEngaged = false;
		facingNest = false;
		inBounds = false;
		GameObject gameObject = GameObject.Find("WindSource");
		if ((bool)gameObject)
		{
			gameObject.audio.volume = 0f;
		}
		flyAway = true;
		flyAwayCurrent = flyAwayMax;
		playerRef.GetComponent<move>().freezeControls = false;
		playerRef.GetComponent<Rigidbody>().isKinematic = false;
		midStruggle = false;
		offsetX = 0f;
		offsetY = 0f;
		offsetZ = 0f;
		cameraRef.GetComponent<NewCamera>().damping = originalDamping;
		GameObject.Find("HawkBig").GetComponent<BigHawkBehavior>().enabled = false;
		spawnPoint.canRespawn = true;
		spawnPoint.ClearSpawns();
		Screech();
		if (!myTrainer)
		{
			return;
		}
		if (PhoneMemory.trainer_challenge == myTrainer)
		{
			if (PhoneMemory.IsBattlingTrainer(myTrainer))
			{
				PhoneController.instance.LoadScreenQuiet(PhoneController.instance.startscreen);
			}
			myTrainer.UnChallenge();
		}
		myTrainer.gameObject.active = false;
		myTrainer.can_challenge = false;
	}

	private void FlyAway()
	{
		if (flyAwayCurrent > 0f)
		{
			flyAwayCurrent -= Time.fixedDeltaTime;
			base.transform.position += flightVector.normalized * 10f * Time.fixedDeltaTime;
		}
		else
		{
			flyAway = false;
			Disappear();
		}
	}

	private void Screech()
	{
		AudioSource.PlayClipAtPoint(screech, new Vector3(5f, 1f, 2f));
	}

	private void Fly()
	{
		carry.gameObject.active = false;
		fly.gameObject.active = true;
	}

	private void Carry()
	{
		fly.gameObject.active = false;
		carry.gameObject.active = true;
	}
}
