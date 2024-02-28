using System.Collections.Generic;
using UnityEngine;

public class MailController : MonoBehaviour
{
	public static bool refresh = false;

	public static Dictionary<string, PhoneMail> all_mail = new Dictionary<string, PhoneMail>();

	public static Dictionary<string, PhoneMail> all_tweets = new Dictionary<string, PhoneMail>();

	private static MailController _instance;

	public static List<PhoneMail> active_mail
	{
		get
		{
			return PhoneMemory.messages;
		}
	}

	public static List<PhoneMail> deleted_mail
	{
		get
		{
			return PhoneMemory.deleted_messages;
		}
	}

	public static MailController instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(MailController)) as MailController;
			}
			return _instance;
		}
	}

	public static PhoneMail FindMail(string mailid)
	{
		if (mailid.StartsWith("tw_"))
		{
			return FindTweet(mailid);
		}
		if (!all_mail.ContainsKey(mailid))
		{
			return null;
		}
		return all_mail[mailid];
	}

	public static PhoneMail FindTweet(string mailid)
	{
		if (!all_tweets.ContainsKey(mailid))
		{
			return null;
		}
		return all_tweets[mailid];
	}

	public static void AddMail(PhoneMail mailobj)
	{
		if (mailobj.id.StartsWith("tw_") && !all_tweets.ContainsKey(mailobj.id))
		{
			all_tweets.Add(mailobj.id, mailobj);
		}
		if (!all_mail.ContainsKey(mailobj.id))
		{
			all_mail.Add(mailobj.id, mailobj);
		}
	}

	public static bool SendMailQuiet(string mailid)
	{
		return SendMailQuiet(FindMail(mailid));
	}

	public static bool SendMailQuiet(PhoneMail mailobj)
	{
		if (mailobj == null)
		{
			return false;
		}
		if (!all_mail.ContainsKey(mailobj.id))
		{
			AddMail(mailobj);
		}
		PhoneMemory.SendMailQuiet(mailobj);
		return true;
	}

	public static bool SendMail(string mailid)
	{
		return SendMail(FindMail(mailid));
	}

	public static bool SendMail(PhoneMail mailobj)
	{
		if (mailobj == null)
		{
			return false;
		}
		if (!all_mail.ContainsKey(mailobj.id))
		{
			AddMail(mailobj);
		}
		PhoneMemory.SendMail(mailobj);
		return true;
	}

	public static bool DeleteMail(string mailid)
	{
		return DeleteMail(FindMail(mailid));
	}

	public static bool DeleteMail(PhoneMail mailobj)
	{
		if (mailobj == null)
		{
			return false;
		}
		if (!all_mail.ContainsKey(mailobj.id))
		{
			AddMail(mailobj);
		}
		PhoneMemory.DeleteMail(mailobj);
		return true;
	}
}
