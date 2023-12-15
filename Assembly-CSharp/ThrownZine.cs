using UnityEngine;

public class ThrownZine : MonoBehaviour
{
	public float speed = 20f;

	public float min_dist = 10f;

	public float kill_timer = 5f;

	public float rot_force = 20f;

	public void Init(Vector3 pos)
	{
		base.transform.LookAt(Camera.main.transform);
		base.rigidbody.velocity = pos;
		Object.Destroy(base.gameObject, kill_timer);
		Vector3 up = Vector3.up;
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		up.x += insideUnitCircle.x;
		up.z += insideUnitCircle.y;
		up = up.normalized * 2f;
		base.transform.position += up;
		base.rigidbody.velocity += up * speed;
		base.rigidbody.AddRelativeTorque(Random.onUnitSphere * rot_force, ForceMode.Force);
	}
}
