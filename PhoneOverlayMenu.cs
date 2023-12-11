using System;
using System.Collections.Generic;
using UnityEngine;

public class PhoneOverlayMenu : PhoneScreen
{
	private int menuind;

	public List<PhoneButton> buttons = new List<PhoneButton>();

	public List<PhoneElement> elements = new List<PhoneElement>();

	private bool isactive;

	public PhoneLabel clocklabel;

	public PhoneLabel fpslabel;

	public PhoneLabel moneylabel;

	private float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private float last_money = float.NegativeInfinity;

	private void Start()
	{
		if (clocklabel == null)
		{
			clocklabel = base.transform.Find("ClockLabel").GetComponent<PhoneLabel>();
		}
		if (fpslabel == null)
		{
			fpslabel = base.transform.Find("FPSLabel").GetComponent<PhoneLabel>();
		}
		if (moneylabel == null)
		{
			moneylabel = base.transform.Find("MoneyLabel").GetComponent<PhoneLabel>();
		}
	}

	public override void Init()
	{
		SetupButtons();
		timeleft = updateInterval;
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		foreach (PhoneElement element in elements)
		{
			element.OnLoad();
		}
		menuind = 0;
		UpdateMenuItems();
	}

	private void SetupButtons()
	{
		PhoneElement[] componentsInChildren = base.gameObject.GetComponentsInChildren<PhoneElement>();
		foreach (PhoneElement phoneElement in componentsInChildren)
		{
			elements.Add(phoneElement);
			phoneElement.Init();
		}
		PhoneButton[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<PhoneButton>();
		foreach (PhoneButton item in componentsInChildren2)
		{
			buttons.Add(item);
		}
		UpdateMenuItems();
	}

	public override void UpdateScreen()
	{
		foreach (PhoneElement element in elements)
		{
			element.OnUpdate();
		}
		clocklabel.text = DateTime.Now.ToString("H:mm");
		UpdateFramerate();
		UpdateMoney();
	}

	private void UpdateFramerate()
	{
		timeleft -= base.deltatime;
		accum += Time.timeScale / base.deltatime;
		frames++;
		if (timeleft <= 0f)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F0}", num);
			fpslabel.text = text;
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}

	private void UpdateMoney()
	{
		if (last_money != PhoneMemory.capsule_points)
		{
			last_money = PhoneMemory.capsule_points;
			if ((bool)moneylabel)
			{
				moneylabel.text = string.Format("${0:F0}", last_money);
			}
		}
	}

	public void MenuControls()
	{
		if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
		{
			menuind = -1;
			Vector3 touchPoint = PhoneInput.GetTouchPoint();
			if (touchPoint != Vector3.zero * -1f)
			{
				Vector3 point = PhoneInput.TransformPoint(touchPoint);
				for (int i = 0; i < buttons.Count; i++)
				{
					point.y = buttons[i].transform.position.y;
					if (buttons[i].ContainsPoint(point))
					{
						menuind = i;
					}
				}
			}
			if (menuind >= 0 && PhoneInput.IsPressedDown())
			{
				buttons[menuind].OnPressed();
			}
		}
		else if (isactive)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				menuind--;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				menuind++;
			}
			if (menuind < 0)
			{
				menuind = buttons.Count - 1;
			}
			if (menuind >= buttons.Count)
			{
				menuind = 0;
			}
			if (Input.GetKeyDown(KeyCode.Z))
			{
				buttons[menuind].OnPressed();
			}
		}
	}

	private void UpdateMenuItems()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].selected = i == menuind;
		}
	}
}
