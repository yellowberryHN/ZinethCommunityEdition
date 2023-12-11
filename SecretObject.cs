using System.Collections.Generic;
using UnityEngine;

public class SecretObject : MonoBehaviour
{
	public static List<SecretObject> all_list = new List<SecretObject>();

	public static List<SecretObject> collected_list = new List<SecretObject>();

	public static List<SecretObject> uncollected_list = new List<SecretObject>();

	public string secret_name;

	public int secret_val = 1;

	public bool save_found = true;

	public List<GameObject> send_activate_to = new List<GameObject>();

	private void Start()
	{
		if (string.IsNullOrEmpty(secret_name))
		{
			secret_name = base.gameObject.name;
		}
		if (!all_list.Contains(this))
		{
			all_list.Add(this);
		}
		LoadMyInfo();
	}

	public void Found()
	{
		OnFound();
		SaveMyInfo();
	}

	private void OnFound()
	{
		if (!collected_list.Contains(this))
		{
			collected_list.Add(this);
		}
		if (uncollected_list.Contains(this))
		{
			uncollected_list.Remove(this);
		}
		foreach (GameObject item in send_activate_to)
		{
			item.SendMessage("SecretFound");
		}
	}

	public string GetSaveName()
	{
		return string.Format("secret_{0}", secret_name);
	}

	public int GetSaveInt()
	{
		return secret_val;
	}

	public void SaveMyInfo()
	{
		if (save_found)
		{
			PlayerPrefs.SetInt(GetSaveName(), GetSaveInt());
			PlayerPrefs.Save();
		}
	}

	public void LoadMyInfo()
	{
		int @int = PlayerPrefs.GetInt(GetSaveName(), -1);
		if (@int != -1)
		{
			secret_val = @int;
			OnFound();
		}
		else if (!uncollected_list.Contains(this))
		{
			uncollected_list.Add(this);
		}
	}
}
