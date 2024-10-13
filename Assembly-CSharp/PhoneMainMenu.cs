using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMainMenu : PhoneScreen
{
	public int menuind;

	public PhoneButton button_prefab;

	public PhoneLabel label;

	public bool use_icons = true;

	public bool autocreatebuttons = true;

	public bool unlocked_menus_only;

	public bool resetselection = true;

	public string[] menu_items;

	public List<PhoneButton> buttons = new List<PhoneButton>();

	public bool hide_background = true;

	public bool special_home_load;

	protected bool exit_animating;

	protected bool cancel_exit_animate;

	public bool sort_buttonlist = true;

	private Vector3 centerpos = Vector3.zero;

	public List<PhoneButton> auto_buttons = new List<PhoneButton>();

	public float start_rads = 0.25f;

	public float hradius = 1.5f;

	public float vradius = 2f;

	public float add_rads = 0.25f;

	public bool auto_button_directions = true;

	public bool radial_menu;

	public Transform stick_trans;

	public bool controls_wrap;

	public bool use_button_dir_control;

	public PhoneElement[] elements
	{
		get
		{
			return GetComponentsInChildren<PhoneElement>();
		}
	}

	private float start_angle
	{
		get
		{
			return start_rads * (float)Math.PI;
		}
	}

	private float add_angle
	{
		get
		{
			return add_rads * (float)Math.PI;
		}
	}

	public virtual List<PhoneButton> current_buttons
	{
		get
		{
			return buttons;
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	protected virtual void HideBackground()
	{
		base.renderer.enabled = false;
	}

	public override void Init()
	{
		SetupButtons();
	}

	public override void OnHomeLoad()
	{
		if (!special_home_load)
		{
			return;
		}
		bool flag = true;
		PhoneElement[] array = elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
			if (flag)
			{
				phoneElement.transform.position = base.transform.position + base.transform.right * 5f;
			}
			else
			{
				phoneElement.transform.position = base.transform.position + base.transform.right * -5f;
			}
			flag = !flag;
		}
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		if (exit_animating)
		{
			cancel_exit_animate = true;
		}
		centerpos = base.transform.position + Vector3.forward * 0.15f;
		if (unlocked_menus_only && PhoneMemory.menus_updated)
		{
			PhoneMemory.menus_updated = false;
			ClearAutoButtons();
			AutoCreateButtons();
		}
		if (resetselection)
		{
			menuind = 0;
		}
		PhoneElement[] array = elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		if ((bool)stick_trans && (bool)stick_trans.renderer)
		{
			stick_trans.renderer.material.color = PhoneMemory.settings.selectedTextColor;
		}
		UpdateMenuItems();
	}

	public override void OnExit()
	{
		if (do_exit_effect && base.gameObject.active)
		{
			if (!exit_animating)
			{
				exit_animating = true;
				StartCoroutine(Co_OnExit());
			}
		}
		else
		{
			base.OnExit();
		}
	}

	protected virtual IEnumerator Co_OnExit()
	{
		Vector3 pos = base.transform.position;
		Quaternion rot = base.transform.localRotation;
		Vector3 dir = exit_dir;
		if (use_other_exit_dir && (bool)controller.curscreen && controller.curscreen != this && controller.curscreen.do_exit_effect)
		{
			dir = controller.curscreen.exit_dir;
			dir.x *= -1f;
			dir.z *= -1f;
		}
		float timer = 0.5f;
		while (timer > 0f)
		{
			if (cancel_exit_animate)
			{
				base.transform.position = pos;
				break;
			}
			timer -= Time.deltaTime;
			base.transform.position = Vector3.Lerp(base.transform.position, pos + dir, Time.deltaTime * 5f);
			yield return null;
		}
		exit_animating = false;
		if (cancel_exit_animate)
		{
			cancel_exit_animate = false;
		}
		else
		{
			base.OnExit();
		}
	}

	protected virtual void AutoCreateButtons_Radial()
	{
		centerpos = base.transform.position + Vector3.forward * 0.15f;
		float num = start_angle;
		int num2 = 0;
		Vector3 position = centerpos + Vector3.right * hradius * Mathf.Cos(num) + Vector3.forward * vradius * Mathf.Sin(num);
		string[] array = menu_items;
		foreach (string text in array)
		{
			if (unlocked_menus_only && !PhoneMemory.unlocked_menus.Contains(text))
			{
				num += add_angle;
				position = centerpos + Vector3.right * hradius * Mathf.Cos(num) + Vector3.forward * vradius * Mathf.Sin(num);
				num2++;
				continue;
			}
			PhoneButton phoneButton = ((!button_prefab || !use_icons) ? (UnityEngine.Object.Instantiate(PhoneTextController.buttonprefab) as PhoneButton) : (UnityEngine.Object.Instantiate(button_prefab) as PhoneButton));
			PhoneScreen screen = controller.getScreen(text);
			if ((bool)screen)
			{
				screen.exit_dir = new Vector3(8f * Mathf.Cos(num), screen.exit_dir.y, 8f * Mathf.Sin(num));
			}
			if ((bool)phoneButton.button_icon && use_icons)
			{
				phoneButton.pop_open = true;
				if ((bool)screen && (bool)screen.icon_texture)
				{
					phoneButton.button_icon.renderer.material.mainTexture = screen.icon_texture;
				}
			}
			phoneButton.transform.position = position;
			phoneButton.transform.parent = base.transform;
			phoneButton.textmesh.text = text;
			phoneButton.button_name = text;
			phoneButton.command = "load_screen " + text;
			phoneButton.screen = this;
			phoneButton.textmesh.alignment = TextAlignment.Center;
			phoneButton.textmesh.anchor = TextAnchor.MiddleCenter;
			phoneButton.animateOnLoad = true;
			buttons.Add(phoneButton);
			auto_buttons.Add(phoneButton);
			phoneButton.Init();
			phoneButton.transform.position = base.transform.position;
			if ((bool)screen)
			{
				phoneButton = screen.Button_To(phoneButton);
			}
			num += add_angle;
			position = centerpos + Vector3.right * hradius * Mathf.Cos(num) + Vector3.forward * vradius * Mathf.Sin(num);
			num2++;
		}
	}

	protected virtual void AutoCreateButtons_Vertical()
	{
		Vector3 position = base.transform.position + base.transform.forward * 3.2f + base.transform.up + base.transform.right * 2.2f;
		string[] array = menu_items;
		foreach (string text in array)
		{
			PhoneButton phoneButton = UnityEngine.Object.Instantiate(PhoneTextController.buttonprefab) as PhoneButton;
			phoneButton.transform.position = position;
			phoneButton.transform.parent = base.transform;
			phoneButton.textmesh.text = text;
			phoneButton.button_name = text;
			phoneButton.command = "load_screen " + text;
			phoneButton.screen = this;
			phoneButton.textmesh.alignment = TextAlignment.Right;
			phoneButton.textmesh.anchor = TextAnchor.MiddleRight;
			phoneButton.animateOnLoad = true;
			buttons.Add(phoneButton);
			auto_buttons.Add(phoneButton);
			position += base.transform.forward * -0.75f;
			phoneButton.Init();
		}
	}

	protected virtual void AutoCreateButtons()
	{
		if (radial_menu)
		{
			AutoCreateButtons_Radial();
		}
		else
		{
			AutoCreateButtons_Vertical();
		}
	}

	protected void ClearAutoButtons()
	{
		foreach (PhoneButton auto_button in auto_buttons)
		{
			buttons.Remove(auto_button);
			UnityEngine.Object.Destroy(auto_button.gameObject);
		}
		auto_buttons.Clear();
	}

	protected virtual void SetupButtons()
	{
		PhoneButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<PhoneButton>();
		foreach (PhoneButton item in componentsInChildren)
		{
			if (!buttons.Contains(item))
			{
				buttons.Add(item);
			}
		}
		if (autocreatebuttons)
		{
			AutoCreateButtons();
		}
		if (sort_buttonlist && buttons != null)
		{
			buttons.Sort((PhoneButton b1, PhoneButton b2) => b2.transform.position.z.CompareTo(b1.transform.position.z));
		}
		if (!radial_menu && auto_button_directions)
		{
			for (int j = 0; j < buttons.Count - 1; j++)
			{
				if (!buttons[j].down_button)
				{
					buttons[j].down_button = buttons[j + 1];
				}
				if (!buttons[j].down_button.up_button)
				{
					buttons[j].down_button.up_button = buttons[j];
				}
			}
		}
		UpdateMenuItems();
	}

	public override void UpdateScreen()
	{
		UpdateElements();
		int num = menuind;
		MenuControls();
		if (num != menuind)
		{
			UpdateMenuItems();
		}
	}

	protected void UpdateElements()
	{
		PhoneElement[] componentsInChildren = GetComponentsInChildren<PhoneElement>();
		foreach (PhoneElement phoneElement in componentsInChildren)
		{
			phoneElement.OnUpdate();
		}
	}

	protected virtual void DoMouseControls()
	{
		if ((bool)stick_trans && (bool)stick_trans.renderer)
		{
			stick_trans.renderer.enabled = false;
		}
		menuind = -1;
		Vector3 touchPoint = PhoneInput.GetTouchPoint();
		float num = float.NegativeInfinity;
		if (touchPoint != Vector3.zero * -1f)
		{
			Vector3 point = PhoneInput.TransformPoint(touchPoint);
			for (int i = 0; i < current_buttons.Count; i++)
			{
				if (current_buttons[i].selectable)
				{
					point.y = current_buttons[i].GetBounds().center.y;
					if (point.y > num && current_buttons[i].gameObject.active && current_buttons[i].ContainsPoint(point))
					{
						menuind = i;
						num = point.y;
					}
				}
			}
		}
		if (menuind >= 0 && PhoneInput.IsPressedDown())
		{
			current_buttons[menuind].OnPressed();
		}
	}

	protected virtual void DoStickControls()
	{
		if (radial_menu)
		{
			StickControls_Radial();
		}
		else
		{
			StickControls_Vertical();
		}
	}

	protected virtual void StickControls_Radial()
	{
		Vector2 controlDir = PhoneInput.GetControlDir();
		if ((bool)stick_trans && (bool)stick_trans.renderer)
		{
			stick_trans.renderer.enabled = true;
			stick_trans.transform.position = centerpos + new Vector3(controlDir.x, 1f, controlDir.y);
		}
		if (controlDir.magnitude > 0.6f)
		{
			float num = 30f;
			int num2 = -1;
			Vector3 vector = new Vector3(controlDir.x, 0f, controlDir.y) * 0.75f;
			for (int i = 0; i < buttons.Count; i++)
			{
				Vector3 vector2 = buttons[i].transform.position - base.transform.position;
				vector2.y = 0f;
				float num3 = Vector3.Angle(vector.normalized, vector2.normalized);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num2 != -1)
			{
				menuind = num2;
			}
		}
		if (PhoneInput.IsPressedDown() && buttons.Count > menuind && menuind >= 0)
		{
			buttons[menuind].OnPressed();
		}
	}

	public virtual bool SwitchToButton(PhoneButton button)
	{
		if (button == null)
		{
			return false;
		}
		menuind = current_buttons.IndexOf(button);
		UpdateButtonSelected();
		return true;
	}

	protected virtual void StickControls_ButtonDir()
	{
		menuind = Mathf.Clamp(menuind, 0, current_buttons.Count - 1);
		PhoneButton phoneButton = current_buttons[menuind];
		bool flag = false;
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.y >= 0.5f)
		{
			flag = SwitchToButton(phoneButton.up_button);
		}
		else if (controlDirPressed.y <= -0.5f)
		{
			flag = SwitchToButton(phoneButton.down_button);
		}
		if (!flag)
		{
			if (controlDirPressed.x >= 0.5f)
			{
				flag = SwitchToButton(phoneButton.right_button);
			}
			else if (controlDirPressed.x <= -0.5f)
			{
				flag = SwitchToButton(phoneButton.left_button);
			}
		}
		if (PhoneInput.IsPressedDown() && current_buttons.Count > menuind)
		{
			current_buttons[menuind].OnPressed();
		}
	}

	protected virtual void StickControls_Vertical()
	{
		if (use_button_dir_control)
		{
			StickControls_ButtonDir();
			return;
		}
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
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
				menuind = Mathf.Max(0, current_buttons.Count - 1);
			}
			if (menuind >= current_buttons.Count)
			{
				menuind = 0;
			}
		}
		else
		{
			if (menuind < 0)
			{
				menuind = 0;
			}
			if (menuind >= current_buttons.Count)
			{
				menuind = Mathf.Max(0, current_buttons.Count - 1);
			}
		}
		if (PhoneInput.IsPressedDown() && current_buttons.Count > menuind)
		{
			current_buttons[menuind].OnPressed();
		}
	}

	protected virtual void MenuControls()
	{
		if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
		{
			DoMouseControls();
		}
		else
		{
			DoStickControls();
		}
	}

	protected virtual void UpdateButtonSelected()
	{
		for (int i = 0; i < current_buttons.Count; i++)
		{
			current_buttons[i].selected = i == menuind;
			if (i == menuind)
			{
				current_buttons[i].OnSelected();
				SetMenuLines(current_buttons[i]);
			}
		}
	}

	protected virtual void SetMenuLines(PhoneButton button)
	{
		PhoneMenuLine[] menulines = controller.menulines;
		foreach (PhoneMenuLine phoneMenuLine in menulines)
		{
			phoneMenuLine.start = button;
		}
		controller.menulines[0].end = button.up_button;
		controller.menulines[1].end = button.down_button;
		controller.menulines[2].end = button.left_button;
		controller.menulines[3].end = button.right_button;
	}

	protected void UpdateMenuItems()
	{
		UpdateButtonSelected();
		if ((bool)label)
		{
			if (menuind >= 0 && menuind < current_buttons.Count && use_icons)
			{
				label.text = current_buttons[menuind].button_name;
			}
			else
			{
				label.text = string.Empty;
			}
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message.StartsWith("trailcolor_"))
		{
			PhoneButtonSlider phoneButtonSlider = button as PhoneButtonSlider;
			Color trailColor = PhoneInterface.trailColor;
			if (message.EndsWith("r"))
			{
				trailColor.r = phoneButtonSlider.val;
			}
			else if (message.EndsWith("g"))
			{
				trailColor.g = phoneButtonSlider.val;
			}
			else if (message.EndsWith("b"))
			{
				trailColor.b = phoneButtonSlider.val;
			}
			PhoneInterface.trailColor = trailColor;
			return true;
		}
		if (message.StartsWith("robotcolor_"))
		{
			PhoneButtonSlider phoneButtonSlider2 = button as PhoneButtonSlider;
			Color robotColor = PhoneInterface.robotColor;
			if (message.EndsWith("r"))
			{
				robotColor.r = phoneButtonSlider2.val;
			}
			else if (message.EndsWith("g"))
			{
				robotColor.g = phoneButtonSlider2.val;
			}
			else if (message.EndsWith("b"))
			{
				robotColor.b = phoneButtonSlider2.val;
			}
			PhoneInterface.robotColor = robotColor;
			return true;
		}
		if (message.StartsWith("bgcolor_"))
		{
			if (message.StartsWith("bgcolor_r"))
			{
				PhoneButtonSlider phoneButtonSlider3 = button as PhoneButtonSlider;
				Color back = PhoneMemory.settings.Palette.back;
				back.r = phoneButtonSlider3.val;
				PhoneMemory.settings.Palette.back = back;
				PhoneController.instance.SetBackColor(back);
				return true;
			}
			if (message.StartsWith("bgcolor_g"))
			{
				PhoneButtonSlider phoneButtonSlider4 = button as PhoneButtonSlider;
				Color back2 = PhoneMemory.settings.Palette.back;
				back2.g = phoneButtonSlider4.val;
				PhoneMemory.settings.Palette.back = back2;
				PhoneController.instance.SetBackColor(back2);
				return true;
			}
			if (message.StartsWith("bgcolor_b"))
			{
				PhoneButtonSlider phoneButtonSlider5 = button as PhoneButtonSlider;
				Color back3 = PhoneMemory.settings.Palette.back;
				back3.b = phoneButtonSlider5.val;
				PhoneMemory.settings.Palette.back = back3;
				PhoneController.instance.SetBackColor(back3);
				return true;
			}
		}
		else if (message.StartsWith("volume_"))
		{
			if (message.StartsWith("volume_menu"))
			{
				PhoneButtonSlider phoneButtonSlider6 = button as PhoneButtonSlider;
				PhoneMemory.settings.menu_volume = phoneButtonSlider6.val;
				return true;
			}
			if (message.StartsWith("volume_game"))
			{
				PhoneButtonSlider phoneButtonSlider7 = button as PhoneButtonSlider;
				PhoneMemory.settings.game_volume = phoneButtonSlider7.val;
				return true;
			}
			if (message.StartsWith("volume_music"))
			{
				PhoneButtonSlider phoneButtonSlider8 = button as PhoneButtonSlider;
				PhoneMemory.settings.music_volume = phoneButtonSlider8.val;
				return true;
			}
			if (message.StartsWith("volume_ring"))
			{
				PhoneButtonSlider phoneButtonSlider9 = button as PhoneButtonSlider;
				PhoneMemory.settings.ring_volume = phoneButtonSlider9.val;
				return true;
			}
			if (message.StartsWith("volume_master"))
			{
				PhoneButtonSlider phoneButtonSlider10 = button as PhoneButtonSlider;
				PhoneMemory.settings.master_volume = phoneButtonSlider10.val;
				return true;
			}
			if (message.StartsWith("volume_vibrate"))
			{
				PhoneButtonSlider phoneButtonSlider11 = button as PhoneButtonSlider;
				PhoneMemory.settings.vibrate_amount = phoneButtonSlider11.val;
				return true;
			}
		}
		else if (message.StartsWith("debug_"))
		{
			if (message.StartsWith("debug_mission_dlc"))
			{
				MissionLoadControl.instance.dogui = !MissionLoadControl.instance.dogui;
				button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
				button.text += ((!MissionLoadControl.instance.dogui) ? "(off)" : "(on)");
			}
			else if (message.StartsWith("debug_mouse_cam"))
			{
				NewCamera.use_mouse_look = !NewCamera.use_mouse_look;
				button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
				button.text += ((!NewCamera.use_mouse_look) ? "(off)" : "(on)");
			}
			else if (message.StartsWith("debug_mmo"))
			{
				Networking.instance.enabled = !Networking.instance.enabled;
				button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
				button.text += ((!Networking.instance.enabled) ? "(off)" : "(on)");
			}
			else if (message.StartsWith("debug_wall_control"))
			{
				PhoneInterface.player_move.V1 = !PhoneInterface.player_move.V1;
				button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
				button.text += ((!PhoneInterface.player_move.V1) ? "(off)" : "(on)");
			}
		}
		else if (message.StartsWith("invert_phone_stick"))
		{
			PhoneInput.invert_stick = !PhoneInput.invert_stick;
			button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
			button.text += ((!PhoneInput.invert_stick) ? "(off)" : "(on)");
		}
		else if (message.StartsWith("speedrun_toggle"))
		{
			var old_state = PlayerPrefsX.GetBool("speedrun_mode", false);
			PlayerPrefsX.SetBool("speedrun_mode", !old_state);
			if (old_state && SpeedrunTimer.instance != null)
			{
				 SpeedrunTimer.instance.gameObject.SetActiveRecursively(SpeedrunTimer.instance.enabled = false);
			}
			button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
			button.text += !PlayerPrefsX.GetBool("speedrun_mode", false) ? "(off)" : "(on)";
		}
		else if (message.StartsWith("player_radar"))
		{
			PhoneMapController.player_radar = !PhoneMapController.player_radar;
			PlayerPrefsX.SetBool("player_radar", PhoneMapController.player_radar);
			button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
			button.text += !PhoneMapController.player_radar ? "(off)" : "(on)";
		}
		else if (message.StartsWith("discord_rpc"))
		{
			PlayerPrefsX.SetBool("discord_rpc", !PlayerPrefsX.GetBool("discord_rpc", false));
			DiscordController.instance.enabled = PlayerPrefsX.GetBool("discord_rpc");
			button.text = button.text.Replace("(on)", string.Empty).Replace("(off)", string.Empty);
			button.text += !PlayerPrefsX.GetBool("discord_rpc", false) ? "(off)" : "(on)";
		}
		else if (message.StartsWith("cycle_speedrun_type") && SpeedrunTimer.instance != null)
		{
			PlayerPrefsX.SetEnum("speedrun_type", SpeedrunTimer.instance.CycleRunType());
			button.text = string.Format("Run Type ({0})", PlayerPrefsX.GetEnum("speedrun_type", SpeedrunTimer.RunTypes.Manual));
		}
		else if (message.StartsWith("cycle_phone_theme"))
		{
			PhoneMemory.CycleSelectedTheme();
			PhoneController.instance.SetPhoneTheme(PhoneMemory.current_theme);
			PlayerPrefs.SetString("phone_theme", PhoneMemory.current_theme);
			button.text = string.Format("Phone Theme ({0})", PlayerPrefs.GetString("phone_theme", "white"));
		}
		return true;
	}
	
	public static PhoneMainMenu CreateMenu(string name, PhoneController controller)
	{
		var menuScreen = new GameObject(string.Format("CustomMenu{0}", name));

		menuScreen.transform.parent = GameObject.Find("PhoneScene").transform;
		menuScreen.transform.localScale = new Vector3(4.8f, 0.2f, 8.0f);
		menuScreen.transform.localPosition = Vector3.zero;

		menuScreen.AddComponent<MeshFilter>();
		menuScreen.AddComponent<MeshRenderer>();
		var menu = menuScreen.AddComponent<PhoneMainMenu>();
		menu.screenname = name;
		menu.controller = controller;

		return menu;
	}
}
