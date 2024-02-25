using System;
using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

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

	public virtual void Main()
	{
	}
}
