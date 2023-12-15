using UnityEngine;

public class Critter : MonoBehaviour
{
	public Transform l_wing;

	public Transform r_wing;

	public float flap_upforce = 1.5f;

	public float flap_forwardforce = 2f;

	public float turn_amount = 360f;

	private float current_turn;

	public float flap_time = 0.25f;

	private void Start()
	{
		if (!l_wing)
		{
			l_wing = base.transform.FindChild("LWingJoint").transform;
		}
		if (!r_wing)
		{
			r_wing = base.transform.FindChild("RWingJoint").transform;
		}
		Invoke("Flap", 0.1f);
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.up, current_turn * Time.deltaTime);
		l_wing.localRotation = Quaternion.Lerp(l_wing.localRotation, Quaternion.identity, Time.deltaTime * 5f);
		r_wing.localRotation = Quaternion.Lerp(r_wing.localRotation, Quaternion.identity, Time.deltaTime * 5f);
	}

	public void Flap()
	{
		current_turn = Random.Range(0f - turn_amount, turn_amount);
		Vector3 force = Vector3.up * flap_upforce + Vector3.forward * flap_forwardforce;
		base.rigidbody.AddRelativeForce(force);
		Vector3 localEulerAngles = l_wing.localEulerAngles;
		localEulerAngles.z = 30f;
		l_wing.localEulerAngles = localEulerAngles;
		localEulerAngles = r_wing.localEulerAngles;
		localEulerAngles.z = -30f;
		r_wing.localEulerAngles = localEulerAngles;
		Invoke("Flap", flap_time);
	}
}
