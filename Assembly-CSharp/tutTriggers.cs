using UnityEngine;

public class tutTriggers : MonoBehaviour
{
	private PyramidScript pyramid;

	private Transform _pyramid;

	private PlayerMon playerMon;

	private Action[] zeroOne = new Action[1]
	{
		new Action(stateEnum.talk, 100f, 0f)
	};

	private Action[] zeroTwo = new Action[1]
	{
		new Action(stateEnum.talk, 100f, 0f)
	};

	private Action[] zeroSix = new Action[1]
	{
		new Action(stateEnum.excited, 100f, 0f)
	};

	private Action[] zeroThree = new Action[2]
	{
		new Action(stateEnum.alert, 1.2f, 0f),
		new Action(stateEnum.talk, 100f, 0f)
	};

	private Action[] zeroFour = new Action[2]
	{
		new Action(stateEnum.alert, 1.2f, 0f),
		new Action(stateEnum.talk, 100f, 0f)
	};

	private Action[] zeroEight = new Action[1]
	{
		new Action(stateEnum.excited, 100f, 0f)
	};

	private Action[] zeroNine = new Action[1]
	{
		new Action(stateEnum.alert, 100f, 0f)
	};

	public int xbox;

	private void Start()
	{
		base.renderer.enabled = false;
		pyramid = GameObject.Find("Pyramid").transform.GetComponent<PyramidScript>();
		_pyramid = GameObject.Find("TextHolder").transform;
		playerMon = GameObject.Find("TutObject").GetComponent<PlayerMon>();
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider collision)
	{
		xbox = Input.GetJoystickNames().Length;
		TurnOff();
		turnOn(_pyramid.Find(base.transform.name));
		PhoneInterface.view_controller.SetOpen(true);
		PhoneController.instance.OnNewMessage(1);
		if (base.transform.name == "01")
		{
			pyramid.SwitchAction(zeroOne);
		}
		else if (base.transform.name == "02")
		{
			pyramid.SwitchAction(zeroTwo);
		}
		else if (base.transform.name == "03")
		{
			pyramid.SwitchAction(zeroThree);
		}
		else if (base.transform.name == "04")
		{
			pyramid.SwitchAction(zeroFour);
		}
		else if (base.transform.name == "06")
		{
			pyramid.SwitchAction(zeroSix);
		}
		else if (base.transform.name == "18")
		{
			pyramid.SwitchAction(zeroNine);
		}
		else if (base.transform.name == "05")
		{
			playerMon.rewind = true;
		}
		else if (base.transform.name == "08")
		{
			pyramid.SwitchAction(zeroEight);
			playerMon.rewind = true;
		}
		else
		{
			pyramid.SwitchAction(zeroOne);
		}
		Object.Destroy(base.gameObject);
	}

	private void turnOn(Transform child)
	{
		foreach (Transform item in child)
		{
			if (item.name == "PC" && xbox == 0)
			{
				turnOn(item);
			}
			else if (item.name == "Xbox" && xbox > 0)
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
		child.gameObject.active = false;
	}

	private void TurnOff()
	{
		foreach (Transform item in _pyramid)
		{
			TurnOffHelper(item);
		}
	}
}
