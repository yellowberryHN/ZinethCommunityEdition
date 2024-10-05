using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Rewind_Check : MonoBehaviour
{
	private Transform player;

	private bool dra;

	public bool check;

	public virtual void Start()
	{
		MonoBehaviour.print(this.GetType().ToString());
		player = GameObject.Find("Player").transform;
		GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = false;
	}

	public virtual void OnTriggerEnter(Collider obj)
	{
		if (!check)
		{
			GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = true;
			player.GetComponent<move>().canRewind = true;
			StartCoroutine("Check");
		}
		else if (dra)
		{
			GameObject.Find("Bridge").animation.Play();
		}
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
		GameObject.Find("Draw").GetComponent<Rewind_Check>().Draw();
		transform.GetComponent<DoCommandTrigger>().Activate();
		UnityEngine.Object.Destroy(gameObject);
	}

	public virtual void Draw()
	{
		dra = true;
	}
}
