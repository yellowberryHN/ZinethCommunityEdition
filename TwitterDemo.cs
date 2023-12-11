using System;
using System.Collections;
using System.Collections.Generic;
using Twitter;
using UnityEngine;

public class TwitterDemo : MonoBehaviour
{
	public class TwitterUser
	{
		public string profile_name;

		public string pin;

		public RequestTokenResponse m_RequestTokenResponse;

		public AccessTokenResponse m_AccessTokenResponse;
	}

	public delegate void RegisterCallBack(bool success, string username);

	private const string PLAYER_PREFS_TWITTER_USER_ID = "TwitterUserID";

	private const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "TwitterUserScreenName";

	private const string PLAYER_PREFS_TWITTER_USER_TOKEN = "TwitterUserToken";

	private const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";

	private const string def_PLAYER_PREFS_TWITTER_USER_ID = "538760177";

	private const string def_PLAYER_PREFS_TWITTER_USER_SCREEN_NAME = "gamsfest";

	private const string def_PLAYER_PREFS_TWITTER_USER_TOKEN = "538760177-KvksxG9svWIzEfjtQJANhfQchOUK1iGV1NI8KkCp";

	private const string def_PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "K6n5IVguaBVF2Ik9yqw2qkNWNGXmfgGfwnzTnq3Uzg";

	private const string def_m_PIN = "1146604";

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

	public TwitterUser defaultUser;

	public TwitterUser playerUser;

	private RequestTokenResponse m_RequestTokenResponse;

	private AccessTokenResponse m_AccessTokenResponse;

	private string m_PIN = "Please enter your PIN here.";

	private string m_Tweet = "Please enter your tweet here.";

	public bool _has_coord_pos;

	public string _pos_lat = string.Empty;

	public string _pos_long = string.Empty;

	private DateTime start_time;

	public Texture2D imgtex;

	private string searchstring = "source:zineth_game #wow";

	private string tweetstring = string.Empty;

	public bool get_mentions_with_timeline = true;

	private bool _hasgottimeline;

	public float timeline_refresh_countdown = 30f;

	private string _customScreenName;

	public bool _isConnected;

	public static bool requireCustom = true;

	public bool use_hardcoded_account = true;

	public static RegisterCallBack registercallback;

	public static ulong newest_id;

	public bool updated;

	public int newtweets;

	public List<PhoneMail> twitter_messages = new List<PhoneMail>();

	public GameObject mail_obj;

	private bool _use_hash_tag = true;

	public static string hash_tag = "#ILoveCatco";

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
			return PhoneMemory.settings.allow_twitter && !Application.isWebPlayer;
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

	public bool _canTweet
	{
		get
		{
			return !requireCustom || _isCustom || Application.isEditor;
		}
	}

	private bool use_hash_tag
	{
		get
		{
			return _use_hash_tag && !string.IsNullOrEmpty(hash_tag);
		}
	}

	public static bool PostTweet(string tweet)
	{
		instance.DoPostTweet(tweet);
		return true;
	}

	public static bool PostTweet(string tweet, PostTweetCallback callback)
	{
		instance.DoPostTweet(tweet, callback);
		return true;
	}

	public static bool GetMentions()
	{
		instance.DoGetMentions();
		return true;
	}

	public static bool GetTimeLine()
	{
		instance.DoGetTimeLine();
		return true;
	}

	public static bool LogIn()
	{
		if (instance.LoadTwitterUserInfo(true))
		{
			return true;
		}
		RegisterUser();
		return false;
	}

	public static bool LogOut()
	{
		instance.LoadTwitterUserInfo(false);
		return true;
	}

	public static bool GetAccess(string pin)
	{
		instance.GetAccessToken(pin);
		return true;
	}

	public static bool RegisterUser()
	{
		instance.GetRequestToken();
		return true;
	}

	private void Awake()
	{
		base.useGUILayout = false;
	}

	private void Start()
	{
		if (!Application.isWebPlayer)
		{
			registercallback = OnRegister;
			start_time = DateTime.Now;
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString("TwitterUserID", string.Empty)))
			{
				LoadTwitterUserInfo(false);
			}
			else
			{
				LoadTwitterUserInfo(true);
			}
		}
	}

	private void OnGUI()
	{
		if (!draw_gui)
		{
			return;
		}
		Rect position = new Rect((float)Screen.width * USER_LOG_IN_X, (float)Screen.height * USER_LOG_IN_Y, (float)Screen.width * USER_LOG_IN_WIDTH, (float)Screen.height * USER_LOG_IN_HEIGHT);
		if (string.IsNullOrEmpty(CONSUMER_KEY) || string.IsNullOrEmpty(CONSUMER_SECRET))
		{
			string text = "You need to register your game or application first.\n Click this button, register and fill CONSUMER_KEY and CONSUMER_SECRET of Demo game object.";
			if (GUI.Button(position, text))
			{
				Application.OpenURL("http://dev.twitter.com/apps/new");
			}
		}
		else
		{
			string empty = string.Empty;
			if (GUI.Button(text: string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) ? "You need to register your game or application first." : (m_AccessTokenResponse.ScreenName + "\nClick to register with a different Twitter account"), position: position))
			{
				StartCoroutine(API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET, OnRequestTokenCallback));
			}
		}
		position.x = (float)Screen.width * PIN_INPUT_X;
		position.y = (float)Screen.height * PIN_INPUT_Y;
		position.width = (float)Screen.width * PIN_INPUT_WIDTH;
		position.height = (float)Screen.height * PIN_INPUT_HEIGHT;
		m_PIN = GUI.TextField(position, m_PIN);
		position.x = (float)Screen.width * PIN_ENTER_X;
		position.y = (float)Screen.height * PIN_ENTER_Y;
		position.width = (float)Screen.width * PIN_ENTER_WIDTH;
		position.height = (float)Screen.height * PIN_ENTER_HEIGHT;
		if (GUI.Button(position, "Enter PIN"))
		{
			StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.Token, m_PIN, OnAccessTokenCallback));
		}
		position.x = (float)Screen.width * TWEET_INPUT_X;
		position.y = (float)Screen.height * TWEET_INPUT_Y;
		position.width = (float)Screen.width * TWEET_INPUT_WIDTH;
		position.height = (float)Screen.height * TWEET_INPUT_HEIGHT;
		m_Tweet = GUI.TextField(position, m_Tweet);
		position.x = (float)Screen.width * POST_TWEET_X;
		position.y = (float)Screen.height * POST_TWEET_Y;
		position.width = (float)Screen.width * POST_TWEET_WIDTH;
		position.height = (float)Screen.height * POST_TWEET_HEIGHT;
		if (GUI.Button(position, "Post Tweet"))
		{
			DoPostTweet(m_Tweet);
		}
		position.y += POST_TWEET_HEIGHT * (float)Screen.height;
		if (GUI.Button(position, "Get Mentions"))
		{
			DoGetMentions();
		}
		GUILayout.BeginHorizontal("Box");
		searchstring = GUILayout.TextField(searchstring, GUILayout.MinWidth(120f));
		if (GUILayout.Button("Search!"))
		{
			DoGetSearchResults(searchstring);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Box");
		tweetstring = GUILayout.TextField(tweetstring, GUILayout.MinWidth(120f));
		if (GUILayout.Button("Tweet"))
		{
			DoPostTweet(tweetstring);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("sign out"))
		{
			LogIn();
		}
		if (GUILayout.Button("post a neat pic"))
		{
			StartCoroutine(API.UploadImage(OnPostImage));
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	public void GetRequestToken()
	{
		StartCoroutine(API.GetRequestToken(CONSUMER_KEY, CONSUMER_SECRET, OnRequestTokenCallback));
	}

	public void GetAccessToken()
	{
		GetAccessToken(m_PIN);
	}

	public void GetAccessToken(string pin)
	{
		StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, m_RequestTokenResponse.Token, pin, OnAccessTokenCallback));
	}

	public void GetAccessToken(TwitterUser user)
	{
		StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, user.m_RequestTokenResponse.Token, user.pin, OnAccessTokenCallback));
	}

	public void GetAccessToken(string pin, TwitterUser user)
	{
		StartCoroutine(API.GetAccessToken(CONSUMER_KEY, CONSUMER_SECRET, user.m_RequestTokenResponse.Token, pin, OnAccessTokenCallback));
	}

	public void DoPostScreenshot()
	{
		if (!_canTweet)
		{
			Debug.LogWarning("tried to tweet but not allowed...");
		}
		else
		{
			StartCoroutine(API.UploadImage(new TweetContext(), OnPostImage));
		}
	}

	public void DoPostImage(TweetContext tweet)
	{
		if (!_canTweet)
		{
			Debug.LogWarning("tried to tweet but not allowed...");
		}
		else
		{
			StartCoroutine(API.UploadImage(tweet, OnPostImage));
		}
	}

	public void DoPostImage(Texture2D tex)
	{
		if (!_canTweet)
		{
			Debug.LogWarning("tried to tweet but not allowed...");
		}
		else
		{
			StartCoroutine(API.UploadImage(tex, OnPostImage));
		}
	}

	public void DoPostTweet(string tweet, TwitterUser user)
	{
		if (!_canTweet)
		{
			Debug.LogWarning("tried to tweet but not allowed...");
			return;
		}
		tweet = AddHashTag(tweet);
		StartCoroutine(API.PostTweet(tweet, CONSUMER_KEY, CONSUMER_SECRET, user.m_AccessTokenResponse, OnPostTweet));
	}

	public void DoPostTweet(string tweet)
	{
		if (!_canTweet)
		{
			Debug.LogWarning("tried to tweet but not allowed...");
			return;
		}
		tweet = AddHashTag(tweet);
		if (Application.isEditor)
		{
			Debug.Log(tweet);
		}
		StartCoroutine(API.PostTweet(tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, OnPostTweet));
	}

	public void DoPostTweet(string tweet, PostTweetCallback callback)
	{
		StartCoroutine(API.PostTweet(tweet, CONSUMER_KEY, CONSUMER_SECRET, m_AccessTokenResponse, callback));
	}

	public void DoGetMentions()
	{
		DoGetMentions(m_AccessTokenResponse);
	}

	public void DoGetMentions(TwitterUser user)
	{
		DoGetMentions(user.m_AccessTokenResponse);
	}

	public void DoGetMentions(AccessTokenResponse accessTokenResponse)
	{
		StartCoroutine(API.GetMentions(CONSUMER_KEY, CONSUMER_SECRET, accessTokenResponse, OnGetMentions));
		CancelInvoke("DoGetMentions");
	}

	public void DoGetSingleTweet(string tweet_id)
	{
		StartCoroutine(API.GetSingleTweet(tweet_id, OnGetSingleTweet));
	}

	public void DoGetSearchResults(string search)
	{
		StartCoroutine(API.GetSearchResults(search, OnGetSearchResults));
	}

	public void DoGetTimeLine()
	{
		DoGetTimeLine(m_AccessTokenResponse);
	}

	public void DoGetTimeLine(TwitterUser user)
	{
		DoGetTimeLine(user.m_AccessTokenResponse);
	}

	public void DoGetTimeLine(AccessTokenResponse accessTokenResponse)
	{
		if (is_allowed)
		{
			if (should_get_timeline || !_hasgottimeline)
			{
				StartCoroutine(API.GetTimeLine(CONSUMER_KEY, CONSUMER_SECRET, accessTokenResponse, OnGetTimeLine));
			}
			if (get_mentions_with_timeline)
			{
				DoGetMentions();
			}
		}
		CancelInvoke("DoGetTimeLine");
		Invoke("DoGetTimeLine", timeline_refresh_countdown);
	}

	public string GetCurrentUserId()
	{
		return m_AccessTokenResponse.UserId;
	}

	public string GetCurrentScreenName()
	{
		return m_AccessTokenResponse.ScreenName;
	}

	public string GetCustomScreenName()
	{
		if (_customScreenName == null)
		{
			_customScreenName = PlayerPrefs.GetString("PLAYER_PREFS_TWITTER_USER_ID", string.Empty);
		}
		return _customScreenName;
	}

	public bool LoadTwitterUserInfo()
	{
		return LoadTwitterUserInfo(use_hardcoded_account);
	}

	public bool LoadTwitterUserInfo(bool hardcoded)
	{
		m_AccessTokenResponse = new AccessTokenResponse();
		if (hardcoded)
		{
			m_AccessTokenResponse.UserId = "538760177";
			m_AccessTokenResponse.ScreenName = "gamsfest";
			m_AccessTokenResponse.Token = "538760177-KvksxG9svWIzEfjtQJANhfQchOUK1iGV1NI8KkCp";
			m_AccessTokenResponse.TokenSecret = "K6n5IVguaBVF2Ik9yqw2qkNWNGXmfgGfwnzTnq3Uzg";
		}
		else
		{
			m_AccessTokenResponse.UserId = PlayerPrefs.GetString("TwitterUserID");
			m_AccessTokenResponse.ScreenName = PlayerPrefs.GetString("TwitterUserScreenName");
			m_AccessTokenResponse.Token = PlayerPrefs.GetString("TwitterUserToken");
			m_AccessTokenResponse.TokenSecret = PlayerPrefs.GetString("TwitterUserTokenSecret");
			_customScreenName = m_AccessTokenResponse.ScreenName;
		}
		if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) && !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) && !string.IsNullOrEmpty(m_AccessTokenResponse.Token) && !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
		{
			string text = "LoadTwitterUserInfo - succeeded";
			text = text + "\n    UserId : " + m_AccessTokenResponse.UserId;
			text = text + "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
			twitter_messages.Clear();
			newtweets = 0;
			updated = true;
			newest_id = 0uL;
			DoGetTimeLine();
			return true;
		}
		return false;
	}

	private void OnRequestTokenCallback(bool success, RequestTokenResponse response)
	{
		if (success)
		{
			string message = "OnRequestTokenCallback - succeeded";
			if (Application.isEditor)
			{
				MonoBehaviour.print(message);
			}
			m_RequestTokenResponse = response;
			API.OpenAuthorizationPage(response.Token);
			_isConnected = true;
		}
		else
		{
			MonoBehaviour.print("OnRequestTokenCallback - failed.");
			_isConnected = false;
		}
	}

	public void OnRegister(bool success, string username)
	{
	}

	private void OnAccessTokenCallback(bool success, AccessTokenResponse response)
	{
		if (success)
		{
			string text = "OnAccessTokenCallback - succeeded";
			text = text + "\n    UserId : " + response.UserId;
			text = text + "\n    ScreenName : " + response.ScreenName;
			if (Application.isEditor)
			{
				MonoBehaviour.print(text);
			}
			m_AccessTokenResponse = response;
			PlayerPrefs.SetString("TwitterUserID", response.UserId);
			PlayerPrefs.SetString("TwitterUserScreenName", response.ScreenName);
			PlayerPrefs.SetString("TwitterUserToken", response.Token);
			PlayerPrefs.SetString("TwitterUserTokenSecret", response.TokenSecret);
			registercallback(true, response.ScreenName);
			_isConnected = true;
			LoadTwitterUserInfo(false);
		}
		else
		{
			MonoBehaviour.print("OnAccessTokenCallback - failed.");
			registercallback(false, "FAILED TO REGISTER");
		}
	}

	private void OnPostTweet(bool success, string text)
	{
		if (success)
		{
			OnGetTimeLine(success, text);
			_isConnected = true;
		}
		else
		{
			MonoBehaviour.print("OnPostTweet - " + ((!success) ? "failed." : "succeeded."));
		}
	}

	private void OnGetMentions(bool success, string text)
	{
		MonoBehaviour.print("OnGetMentions - " + ((!success) ? "failed." : "succeeded."));
		if (!success)
		{
			return;
		}
		_isConnected = true;
		List<PhoneMail> list = Parser.ParseToMail(text);
		foreach (PhoneMail item in list)
		{
			if (MailController.FindMail(item.id) != null || (!item.body.StartsWith("@gamsfest ") && 1 == 0))
			{
				continue;
			}
			string text2 = item.id.Replace("tw_", string.Empty);
			if (MailController.FindMail(text2) != null)
			{
				continue;
			}
			item.id = text2;
			string text3 = item.body.Replace("@gamsfest", string.Empty);
			text3 = text3.TrimStart(' ');
			if (item.subject == "@gamsfest" || item.subject == "@The_Joke_Master")
			{
				if (item.time.CompareTo(start_time) > 0)
				{
					PhoneController.DoPhoneCommand(text3);
				}
				MailController.AddMail(item);
				continue;
			}
			if (Application.isEditor)
			{
				MonoBehaviour.print(item.subject);
			}
			item.body = text3;
			MailController.AddMail(item);
			MailController.SendMail(item);
		}
	}

	private void OnGetTimeLine(bool success, string text)
	{
		_hasgottimeline = true;
		bool flag = updated;
		if (!success)
		{
			MonoBehaviour.print("OnGetTimeLine - " + ((!success) ? "failed." : "succeeded."));
		}
		if (success)
		{
			_isConnected = true;
			List<PhoneMail> list = Parser.ParseToMail(text);
			list.Reverse();
			foreach (PhoneMail item in list)
			{
				if (!twitter_messages.Contains(item))
				{
					if (item.body.StartsWith("@gamsfest"))
					{
					}
					flag = true;
					newtweets++;
					twitter_messages.Insert(0, item);
					if (item.id_number > newest_id)
					{
						newest_id = item.id_number;
					}
				}
			}
		}
		updated = flag;
	}

	private void OnGetSearchResults(bool success, string text)
	{
		if (!success)
		{
			MonoBehaviour.print("OnGetSearchResults - " + ((!success) ? "failed." : "succeeded."));
		}
		if (!success)
		{
			return;
		}
		Debug.Log(text);
		List<PhoneMail> list = Parser.ParseAtomToMail(text);
		list.Reverse();
		foreach (PhoneMail item in list)
		{
			MonoBehaviour.print(item.sender + " - " + item.body + item.position);
			if (item.position != Vector3.zero)
			{
				Debug.Log("looking up " + item.id);
				item.id.Replace("tw_", string.Empty);
				if (MailController.FindMail("tw_" + item.id) == null)
				{
					DoGetSingleTweet(item.id);
				}
				else if (MailController.FindMail("mail_tw_" + item.id) == null)
				{
					item.id = "mail_tw_" + item.id;
					MailController.AddMail(item);
					MakeMailObj(item);
				}
			}
		}
	}

	private void MakeMailObj(string id)
	{
		MakeMailObj(MailController.FindMail(id));
	}

	private void MakeMailObj(PhoneMail mail)
	{
		Vector3 vector = mail.position + Vector3.up * 5f;
		Debug.Log("position: " + vector);
		GameObject gameObject = UnityEngine.Object.Instantiate(mail_obj, vector, Quaternion.identity) as GameObject;
		gameObject.GetComponent<DoCommandTrigger>().command_string = "mail_send " + mail.id;
	}

	private void OnGetSingleTweet(bool success, string text)
	{
		_isConnected = success;
		if (!success)
		{
			MonoBehaviour.print("OnGetSingleTweet - " + ((!success) ? "failed." : "succeeded."));
		}
		if (!success)
		{
			return;
		}
		Debug.Log(text);
		List<PhoneMail> list = Parser.ParseToMail(text);
		foreach (PhoneMail item in list)
		{
			if (item.position != Vector3.zero && MailController.FindMail("mail_" + item.id) == null)
			{
				item.id = "mail_" + item.id;
				MailController.AddMail(item);
				MakeMailObj(item);
			}
		}
	}

	private void OnPostImage(bool success, string text)
	{
		_isConnected = success;
		if (!success)
		{
			Debug.LogWarning("OnPostImage-failed: " + text);
			return;
		}
		string text2 = Parser.ParseImgurResponse(text);
		if (text2 != string.Empty)
		{
			string tweet = text2;
			tweet = AddHashTag(tweet);
			if (Application.loadedLevelName == "Loader 1")
			{
				tweet = "pos:" + PlaytomicController.TranslatePlayerPosToGPSString() + ";" + tweet;
			}
			DoPostTweet(tweet);
		}
	}

	private void OnPostImage(bool success, string text, TweetContext tweet)
	{
		_isConnected = success;
		if (!success)
		{
			Debug.LogWarning("OnPostImage-failed: " + text);
			return;
		}
		string text2 = Parser.ParseImgurResponse(text);
		if (text2 != string.Empty)
		{
			tweet.text += text2;
			tweet.text = AddHashTag(tweet.text);
			AddMentions(ref tweet);
			if (Application.loadedLevelName == "Loader 1")
			{
				tweet.text = "pos:" + PlaytomicController.TranslatePlayerPosToGPSString() + ";" + tweet.text;
			}
			DoPostTweet(tweet.text);
		}
	}

	private string AddHashTag(string tweet)
	{
		if (!use_hash_tag)
		{
			return tweet;
		}
		if (tweet.Contains(hash_tag))
		{
			return tweet;
		}
		return string.Format("{0} {1}", tweet, hash_tag);
	}

	private void AddMentions(ref TweetContext tweet)
	{
		foreach (string mention in tweet.mentions)
		{
			string text = mention;
			if (!text.StartsWith("@"))
			{
				text = "@" + text;
			}
			if (tweet.text.Length > 0)
			{
				TweetContext obj = tweet;
				obj.text = obj.text + " " + text;
			}
		}
	}

	private IEnumerator GetLocation(string ip)
	{
		string url = "http://api.ipinfodb.com/v3/ip-city/?key=7999984451273a720a4f8904a9b64991e4156211e893d394072602cd7f7c6657";
		WWW locweb = new WWW(url);
		yield return locweb;
		if (locweb.error == null)
		{
			string[] items = locweb.text.Split(';');
			string latitude = items[8];
			string longitude = items[9];
			has_coord_pos = true;
			pos_lat = latitude;
			pos_long = longitude;
		}
		else
		{
			Debug.LogWarning("could not get location: " + locweb.error);
		}
	}
}
