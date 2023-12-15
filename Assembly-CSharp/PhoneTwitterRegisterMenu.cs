using UnityEngine;

public class PhoneTwitterRegisterMenu : PhoneMainMenu
{
	public PhoneLabel pin_button;

	public PhoneLabel status_text;

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		if ((bool)status_text)
		{
			status_text.text = string.Empty;
		}
	}

	public override void UpdateScreen()
	{
		DoNumberKeyInput();
		base.UpdateScreen();
	}

	protected virtual bool AddToPin(string text)
	{
		if ((bool)status_text && status_text.text == "Need 7 digits!")
		{
			status_text.text = string.Empty;
		}
		if (pin_button.text.Length >= 7)
		{
			return false;
		}
		pin_button.text += text.Substring(0, 1);
		return true;
	}

	protected virtual void DoNumberKeyInput()
	{
		for (var i = 0; i < 10; i++)
		{
			if (Input.GetKeyDown(KeyCode.Alpha0 + i))
			{
				AddToPin(i.ToString());
				break;
			}
		}
	}
	
	protected override void StickControls_Vertical()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (buttons.Count == 0)
		{
			return;
		}
		if (menuind >= buttons.Count)
		{
			menuind = 0;
		}
		if (menuind < 0)
		{
			menuind = 0;
		}
		PhoneButton phoneButton = current_buttons[menuind];
		if (controlDirPressed.y >= 0.5f && Mathf.Abs(controlDirPressed.x) < 0.3f)
		{
			if ((bool)phoneButton.up_button)
			{
				menuind = current_buttons.IndexOf(phoneButton.up_button);
			}
		}
		else if (controlDirPressed.y <= -0.5f && Mathf.Abs(controlDirPressed.x) < 0.3f)
		{
			if ((bool)phoneButton.down_button)
			{
				menuind = current_buttons.IndexOf(phoneButton.down_button);
			}
		}
		else if (controlDirPressed.x >= 0.5f && Mathf.Abs(controlDirPressed.y) < 0.3f)
		{
			if ((bool)phoneButton.right_button)
			{
				menuind = current_buttons.IndexOf(phoneButton.right_button);
			}
		}
		else if (controlDirPressed.x <= -0.5f && Mathf.Abs(controlDirPressed.y) < 0.3f && (bool)phoneButton.left_button)
		{
			menuind = current_buttons.IndexOf(phoneButton.left_button);
		}
		if (PhoneInput.IsPressedDown() && current_buttons.Count > menuind)
		{
			current_buttons[menuind].OnPressed();
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		switch (message)
		{
		case "pin_add":
			AddToPin(button.text);
			break;
		case "pin_erase":
		{
			string text = pin_button.text;
			if (text.Length <= 0)
			{
				controller.LoadPrevious();
				return false;
			}
			pin_button.text = text.Substring(0, text.Length - 1);
			break;
		}
		case "pin_submit":
			if (pin_button.text.Length < 7)
			{
				if ((bool)status_text)
				{
					status_text.text = "Need 7 digits!";
					status_text.textmesh.renderer.material.color = Color.red;
				}
				return false;
			}
			if ((bool)status_text)
			{
				status_text.text = "Submitting...";
				status_text.textmesh.renderer.material.color = Color.red;
			}
			break;
		default:
			return base.ButtonMessage(button, message);
		}
		return true;
	}

	public void OnRegistered(bool success, string username)
	{
		if (success)
		{
			if ((bool)status_text)
			{
				status_text.text = "Success!";
			}
			pin_button.text = string.Empty;
			controller.LoadScreen("AccountMenu");
		}
		else if ((bool)status_text)
		{
			status_text.text = "Failed!";
			status_text.color = Color.red;
		}
	}
}
