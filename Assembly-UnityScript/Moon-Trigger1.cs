using System;
using UnityEngine;

[Serializable]
public class Moon_Trigger1 : MonoBehaviour
{
	public virtual void OnTriggerEnter(Collider obj)
	{
		if (obj.gameObject.name == "Player" && !obj.GetComponent<move>().freezeControls)
		{
			StartCoroutine(GameObject.Find("Moon").GetComponent<Moon_Script>().Exit());
		}
	}
}
