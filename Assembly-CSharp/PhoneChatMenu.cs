using UnityEngine;

public class PhoneChatMenu : PhoneMainMenu
{
	public PhoneLabel text_label;

	public PhoneTextInput text_input;

	private string lastmessage = string.Empty;

	public bool use_network_chat = true;

	private int chat_limit = 14;

	public void SetText(string txt)
	{
		if (text_label.text != txt)
		{
			text_label.text = txt;
		}
	}

	public void AddEntry(string txt)
	{
		string text = text_label.text;
		if (text.Length > 0)
		{
			text += "\n";
		}
		text += txt;
		text_label.text = text;
		lastmessage = txt;
	}

	private void Awake()
	{
		Init();
		SetupChat();
	}

	public override void UpdateScreen()
	{
		base.UpdateScreen();
		UpdateChat();
	}

	private void SetupChat()
	{
		if (use_network_chat && Networking.chat_log != null)
		{
			SetText(string.Empty);
			for (int i = Mathf.Max(0, Networking.chat_log.Count - chat_limit); i < Networking.chat_log.Count; i++)
			{
				string txt = Networking.chat_log[i];
				AddEntry(txt);
			}
		}
	}

	private void UpdateChat()
	{
		if (use_network_chat && Networking.chat_log != null && Networking.chat_log.Count > 0)
		{
			SetupChat();
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message == "post_chat")
		{
			if (button.id_info != string.Empty)
			{
				if ((bool)Networking.instance && Networking.my_net_player != null)
				{
					Networking.instance.SendChatMessage(button.id_info);
				}
				else
				{
					AddEntry(button.id_info);
				}
				if ((bool)text_input)
				{
					text_input.input_text = string.Empty;
				}
			}
			return true;
		}
		return base.ButtonMessage(button, message);
	}
}
