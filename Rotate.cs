using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 rotationAmount;

	private void Update()
	{
		base.transform.Rotate(rotationAmount * Time.deltaTime);
	}
}
