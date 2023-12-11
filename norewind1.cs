using UnityEngine;

public class norewind1 : MonoBehaviour
{
	private PlayerMon player;

	private void Start()
	{
		player = GameObject.Find("TutObject").GetComponent<PlayerMon>();
		base.renderer.enabled = false;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		player.wallCheck = true;
	}

	private void OnTriggerExit(Collider collider)
	{
		player.wallCheck = false;
	}
}
