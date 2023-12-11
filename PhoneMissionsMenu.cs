using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMissionsMenu : PhoneMailMenu
{
	public PhoneElement focus_marker;

	protected override List<PhoneMail> mail_list
	{
		get
		{
			return MissionController.ActiveMissionsAsMail();
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	protected override bool CheckUpdates()
	{
		if (MissionController.refresh)
		{
			if (Application.isEditor)
			{
				MonoBehaviour.print("refreshing");
			}
			if (mode == mailmode.inbox)
			{
				SetupMail();
			}
			MissionController.refresh = false;
			return true;
		}
		return false;
	}

	protected override void SetupMail()
	{
		HideFocusMarker();
		base.SetupMail();
		mail_title_label.text = "<Missions " + MissionController.completed_missions.Count + "/" + MissionController.all_missions.Count + ">";
		StartCoroutine("SetupFocusMarker");
	}

	private IEnumerator SetupFocusMarker()
	{
		yield return !setting_up_mail;
		while (setting_up_mail)
		{
			yield return null;
		}
		if (!MissionController.focus_mission)
		{
			yield break;
		}
		foreach (PhoneButton button in mailbuttons)
		{
			if ((bool)MissionController.focus_mission && "m_" + MissionController.focus_mission.id == button.id_info)
			{
				SetFocusMarker(button);
			}
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message.StartsWith("mission_focus"))
		{
			return SetFocus(current_mail);
		}
		if (message.StartsWith("accept"))
		{
			return true;
		}
		if (message.StartsWith("openmessage"))
		{
			return SetFocus(button);
		}
		return base.ButtonMessage(button, message);
	}

	protected virtual bool SetFocus(PhoneButton button)
	{
		bool refresh = MissionController.refresh;
		bool flag = false;
		if ((bool)MissionController.focus_mission && "m_" + MissionController.focus_mission.id == button.id_info)
		{
			focus_marker.renderer.enabled = false;
			MissionController.Unfocus();
			flag = true;
		}
		else
		{
			SetFocusMarker(button);
			flag = SetFocus(MailController.FindMail(button.id_info));
		}
		MissionController.refresh = refresh;
		return flag;
	}

	protected virtual bool SetFocus(PhoneMail mail)
	{
		if (mail == null)
		{
			return false;
		}
		string focus = mail.id.Substring(2);
		MissionController.SetFocus(focus);
		return true;
	}

	private void SetFocusMarker(PhoneButton button)
	{
		if ((bool)focus_marker)
		{
			focus_marker.renderer.enabled = true;
			focus_marker.renderer.material.color = button_prefab.back_normal_color;
			if (button.animateOnLoad)
			{
				MonoBehaviour.print("button animated");
				Vector3 wantedpos = button.wantedpos + Vector3.right * 2.4f - Vector3.forward * 0.4f;
				focus_marker.wantedpos = wantedpos;
			}
			else
			{
				Vector3 wantedpos = button.GetCenter();
				wantedpos.x = button.GetBounds().max.x + 0.25f;
				focus_marker.transform.position = wantedpos;
				focus_marker.wantedpos = focus_marker.transform.localPosition;
			}
		}
	}

	private void HideFocusMarker()
	{
		if ((bool)focus_marker)
		{
			focus_marker.renderer.enabled = false;
		}
	}
}
