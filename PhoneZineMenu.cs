using System.Collections.Generic;
using UnityEngine;

public class PhoneZineMenu : PhoneMainMenu
{
	public PhoneLabel namelabel;

	public PhoneLabel lvllabel;

	public PhoneElement texturelabel;

	public PhoneButton nextbut;

	public PhoneButton prevbut;

	public PhoneButton showhidebutton;

	public int zine_ind;

	public string nextscreenname;

	public Texture2D last_zine_tex;

	public Texture2D current_texture
	{
		get
		{
			if (zine_ind >= zine_images.Count)
			{
				Debug.LogWarning("nonnonono: your ind:" + zine_ind + " list size:" + zine_images.Count);
			}
			return zine_images[zine_ind];
		}
	}

	public List<Texture2D> zine_images
	{
		get
		{
			return PhoneMemory.unlocked_zines;
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		menuind = 0;
		UpdateLevelTexture();
		PhoneElement[] array = base.elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		UpdateMenuItems();
	}

	public void UpdateLevelTexture()
	{
		if ((bool)texturelabel)
		{
			texturelabel.renderer.material.mainTexture = current_texture;
		}
	}

	private void NextLevel()
	{
		zine_ind++;
		if (zine_ind >= zine_images.Count)
		{
			zine_ind = 0;
		}
		UpdateLevelTexture();
		if (PhoneInterface.IsZineVisible())
		{
			SetZineVisible();
		}
		if ((bool)nextbut)
		{
			nextbut.transform.position -= base.transform.right * 0.15f;
		}
	}

	private void PreviousLevel()
	{
		zine_ind--;
		if (zine_ind < 0)
		{
			zine_ind = zine_images.Count - 1;
		}
		UpdateLevelTexture();
		if (PhoneInterface.IsZineVisible())
		{
			SetZineVisible();
		}
		if ((bool)prevbut)
		{
			prevbut.transform.position += base.transform.right * 0.15f;
		}
	}

	public override bool ButtonMessage(PhoneButton button, string command)
	{
		switch (command)
		{
		case "next":
			NextLevel();
			break;
		case "previous":
			PreviousLevel();
			break;
		case "toggle":
			if (PhoneInterface.IsZineVisible())
			{
				showhidebutton.text = "Show";
				return HideZine();
			}
			showhidebutton.text = "Hide";
			return SetZineVisible();
		case "show":
			Playtomic.Log.CustomMetric("tShowedZine", "tPhone", true);
			return SetZineVisible();
		case "hide":
			Playtomic.Log.CustomMetric("tHidZine", "tPhone", true);
			return HideZine();
		default:
			return base.ButtonMessage(button, command);
		case "accept":
			break;
		}
		return true;
	}

	public bool SetZineVisible()
	{
		return SetZineVisible(zine_ind);
	}

	public bool SetZineVisible(int index)
	{
		return SetZineVisible(zine_images[index]);
	}

	public bool SetZineVisible(Texture2D tex)
	{
		last_zine_tex = tex;
		return PhoneInterface.ShowZine(tex);
	}

	public bool HideZine()
	{
		return PhoneInterface.HideZine();
	}

	protected override void MenuControls()
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
			return;
		}
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.x >= 0.5f)
		{
			NextLevel();
		}
		else if (controlDirPressed.x <= -0.5f)
		{
			PreviousLevel();
		}
		if (controlDirPressed.y >= 0.5f)
		{
			menuind--;
		}
		if (controlDirPressed.y <= -0.5f)
		{
			menuind++;
		}
		if (controls_wrap)
		{
			if (menuind < 0)
			{
				menuind = Mathf.Max(0, buttons.Count - 1);
			}
			if (menuind >= buttons.Count)
			{
				menuind = 0;
			}
			while (buttons[menuind] == nextbut || buttons[menuind] == prevbut)
			{
				menuind++;
			}
			if (menuind >= buttons.Count)
			{
				menuind = 0;
			}
		}
		else
		{
			menuind = Mathf.Clamp(menuind, 0, buttons.Count - 1);
			while (buttons[menuind] == nextbut || buttons[menuind] == prevbut)
			{
				menuind++;
			}
			if (menuind >= buttons.Count)
			{
				menuind = 0;
			}
		}
		if (PhoneInput.IsPressedDown() && buttons.Count > menuind)
		{
			buttons[menuind].OnPressed();
		}
	}
}
