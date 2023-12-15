using System;
using UnityEngine;

[Serializable]
public class Tut_Trigger : MonoBehaviour
{
	private bool triggered;

	private bool neverTriggered;

	private Transform target;

	public Material staticMat;

	public Material tutMat;

	private float videoTimer;

	public bool kill;

	public Tut_Trigger()
	{
		neverTriggered = true;
	}

	public virtual void Start()
	{
		target = transform.parent.Find("screens").transform;
	}

	public virtual void Update()
	{
		target.renderer.material.mainTextureOffset = new Vector2(UnityEngine.Random.Range(0.1f, 1.1f), UnityEngine.Random.Range(0.1f, 1.1f));
	}
}
