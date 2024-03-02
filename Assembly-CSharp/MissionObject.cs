using System.Collections.Generic;
using UnityEngine;

public class MissionObject : MonoBehaviour
{
	public string title = "Default Title";

	public string id = "zzzzz";

	public bool auto_add = true;

	public bool auto_active;

	public int status;

	public string description = "This describes the mission. The player will love to read this description.";

	public string introText = "This is the text that you see when receiving a mission.";

	public string outroText = "This is the text that you see when completing a mission.";

	public bool is_new = true;

	public bool set_focus_after_first = true;

	public bool play_sound = true;

	public AudioClip completed_sound;

	public AudioClip start_sound;

	public AudioClip failed_sound;

	public string command_string = string.Empty;

	public string gui_message = string.Empty;

	public bool show_gui;

	public bool send_mail;

	public PhoneMail completed_mail;

	public List<MissionObjective> objectives = new List<MissionObjective>();

	public MissionInfo GetMissionInfo()
	{
		return new MissionInfo(title, id, status, description, introText, outroText);
	}

	public List<MissionObjective> GetCurrentObjectives()
	{
		List<MissionObjective> list = new List<MissionObjective>();
		for (int i = 0; i < objectives.Count; i++)
		{
			if (!objectives[i].completed && !objectives[i].skipAsCurrent)
			{
				list.Add(objectives[i]);
				if (objectives[i].block_next)
				{
					return list;
				}
			}
		}
		return list;
	}

	private void Awake()
	{
		if (objectives.Count == 0)
		{
			MissionObjective[] componentsInChildren = GetComponentsInChildren<MissionObjective>();
			MissionObjective[] array = componentsInChildren;
			foreach (MissionObjective item in array)
			{
				objectives.Add(item);
			}
		}
		LoadMyInfo();
		if (MissionController.all_missions.Count > 0)
		{
			MissionController.AddMission(this);
		}
	}

	public string GetSaveName()
	{
		return string.Format("mission_{0}", id);
	}

	public int GetSaveValue()
	{
		return status;
	}

	public void SaveMyInfo()
	{
		PlayerPrefs.SetInt(GetSaveName(), GetSaveValue());
		PlayerPrefs.Save();
	}

	public void LoadMyInfo()
	{
		var saved_status = PlayerPrefs.GetInt(GetSaveName(), -1);
		if (saved_status != -1 && saved_status >= 0 && saved_status <= 2)
		{
			status = saved_status;
		}
		else
		{
			status = 0;
		}
	}

	private void Update()
	{
		if (status == 1)
		{
			string guitext = MissionController.guitext;
			CheckObjectives();
			if (MissionController.focus_mission != this)
			{
				MissionController.guitext = guitext;
			}
		}
	}

	public void StartMission()
	{
		foreach (MissionObjective objective in objectives)
		{
			objective.completed = false;
			objective.gameObject.SetActiveRecursively(false);
		}
	}

	public void FailMission()
	{
		if (play_sound)
		{
			if ((bool)failed_sound)
			{
				PhoneAudioController.PlayAudioClip(failed_sound, SoundType.other);
			}
			else if ((bool)MissionController.mission_fail_sound)
			{
				PhoneAudioController.PlayAudioClip(MissionController.mission_fail_sound, SoundType.other);
			}
		}
		DoFailedGUI();
		RestartMission();
	}

	public void RestartMission()
	{
		StartMission();
	}

	private void CheckObjectives()
	{
		if (show_gui)
		{
			MissionController.guitext = gui_message + "\n";
		}
		else
		{
			MissionController.guitext = string.Empty;
		}
		bool flag = true;
		for (int i = 0; i < objectives.Count; i++)
		{
			MissionObjective missionObjective = objectives[i];
			if (missionObjective.completed)
			{
				continue;
			}
			if (!missionObjective.gameObject.active)
			{
				missionObjective.OnBegin();
			}
			if (missionObjective.CheckCompleted())
			{
				if (i == 0 && set_focus_after_first)
				{
					MissionController.SetFocus(id);
				}
				missionObjective.completed = true;
				missionObjective.OnCompleted();
				continue;
			}
			flag = false;
			if (missionObjective.failed)
			{
				RestartMission();
				return;
			}
			if (missionObjective.block_next)
			{
				return;
			}
		}
		if (flag)
		{
			if (Application.isEditor)
			{
				MonoBehaviour.print("all completed");
			}
			SetComplete();
		}
	}

	private void SetComplete()
	{
		MissionController.SetComplete(id);
		OnComplete();
	}

	protected virtual void OnComplete()
	{
		if (play_sound)
		{
			AudioClip audioClip = ((!completed_sound) ? MissionController.mission_complete_sound : completed_sound);
			if (audioClip != null)
			{
				PhoneAudioController.PlayAudioClip(audioClip, SoundType.other);
			}
		}
		if (Application.isEditor)
		{
			MonoBehaviour.print("mission completed: " + title);
		}
		DoCompletedGUI();
		foreach (MissionObjective objective in objectives)
		{
			objective.OnEnd();
		}
		if (send_mail)
		{
			MailController.SendMail(completed_mail);
		}
		if (command_string != string.Empty)
		{
			PhoneController.DoPhoneCommand(command_string);
		}
	}

	protected virtual void DoCompletedGUI()
	{
		MissionGUIText missionGUIText = MissionGUIText.Create(title + " Complete!", new Vector3(0.024f, 0.2857143f, 0f), Vector3.one * 10f);
		missionGUIText.color = Color.green;
		missionGUIText.velocity = Vector3.up * 0.25f;
		missionGUIText.stopAfter = 0.15f;
		missionGUIText.lifeTime = 2f;
	}

	protected virtual void DoFailedGUI()
	{
		MissionGUIText missionGUIText = MissionGUIText.Create(title + " Failed", new Vector3(0.024f, 2f / 3f, 0f), Vector3.one * 10f);
		missionGUIText.color = Color.red;
		missionGUIText.velocity = Vector3.down * 0.12f;
		missionGUIText.shake = Vector2.right * 3f;
		missionGUIText.stopAfter = 1f;
		missionGUIText.lifeTime = 2f;
	}
}
