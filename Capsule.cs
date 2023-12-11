using System;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
	public static List<Capsule> all_list = new List<Capsule>();

	public static List<Capsule> collected_list = new List<Capsule>();

	public double respawnSeconds;

	public double respawnMinutes;

	public double respawnHours;

	public double respawnDays = 1.0;

	public int initialValue = 5;

	public int secondaryValue = 1;

	public int capsule_index = -1;

	public GameObject capsuleRef;

	private int currentValue;

	private DateTime collectedTime;

	private DateTime respawnTime;

	public bool canCollect = true;

	private AudioClip clip
	{
		get
		{
			return MissionController.get_capsule_sound;
		}
	}

	private static bool has_gotten
	{
		get
		{
			return collected_list.Count > 0;
		}
	}

	private void Awake()
	{
		currentValue = initialValue;
		if (capsule_index != -1)
		{
			if (!all_list.Contains(this))
			{
				all_list.Add(this);
			}
			else
			{
				Debug.Log("capsule duplicate..." + base.name);
			}
			LoadMyInfo();
		}
		else
		{
			canCollect = false;
		}
	}

	public string GetSaveName()
	{
		return string.Format("capsule_{0}", capsule_index.ToString());
	}

	public string GetSaveString()
	{
		return respawnTime.ToString();
	}

	public void SaveMyInfo()
	{
		PlayerPrefs.SetString(GetSaveName(), GetSaveString());
	}

	public void LoadMyInfo()
	{
		string @string = PlayerPrefs.GetString(GetSaveName(), string.Empty);
		if (@string != string.Empty)
		{
			Disable();
		}
	}

	private void CheckRespawn()
	{
		if (!canCollect)
		{
			CheckTimeElapsed();
		}
		else
		{
			CancelInvoke("CheckRespawn");
		}
	}

	private void CheckTimeElapsed()
	{
		if (!canCollect && DateTime.Compare(DateTime.Now, respawnTime) == 1)
		{
			Enable();
		}
	}

	public void Enable()
	{
		if (collected_list.Contains(this))
		{
			collected_list.Remove(this);
		}
		canCollect = true;
		if ((bool)capsuleRef.transform.GetComponent<MeshRenderer>())
		{
			capsuleRef.transform.GetComponent<MeshRenderer>().enabled = true;
		}
		if ((bool)capsuleRef.transform.GetComponent<CapsuleCollider>())
		{
			capsuleRef.transform.GetComponent<CapsuleCollider>().enabled = true;
		}
		base.collider.enabled = true;
	}

	public void Disable()
	{
		if (!collected_list.Contains(this))
		{
			collected_list.Add(this);
		}
		canCollect = false;
		if ((bool)capsuleRef.transform.GetComponent<MeshRenderer>())
		{
			capsuleRef.transform.GetComponent<MeshRenderer>().enabled = false;
		}
		if ((bool)capsuleRef.transform.GetComponent<CapsuleCollider>())
		{
			capsuleRef.transform.GetComponent<CapsuleCollider>().enabled = false;
		}
		base.collider.enabled = false;
	}

	private void Collect()
	{
		if (!has_gotten)
		{
			PhoneMemory.SendMail("tut_capsule");
			PhoneInterface.SendPhoneCommand("open_mail tut_capsule");
		}
		Disable();
		collectedTime = DateTime.Now;
		respawnTime = DateTime.Now;
		respawnTime = respawnTime.AddSeconds(respawnSeconds);
		respawnTime = respawnTime.AddMinutes(respawnMinutes);
		respawnTime = respawnTime.AddHours(respawnHours);
		respawnTime = respawnTime.AddDays(respawnDays);
		if ((bool)clip)
		{
			AudioSource.PlayClipAtPoint(clip, Vector3.zero);
		}
		PhoneInterface.AddCapsulePoints(currentValue);
		PlaytomicController.LogPosition("tGotCapsule", base.transform.position);
		Playtomic.Log.CustomMetric("tGotACapsule", PlaytomicController.current_group, true);
		Playtomic.Log.CustomMetric("Capsule_" + capsule_index, PlaytomicController.current_group, true);
		SaveMyInfo();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (canCollect && collider.name == "Player")
		{
			Collect();
		}
	}
}
