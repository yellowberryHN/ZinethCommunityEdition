using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionLoadControl : MonoBehaviour
{
	private static MissionLoadControl _instance;

	public string mission_name = "MissionTest1";

	private string file_ext = ".unity3d";

	public string local_path = "file://";

	public string web_path = "https://dl.dropbox.com/u/15013465/dlc_test/";

	public string web_list_path = "https://dl.dropbox.com/u/15013465/dlc_test/mission_list_{0}.txt";

	private List<MissionDLC> mission_list = new List<MissionDLC>();

	public bool online = true;

	public static MissionLoadControl instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(MissionLoadControl)) as MissionLoadControl;
			}
			return _instance;
		}
	}

	public bool dogui
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
		base.useGUILayout = false;
	}

	public virtual void LoadMission(string path)
	{
		StartCoroutine("Co_LoadMission", path);
	}

	public virtual IEnumerator Co_LoadMission(string path)
	{
		WWW web = new WWW(path);
		yield return web;
		if ((bool)web.assetBundle)
		{
			GameObject gob = Object.Instantiate(web.assetBundle.mainAsset) as GameObject;
			MissionObject mobj = gob.GetComponent<MissionObject>();
			MissionController.AddMission(mobj);
			MissionController.SetActive(mobj);
		}
		else
		{
			Debug.LogWarning("could not find asset bundle in " + path);
		}
	}

	private MissionDLC ParseMission(string name_text, string url_text)
	{
		GameObject gameObject = new GameObject();
		MissionDLC missionDLC = gameObject.AddComponent<MissionDLC>();
		missionDLC.Init(name_text, url_text);
		return missionDLC;
	}

	private void SetupMissions()
	{
		foreach (DLCControl.DLCInfo dlcInfo in DLCControl.dlcInfoList)
		{
			MissionDLC missionDLC = ParseMission(dlcInfo.name, dlcInfo.file_url);
			missionDLC.auto_load = true;
			missionDLC.Download();
			mission_list.Add(missionDLC);
		}
	}

	private void OnGUI()
	{
		if (!dogui)
		{
			return;
		}
		if (mission_list.Count <= 0)
		{
			if (GUILayout.Button("Setup"))
			{
				SetupMissions();
			}
			return;
		}
		foreach (MissionDLC item in mission_list)
		{
			if (item.progress > 0f)
			{
				if (GUILayout.Button("Load " + item.dlc_name))
				{
					item.LoadMission();
				}
			}
			else
			{
				GUILayout.Box(item.dlc_name + "-downloading...");
			}
		}
	}
}
