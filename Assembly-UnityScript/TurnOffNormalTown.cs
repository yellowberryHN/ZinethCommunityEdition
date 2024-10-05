using System;
using UnityEngine;

[Serializable]
public class TurnOffNormalTown : MonoBehaviour
{
	public virtual void Start()
	{
		foreach (var obj in FindSceneObjectsOfType(typeof(GameObject)))
		{
			var go = (GameObject)obj;
			if (go.GetComponent<Renderer>() != null && go.renderer.materials.Length > 1 &&
			    (go.renderer.materials[0].name == "lambert1 (Instance)" || go.renderer.materials[1].name == "lambert1 (Instance)"))
			{
				go.renderer.materials = new Material[1] { go.renderer.materials[1] }; 
			}
		}
	}
}
