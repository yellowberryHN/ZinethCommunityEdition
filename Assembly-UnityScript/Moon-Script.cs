using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Moon_Script : MonoBehaviour
{
	private Transform player;

	private new Camera camera;

	private bool onMoon;

	private Transform highMoon;

	private Transform lowMoon;

	public Transform[] earth;

	public Transform[] space;

	private bool inMoonArea;
	
	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		highMoon = transform.Find("Moon High");
		lowMoon = transform.Find("Moon Low");
		for (int i = 0; i < space.Length; i++)
		{
			TurnOff(space[i]);
		}
		StartCoroutine("Check");
	}

	public IEnumerator Check()
	{
		while (true)
		{
			float distance = Vector3.Distance(transform.position, player.position);

			if (!(distance >= 40000f))
			{
				highMoon.gameObject.active = true;
				lowMoon.renderer.enabled = false;
			}
			else
			{
				highMoon.gameObject.active = false;
				lowMoon.renderer.enabled = true;
			}
			
			if (!(distance >= 65000f))
			{
				Color backgroundColor = Camera.mainCamera.backgroundColor;
				backgroundColor.r = ((distance - 6060f) / 746f * 1.61f + 17f) / 255f;
				backgroundColor.g = ((distance - 6060f) / 746f * 2.71f + 17f) / 255f;
				backgroundColor.b = ((distance - 6060f) / 746f * 1f + 17f) / 255f;

				Camera.main.backgroundColor = backgroundColor;

				Camera.mainCamera.GetComponent<AudioLowPassFilter>().cutoffFrequency = (distance - 6060f) / 746f * 59.82f + 274f;
				Camera.mainCamera.GetComponent<Vignetting>().intensity = (distance - 6060f) / 746f * 0.05797f;

				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	// TODO: wrong
	public IEnumerator Leave()
	{
		while (true)
		{
			if (!inMoonArea)
			{
				inMoonArea = true;
				GameObject stars = GameObject.Find("Stars");

				stars.GetComponent<ParticleEmitter>().emit = true;
				stars.GetComponent<Renderer>().enabled = true;

				Camera.mainCamera.GetComponent<BloomAndLensFlares>().enabled = true;
				Camera.mainCamera.GetComponent<Vignetting>().enabled = true;
				Camera.mainCamera.GetComponent<AudioLowPassFilter>().enabled = true;

				foreach (var earthObject in earth)
				{
					TurnOff(earthObject);
				}

				foreach (var spaceObject in space)
				{
					TurnOn(spaceObject);
				}
				
				yield return new WaitForSeconds(0.5f);
				
				GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().ClearSpawns();
			}
			else
			{
				yield break;
			}
		}
	}

	public IEnumerator Enter()
	{
		while (true)
		{
			if (inMoonArea)
			{
				GameObject.Find("Stars").GetComponent<ParticleEmitter>().emit = false;
				GameObject.Find("Stars").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Particle System").GetComponent<ParticleSystem>().enableEmission = true;
				
				yield return new WaitForSeconds(0.5f);
				
				GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().ClearSpawns();
			}
			else
			{
				yield break;
			}
		}
	}

	public IEnumerator Exit()
	{
		while (true)
		{
			MonoBehaviour.print("sup");
			if (inMoonArea)
			{
				foreach (var earthObject in earth)
				{
					TurnOn(earthObject);
				}

				foreach (var spaceObject in space)
				{
					TurnOff(spaceObject);
				}

				GameObject particleSystem = GameObject.Find("Particle System");
				particleSystem.GetComponent<ParticleSystem>().enableEmission = false;

				Camera.mainCamera.GetComponent<BloomAndLensFlares>().enabled = false;
				Camera.mainCamera.GetComponent<Vignetting>().enabled = false;
				Camera.mainCamera.GetComponent<AudioLowPassFilter>().enabled = false;

				inMoonArea = false;
				
				yield return new WaitForSeconds(0.5f);

				GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().ClearSpawns();
			}
			else
			{
				yield break;
			}
		}
	}

	public virtual void Land()
	{
		onMoon = true;
		player.GetComponent<move>().LandOnMoon();
		
		// TODO: make this configurable, might not want to stop on moon
		if(SpeedrunTimer.instance != null) SpeedrunTimer.instance.StopTimer();
	}

	public virtual void OnCollisionEnter(Collision obj)
	{
		if (!onMoon)
		{
			Land();
		}
	}

	public virtual void TurnOff(Transform obj)
	{
		foreach (Transform childTransform in obj)
		{
			TurnOff(childTransform);
		}

		obj.gameObject.active = false;
	}

	public virtual void TurnOn(Transform obj)
	{
		foreach (Transform childTransform in obj)
		{
			TurnOn(childTransform);
		}
		
		obj.gameObject.active = true;
	}

	public virtual void Main()
	{
	}
}
