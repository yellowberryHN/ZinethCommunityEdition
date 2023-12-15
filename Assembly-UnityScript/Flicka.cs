using System;
using UnityEngine;

[Serializable]
public class Flicka : MonoBehaviour
{
	public Material matOne;

	public Material matTwo;

	public virtual void Update()
	{
		if (!(UnityEngine.Random.Range(1f, 2f) <= 1.3f))
		{
			renderer.material = matOne;
		}
		else
		{
			renderer.material = matTwo;
		}
	}
}
