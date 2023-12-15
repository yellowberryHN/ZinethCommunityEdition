using UnityEngine;

public class CactusBehavior : MonoBehaviour
{
	public static int cactusBreaks;

	public static int recentCactusBreaks;

	public float minForce = 10f;

	public float maxForce = 20f;

	public float torqueForce = 2f;

	private bool hasCollided;

	private int destroyTimer = 1000;

	private int offscreenTimer = 800;

	public AudioClip breakSound;

	public bool mirage;

	private float mirageTimer = 60f;

	public Renderer[] mirageRenderers;

	public float drag = 2f;

	public float angularDrag = 2f;

	public float mass = 0.001f;

	private void Awake()
	{
		float num = Random.Range(0f, 0.1f);
		base.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
		base.transform.localScale += new Vector3(num, num, num);
	}

	private void FixedUpdate()
	{
		if (hasCollided)
		{
			IncreaseGravity();
			destroyTimer--;
			if (destroyTimer <= 0)
			{
				DestroySelf();
			}
			return;
		}
		if (offscreenTimer <= 0)
		{
			DestroySelf();
		}
		if (!base.transform.GetChild(0).renderer.isVisible)
		{
			offscreenTimer--;
		}
		else
		{
			if (!mirage)
			{
				return;
			}
			base.transform.LookAt(Camera.main.transform.position);
			base.transform.Rotate(Vector3.up * -90f);
			float num = Vector3.Distance(base.transform.position, Camera.main.transform.position);
			float num2 = (num - 300f) / 1000f;
			if (num2 >= 0.5f)
			{
				num2 = 1f;
			}
			Renderer[] array = mirageRenderers;
			foreach (Renderer renderer in array)
			{
				float a = Mathf.Clamp01(num2 * Random.Range(0.9f, 1.1f));
				Color color = renderer.material.color;
				color.a = a;
				renderer.material.color = color;
			}
			if (num2 <= 0f)
			{
				mirageTimer -= 1f;
				if (mirageTimer <= 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!hasCollided && collider.name == "Player")
		{
			cactusBreaks++;
			recentCactusBreaks++;
			hasCollided = true;
			ApplyForces(collider);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!hasCollided && collision.collider.name == "Player")
		{
			hasCollided = true;
			ApplyForces(collision.collider);
		}
	}

	private void ApplyForces(Collider collider)
	{
		if ((bool)breakSound)
		{
			AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position);
		}
		if (mirage)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		XInput.AddVibrateForce(0.5f, 0.1f, 0.1f, false);
		foreach (Transform item in base.transform)
		{
			float num = Random.Range(minForce, maxForce);
			num /= 15f;
			item.gameObject.AddComponent<Rigidbody>();
			item.gameObject.AddComponent<CapsuleCollider>();
			Rigidbody component = item.gameObject.GetComponent<Rigidbody>();
			component.drag = drag;
			component.angularDrag = angularDrag;
			component.mass = mass;
			component.velocity = collider.transform.rigidbody.velocity;
			component.AddForce(collider.transform.up * num);
			component.AddForce(collider.transform.right * Random.Range(0f - maxForce, maxForce) / 15f);
			component.AddTorque(Random.Range(0, 360), Random.Range(0, 360), (float)Random.Range(0, 360) * torqueForce);
		}
	}

	private void IncreaseGravity()
	{
		foreach (Transform item in base.transform)
		{
			Rigidbody component = item.gameObject.GetComponent<Rigidbody>();
			component.AddForce(Physics.gravity * 8f * component.mass);
		}
	}

	private void DestroySelf()
	{
		Object.Destroy(base.transform.gameObject);
		CactusPlacer.instance.currentNum--;
	}

	private void LateUpdate()
	{
		recentCactusBreaks = 0;
	}
}
