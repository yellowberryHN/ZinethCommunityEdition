using System.Collections.Generic;
using UnityEngine;

public class PhoneTwitterMenu : PhoneMailMenu
{
	public TwitterDemo demo;

	public float mailsep = 0.05f;

	public PhoneButton new_tweets_button;

	public PhoneElement[] load_elements;

	public List<PhoneButton> message_buttons = new List<PhoneButton>();

	public PhoneTweetButton single_tweet_obj;

	public Transform single_message_holder;

	public Renderer media_plane;

	public PhoneButton link_button;

	private Vector3 single_tweet_scale = Vector3.zero;

	private Vector3 single_tweet_pos = Vector3.zero;

	public float scroll_cutoff = 0.25f;

	public float scroll_counter;

	private static PhoneTwitterMenu _instance;

	protected override List<PhoneMail> mail_list
	{
		get
		{
			return demo.twitter_messages;
		}
	}

	public override List<PhoneButton> current_buttons
	{
		get
		{
			if (mode == mailmode.single)
			{
				return message_buttons;
			}
			return base.current_buttons;
		}
	}

	public override List<PhoneButton> center_buttons
	{
		get
		{
			List<PhoneButton> list = new List<PhoneButton>(mailbuttons);
			list.Add(new_tweets_button);
			return list;
		}
	}

	public static PhoneTwitterMenu instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneTwitterMenu)) as PhoneTwitterMenu;
			}
			return _instance;
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
		if (mail_reply_button == null)
		{
			mail_reply_button = base.transform.FindChild("MailReplyButton").GetComponent<PhoneButton>();
		}
		if (message_holder == null)
		{
			message_holder = base.transform.FindChild("MessageHolder").transform;
		}
	}

	public override void OnExit()
	{
		if (mode == mailmode.single)
		{
			CloseMail();
		}
		base.OnExit();
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		LoadAnimationCancel();
		if (zone != MenuZone.Center && center_buttons.Count > 0)
		{
			zone = MenuZone.Center;
			menuind = center_menu_ind;
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
		PhoneElement[] array = load_elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		UpdateButtonSelected();
	}

	public virtual bool RefreshTwitter()
	{
		return TwitterDemo.GetTimeLine();
	}

	protected override bool CheckUpdates()
	{
		if (demo.updated)
		{
			if (!new_tweets_button.gameObject.active)
			{
				new_tweets_button.gameObject.SetActiveRecursively(true);
			}
			string text = demo.newtweets + " new Tweet";
			if (demo.newtweets > 1)
			{
				text += "s";
			}
			if (new_tweets_button.text != text)
			{
				new_tweets_button.text = text;
			}
			if (mailbuttons.Count > 0)
			{
				mailbuttons[0].up_button = new_tweets_button;
				new_tweets_button.down_button = mailbuttons[0];
			}
			if (center_buttons.Count > 1)
			{
				center_buttons[0].up_button = new_tweets_button;
				new_tweets_button.down_button = center_buttons[0];
				UpdateButtonSelected();
			}
			return true;
		}
		if (new_tweets_button.gameObject.active)
		{
			new_tweets_button.gameObject.SetActiveRecursively(false);
		}
		return false;
	}

	protected virtual bool RefreshList()
	{
		demo.updated = false;
		demo.newtweets = 0;
		new_tweets_button.gameObject.SetActiveRecursively(false);
		menuind = 0;
		SetupMail();
		Vector3 localPosition = message_holder.transform.localPosition;
		localPosition.z = 0f;
		message_holder.transform.localPosition = localPosition;
		return true;
	}

	public override void OpenMail(int index)
	{
		mailindex = index;
		OpenMail(demo.twitter_messages[index]);
	}

	protected override void OpenMail(PhoneMail mail)
	{
		if (zone == MenuZone.Center && mode == mailmode.inbox)
		{
			center_menu_ind = menuind;
		}
		oldmenuind = menuind;
		current_mail = mail;
		mode = mailmode.single;
		foreach (PhoneButton message_button in message_buttons)
		{
			message_button.OnUnSelected();
		}
		menuind = 0;
		if (!single_tweet_obj)
		{
			single_tweet_obj = Object.Instantiate(button_prefab) as PhoneTweetButton;
			single_tweet_obj.transform.parent = single_message_holder;
			single_tweet_obj.transform.position = single_message_holder.position + Vector3.forward * first_mail_forward_pos + Vector3.up * 3f;
			single_tweet_obj.selected = true;
		}
		if (single_tweet_scale == Vector3.zero)
		{
			single_tweet_scale = single_tweet_obj.transform.localScale;
			single_tweet_pos = single_tweet_obj.transform.localPosition;
		}
		single_message_holder.gameObject.SetActiveRecursively(true);
		single_tweet_obj.my_mail = mail;
		single_tweet_obj.Init();
		single_tweet_obj.OnLoad();
		if ((bool)media_plane)
		{
			if (single_tweet_obj.my_mail.media_urls.Count > 0)
			{
				media_plane.renderer.enabled = true;
				Texture2D mainTexture = single_tweet_obj.NewImage(single_tweet_obj.my_mail.media_urls[0]);
				media_plane.renderer.material.mainTexture = mainTexture;
			}
			else
			{
				media_plane.renderer.enabled = false;
			}
		}
		single_tweet_obj.animateOnLoad = true;
		single_tweet_obj.wantedpos = single_tweet_pos;
		single_tweet_obj.transform.position = PhoneController.presspos;
		Vector3 localPosition = single_tweet_obj.transform.localPosition;
		localPosition.x = single_tweet_pos.x;
		localPosition.y = single_tweet_pos.y;
		single_tweet_obj.transform.localPosition = Vector3.Lerp(localPosition, single_tweet_pos, 0.5f);
		single_tweet_obj.wantedscale = single_tweet_scale;
		single_tweet_obj.changeScale = true;
		single_tweet_obj.transform.localScale = new Vector3(0.00433f, 0.00158f, 0.5f);
		if ((bool)link_button)
		{
			if (mail.link_urls.Count > 0)
			{
				link_button.selectable = true;
				link_button.id_info = mail.link_urls[0];
				if ((bool)link_button.down_button)
				{
					link_button.down_button.up_button = link_button;
				}
			}
			else
			{
				link_button.selectable = false;
				link_button.gameObject.SetActiveRecursively(false);
				if ((bool)link_button.down_button)
				{
					link_button.down_button.up_button = null;
				}
			}
		}
		UpdateButtonSelected();
		mail.OnOpen();
	}

	protected override void CloseMail()
	{
		current_mail.OnClose();
		current_mail = null;
		mode = mailmode.inbox;
		zone = MenuZone.Center;
		single_tweet_obj.selected = false;
		single_message_holder.gameObject.SetActiveRecursively(false);
		menuind = center_menu_ind;
		UpdateButtonSelected();
	}

	protected override void StickControls_Vertical_Inbox()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.y >= 0.5f)
		{
			scroll_counter = 1f;
		}
		else if (controlDirPressed.y <= -0.5f)
		{
			scroll_counter = -1f;
		}
		else
		{
			controlDirPressed = PhoneInput.GetControlDir();
			if (controlDirPressed.y > 0f && scroll_counter >= 1f)
			{
				scroll_counter += controlDirPressed.y * base.deltatime;
				if (scroll_counter - 1f >= scroll_cutoff)
				{
					scroll_counter -= scroll_cutoff;
					PhoneButton up_button = current_buttons[menuind].up_button;
					if ((bool)up_button)
					{
						menuind = current_buttons.IndexOf(up_button);
						UpdateButtonSelected();
					}
				}
			}
			else if (controlDirPressed.y < 0f && scroll_counter <= -1f)
			{
				scroll_counter += controlDirPressed.y * base.deltatime;
				if (scroll_counter + 1f <= 0f - scroll_cutoff)
				{
					scroll_counter += scroll_cutoff;
					PhoneButton down_button = current_buttons[menuind].down_button;
					if ((bool)down_button)
					{
						menuind = current_buttons.IndexOf(down_button);
						UpdateButtonSelected();
					}
				}
			}
			else
			{
				scroll_counter = 0f;
			}
		}
		base.StickControls_Vertical_Inbox();
	}

	protected override void StickControls_Vertical_Message()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		menuind = Mathf.Clamp(menuind, 0, current_buttons.Count - 1);
		if (controlDirPressed.x >= 0.5f)
		{
			PhoneButton right_button = current_buttons[menuind].right_button;
			if ((bool)right_button && right_button.gameObject.active && right_button.selectable)
			{
				menuind = current_buttons.IndexOf(right_button);
			}
		}
		else if (controlDirPressed.x <= -0.5f)
		{
			PhoneButton left_button = current_buttons[menuind].left_button;
			if ((bool)left_button && left_button.gameObject.active && left_button.selectable)
			{
				menuind = current_buttons.IndexOf(left_button);
			}
		}
		if (controlDirPressed.y >= 0.5f)
		{
			PhoneButton up_button = current_buttons[menuind].up_button;
			if ((bool)up_button && up_button.gameObject.active && up_button.selectable)
			{
				menuind = current_buttons.IndexOf(up_button);
			}
		}
		else if (controlDirPressed.y <= -0.5f)
		{
			PhoneButton down_button = current_buttons[menuind].down_button;
			if ((bool)down_button && down_button.gameObject.active && down_button.selectable)
			{
				menuind = current_buttons.IndexOf(down_button);
			}
		}
		if (Input.GetKeyDown(KeyCode.Semicolon) && single_tweet_obj.my_mail.media_urls.Count > 0)
		{
			PhoneInterface.ShowZine(media_plane.renderer.material.mainTexture as Texture2D, true);
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
		case "close":
			CloseMail();
			break;
		case "reply":
			if (TwitterDemo.instance._canTweet && current_mail != null && mode == mailmode.single)
			{
				bool result = ReplyToTweet(current_mail);
				CloseMail();
				return result;
			}
			return false;
		case "post_tweet":
			if (TwitterDemo.instance._canTweet)
			{
				return PostTweet();
			}
			return false;
		case "get_mentions":
			return TwitterDemo.GetMentions();
		case "get_timeline":
			return TwitterDemo.GetTimeLine();
		default:
			if (message.StartsWith("openmessage"))
			{
				OpenMail(button.id_info);
			}
			else if (message.StartsWith("open_link"))
			{
				if (button.id_info.StartsWith("m_"))
				{
					string focus = button.id_info.Remove(0, 2);
					MissionController.SetFocus(focus);
				}
				else
				{
					Application.OpenURL(button.id_info);
				}
			}
			else if (!message.StartsWith("focus_mission"))
			{
				if (!(message == "refresh"))
				{
					return base.ButtonMessage(button, message);
				}
				RefreshList();
			}
			break;
		}
		return true;
	}

	protected override void SetupButtons()
	{
		base.SetupButtons();
		menuind = 0;
		oldmenuind = 0;
	}

	protected virtual string MakeTweet()
	{
		return TweetComposer.MakeTweet();
	}

	public virtual bool PostTweet()
	{
		return PostTweet(MakeTweet());
	}

	public virtual bool PostTweet(string tweet)
	{
		if (Application.loadedLevelName == "Loader 1")
		{
			//tweet = "pos:" + PlaytomicController.TranslatePlayerPosToGPSString() + ";" + tweet;
		}
		return TwitterDemo.PostTweet(tweet);
	}

	protected virtual bool ReplyToTweet(string mailid)
	{
		return ReplyToTweet(MailController.FindTweet(mailid));
	}

	protected virtual bool ReplyToTweet(PhoneMail mail)
	{
		string id = mail.id;
		id = id.Replace("tw_", string.Empty);
		string text = MakeTweet();
		text = "replyto:" + id + ";" + mail.subject + " " + text;
		return PostTweet(text);
	}
}
