using System;
using UnityEngine;

[Serializable]
public class StopScript : MonoBehaviour
{
	private Transform player;

	public string slidesToLoad;

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().ClearSpawns();
		player.GetComponent<move>().Stop();
	}
}
