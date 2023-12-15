using System;
using UnityEngine;

[Serializable]
public class Jitter : MonoBehaviour
{
	private Vector3 startPosition;

	public float jitterAmount;

	private float jitterCounter;

	public float jitterSpeed;

	public Jitter()
	{
		jitterAmount = 0.3f;
		jitterSpeed = 0.1f;
	}

	public virtual void Start()
	{
		startPosition = transform.localPosition;
	}

	public virtual void Update()
	{
		jitterCounter += Time.deltaTime;
		if (!(jitterCounter <= jitterSpeed))
		{
			jitterCounter = 0f;
			transform.localPosition = new Vector3(0f, UnityEngine.Random.Range(0f - jitterAmount, jitterAmount), UnityEngine.Random.Range(0f - jitterAmount, jitterAmount)) + startPosition;
		}
	}

	public virtual void Main()
	{
	}
}
