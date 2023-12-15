using System.Collections.Generic;
using UnityEngine;

public class TwitterDemo : MonoBehaviour
{
	private static TwitterDemo _instance;

	public float USER_LOG_IN_X;

	public float USER_LOG_IN_Y;

	public float USER_LOG_IN_WIDTH;

	public float USER_LOG_IN_HEIGHT;

	public float PIN_INPUT_X;

	public float PIN_INPUT_Y;

	public float PIN_INPUT_WIDTH;

	public float PIN_INPUT_HEIGHT;

	public float PIN_ENTER_X;

	public float PIN_ENTER_Y;

	public float PIN_ENTER_WIDTH;

	public float PIN_ENTER_HEIGHT;

	public float TWEET_INPUT_X;

	public float TWEET_INPUT_Y;

	public float TWEET_INPUT_WIDTH;

	public float TWEET_INPUT_HEIGHT;

	public float POST_TWEET_X;

	public float POST_TWEET_Y;

	public float POST_TWEET_WIDTH;

	public float POST_TWEET_HEIGHT;

	public string CONSUMER_KEY = "1CwngTBK8ruSsXscdc42ZQ";

	public string CONSUMER_SECRET = "TjMbFSxBhojxE6QAhhlwasIG5C527eOdhDmnurmHxhI";

	public bool _has_coord_pos;

	public string _pos_lat = string.Empty;

	public string _pos_long = string.Empty;

	public Texture2D imgtex;
	
	public bool get_mentions_with_timeline = true;

	public float timeline_refresh_countdown = 30f;

	public bool _isConnected;
	
	public bool use_hardcoded_account = true;

	public bool updated;

	public int newtweets;

	public List<PhoneMail> twitter_messages = new List<PhoneMail>();

	public GameObject mail_obj;
	
	public static TwitterDemo instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(TwitterDemo)) as TwitterDemo;
			}
			return _instance;
		}
	}

	public static bool has_coord_pos
	{
		get
		{
			return instance._has_coord_pos;
		}
		set
		{
			instance._has_coord_pos = value;
		}
	}

	public static string pos_lat
	{
		get
		{
			return instance._pos_lat;
		}
		set
		{
			instance._pos_lat = value;
		}
	}

	public static string pos_long
	{
		get
		{
			return instance._pos_long;
		}
		set
		{
			instance._pos_long = value;
		}
	}

	public bool draw_gui
	{
		get
		{
			return base.useGUILayout;
		}
		set
		{
			base.useGUILayout = value;
		}
	}

	public bool is_allowed
	{
		get
		{
			return false;
		}
	}

	public bool should_get_timeline
	{
		get
		{
			return PhoneMemory.IsMenuUnlocked("Twitter");
		}
	}

	public bool _isCustom
	{
		get
		{
			return GetCurrentScreenName() != "gamsfest";
		}
	}
	
	public string GetCurrentScreenName()
	{ 
		return "";
	}

	public string GetCurrentUserId()
	{
		return "0";
	}
}
