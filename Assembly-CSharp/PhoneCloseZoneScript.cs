using UnityEngine;

public class PhoneCloseZoneScript : MonoBehaviour
{
	private PlayerMon temp;

	private void Start()
	{
		base.renderer.enabled = false;
		temp = GameObject.Find("TutObject").GetComponent<PlayerMon>();
	}

	private void Update()
	{
	}

	private void OnTriggerEnter()
	{
		PhoneInterface.view_controller.SetOpen(false);
		temp.canMaster = true;
		temp.TurnOff();
		temp.previous = null;
		Object.Destroy(base.gameObject);
	}
}
