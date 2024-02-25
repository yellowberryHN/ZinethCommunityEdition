using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class TurnOffNormalTown : MonoBehaviour
{
	public virtual void Start()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(GameObject));
		int i = 0;
		UnityEngine.Object[] array2 = array;
		for (int length = array2.Length; i < length; i++)
		{
			if (((GameObject)array2[i]).GetComponent<Renderer>() != null && Extensions.get_length((System.Array)((GameObject)array2[i]).renderer.materials) > 1 && (((GameObject)array2[i]).renderer.materials[0].name == "lambert1 (Instance)" || ((GameObject)array2[i]).renderer.materials[1].name == "lambert1 (Instance)"))
			{
				((GameObject)array2[i]).renderer.materials = new Material[1] { ((GameObject)array2[i]).renderer.materials[1] };
			}
		}
	}
}
