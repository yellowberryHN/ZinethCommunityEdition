using UnityEngine;

public class HawkZoneOuterScript : MonoBehaviour
{
	private BigHawkBehavior bigHawkScript;

	private void Awake()
	{
		bigHawkScript = GameObject.Find("HawkBig").GetComponent<BigHawkBehavior>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player")
		{
			move component = other.GetComponent<move>();
			if (component != null && component.freezeControls)
			{
				bigHawkScript.active = true;
				bigHawkScript.inBounds = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name == "Player")
		{
			bigHawkScript.inBounds = false;
		}
	}
}
