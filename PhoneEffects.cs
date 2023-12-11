using UnityEngine;

public class PhoneEffects : MonoBehaviour
{
	private static PhoneEffects _instance;

	public Camera phonecam;

	public Vector3 camlocalpos;

	private float shakeamount;

	private static PhoneEffects instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneEffects)) as PhoneEffects;
			}
			return _instance;
		}
	}

	public static void AddCamShake(float amount)
	{
		instance.AddShake(amount);
	}

	private void Awake()
	{
		camlocalpos = phonecam.transform.localPosition;
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		DoShaking();
	}

	public void AddShake(float amount)
	{
		shakeamount += amount;
	}

	public void DoShaking()
	{
		if (!(shakeamount <= 0f))
		{
			float num = shakeamount * 0.2f;
			Vector3 vector = new Vector3(Random.Range(0f - num, num), 0f, Random.Range(0f - num, num));
			phonecam.transform.localPosition = camlocalpos + vector;
			shakeamount = Mathf.Lerp(shakeamount, 0f, Time.fixedDeltaTime * 3f);
			if (shakeamount <= 0f)
			{
				phonecam.transform.localPosition = camlocalpos;
			}
		}
	}
}
