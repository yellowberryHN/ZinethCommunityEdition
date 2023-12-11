using UnityEngine;

public class DoSandParticles : MonoBehaviour
{
	private Vector3 lastpos;

	public bool net_mode;

	public ParticleSystem part_sys;

	public float start_cutoff = 20f;

	public float stop_cutoff = 15f;

	public float downspeed_cutoff = 15f;

	public float min_height = 10f;

	private bool last_grounded;

	private float lastTime;

	private void Start()
	{
		lastpos = base.transform.position;
		if (!part_sys)
		{
			part_sys = base.gameObject.GetComponent<ParticleSystem>();
		}
	}

	private void FixedUpdate()
	{
		if (net_mode)
		{
			if (base.transform.position != lastpos)
			{
				DoUpdate(Time.fixedTime - lastTime);
				lastTime = Time.fixedTime;
			}
		}
		else
		{
			DoUpdate(Time.fixedDeltaTime);
		}
	}

	private void DoUpdate(float timelapse)
	{
		bool flag = false;
		bool flag2 = false;
		float num = (base.transform.position - lastpos).magnitude / timelapse;
		float f = (base.transform.position - lastpos).y / timelapse;
		if (part_sys.enableEmission)
		{
			if (num >= stop_cutoff || Mathf.Abs(f) >= downspeed_cutoff)
			{
				flag = true;
			}
		}
		else if (num >= start_cutoff || Mathf.Abs(f) >= downspeed_cutoff)
		{
			flag = true;
		}
		Vector3 origin = base.transform.parent.position + base.transform.parent.up * 10f;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, -base.transform.parent.up, out hitInfo, min_height + 10f) && (hitInfo.collider.name == "Terrain" || hitInfo.collider.tag == "Terrain"))
		{
			flag2 = true;
			base.transform.position = hitInfo.point;
		}
		if (flag && flag2)
		{
			EnableParticles();
		}
		else
		{
			DisableParticles();
		}
		lastpos = base.transform.position;
	}

	private void EnableParticles()
	{
		if (!part_sys.enableEmission)
		{
			part_sys.enableEmission = true;
			part_sys.Play();
		}
	}

	private void DisableParticles()
	{
		if (part_sys.enableEmission)
		{
			part_sys.enableEmission = false;
			part_sys.Stop();
		}
	}
}
