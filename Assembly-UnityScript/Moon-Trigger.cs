using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Moon_Trigger : MonoBehaviour
{
	public void Start()
	{
		Debug.Log("Moon-Trigger");
	}
	
	public virtual void OnTriggerEnter(Collider obj)
	{
		if (obj.gameObject.name == "Player" && !obj.GetComponent<move>().freezeControls)
		{
			var enumerator = GameObject.Find("Moon").GetComponent<Moon_Script>().Enter();
		}
	}

	public virtual void OnTriggerExit(Collider obj)
	{
		if (obj.gameObject.name == "Player" && !obj.GetComponent<move>().freezeControls)
		{
			var enumerator = GameObject.Find("Moon").GetComponent<Moon_Script>().Leave();
		}
	}
}
