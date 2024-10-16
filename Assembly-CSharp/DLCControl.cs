using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DLCControl : MonoBehaviour
{
	public class StatusInfo
	{
		public string current_version;

		public string url;

		public string message;

		public StatusInfo(XmlNode node)
		{
			current_version = URLDecode(node["current"]);
			url = URLDecode(node["url"]);
			message = URLDecode(node["message"]);
		}
	}

	public class VersionInfo
	{
		public string version;

		public string info;

		public string date;

		public string url;

		public string file_url;

		public List<DLCInfo> dlc = new List<DLCInfo>();

		public List<string> changes = new List<string>();

		public VersionInfo(XmlNode node)
		{
			version = node.Name;
			info = URLDecode(node["info"]);
			date = URLDecode(node["date"]);
			url = URLDecode(node["url"]);
			if (node["file_url"] != null)
			{
				file_url = URLDecode(node["file_url"]);
			}
			if (node["changes"] != null)
			{
				foreach (XmlNode item in node["changes"])
				{
					changes.Add(URLDecode(item));
				}
			}
			if (node["dlc"] == null)
			{
				return;
			}
			foreach (XmlNode item2 in node["dlc"])
			{
				dlc.Add(new DLCInfo(item2));
			}
		}
	}

	// TODO: this is crazy, a whole DLC mission system, seemingly mostly complete, completely unused. Explore this more.
	public class DLCInfo
	{
		public string name;

		public string file_url;

		public bool gui_expand;

		public DLCInfo(XmlNode node)
		{
			name = URLDecode(node["name"]);
			file_url = URLDecode(node["file_url"]);
		}
	}

	public static DLCControl instance;

	public static string websiteUrl = "https://yello.ooo/projects/zineth";

	public static string infoUrl = "https://yello.ooo/projects/zineth/info.xml";

	private int connection_trouble;

	private StatusInfo statusInfo;

	private int _compared = -2;

	private VersionInfo localInfo;

	private VersionInfo currentInfo;

	private static string dirUrl;

	public static List<DLCInfo> dlcInfoList = new List<DLCInfo>();

	public static bool showGUI = true;

	private bool gui_show_status = true;

	private bool gui_show_current;

	private bool gui_show_local;
#if DEBUG
	private bool debug_gui = true;
#else
	private bool debug_gui = false;
#endif

	public string current_version
	{
		get
		{
			if (statusInfo != null)
			{
				return statusInfo.current_version;
			}
			return string.Empty;
		}
	}

	public string local_version
	{
		get
		{
			return PhoneInterface.version;
		}
	}

	public int is_current
	{
		get
		{
			if (_compared == -2)
			{
				_compared = CompareVersions(current_version, local_version);
			}
			return _compared;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(this);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		SpawnModController();
		DownloadInfo();
	}

	private void SpawnModController()
	{
		var mc = new GameObject("ModController");
		mc.AddComponent<ModController>();
	}

	public static int CompareVersions(string a, string b)
	{
		float num = VersionToFloat(a);
		float num2 = VersionToFloat(b);
		if (num > num2)
		{
			return -1;
		}
		if (num == num2)
		{
			return 0;
		}
		return 1;
	}

	public static float VersionToFloat(string version)
	{
		if (version.StartsWith("v"))
		{
			version = version.Remove(0, 1);
		}
		version = version.Replace("_", ".");
		float result;
		if (float.TryParse(version, out result))
		{
			return result;
		}
		Debug.LogWarning("could not parse the version " + version);
		return -2f;
	}

	public void DownloadInfo()
	{
		StartCoroutine(DownloadInfo(infoUrl));
	}

	public IEnumerator DownloadInfo(string url)
	{
		WWW web = new WWW(url);
		yield return web;
		if (web.error == null)
		{
			string text = web.text;
			ParseInfo(text);
			yield break;
		}
		connection_trouble++;
		Debug.LogWarning(web.error + "\n" + url);
		if (connection_trouble < 3)
		{
			Invoke("DownloadInfo", 1f);
		}
		else if (connection_trouble < 6)
		{
			Invoke("DownloadInfo", 6f);
		}
	}

	public void ParseInfo(string text)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		XmlNode firstChild = xmlDocument.FirstChild;
		dirUrl = URLDecode(firstChild["dir_url"]);
		statusInfo = new StatusInfo(firstChild["status"]);
		XmlNode xmlNode = firstChild["versions"];
		XmlNode xmlNode2 = xmlNode[local_version];
		if (xmlNode2 != null)
		{
			localInfo = new VersionInfo(xmlNode2);
			foreach (DLCInfo item in localInfo.dlc)
			{
				dlcInfoList.Add(item);
			}
		}
		else
		{
			Debug.LogWarning("could not find node for version: " + local_version);
		}
		if (is_current < 0)
		{
			XmlNode xmlNode3 = xmlNode[current_version];
			if (xmlNode3 != null)
			{
				currentInfo = new VersionInfo(xmlNode3);
			}
			else
			{
				Debug.Log("could not find node for current version: " + current_version);
			}
		}
	}

	public void DoGUI()
	{
		if (!showGUI)
		{
			showGUI = false;
			base.useGUILayout = false;
		}
		else if (debug_gui)
		{
			DebugGUI();
		}
		else
		{
			NormalGUI();
		}
	}
	
	private readonly GUIStyle communityEditionStyle = new GUIStyle
	{
		fontStyle = FontStyle.Bold,
		normal = { textColor = new Color32(0xFF, 0xC7, 0x00) },
		padding = { left = 4, top = 4 }
	};

	public void NormalGUI()
	{
		GUILayout.Label(PhoneInterface.version, communityEditionStyle);
		if (statusInfo != null)
		{
			if (is_current < 0)
			{
				GUILayout.BeginVertical("Box");
				if (gui_show_status)
				{
					GUILayout.Label("New Version Available!");
				}
				else if (GUILayout.Button("New Version Available!"))
				{
					gui_show_status = true;
				}
				if (gui_show_status)
				{
					DrawInfo(currentInfo);
				}
				GUILayout.EndVertical();
			}
			else if (is_current > 0)
			{
				GUILayout.BeginVertical("Box");
				GUILayout.Label("Living in the future!");
				GUILayout.EndVertical();
			}
		}
		else
		{
			if (connection_trouble < 3)
			{
				return;
			}
			GUILayout.BeginVertical("Box");
			if (connection_trouble < 6)
			{
				GUILayout.Label("Having trouble connecting to update info...");
			}
			else
			{
				GUILayout.Label("Couldn't connect to update info!");
				if (GUILayout.Button("Open Zineth Website"))
				{
					Application.OpenURL(websiteUrl);
				}
			}
			GUILayout.EndVertical();
		}
	}

	public void DebugGUI()
	{
		if (statusInfo == null)
		{
			if (GUILayout.Button("Download Info"))
			{
				DownloadInfo();
			}
			return;
		}
		GUILayout.BeginVertical("Box");
		gui_show_status = GUILayout.Toggle(gui_show_status, "Status:");
		if (gui_show_status)
		{
			GUILayout.Label("Newest version: " + statusInfo.current_version);
			GUILayout.Label("News: " + statusInfo.message);
			if (GUILayout.Button(statusInfo.url))
			{
				Application.OpenURL(statusInfo.url);
			}
		}
		GUILayout.EndVertical();
		if (is_current < 0)
		{
			GUILayout.BeginVertical("Box");
			gui_show_current = GUILayout.Toggle(gui_show_current, "New Version!");
			if (gui_show_current)
			{
				DrawInfo(currentInfo);
			}
			GUILayout.EndVertical();
		}
		if (localInfo == null)
		{
		}
	}

	public void DrawInfo(VersionInfo info)
	{
		GUILayout.BeginVertical("Box");
		GUILayout.Label("Version: " + info.version + " (" + info.date + ")");
		if (!string.IsNullOrEmpty(info.info))
		{
			GUILayout.Label(info.info);
		}
		string url = statusInfo.url;
		if (!string.IsNullOrEmpty(info.url))
		{
			url = info.url;
		}
		if (GUILayout.Button("Open Website"))
		{
			Application.OpenURL(url);
		}
		if (info.changes.Count > 0)
		{
			GUILayout.Label("Changes:");
			foreach (string change in info.changes)
			{
				GUILayout.Label("-" + change);
			}
		}
		if (info.dlc.Count > 0 && debug_gui)
		{
			GUILayout.Label("DLC:");
			foreach (DLCInfo item in info.dlc)
			{
				GUILayout.BeginVertical("Box");
				item.gui_expand = GUILayout.Toggle(item.gui_expand, item.name);
				if (item.gui_expand)
				{
					GUILayout.Label(item.file_url);
				}
				GUILayout.EndVertical();
			}
		}
		GUILayout.EndVertical();
	}

	private static string URLDecode(XmlNode node)
	{
		return node == null ? string.Empty : URLDecode(node.InnerText);
	}

	private static string URLDecode(string url)
	{
		return WWW.UnEscapeURL(url).Replace("{dir}", dirUrl);
	}
}
