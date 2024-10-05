using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HqMan : MonoBehaviour
{
	public bool on;

	public Transform signs;

	public Transform rails;

	public Transform capsules;

	public HqMan()
	{
		on = true;
	}

	public virtual void Start()
	{
		TurnOff(signs);
		TurnOff(capsules);
	}

	public virtual void TurnOff(Transform obj)
	{
		obj.gameObject.SetActiveRecursively(false);
	}

	public virtual void TurnOn(Transform obj)
	{
		obj.gameObject.SetActiveRecursively(true);
	}

	public virtual void OnTriggerExit()
	{
		TurnOff(signs);
		TurnOff(capsules);
	}

	public virtual void OnTriggerEnter(Collider obj)
	{
		if (obj.name == "Player")
		{
			TurnOn(signs);
			TurnOn(capsules);
		}
	}
}
