using System;
using UnityEngine;

[Serializable]
public class spinClouds : MonoBehaviour
{
	public float spinAmount;

	public bool one;

	public spinClouds()
	{
		one = true;
	}

	public virtual void FixedUpdate()
	{
		if (one)
		{
			transform.Rotate(0f, spinAmount, 0f);
		}
		else
		{
			transform.Rotate(0f, 0f, spinAmount);
		}
	}
}
