using System.Collections.Generic;
using UnityEngine;

public class PhoneTutorialScreen : PhoneMainMenu
{
	public string next_screen;

	public List<PhoneButton> required_buttons = new List<PhoneButton>();

	private List<PhoneButton> uncompleted_buttons = new List<PhoneButton>();

	public bool require_stick_reset = true;

	private bool _stick_has_reset;

	public bool complete_on_select;

	public float require_stick_move;

	private float _stick_moved;

	public bool require_stick_click;

	private bool _stick_clicked;

	public bool require_mouse_click;

	private bool _mouse_clicked;

	private Vector2 my_last_control_dir = Vector2.zero;

	public bool change_text_on_select;

	private bool _stick_complete
	{
		get
		{
			return _stick_moved >= require_stick_move;
		}
	}

	private bool _buttons_complete
	{
		get
		{
			return uncompleted_buttons.Count <= 0;
		}
	}

	private bool _click_complete
	{
		get
		{
			return !require_stick_click || _stick_clicked;
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
		base.OnLoad();
		uncompleted_buttons.Clear();
		foreach (PhoneButton required_button in required_buttons)
		{
			required_button.selected = false;
			uncompleted_buttons.Add(required_button);
		}
		my_last_control_dir = Vector2.zero;
		_stick_moved = 0f;
		_stick_has_reset = false;
		_stick_clicked = false;
		menuind = -1;
	}

	public override void UpdateScreen()
	{
		base.UpdateScreen();
		if (CheckComplete())
		{
			PhoneController.instance.LoadScreen(next_screen);
		}
	}

	protected virtual void CheckCompletedOnSelects()
	{
		if (require_stick_reset && !_stick_has_reset)
		{
			return;
		}
		PhoneButton[] array = uncompleted_buttons.ToArray();
		PhoneButton[] array2 = array;
		foreach (PhoneButton phoneButton in array2)
		{
			if (phoneButton.selected && phoneButton.gameObject.active)
			{
				phoneButton.DoPressedParticles();
				CompleteButton(phoneButton);
			}
		}
	}

	protected virtual void CheckStickComplete()
	{
		Vector2 controlDir = PhoneInput.GetControlDir();
		if (controlDir.magnitude < 0.6f)
		{
			_stick_has_reset = true;
		}
		_stick_moved += (controlDir - my_last_control_dir).magnitude * base.deltatime;
		my_last_control_dir = controlDir;
		if (PhoneInput.IsPressedDown())
		{
			_stick_clicked = true;
		}
	}

	public virtual bool CheckComplete()
	{
		if (complete_on_select)
		{
			CheckCompletedOnSelects();
		}
		CheckStickComplete();
		return _buttons_complete && _stick_complete && _click_complete;
	}

	protected virtual void CompleteButton(PhoneButton button)
	{
		if (uncompleted_buttons.Contains(button))
		{
			PhoneAudioController.PlayAudioClip("click", SoundType.menu);
			uncompleted_buttons.Remove(button);
			if (change_text_on_select)
			{
				button.text = string.Empty;
			}
			else
			{
				button.gameObject.SetActiveRecursively(false);
			}
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message.StartsWith("complete"))
		{
			CompleteButton(button);
			return true;
		}
		if (message.StartsWith("skip"))
		{
			return true;
		}
		return base.ButtonMessage(button, message);
	}
}
