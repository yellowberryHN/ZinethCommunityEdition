using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Camera-Control/Smooth Look At")]
public class SmoothLookAt : MonoBehaviour
{
	public Transform target;

	public float damping;

	public bool smooth;

	public SmoothLookAt()
	{
		damping = 6f;
		smooth = true;
	}

	public virtual void LateUpdate()
	{
		if ((bool)target)
		{
			if (smooth)
			{
				Quaternion to = Quaternion.LookRotation(target.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, to, Time.deltaTime * damping);
			}
			else
			{
				transform.LookAt(target);
			}
		}
	}

	public virtual void Start()
	{
		if ((bool)rigidbody)
		{
			rigidbody.freezeRotation = true;
		}
	}

	public virtual void Main()
	{
	}
}
