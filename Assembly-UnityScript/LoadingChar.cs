using System;
using UnityEngine;

[Serializable]
public class LoadingChar : MonoBehaviour
{
	public virtual void Start()
	{
		animation["Skate"].speed = 4f;
		Debug.Log("LoadingChar");
	}

	public virtual void Update()
	{
		animation.CrossFade("Skate");
	}

	public virtual void Main()
	{
	}
}
