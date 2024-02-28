using UnityEngine;

public class norewind : MonoBehaviour
{
	private PlayerMon player;

	private void Start()
	{
		player = GameObject.Find("TutObject").GetComponent<PlayerMon>();
		base.renderer.enabled = false;
	}

	private void OnTriggerEnter(Collider collider)
	{
		player.ignore = true;
	}

	private void OnTriggerExit(Collider collider)
	{
		player.ignore = false;
	}
}
