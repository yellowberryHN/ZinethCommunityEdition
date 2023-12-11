using UnityEngine;

public class HawkZoneScript : MonoBehaviour
{
	private HawkBehavior hawkScript;

	private void Awake()
	{
		hawkScript = GameObject.Find("Hawk").GetComponent<HawkBehavior>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.name == "Player")
		{
			hawkScript.active = true;
			hawkScript.inBounds = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name == "Player")
		{
			hawkScript.inBounds = false;
		}
	}
}
