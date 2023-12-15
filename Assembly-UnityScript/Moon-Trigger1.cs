using System;
using UnityEngine;

[Serializable]
public class Moon_Trigger1 : MonoBehaviour
{
	public void Start()
	{
		Debug.Log("Moon-Trigger1");
	}
	public virtual void OnTriggerEnter(Collider obj)
	{
		if (obj.gameObject.name == "Player" && !obj.GetComponent<move>().freezeControls)
		{
			var enumerator = GameObject.Find("Moon").GetComponent<Moon_Script>().Exit();
		}
	}
}
