using System.Collections.Generic;
using UnityEngine;

public class PhonePlayerListMenu : PhoneTwitterMenu
{
	public int numPlayers;

	public List<PhoneMail> player_list;

	protected override List<PhoneMail> mail_list
	{
		get
		{
			return player_list;
		}
	}

	public override List<PhoneButton> center_buttons
	{
		get
		{
			return mailbuttons;
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
		if (mail_back_button == null)
		{
			mail_back_button = base.transform.FindChild("MailBackButton").GetComponent<PhoneButton>();
		}
		if (message_holder == null)
		{
			message_holder = base.transform.FindChild("MessageHolder").transform;
		}
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		LoadAnimationCancel();
		if (zone != MenuZone.Center && center_buttons.Count > 0)
		{
			zone = MenuZone.Center;
		}
		if (mode == mailmode.single)
		{
			CloseMail();
		}
		RefreshTwitter();
		if (CheckUpdates())
		{
			RefreshList();
		}
		if ((bool)new_tweets_button)
		{
			new_tweets_button.gameObject.SetActiveRecursively(false);
		}
		if ((bool)single_message_holder)
		{
			single_message_holder.gameObject.SetActiveRecursively(false);
		}
		foreach (PhoneButton left_button in left_buttons)
		{
			left_button.OnLoad();
		}
		foreach (PhoneButton center_button in center_buttons)
		{
			center_button.OnLoad();
		}
		foreach (PhoneButton right_button in right_buttons)
		{
			right_button.OnLoad();
		}
		UpdateButtonSelected();
	}

	public override bool RefreshTwitter()
	{
		return true;
	}

	protected override bool CheckUpdates()
	{
		if ((bool)Networking.instance && Networking.netplayer_dic.Count != numPlayers && mode == mailmode.inbox)
		{
			numPlayers = Networking.netplayer_dic.Count;
			SetupMail();
			return true;
		}
		return false;
	}

	protected new virtual bool RefreshList()
	{
		PhoneMemory.mail_updated = false;
		menuind = 0;
		SetupMail();
		Vector3 localPosition = message_holder.transform.localPosition;
		localPosition.z = 0f;
		message_holder.transform.localPosition = localPosition;
		return true;
	}

	public static PhoneMail NetPlayerToMail(NetPlayer player)
	{
		PhoneMail phoneMail = new PhoneMail();
		phoneMail.subject = player.userName;
		return phoneMail;
	}

	public override void OpenMail(int index)
	{
		mailindex = index;
		OpenMail(mail_list[index]);
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		switch (message)
		{
		case "close":
			CloseMail();
			break;
		case "reply":
			return false;
		case "post_tweet":
			return false;
		case "get_mentions":
			return false;
		case "get_timeline":
			return false;
		default:
			if (message.StartsWith("openmessage"))
			{
				OpenMail(button.id_info);
				break;
			}
			if (message == "refresh")
			{
				RefreshList();
				break;
			}
			return base.ButtonMessage(button, message);
		}
		return true;
	}
}
