using UnityEngine;

public class BigHawkBehavior : MonoBehaviour
{
	public Transform playerRef;

	public Transform cameraRef;

	public Transform dropLocation;

	public Transform carryPoint;

	public AudioClip screech;

	public Transform fly;

	public Transform carry;

	public bool inBounds;

	private float inBound;

	public bool targetEngaged;

	public bool targetHeld;

	public bool flyAway;

	public bool facingNest;

	public bool midStruggle;

	public bool hasSwoopedIn;

	public bool canControl;

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

	private float dropDistance = 300f;

	private float flyAwayMax = 3f;

	private float flyAwayCurrent;

	private float originalDamping;

	private float nestAngle = 0.1f;

	private Vector3 flightVector = new Vector3(0f, 0f, 0f);

	private void FixedUpdate()
	{
		if (!active || GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().isRespawning)
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
		Fly();
		base.transform.position = playerRef.position + new Vector3(0f, startSwoopDistance, 0f);
		base.transform.rotation = Quaternion.Euler(0f, playerRef.eulerAngles.y, 0f);
		startSwoopDistance -= startSwoopInc;
		if (carryPoint.position.y - playerRef.position.y <= distanceAbove)
		{
			startSwoopDistance = maxStartSwoopDistance;
			hasSwoopedIn = true;
			Screech();
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
			swoopDistance = distanceAbove;
			timeFollowed = 0f;
			targetEngaged = true;
			hasSwoopedIn = false;
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
		active = false;
		base.transform.Find("interiorHawk").gameObject.SetActiveRecursively(false);
	}

	private void SwoopDown()
	{
		base.transform.position = playerRef.position + new Vector3(0f, swoopDistance, 0f);
		base.transform.rotation = Quaternion.Euler(0f, playerRef.eulerAngles.y, 0f);
		swoopDistance -= swoopInc;
		if (swoopDistance <= 0f)
		{
			Fly();
			swoopDistance = distanceAbove;
			targetHeld = true;
			facingNest = false;
			playerRef.rotation = base.transform.rotation;
			playerRef.GetComponent<HawkBehavior>().isHeld = true;
			GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = false;
		}
	}

	private void CarryToNest()
	{
		if (takeOffTime > 0f)
		{
			takeOffTime -= Time.fixedDeltaTime;
			base.transform.rotation *= Quaternion.Euler(-50f * Time.fixedDeltaTime, 0f, 0f);
			base.transform.position += base.transform.forward * flightSpeed * Time.fixedDeltaTime;
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
			return;
		}
		float num = flightSpeed;
		if (!facingNest)
		{
			carryPoint.transform.LookAt(dropLocation);
			Quaternion rotation = carryPoint.transform.rotation;
			if (Vector3.Distance(base.transform.position, dropLocation.position) < 10000f)
			{
				num *= Vector3.Distance(base.transform.position, dropLocation.position) / 10000f;
				num = Mathf.Clamp(num, 2000f, flightSpeed);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, rotateAngle * Time.fixedDeltaTime);
			base.transform.position += base.transform.forward * num * Time.fixedDeltaTime;
			playerRef.position = carryPoint.position;
			playerRef.rotation = base.transform.rotation;
			playerRef.position += new Vector3(offsetX, offsetY, offsetZ);
			if (Quaternion.Angle(base.transform.rotation, rotation) <= nestAngle)
			{
				base.transform.rotation = rotation;
				playerRef.rotation = base.transform.rotation;
				facingNest = true;
			}
			if (Vector3.Distance(base.transform.position, dropLocation.position) <= dropDistance)
			{
				Drop();
			}
		}
		else
		{
			Vector3 position = base.transform.position;
			if (Vector3.Distance(base.transform.position, dropLocation.position) < 10000f)
			{
				num *= Vector3.Distance(base.transform.position, dropLocation.position) / 10000f;
				num = Mathf.Clamp(num, 2000f, flightSpeed);
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, dropLocation.position, num * Time.fixedDeltaTime);
			flightVector = base.transform.position - position;
			playerRef.transform.position = carryPoint.position;
			playerRef.position += new Vector3(offsetX, offsetY, offsetZ);
			if (Vector3.Distance(base.transform.position, dropLocation.position) <= dropDistance)
			{
				Drop();
			}
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

	private void Drop()
	{
		MonoBehaviour.print("hey dogs");
		targetHeld = false;
		targetEngaged = false;
		facingNest = false;
		inBounds = false;
		flyAway = true;
		flyAwayCurrent = flyAwayMax;
		midStruggle = false;
		offsetX = 0f;
		offsetY = 0f;
		offsetZ = 0f;
		playerRef.GetComponent<HawkBehavior>().isHeld = false;
		Screech();
		MonoBehaviour.print("hey dogs");
	}

	private void FlyAway()
	{
		if (flyAwayCurrent > 0f)
		{
			flyAwayCurrent -= Time.fixedDeltaTime;
			base.transform.position += flightVector;
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
