using UnityEngine;

public class PhoneTextInput : PhoneButton
{
	public PhoneLabel text_label;

	public bool has_focus;

	public PhoneButton enter_button;

	public string input_text
	{
		get
		{
			return id_info;
		}
		set
		{
			id_info = value;
			text_label.text = ">" + value;
		}
	}

	private void Awake()
	{
		Init();
		text_label.overrideColor = true;
		text_label.color = Color.gray;
		text_label.SetText(">");
	}

	public override void OnLoad()
	{
		text_label.SetColor(Color.gray);
		base.OnLoad();
		if (text_label.text == string.Empty)
		{
			text_label.SetText(">");
		}
	}

	public override void OnUpdate()
	{
		if (has_focus)
		{
			HandleInput();
			if ((bool)enter_button)
			{
				enter_button.id_info = id_info;
			}
			string text = ">" + input_text + "_";
			if (text != text_label.text)
			{
				text_label.text = text;
			}
		}
		if (!selected && has_focus && (Input.GetButtonDown("CellClick") || Input.GetMouseButtonDown(0) || PhoneInput.controltype == PhoneInput.ControlType.Keyboard))
		{
			has_focus = false;
			text_label.SetColor(Color.gray);
			text_label.text = ">" + input_text;
		}

		base.OnUpdate();
	}

	public override void OnPressed()
	{
		has_focus = true;
		text_label.SetColor(Color.black);
		Input.ResetInputAxes();
	}

	public virtual void HandleInput()
	{
		string inputString = Input.inputString;
		foreach (char chr in inputString)
		{
			InputChar(chr);
		}
	}

	public virtual void InputChar(char chr)
	{
		if (chr == "\b"[0])
		{
			if (input_text.Length > 0)
			{
				input_text = input_text.Remove(input_text.Length - 1);
			}
		}
		else if (chr == "\n"[0] || chr == "\r"[0])
		{
			if ((bool)enter_button)
			{
				enter_button.id_info = id_info;
				SubmitText();
			}
			else if (!string.IsNullOrEmpty(command))
			{
				SubmitText();
			}
		}
		else
		{
			input_text += chr;
		}
	}

	public virtual void SubmitText()
	{
		RunCommand(command);
		input_text = string.Empty;
	}

	public override void OnSelected()
	{
		if ((bool)textmesh)
		{
			textscale = text_size + Mathf.Min(text_size * 0.2f, 0.1f);
		}
		if ((bool)background_box)
		{
			SetBackColor(back_selected_color);
		}
		SetBorderActive(always_use_background_border);
		if (expand_on_select)
		{
			wantedscale = normal_scale * expand_size;
		}
	}

	public override void OnUnSelected()
	{
		if ((bool)textmesh)
		{
			textscale = text_size;
		}
		if ((bool)background_box)
		{
			SetBackColor(back_normal_color);
		}
		SetBorderActive(always_use_background_border);
		if (expand_on_select)
		{
			wantedscale = normal_scale;
		}
	}

	public override bool RelayPress(PhoneButton button)
	{
		if (button == enter_button)
		{
			SubmitText();
			return true;
		}
		return base.RelayPress(button);
	}

	private void OnGUI()
	{
		if (has_focus && PhoneController.powerstate == PhoneController.PowerState.open)
		{
			GUI.SetNextControlName("hahaok");
			GUI.TextField(new Rect(-10f, -10f, 1f, 1f), string.Empty);
			GUI.FocusControl("hahaok");
		}
	}
}
