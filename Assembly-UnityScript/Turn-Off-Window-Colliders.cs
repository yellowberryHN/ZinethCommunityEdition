using System;
using UnityEngine;

[Serializable]
public class Turn_Off_Window_Colliders : MonoBehaviour
{
	public virtual void Start()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(GameObject));
		int i = 0;
		UnityEngine.Object[] array2 = array;
		for (int length = array2.Length; i < length; i++)
		{
			if (((GameObject)array2[i]).GetComponent<Renderer>() != null && ((GameObject)array2[i]).renderer.materials.Length > 1 && (((GameObject)array2[i]).renderer.materials[0].name == "lambert1 (Instance)" || ((GameObject)array2[i]).renderer.materials[1].name == "lambert1 (Instance)"))
			{
				((GameObject)array2[i]).renderer.materials = new Material[1] { ((GameObject)array2[i]).renderer.materials[1] };
			}
		}
	}

	public virtual void Main()
	{
	}
}
