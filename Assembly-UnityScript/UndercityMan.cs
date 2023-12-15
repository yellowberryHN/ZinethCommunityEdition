using System;
using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

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
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(obj);
		while (enumerator.MoveNext())
		{
			object obj2 = enumerator.Current;
			if (!(obj2 is Transform))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(Transform));
			}
			Transform transform = (Transform)obj2;
			TurnOff(transform);
			UnityRuntimeServices.Update(enumerator, transform);
			transform.gameObject.active = false;
			UnityRuntimeServices.Update(enumerator, transform);
		}
	}

	public virtual void TurnOn(Transform obj)
	{
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(obj);
		while (enumerator.MoveNext())
		{
			object obj2 = enumerator.Current;
			if (!(obj2 is Transform))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(Transform));
			}
			Transform transform = (Transform)obj2;
			TurnOn(transform);
			UnityRuntimeServices.Update(enumerator, transform);
			transform.gameObject.active = true;
			UnityRuntimeServices.Update(enumerator, transform);
		}
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

	public virtual void Main()
	{
	}
}
