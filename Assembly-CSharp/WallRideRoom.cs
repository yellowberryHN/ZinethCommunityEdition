using System.Collections;
using UnityEngine;

public class WallRideRoom : MonoBehaviour
{
	private move player;

	public Transform bar;

	private float barScale;

	private Transform _pyramid;

	private bool xbox
	{
		get
		{
			return PlayerMon.xbox;
		}
	}

	private void Start()
	{
		player = GameObject.Find("Player").GetComponent<move>();
		player.canWallRide = false;
		barScale = bar.localScale.x;
		bar.localScale = new Vector3(0f, bar.localScale.y, bar.localScale.z);
		_pyramid = GameObject.Find("TextHolder").transform;
		base.renderer.enabled = false;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		player.Stop();
		player.canWallRide = true;
		TurnOff();
		turnOn(_pyramid.Find("09").transform);
		PhoneInterface.view_controller.SetOpen(true);
		PhoneController.instance.OnNewMessage(1);
		StartCoroutine("Wait");
	}

	private IEnumerator Wait()
	{
		while (bar.localScale.x < barScale)
		{
			bar.localScale = new Vector3(bar.localScale.x + 0.1f, bar.localScale.y, bar.localScale.z);
			yield return new WaitForSeconds(0.05f);
		}
		TurnOff();
		turnOn(_pyramid.Find("10").transform);
		PhoneInterface.view_controller.SetOpen(true);
		PhoneController.instance.OnNewMessage(1);
		Object.Destroy(base.gameObject);
	}

	private void turnOn(Transform child)
	{
		foreach (Transform item in child)
		{
			if (item.name == "PC" && !xbox)
			{
				turnOn(item);
			}
			else if (item.name == "Xbox" && !xbox)
			{
				turnOn(item);
			}
			else if (item.name != "PC" && item.name != "Xbox")
			{
				turnOn(item);
			}
		}
		child.gameObject.active = true;
	}

	private void TurnOffHelper(Transform child)
	{
		foreach (Transform item in child)
		{
			TurnOffHelper(item);
		}
		if (child.gameObject.active)
		{
			child.gameObject.active = false;
		}
	}

	private void TurnOff()
	{
		foreach (Transform item in _pyramid)
		{
			TurnOffHelper(item);
		}
	}
}
