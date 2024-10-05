using System;
using UnityEngine;

[Serializable]
public class UndercityMan : MonoBehaviour
{
	public bool on;

	public Transform undercity;

	public Transform signs;

	public Transform rails;

	public Transform capsules;

	public Transform npcs;

	public Transform c_caps;

	public Transform c_signs;

	public Transform tut;

	public Transform cliff;

	[NonSerialized]
	public static bool has_init;

	public UndercityMan()
	{
		on = true;
	}

	public virtual void Start()
	{
		if (!has_init)
		{
			has_init = true;
			TurnOff(undercity);
			TurnOff(signs);
			TurnOff(rails);
			TurnOff(capsules);
			TurnOff(npcs);
		}
	}

	public virtual void TurnOff(Transform obj)
	{
		obj.gameObject.SetActiveRecursively(false);
	}

	public virtual void TurnOn(Transform obj)
	{
		obj.gameObject.SetActiveRecursively(true);
	}

	public virtual void OnTriggerEnter()
	{
		if (on)
		{
			TurnOn(undercity);
			TurnOn(signs);
			TurnOn(rails);
			TurnOn(capsules);
			TurnOn(npcs);
			TurnOff(c_caps);
			TurnOff(c_signs);
			TurnOff(tut);
			TurnOff(cliff);
		}
		else
		{
			TurnOff(undercity);
			TurnOff(signs);
			TurnOff(rails);
			TurnOff(capsules);
			TurnOff(npcs);
			TurnOn(c_caps);
			TurnOn(c_signs);
			TurnOn(tut);
			TurnOn(cliff);
		}
	}
}
