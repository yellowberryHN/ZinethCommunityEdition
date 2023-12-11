using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NetCodes : MonoBehaviour
{
	private string ip_str;

	public static Dictionary<string, List<string>> word_dic = new Dictionary<string, List<string>>();

	private static readonly string startSymbol = "*^s*";

	private static readonly string endSymbol = "*^e";

	private string genstr = string.Empty;

	private string genurl = string.Empty;

	public bool has_got_loc;

	public string country;

	public string state;

	public string city;

	public string zip;

	public string latitude;

	public string longitude;

	public WeatherInfo weather_info;

	private Texture2D maptexture;

	public bool show_gui
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

	private void Awake()
	{
		ip_str = Network.player.ipAddress;
		show_gui = false;
	}

	public static void AddWord(string a, string b)
	{
		if (!word_dic.ContainsKey(a))
		{
			word_dic.Add(a, new List<string>());
		}
		word_dic[a].Add(b);
	}

	public static void AddPhrase(string text)
	{
		text = text.Trim();
		if (text.Length <= 5)
		{
			return;
		}
		text = string.Format("{0} {1} {2}", startSymbol, text, endSymbol);
		string[] array = text.Split(' ');
		if (array.Length != 0)
		{
			for (int i = 0; i < array.Length - 1; i++)
			{
				AddWord(array[i], array[i + 1]);
			}
		}
	}

	public static void AddString(string text)
	{
		text = text.Replace(",", " , ");
		string[] array = text.Split('.', '?', '!', '\n');
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			AddPhrase(text2);
		}
	}

	public static string GetNext(string a)
	{
		List<string> list = word_dic[a];
		return list[Random.Range(0, list.Count)];
	}

	public static string GetFirst()
	{
		return GetNext(startSymbol);
	}

	public static string GenerateString()
	{
		string text = string.Empty;
		string text2 = GetFirst();
		while (text2 != endSymbol)
		{
			text = text + text2 + " ";
			text2 = GetNext(text2);
		}
		return text.Trim();
	}

	public static IEnumerator AddWebText(string url)
	{
		WWW web = new WWW(url);
		yield return web;
		string txt = web.text;
		AddString(txt);
	}

	private void OnGUI()
	{
		if (show_gui)
		{
			GUILayout.BeginHorizontal();
			if (word_dic.Count > 0 && GUILayout.Button("generate"))
			{
				genstr = GenerateString();
			}
			if (genstr != string.Empty)
			{
				GUILayout.Box(genstr);
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Get Location"))
			{
				StartCoroutine("GetLocation", ip_str);
			}
			if ((bool)maptexture)
			{
				GUILayout.Box(maptexture);
			}
			if (weather_info != null)
			{
				string text = "The Weather in your area is: " + weather_info.condition;
				text = text + "\n" + weather_info.temp_f + "F";
				text = text + "\n" + weather_info.humidity;
				text = text + "\n" + weather_info.wind_conditions;
				GUILayout.Box(text);
			}
		}
	}

	private IEnumerator GetLocation(string ip)
	{
		string url = "http://api.ipinfodb.com/v3/ip-city/?key=7999984451273a720a4f8904a9b64991e4156211e893d394072602cd7f7c6657";
		WWW locweb = new WWW(url);
		yield return locweb;
		string[] items = locweb.text.Split(';');
		has_got_loc = true;
		country = items[4];
		state = items[5];
		city = items[6];
		zip = items[7];
		latitude = items[8];
		longitude = items[9];
		StartCoroutine("GetGoogleMap");
	}

	private IEnumerator GetGoogleMap()
	{
		string url = "http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude + "&zoom=11&size=200x200&sensor=false";
		WWW mapweb = new WWW(url);
		yield return mapweb;
		maptexture = mapweb.texture;
		StartCoroutine("GetWeather");
	}

	private IEnumerator GetWeather()
	{
		string url = "http://www.google.com/ig/api?weather=" + zip;
		WWW wethweb = new WWW(url);
		yield return wethweb;
		string text = wethweb.text;
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(text);
		XmlNodeList nodelist = doc.GetElementsByTagName("current_conditions");
		XmlNode node = nodelist[0];
		weather_info = new WeatherInfo(node);
	}
}
