using UnityEngine;

public class PhoneElement : MonoBehaviour
{
	public Vector3 wantedpos;

	public Quaternion wantedrot;

	public Vector3 wantedscale;

	public bool animateOnLoad;

	public Vector3 velocity = Vector3.zero;

	public float animateRate = 5f;

	public bool changeScale;

	private Transform _transform;

	public new Transform transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = GetComponent<Transform>();
			}
			return _transform;
		}
	}

	public static bool _use_fixed_update
	{
		get
		{
			return PhoneController._use_fixed_update;
		}
	}

	public static float deltatime
	{
		get
		{
			return PhoneController.deltatime;
		}
	}

	private void Awake()
	{
		Init();
	}

	public virtual void Init()
	{
		wantedpos = transform.localPosition;
		wantedrot = transform.localRotation;
		wantedscale = transform.localScale;
	}

	public virtual void OnLoad()
	{
		if (animateOnLoad)
		{
			PressPos();
		}
	}

	public virtual void PressPos()
	{
		transform.position = PhoneController.presspos + (transform.position - GetCenter());
	}

	public virtual void RandomPos()
	{
		Vector3 localPosition = transform.localPosition;
		float num = Random.Range(1f, 2f);
		if (Random.Range(-1f, 1f) > 0f)
		{
			num *= -1f;
		}
		localPosition.x += num;
		num = Random.Range(1f, 2f);
		if (Random.Range(-1f, 1f) > 0f)
		{
			num *= -1f;
		}
		localPosition.z += num;
		transform.localPosition = localPosition;
	}

	public virtual void OnUpdate()
	{
		if (animateOnLoad)
		{
			MovetoWanted();
		}
		if (changeScale)
		{
			ChangeScale();
		}
		transform.position += velocity * deltatime;
	}

	public virtual void ChangeScale()
	{
		if (wantedscale != transform.localScale)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, wantedscale, Time.deltaTime * animateRate * 2f);
		}
	}

	public virtual void MovetoWanted()
	{
		if (transform.localPosition != wantedpos)
		{
			if (Vector3.Distance(transform.localPosition, wantedpos) < 0.001f)
			{
				transform.localPosition = wantedpos;
			}
			else
			{
				transform.localPosition = Vector3.Lerp(transform.localPosition, wantedpos, deltatime * animateRate);
			}
		}
		if (transform.localRotation != wantedrot)
		{
			if (Quaternion.Angle(transform.localRotation, wantedrot) < 0.001f)
			{
				transform.localRotation = wantedrot;
			}
			else
			{
				transform.localRotation = Quaternion.Slerp(transform.localRotation, wantedrot, deltatime * animateRate);
			}
		}
	}

	public virtual Vector3 GetCenter()
	{
		TextMesh component = GetComponent<TextMesh>();
		if ((bool)component)
		{
			return component.renderer.bounds.center;
		}
		return transform.position;
	}
}
