using UnityEngine;

public class tutSpeed : MonoBehaviour
{
	private PyramidScript pyramid;

	private Transform _pyramid;

	private Action[] zeroOne = new Action[1]
	{
		new Action(stateEnum.frown, 100f, 0f)
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

	public string slide;

	public float speed;

	public Transform nevermind;

	private void Start()
	{
		base.renderer.enabled = false;
		pyramid = GameObject.Find("Pyramid").transform.GetComponent<PyramidScript>();
		_pyramid = GameObject.Find("TextHolder").transform;
	}

	private void OnTriggerEnter(Collider collision)
	{
		Transform transform = GameObject.Find("Player").transform;
		float z = transform.InverseTransformDirection(transform.rigidbody.velocity).z;
		if (z < speed)
		{
			xbox = Input.GetJoystickNames().Length;
			TurnOff();
			turnOn(_pyramid.Find(slide));
			PhoneInterface.view_controller.SetOpen(true);
			PhoneController.instance.OnNewMessage(1);
			pyramid.SwitchAction(zeroOne);
			nevermind.gameObject.active = true;
		}
		else
		{
			Object.Destroy(nevermind.gameObject);
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
