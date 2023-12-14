using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneMail
{
	public string id;

	public string sender;

	public string subject;

	public string body;

	[HideInInspector]
	public ulong id_number;

	public string image_url = string.Empty;

	public string accept_command = string.Empty;

	public string open_command = string.Empty;

	public string close_command = string.Empty;

	public bool can_delete = true;

	public bool can_reply;

	public string accept_button_text = "Reply";

	public bool delete_on_reply = true;

	public bool is_new = true;

	public Vector3 position = Vector3.zero;

	public DateTime time = new DateTime(0L);

	public List<string> media_urls = new List<string>();

	public List<string> link_urls = new List<string>();

	[NonSerialized]
	public Color color = Color.clear;

	public PhoneMail()
	{
	}

	public PhoneMail(string id_text)
	{
		id = id_text;
	}

	public PhoneMail(string id_text, string body_text)
	{
		id = id_text;
		body = body_text;
	}

	public PhoneMail(string id_text, string subject_text, string body_text, string sender_text)
	{
		id = id_text;
		subject = subject_text;
		body = body_text;
		sender = sender_text;
	}

	public PhoneMail(string id_text, string subject_text, string body_text, string sender_text, bool replyable)
	{
		id = id_text;
		subject = subject_text;
		body = body_text;
		sender = sender_text;
		can_reply = replyable;
	}

	public virtual bool OnOpen()
	{
		if (is_new)
		{
			is_new = false;
			PhoneMemory.SaveMail();
			if (open_command != string.Empty)
			{
				return PhoneController.DoPhoneCommand(open_command);
			}
		}
		return true;
	}

	public virtual bool OnClose()
	{
		if (close_command != string.Empty)
		{
			return PhoneController.DoPhoneCommand(close_command);
		}
		return true;
	}

	public virtual bool OnAccept()
	{
		if (accept_command != string.Empty)
		{
			return PhoneController.DoPhoneCommand(accept_command);
		}
		return true;
	}

	public virtual bool OnReject()
	{
		return true;
	}
}
