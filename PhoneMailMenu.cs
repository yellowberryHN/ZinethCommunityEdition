using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMailMenu : PhoneMainMenu
{
	protected enum mailmode
	{
		inbox,
		single
	}

	protected class Zone
	{
		public List<PhoneButton> buttons = new List<PhoneButton>();

		public int menuind;

		public bool vertical = true;

		public bool horizontal;

		public bool wrap;

		public Zone left_zone;

		public Zone right_zone;

		public Zone up_zone;

		public Zone down_zone;

		public virtual void StickControls()
		{
			Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
			if (vertical)
			{
				if (controlDirPressed.y >= 0.5f)
				{
					menuind--;
				}
				if (controlDirPressed.y <= -0.5f)
				{
					menuind++;
				}
				DoWrapping();
			}
		}

		private void DoWrapping()
		{
			if (wrap)
			{
				if (menuind < 0)
				{
					menuind = buttons.Count - 1;
				}
				else if (menuind >= buttons.Count)
				{
					menuind = 0;
				}
			}
			else
			{
				menuind = Mathf.Clamp(menuind, 0, buttons.Count - 1);
			}
		}
	}

	protected enum MenuZone
	{
		Left,
		Center,
		Right,
		Message
	}

	public PhoneLabel mail_text_label;

	public PhoneLabel mail_subject_label;

	public PhoneLabel mail_sender_label;

	public PhoneLabel mail_title_label;

	public PhoneButton mail_back_button;

	public PhoneButton mail_reply_button;

	public PhoneButton nextbut;

	public PhoneButton prevbut;

	public Transform message_holder;

	private Vector3 message_holder_offset;

	private MenuZone oldzone;

	public List<PhoneButton> mailbuttons = new List<PhoneButton>();

	public float maildistance = 0.5f;

	public float mailxpos = -1.8f;

	public bool space_by_bounds;

	public bool setting_up_mail;

	protected Bounds mail_bounds = default(Bounds);

	public float first_mail_forward_pos = 3f;

	public string inbox_exit_text = "E\nx\ni\nt\n\nM\na\ni\nl";

	protected mailmode mode;

	public PhoneMail current_mail;

	protected int mailindex;

	protected int oldmenuind;

	protected int center_menu_ind;

	private Vector3 spawnpos;

	public PhoneLabel newmail_prefab;

	private Vector3 lastmousepos = Vector3.one * -1f;

	protected MenuZone zone = MenuZone.Center;

	public List<PhoneButton> left_buttons;

	public List<PhoneButton> right_buttons;

	private int tempmenuind;

	protected virtual List<PhoneMail> mail_list
	{
		get
		{
			return PhoneMemory.messages;
		}
	}

	private int mailsize
	{
		get
		{
			return PhoneMemory.messages.Count;
		}
	}

	public virtual List<PhoneButton> center_buttons
	{
		get
		{
			return mailbuttons;
		}
	}

	public override List<PhoneButton> current_buttons
	{
		get
		{
			if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
			{
				return buttons;
			}
			if (mode == mailmode.single)
			{
				return buttons;
			}
			if (zone == MenuZone.Left)
			{
				return left_buttons;
			}
			if (zone == MenuZone.Center)
			{
				return center_buttons;
			}
			if (zone == MenuZone.Right)
			{
				return right_buttons;
			}
			return buttons;
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
		if (mail_text_label == null)
		{
			mail_text_label = base.transform.FindChild("MailTextLabel").GetComponent<PhoneLabel>();
		}
		if (mail_subject_label == null)
		{
			mail_subject_label = base.transform.FindChild("MailSubjectLabel").GetComponent<PhoneLabel>();
		}
		if (mail_sender_label == null)
		{
			mail_sender_label = base.transform.FindChild("MailSenderLabel").GetComponent<PhoneLabel>();
		}
		if (mail_title_label == null)
		{
			mail_title_label = base.transform.FindChild("MailTitleLabel").GetComponent<PhoneLabel>();
		}
		if (mail_back_button == null)
		{
			mail_back_button = base.transform.FindChild("MailBackButton").GetComponent<PhoneButton>();
		}
		if (mail_reply_button == null)
		{
			mail_reply_button = base.transform.FindChild("MailReplyButton").GetComponent<PhoneButton>();
		}
		if (message_holder == null)
		{
			message_holder = base.transform.FindChild("MessageHolder").transform;
		}
		if ((bool)message_holder)
		{
			message_holder_offset = message_holder.localPosition;
		}
	}

	public override void Init()
	{
		if (button_prefab == null)
		{
			button_prefab = PhoneTextController.buttonprefab;
		}
		base.Init();
	}

	public override void UpdateScreen()
	{
		CheckUpdates();
		if (zone != oldzone)
		{
			UpdateMenuItems();
		}
		oldzone = zone;
		base.UpdateScreen();
	}

	protected virtual bool CheckUpdates()
	{
		if (PhoneMemory.mail_updated)
		{
			if (mode == mailmode.inbox)
			{
				SetupMail();
			}
			PhoneMemory.mail_updated = false;
			return true;
		}
		return false;
	}

	protected virtual void LoadAnimationCancel()
	{
		if (exit_animating)
		{
			Debug.LogWarning("looks like it is exit animating...");
			cancel_exit_animate = true;
		}
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		LoadAnimationCancel();
		menuind = 0;
		mode = mailmode.inbox;
		if (mail_list.Count <= 0 && zone == MenuZone.Center)
		{
			zone = MenuZone.Left;
		}
		current_mail = null;
		if (resetselection)
		{
			mailindex = 0;
			oldmenuind = 0;
		}
		message_holder.localPosition = message_holder_offset;
		buttons.Remove(mail_reply_button);
		mail_reply_button.OnUnSelected();
		if (!CheckUpdates())
		{
			SetupMail();
		}
		PhoneElement[] array = base.elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		UpdateMenuItems();
		mail_text_label.text = string.Empty;
		mail_sender_label.text = string.Empty;
		mail_subject_label.text = string.Empty;
		HideReplyButton();
		lastmousepos = Vector3.one * -1f;
		if (menuind < current_buttons.Count)
		{
			current_buttons[menuind].OnSelected();
		}
	}

	protected virtual void SetupMail()
	{
		if (!setting_up_mail)
		{
			setting_up_mail = true;
			StartCoroutine("Co_SetupMail");
		}
	}

	protected virtual IEnumerator Co_SetupMail()
	{
		mail_bounds = default(Bounds);
		mail_back_button.textmesh.text = inbox_exit_text;
		if ((bool)mail_title_label)
		{
			mail_title_label.textmesh.text = string.Empty;
		}
		ClearMailButtons();
		Vector3 pos = base.transform.forward * first_mail_forward_pos + base.transform.up + base.transform.right * mailxpos;
		int old_menuind = menuind;
		int mailind = 0;
		List<PhoneMail> temp_list = new List<PhoneMail>(mail_list);
		PhoneButton lastbutton = null;
		foreach (PhoneMail message in temp_list)
		{
			PhoneButton button = Object.Instantiate(button_prefab) as PhoneButton;
			button.transform.parent = message_holder;
			button.transform.localPosition = pos;
			button.wantedpos = pos;
			string subjtext = ((!message.is_new) ? message.subject : ("*" + message.subject));
			int maxlength = 20;
			if (subjtext.Length > maxlength)
			{
				subjtext = subjtext.TrimEnd(' ').Substring(0, maxlength - 3).TrimEnd(' ') + "...";
			}
			button.button_name = subjtext;
			button.screen = this;
			button.command = ".openmessage " + mailind;
			button.id_info = message.id;
			button.up_button = lastbutton;
			if ((bool)lastbutton)
			{
				lastbutton.down_button = button;
			}
			button.left_button = mail_back_button;
			if (right_buttons.Count > 0)
			{
				button.right_button = right_buttons[0];
			}
			lastbutton = button;
			button.textmesh.characterSize = 0.65f;
			button.textmesh.alignment = TextAlignment.Left;
			button.textmesh.anchor = TextAnchor.MiddleLeft;
			button.pressed_particles = false;
			button.animateRate = 7f;
			mailbuttons.Add(button);
			buttons.Add(button);
			button.id_info = message.id;
			button.Init();
			button.OnLoad();
			if (message != current_mail && button.animateOnLoad)
			{
				button.transform.position = PhoneController.presspos;
				if (mailind % 2 == 0)
				{
					button.transform.position += base.transform.right * 3f;
				}
				else
				{
					button.transform.position -= base.transform.right * 3f;
				}
			}
			if (space_by_bounds)
			{
				pos.z -= button_prefab.GetBounds().size.z;
			}
			pos += base.transform.forward * (0f - maildistance);
			mailind++;
			yield return null;
		}
		if (sort_buttonlist)
		{
			buttons.Sort((PhoneButton b1, PhoneButton b2) => b2.wantedpos.z.CompareTo(b1.wantedpos.z));
		}
		menuind = old_menuind;
		if (menuind < 0)
		{
			menuind = 0;
		}
		if (buttons.Count > menuind)
		{
			mail_back_button.right_button = buttons[menuind];
		}
		setting_up_mail = false;
		UpdateButtonSelected();
	}

	private void ClearMailButtons()
	{
		foreach (PhoneButton mailbutton in mailbuttons)
		{
			buttons.Remove(mailbutton);
			Object.Destroy(mailbutton.gameObject);
		}
		mailbuttons.Clear();
	}

	public virtual void NextMail()
	{
		mailindex++;
		if (mailindex >= mailsize)
		{
			mailindex = 0;
		}
		OpenMail(mailindex);
		if ((bool)nextbut)
		{
			nextbut.transform.position -= base.transform.right * 0.15f;
		}
	}

	public virtual void PreviousMail()
	{
		mailindex--;
		if (mailindex < 0)
		{
			mailindex = mailsize - 1;
		}
		OpenMail(mailindex);
		if ((bool)prevbut)
		{
			prevbut.transform.position += base.transform.right * 0.15f;
		}
	}

	public virtual void OpenMail(string mail_id)
	{
		OpenMail(MailController.FindMail(mail_id));
	}

	public virtual void OpenMail(int index)
	{
		oldmenuind = menuind;
		mailindex = index;
		OpenMail(PhoneMemory.GetMail(mailindex));
	}

	protected virtual void OpenMail(PhoneMail mail)
	{
		if (zone == MenuZone.Center && mode == mailmode.inbox)
		{
			center_menu_ind = menuind;
		}
		current_mail = mail;
		mode = mailmode.single;
		zone = MenuZone.Left;
		ClearMailButtons();
		if (setting_up_mail)
		{
			StopCoroutine("Co_SetupMail");
			setting_up_mail = false;
		}
		mail_subject_label.transform.position = spawnpos;
		mail_sender_label.transform.localPosition = mail_sender_label.wantedpos + base.transform.right * -2f;
		mail_text_label.transform.localPosition = mail_text_label.wantedpos + base.transform.right * 2f;
		if ((bool)mail_subject_label)
		{
			mail_subject_label.text = mail.subject;
		}
		if ((bool)mail_sender_label)
		{
			mail_sender_label.text = "From: " + mail.sender;
		}
		if ((bool)mail_text_label)
		{
			mail_text_label.text = mail.body;
		}
		mail_back_button.textmesh.text = "Close";
		mail_back_button.transform.position = spawnpos;
		mail_back_button.pressed_particles = false;
		if (mail.can_reply)
		{
			ShowReplyButton();
			mail_reply_button.textmesh.text = mail.accept_button_text;
		}
		else
		{
			HideReplyButton();
		}
		mail_reply_button.transform.position += base.transform.right * -0.2f;
		if (current_mail.can_reply)
		{
			buttons.Add(mail_reply_button);
			mail_back_button.right_button = mail_reply_button;
		}
		else
		{
			mail_back_button.right_button = null;
		}
		mail_title_label.text = string.Empty;
		menuind = 0;
		UpdateButtonSelected();
		mail.OnOpen();
	}

	protected virtual void HideReplyButton()
	{
		mail_reply_button.renderer.enabled = false;
		if ((bool)mail_reply_button.background_box)
		{
			mail_reply_button.background_box.renderer.enabled = false;
		}
		mail_reply_button.OnUnSelected();
		mail_reply_button.textmesh.text = string.Empty;
	}

	protected virtual void ShowReplyButton()
	{
		mail_reply_button.renderer.enabled = true;
		if ((bool)mail_reply_button.background_box)
		{
			mail_reply_button.background_box.renderer.enabled = true;
		}
	}

	protected virtual void CloseMail()
	{
		current_mail.OnClose();
		mode = mailmode.inbox;
		if ((bool)mail_text_label)
		{
			mail_text_label.text = string.Empty;
		}
		if ((bool)mail_sender_label)
		{
			mail_sender_label.text = string.Empty;
		}
		if ((bool)mail_subject_label)
		{
			mail_subject_label.text = string.Empty;
		}
		HideReplyButton();
		buttons.Remove(mail_reply_button);
		zone = MenuZone.Center;
		current_mail = null;
		menuind = center_menu_ind;
		if (base.gameObject.active)
		{
			SetupMail();
		}
		mail_back_button.textmesh.text = inbox_exit_text;
		mail_back_button.transform.localPosition = mail_back_button.wantedpos + Vector3.right * 2f;
		mail_back_button.pressed_particles = true;
	}

	private bool ReplyToMail()
	{
		bool result = current_mail.OnAccept();
		if (current_mail.delete_on_reply)
		{
			PhoneMemory.DeleteMail(current_mail);
		}
		CloseMail();
		return result;
	}

	public override bool ButtonMessage(PhoneButton button, string command)
	{
		switch (command)
		{
		case "reply":
			return ReplyToMail();
		case "next":
			if (mode == mailmode.single)
			{
				NextMail();
			}
			break;
		case "previous":
			if (mode == mailmode.single)
			{
				PreviousMail();
			}
			break;
		case "back":
			if (mode == mailmode.inbox)
			{
				controller.LoadScreen("Main_Menu");
			}
			else if (mode == mailmode.single)
			{
				CloseMail();
			}
			break;
		default:
			if (command.StartsWith("openmessage"))
			{
				spawnpos = button.transform.position;
				int index = mailbuttons.IndexOf(button);
				OpenMail(index);
				break;
			}
			return base.ButtonMessage(button, command);
		}
		return true;
	}

	public override PhoneButton Button_To(PhoneButton button)
	{
		if (!newmail_prefab)
		{
			return button;
		}
		PhoneLabel phoneLabel = Object.Instantiate(newmail_prefab) as PhoneLabel;
		phoneLabel.transform.position = button.transform.position + new Vector3(0.2f, 0.25f, -0.25f);
		phoneLabel.transform.parent = button.button_icon.transform;
		phoneLabel.wantedpos = phoneLabel.transform.localPosition;
		phoneLabel.wantedrot = phoneLabel.transform.localRotation;
		return button;
	}

	protected override void DoMouseControls()
	{
		base.DoMouseControls();
		DoMouseScrolling();
	}

	protected virtual void DoMouseScrolling()
	{
		Vector3 transformedTouchPoint = PhoneInput.GetTransformedTouchPoint();
		bool flag = PhoneInput.IsPressed() && mode != mailmode.single;
		if (PhoneInput.IsPressedDown() && mode != mailmode.single)
		{
			lastmousepos = transformedTouchPoint;
		}
		if (flag)
		{
			if (lastmousepos != Vector3.one * -1f)
			{
				float num = transformedTouchPoint.z - lastmousepos.z;
				message_holder.transform.position += Vector3.forward * num;
			}
			lastmousepos = transformedTouchPoint;
		}
		else
		{
			lastmousepos = Vector3.one * -1f;
		}
		float num2 = Input.GetAxis("Mouse ScrollWheel") * -5f;
		if (num2 != 0f && mode != mailmode.single)
		{
			message_holder.transform.position += Vector3.forward * num2;
		}
		Bounds bounds = default(Bounds);
		Bounds bounds2 = base.renderer.bounds;
		bounds2.center -= Vector3.forward * 0.25f;
		bounds2.size -= Vector3.forward * 1f;
		if (center_buttons.Count > 0 && center_buttons[0].gameObject.active)
		{
			bounds.Encapsulate(center_buttons[0].GetBounds());
		}
		if (center_buttons.Count > 1 && center_buttons[center_buttons.Count - 1].gameObject.active)
		{
			bounds.Encapsulate(center_buttons[center_buttons.Count - 1].GetBounds());
		}
		if (center_buttons.Count > 2 && center_buttons[center_buttons.Count - 2].gameObject.active)
		{
			bounds.Encapsulate(center_buttons[center_buttons.Count - 2].GetBounds());
		}
		if (bounds.size.z > bounds2.size.z)
		{
			if (bounds.max.z < bounds2.max.z)
			{
				message_holder.transform.position += Vector3.forward * (bounds2.max.z - bounds.max.z);
			}
			if (bounds.min.z > bounds2.min.z && bounds.size.z > bounds2.size.z)
			{
				message_holder.transform.position += -Vector3.forward * (bounds.min.z - bounds2.min.z);
			}
		}
		else
		{
			if (bounds.max.z > bounds2.max.z)
			{
				message_holder.transform.position += Vector3.forward * (bounds2.max.z - bounds.max.z);
			}
			if (bounds.min.z < bounds2.min.z)
			{
				message_holder.transform.position += -Vector3.forward * (bounds.min.z - bounds2.min.z);
			}
		}
	}

	protected virtual void StickControls_Vertical_Message()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.x >= 0.5f)
		{
			menuind++;
		}
		else if (controlDirPressed.x <= -0.5f)
		{
			menuind--;
		}
		if (menuind < 0)
		{
			menuind = 0;
		}
		if (menuind >= current_buttons.Count)
		{
			menuind = current_buttons.Count - 1;
		}
		if (PhoneInput.IsPressedDown() && current_buttons.Count > menuind)
		{
			current_buttons[menuind].OnPressed();
		}
	}

	protected virtual void StickControls_Vertical_Inbox()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (zone == MenuZone.Left)
		{
			if (controlDirPressed.x >= 0.5f && center_buttons.Count > 0)
			{
				int num = menuind;
				zone = MenuZone.Center;
				menuind = center_menu_ind;
				tempmenuind = num;
			}
			else if (controlDirPressed.y >= 0.5f)
			{
				if (SwitchToButton(current_buttons[menuind].up_button))
				{
					UpdateButtonSelected();
				}
			}
			else if (controlDirPressed.y <= -0.5f && SwitchToButton(current_buttons[menuind].down_button))
			{
				UpdateButtonSelected();
			}
		}
		else if (zone == MenuZone.Center)
		{
			if (controlDirPressed.x <= -0.5f && left_buttons.Count > 0)
			{
				int num2 = menuind;
				zone = MenuZone.Left;
				menuind = 0;
				tempmenuind = num2;
			}
			else if (controlDirPressed.x >= 0.5f && right_buttons.Count > 0)
			{
				int num3 = menuind;
				zone = MenuZone.Right;
				menuind = tempmenuind;
				tempmenuind = num3;
			}
		}
		else if (zone == MenuZone.Right && controlDirPressed.x <= -0.5f && center_buttons.Count > 0)
		{
			int num4 = menuind;
			zone = MenuZone.Center;
			menuind = tempmenuind;
			tempmenuind = num4;
			menuind = center_menu_ind;
		}
		menuind = Mathf.Clamp(menuind, 0, current_buttons.Count - 1);
		if (zone == MenuZone.Center)
		{
			center_menu_ind = menuind;
		}
		if (controlDirPressed.y >= 0.5f)
		{
			if ((bool)current_buttons[menuind].up_button)
			{
				menuind = current_buttons.IndexOf(current_buttons[menuind].up_button);
			}
		}
		else if (controlDirPressed.y <= -0.5f && (bool)current_buttons[menuind].down_button)
		{
			menuind = current_buttons.IndexOf(current_buttons[menuind].down_button);
		}
		menuind = Mathf.Clamp(menuind, 0, current_buttons.Count - 1);
		if (zone == MenuZone.Center && menuind >= 0)
		{
			Bounds bounds = base.renderer.bounds;
			Vector3 size = bounds.size;
			size.z *= 0.9f;
			bounds.size = size;
			Vector3 max = bounds.max;
			max.z -= 0.3f;
			bounds.max = max;
			PhoneButton phoneButton = current_buttons[menuind];
			Vector3 center = phoneButton.GetCenter();
			center.z = phoneButton.GetBounds().max.z;
			center.y = bounds.center.y;
			Vector3 point = center * 1f;
			point.z = phoneButton.GetBounds().min.z;
			bool flag = !phoneButton.animateOnLoad || phoneButton.transform.localPosition == phoneButton.wantedpos;
			if (!flag && Vector3.Distance(phoneButton.transform.localPosition, phoneButton.wantedpos) <= 0.01f)
			{
				flag = true;
				phoneButton.transform.localPosition = phoneButton.wantedpos;
			}
			if (flag)
			{
				if (!bounds.Contains(center) && center.z > base.transform.position.z)
				{
					message_holder.transform.localPosition += Vector3.forward * base.deltatime * -1f * Mathf.Abs(center.z - base.transform.position.z) / 5f;
				}
				else if (!bounds.Contains(point) && point.z < base.transform.position.z)
				{
					message_holder.transform.localPosition += Vector3.forward * base.deltatime * 1f * Mathf.Abs(point.z - base.transform.position.z) / 5f;
				}
			}
		}
		if (PhoneInput.IsPressedDown() && current_buttons.Count > menuind && menuind >= 0)
		{
			current_buttons[menuind].OnPressed();
		}
	}

	protected override void StickControls_Vertical()
	{
		if (mode == mailmode.single)
		{
			StickControls_Vertical_Message();
		}
		else
		{
			StickControls_Vertical_Inbox();
		}
	}

	protected override void UpdateButtonSelected()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			bool flag = false;
			if (menuind < current_buttons.Count && menuind >= 0 && current_buttons[menuind] == buttons[i])
			{
				flag = true;
			}
			buttons[i].selected = flag;
			if (flag)
			{
				buttons[i].OnSelected();
				SetMenuLines(buttons[i]);
			}
		}
	}
}
