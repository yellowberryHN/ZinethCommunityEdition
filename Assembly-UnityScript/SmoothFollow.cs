using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Camera-Control/Smooth Follow")]
public class SmoothFollow : MonoBehaviour
{
	public Transform target;

	public float distance;

	public float height;

	public float heightDamping;

	public float rotationDamping;

	public SmoothFollow()
	{
		distance = 10f;
		height = 5f;
		heightDamping = 2f;
		rotationDamping = 3f;
	}

	public virtual void LateUpdate()
	{
		if ((bool)target)
		{
			float y = target.eulerAngles.y;
			float to = target.position.y + height;
			float y2 = transform.eulerAngles.y;
			float y3 = transform.position.y;
			y2 = Mathf.LerpAngle(y2, y, rotationDamping * Time.deltaTime);
			y3 = Mathf.Lerp(y3, to, heightDamping * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
			transform.position = target.position;
			transform.position -= quaternion * Vector3.forward * distance;
			float y4 = y3;
			Vector3 position = transform.position;
			float num = (position.y = y4);
			Vector3 vector2 = (transform.position = position);
			transform.LookAt(target);
		}
	}

	public virtual void Main()
	{
	}
}
