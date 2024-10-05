using System;
using System.Collections;
using UnityEngine;

// TODO: probably unused, matches expected behavior for old tutorial

[Serializable]
public class SpeedCheck : MonoBehaviour
{
	private Transform player;

	public float speed;

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		StartCoroutine("Check");
	}

	public virtual IEnumerator Check()
	{
		while (true)
		{
			if (player.GetComponent<move>().freezeControls)
			{
				ReachedSpeed();
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				yield break;
			}
		}
	}

	public virtual void ReachedSpeed()
	{
		GameObject.Find("Door One Trigger").GetComponent<DoorScript>().Open2();
		transform.GetComponent<DoCommandTrigger>().Activate();
	}
}
