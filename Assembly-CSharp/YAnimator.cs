using UnityEngine;

public class YAnimator : MonoBehaviour
{
	public float passedTime;

	public float yOffset;

	public float speed;

	private Vector3 normalPosition;

	private void Start()
	{
		normalPosition = base.transform.position;
	}

	private void Update()
	{
		passedTime += Time.deltaTime * speed;
		base.transform.position = normalPosition + Vector3.up * yOffset * Mathf.Sin(passedTime);
	}
}
