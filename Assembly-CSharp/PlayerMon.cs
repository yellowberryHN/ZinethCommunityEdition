using System.Collections;
using UnityEngine;

public class PlayerMon : MonoBehaviour
{
	private Transform _player;

	private Transform _pyramid;

	private move player;

	private PyramidScript pyramid;

	private Action[] zeroOne = new Action[1]
	{
		new Action(stateEnum.talk, 100f, 0f)
	};

	private Action[] zeroTwo = new Action[1]
	{
		new Action(stateEnum.love, 100f, 0f)
	};

	private float tempLastSpeed = 3f;

	private float lastSpeed;

	public bool ignore;

	public Transform previous;

	public static bool xbox;

	public bool rewind;

	public bool rewindZone;

	private bool rewindQueued;

	private bool rewindCheck;

	private float timeSinceFuckUp = 3f;

	private int correctRewinds;

	public bool canMaster;

	private float lengthHeld;

	private float doTheyGetIt;

	private bool probalbyUnderstandsSkating;

	private bool messageUp;

	private int skates;

	public bool notNowBernad;

	private bool rewindControl;

	private bool vertControl;

	public bool jumpReminder = true;

	private float timeInAir;

	private bool jumped;

	private RaycastHit floor;

	private bool jumpUP;

	private float jumpUPCounter;

	private RaycastHit tempWallRay;

	private RaycastHit wallRay;

	public bool wallCheck;

	private bool triggered;

	private void Start()
	{
		_player = GameObject.Find("Player").transform;
		player = _player.GetComponent<move>();
		pyramid = GameObject.Find("Pyramid").transform.GetComponent<PyramidScript>();
		_pyramid = GameObject.Find("TextHolder").transform;
	}

	private void Update()
	{
		xbox = Input.GetJoystickNames().Length > 0;
		rewindControl = Input.GetButton("Rewind");
		vertControl = Input.GetAxis("Vertical") >= 0f;
		if (rewind)
		{
			if (timeSinceFuckUp > 5f && !rewindCheck && rewindQueued)
			{
				rewindCheck = false;
				rewindQueued = false;
				TurnOff();
				if (previous != null)
				{
					turnOn(previous);
				}
				else
				{
					PhoneInterface.view_controller.SetOpen(false);
				}
			}
			timeSinceFuckUp += Time.deltaTime;
		}
		if (rewindCheck && Input.GetButton("Skate"))
		{
			rewindCheck = false;
			rewindQueued = false;
			TurnOff();
			if (previous != null)
			{
				turnOn(previous);
			}
			else
			{
				PhoneInterface.view_controller.SetOpen(false);
			}
		}
		if (Physics.Raycast(_player.position + _player.up / 4f, -_player.up, out floor, 0.6f))
		{
			if (floor.transform.name == "ramps")
			{
				jumpReminder = true;
			}
			else
			{
				jumpReminder = false;
			}
		}
		if (jumpReminder && !player.freezeControls)
		{
			if (player.grounded)
			{
				jumped = false;
			}
			else if (!player.grounded && Input.GetButton("Jump"))
			{
				jumped = true;
			}
			if (player.grounded || jumped)
			{
				timeInAir = 0f;
			}
			else
			{
				timeInAir += Time.deltaTime;
				if ((double)timeInAir > 0.3)
				{
					Change("99");
					jumped = true;
					jumpUP = true;
					jumpUPCounter = 0f;
				}
			}
		}
		if (jumpUP)
		{
			jumpUPCounter += Time.deltaTime;
			if (jumpUPCounter >= 5f && Input.GetButton("Rewind"))
			{
				TurnOff();
				if (previous != null)
				{
					turnOn(previous);
				}
				else
				{
					PhoneInterface.view_controller.SetOpen(false);
				}
				jumpUP = false;
				jumpUPCounter = 0f;
			}
		}
		if (Input.GetButtonDown("Skate"))
		{
			skates++;
		}
		if (Input.GetButton("Skate"))
		{
			lengthHeld += Time.deltaTime;
			if (lengthHeld > 3f && !messageUp)
			{
				TurnOff();
				turnOn(_pyramid.Find("15"));
				PhoneInterface.view_controller.SetOpen(true);
				PhoneController.instance.OnNewMessage(1);
				messageUp = true;
				skates = 0;
			}
		}
		else
		{
			lengthHeld = 0f;
			if (messageUp && skates > 2)
			{
				TurnOff();
				if (previous != null)
				{
					turnOn(previous);
				}
				skates = 0;
				if (previous != null)
				{
					PhoneInterface.view_controller.SetOpen(true);
					PhoneController.instance.OnNewMessage(1);
				}
				else
				{
					PhoneInterface.view_controller.SetOpen(false);
				}
				messageUp = false;
			}
		}
		if (wallCheck)
		{
			if (!triggered && !player.grounded && !player.wallRiding && !player.freezeControls)
			{
				bool flag = false;
				if (Physics.Raycast(_player.position, _player.right, out tempWallRay, 2f))
				{
					flag = true;
					wallRay = tempWallRay;
				}
				else if (Physics.Raycast(_player.position, -_player.right, out tempWallRay, 2f))
				{
					flag = true;
					wallRay = tempWallRay;
				}
				if (flag)
				{
					if (!Input.GetButton("Jump"))
					{
						Change("96");
						triggered = true;
					}
					else if (_player.rigidbody.velocity.y < -2f)
					{
						Change("95");
						triggered = true;
					}
					else if (!(_player.rigidbody.velocity.z + _player.rigidbody.velocity.x < 30f))
					{
					}
				}
			}
			if (player.grounded)
			{
				triggered = false;
			}
		}
		if (triggered && player.wallRiding)
		{
			triggered = false;
			TurnOff();
			if (previous != null)
			{
				turnOn(previous);
			}
			if (previous != null)
			{
				PhoneInterface.view_controller.SetOpen(true);
				PhoneController.instance.OnNewMessage(1);
			}
			else
			{
				PhoneInterface.view_controller.SetOpen(false);
			}
		}
	}

	private void FixedUpdate()
	{
		float num = _player.InverseTransformDirection(_player.rigidbody.velocity).z - player.overSpeed;
		if ((double)num < (double)lastSpeed - (double)lastSpeed * 0.4 && vertControl && _player.rotation.eulerAngles.x > -2f && lastSpeed > 40f && !player.isGrinding && !ignore && !rewindQueued && !rewindControl && rewind)
		{
			rewindQueued = true;
			Change("11");
			pyramid.SwitchAction(zeroOne);
			timeSinceFuckUp = 0f;
		}
		else if (player.freezeControls && !player.isGrinding && !rewindCheck)
		{
			if (correctRewinds < 5)
			{
				correctRewinds++;
				rewindCheck = true;
				Change("07");
				pyramid.SwitchAction(zeroTwo);
				rewindCheck = true;
			}
			else
			{
				rewindCheck = false;
				rewindQueued = false;
				TurnOff();
				if (previous != null)
				{
					turnOn(previous);
				}
				else
				{
					PhoneInterface.view_controller.SetOpen(false);
				}
			}
		}
		lastSpeed = tempLastSpeed;
		tempLastSpeed = num;
	}

	private IEnumerator CloseMe()
	{
		MonoBehaviour.print("yo");
		yield return new WaitForSeconds(5f);
		TurnOff();
		MonoBehaviour.print("gahy");
		if (previous != null)
		{
			turnOn(previous);
		}
		else
		{
			PhoneInterface.view_controller.SetOpen(false);
		}
	}

	private void Change(string thing)
	{
		TurnOff();
		turnOn(_pyramid.Find(thing));
		PhoneInterface.view_controller.SetOpen(true);
		PhoneController.instance.OnNewMessage(1);
	}

	private void turnOn(Transform child)
	{
		if (!child)
		{
			Debug.LogWarning("child is null... ");
			return;
		}
		foreach (Transform item in child)
		{
			if (item.name == "PC" && !xbox)
			{
				turnOn(item);
			}
			else if (item.name == "Xbox" && xbox)
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
			if (child.parent.name != "08" && child.parent.name != "11" && child.parent.name != "07" && child.parent.name != "15" && child.parent.name != "99" && child.parent.name != "96" && child.parent.name != "95" && child.parent.name != "TextHolder")
			{
				previous = child.parent;
			}
		}
	}

	public void TurnOff()
	{
		foreach (Transform item in _pyramid)
		{
			TurnOffHelper(item);
		}
	}
}
