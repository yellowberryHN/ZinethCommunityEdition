using System.Collections;
using UnityEngine;

public class MissionDLC : MonoBehaviour
{
	public string dlc_name;

	public string url;

	public bool auto_load;

	public WWW web;

	public MissionObject mission_obj;

	public string id
	{
		get
		{
			if ((bool)mission_obj)
			{
				return mission_obj.id;
			}
			return "?";
		}
	}

	public float progress
	{
		get
		{
			if (web == null)
			{
				return 0f;
			}
			return web.progress;
		}
	}

	public string progress_string
	{
		get
		{
			if (web == null && progress <= 0f)
			{
				return "Not Started";
			}
			return progress * 100f + "%";
		}
	}

	public void Init(string name_str, string url_str)
	{
		dlc_name = name_str;
		url = url_str;
	}

	public void LoadMission()
	{
		if ((bool)mission_obj)
		{
			MissionController.AddMission(mission_obj);
		}
	}

	public void Download()
	{
		StartCoroutine("Co_Download");
	}

	private IEnumerator Co_Download()
	{
		string path = url;
		web = new WWW(path);
		yield return web;
		if (web.error != null)
		{
			Debug.LogWarning("Error: could not download mission " + web.error + " " + web.url);
		}
		else if ((bool)web.assetBundle)
		{
			Debug.Log("Found Asset Bundle! " + web.assetBundle.name);
			GameObject gob = Object.Instantiate(web.assetBundle.mainAsset) as GameObject;
			MissionObject mobj = gob.GetComponent<MissionObject>();
			Debug.Log("right here!");
			if ((bool)mission_obj)
			{
				mission_obj = mobj;
				Playtomic.Log.CustomMetric("tDownloadedMission", PlaytomicController.current_group, true);
				if (auto_load)
				{
					LoadMission();
				}
			}
		}
		else
		{
			Debug.LogWarning("could not find asset bundle in " + path);
		}
	}
}
