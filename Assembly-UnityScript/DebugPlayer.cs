using System;
using UnityEngine;

[Serializable]
public class DebugPlayer : MonoBehaviour
{
	private Transform player;

	private SplineGrinding rail;

	private SpawnPointScript spawn;

	private HawkBehavior hawk;

	public GUIText behindText;

	public virtual void Start()
	{
		new GameObject("Speedrunning").AddComponent<SpeedrunTimer>().Setup(guiText);

		player = GameObject.Find("Player").transform;
		rail = GameObject.Find("GrindPoint").GetComponent<SplineGrinding>();
		spawn = GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>();
		hawk = GameObject.Find("Hawk").GetComponent<HawkBehavior>();
		if (!behindText)
		{
			behindText = (GUIText)transform.GetComponentInChildren(typeof(GUIText));
		}
		behindText.material.color = Color.black;
		SetText(string.Empty);
	}

	public virtual void Update()
	{
		int num = 0;
		if (spawn.isRespawning)
		{
			num = (int)(player.InverseTransformDirection(spawn.GetCurrentVelocity()).z + spawn.GetRailVelocity() * 50);
		}
		else if (hawk != null && hawk.targetHeld)
		{
			num = (int)hawk.spd;
		}
		else
		{
			var vector = player.InverseTransformDirection(player.rigidbody.velocity);
			num = (int)(vector.z + rail.currentVelocity * 50f);
		}
		SetText(num.ToString("0000"));
	}

	public virtual void SetText(string text)
	{
		guiText.text = text;
		behindText.text = text;
	}
}
