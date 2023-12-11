using UnityEngine;

public class DoGrindParticles : MonoBehaviour
{
	public SplineGrinding grind_ref;

	public ParticleSystem grind_part_l;

	private void Start()
	{
		if (grind_ref == null)
		{
			grind_ref = Object.FindObjectOfType(typeof(SplineGrinding)) as SplineGrinding;
		}
		if (grind_part_l == null)
		{
			grind_part_l = base.particleSystem;
		}
	}

	private void Update()
	{
		if (grind_ref.isGrinding)
		{
			if ((bool)grind_part_l && !grind_part_l.enableEmission)
			{
				grind_part_l.enableEmission = true;
				grind_part_l.Play();
			}
		}
		else if ((bool)grind_part_l && grind_part_l.enableEmission)
		{
			grind_part_l.enableEmission = false;
			grind_part_l.Stop();
		}
	}
}
