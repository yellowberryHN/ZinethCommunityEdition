using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Twitter;
using UnityEngine;

public class DArtControl : MonoBehaviour
{
	private static DArtControl _instance;

	private static string dart_url = "http://backend.deviantart.com/rss.xml?q=favby%3Ablurpdrab";

	private static string dart_search_url = "http://backend.deviantart.com/rss.xml?q={0}";

	public bool tdart;

	public int indx;

	public bool show;

	public static List<string> url_list = new List<string>();

	public static DArtControl instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(DArtControl)) as DArtControl;
			}
			return _instance;
		}
	}

	private void Update()
	{
		if (tdart)
		{
			GrabSomeDArt();
		}
		if (show)
		{
			ShowDart(indx);
		}
		tdart = false;
		show = false;
	}

	public void ShowDart()
	{
		ShowDart(indx);
	}

	public void ShowDart(int index)
	{
		if (index >= 0 && url_list.Count > index)
		{
			Texture2D texture = ImageDownloadHelper.NewImage(url_list[index]);
			PhoneInterface.ShowZine(texture, true);
		}
	}

	public static void SearchAndGrab(string str)
	{
		string arg = API.UrlEncode(str);
		arg = string.Format(dart_search_url, arg);
		GrabSomeDArt(arg);
	}

	public static void GrabSomeDArt()
	{
		GrabSomeDArt(dart_url);
	}

	public static void GrabSomeDArt(string url)
	{
		instance.StartCoroutine(instance.GetDartXML(url));
	}

	private IEnumerator GetDartXML(string url)
	{
		WWW web = new WWW(url);
		yield return web;
		Parse(web.text);
		ShowDart(0);
	}

	private void Parse(string text)
	{
		url_list.Clear();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("item");
		foreach (XmlNode item in elementsByTagName)
		{
			if (item["media:content"] != null)
			{
				url_list.Add(item["media:content"].Attributes["url"].InnerText);
			}
		}
	}
}
