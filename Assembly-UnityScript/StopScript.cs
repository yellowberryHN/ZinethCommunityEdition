using System;
using UnityEngine;
using UnityScript.Lang;

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
		UnityRuntimeServices.Invoke(GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>(), "ClearSpawns", new object[0], typeof(MonoBehaviour));
		UnityRuntimeServices.Invoke(player.GetComponent<move>(), "Stop", new object[0], typeof(MonoBehaviour));
	}
}
