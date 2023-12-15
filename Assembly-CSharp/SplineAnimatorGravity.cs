using UnityEngine;

public class SplineAnimatorGravity : MonoBehaviour
{
	public Spline spline;

	public float gravityConstant = 9.81f;

	public int iterations = 5;

	private void FixedUpdate()
	{
		if (!(base.rigidbody == null) && !(spline == null))
		{
			Vector3 shortestConnection = spline.GetShortestConnection(base.rigidbody.position, iterations);
			base.rigidbody.AddForce(shortestConnection * (Mathf.Pow(shortestConnection.magnitude, -3f) * gravityConstant * base.rigidbody.mass));
		}
	}
}
