using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
	public Arrow _arrowRef;

	public bool arrowActive;

	public GUIText missionText;

	public GUIText behindText;

	public AudioClip _checkpoint_sound;

	public AudioClip _mission_complete_sound;

	public AudioClip _mission_start_sound;

	public AudioClip _mission_fail_sound;

	public AudioClip _get_capsule_sound;

	public ThrownZine _thrown_zine_prefab;

	public static bool refresh = false;

	public static Dictionary<string, MissionObject> all_missions = new Dictionary<string, MissionObject>();

	public static List<MissionObject> active_missions = new List<MissionObject>();

	public static List<MissionObject> completed_missions = new List<MissionObject>();

	private static bool is_setup = false;

	private string old_guitext = string.Empty;

	private float gui_shake;

	private static MissionController _instance;

	public static MissionObject focus_mission;

	private static bool sort_by_name = true;

	private static bool auto_next_mission = true;

	public MissionObject[] unlock_camera_missions = new MissionObject[0];

	public MissionGUIText missionGUIPrefab;

	public static Arrow arrowRef
	{
		get
		{
			if (GetInstance()._arrowRef == null)
			{
				GetInstance()._arrowRef = GameObject.Find("Arrow").GetComponent<Arrow>();
			}
			return GetInstance()._arrowRef;
		}
	}

	public static AudioClip checkpoint_sound
	{
		get
		{
			return GetInstance()._checkpoint_sound;
		}
	}

	public static AudioClip mission_complete_sound
	{
		get
		{
			return GetInstance()._mission_complete_sound;
		}
	}

	public static AudioClip mission_start_sound
	{
		get
		{
			return GetInstance()._mission_start_sound;
		}
	}

	public static AudioClip mission_fail_sound
	{
		get
		{
			return GetInstance()._mission_fail_sound;
		}
	}

	public static AudioClip get_capsule_sound
	{
		get
		{
			return GetInstance()._get_capsule_sound;
		}
	}

	public static ThrownZine thrown_zine_prefab
	{
		get
		{
			return GetInstance()._thrown_zine_prefab;
		}
	}

	public static List<MissionObjective> focus_objectives
	{
		get
		{
			if (!focus_mission)
			{
				return new List<MissionObjective>();
			}
			List<MissionObjective> list = new List<MissionObjective>();
			foreach (MissionObjective currentObjective in focus_mission.GetCurrentObjectives())
			{
				if (currentObjective.use_position)
				{
					list.Add(currentObjective);
				}
			}
			return list;
		}
	}

	public static List<Vector3> focus_positions
	{
		get
		{
			List<Vector3> list = new List<Vector3>();
			foreach (MissionObjective focus_objective in focus_objectives)
			{
				if (focus_objective.use_position)
				{
					list.Add(focus_objective.objectivePosition);
				}
			}
			return list;
		}
	}

	private bool unlocked_cam
	{
		get
		{
			return PlayerPrefs.GetInt("cool_cam", 0) == 1;
		}
	}

	public static string guitext
	{
		get
		{
			return GetInstance().missionText.text;
		}
		set
		{
			GetInstance().missionText.text = value;
			GetInstance().behindText.text = value;
		}
	}

	public static List<PhoneMail> ActiveMissionsAsMail()
	{
		List<PhoneMail> list = new List<PhoneMail>();
		foreach (MissionObject active_mission in active_missions)
		{
			PhoneMail phoneMail = MissionToMail(active_mission);
			list.Add(phoneMail);
			MailController.AddMail(phoneMail);
		}
		return list;
	}

	public static PhoneMail MissionToMail(MissionObject mission)
	{
		PhoneMail phoneMail = new PhoneMail();
		phoneMail.id = "m_" + mission.id;
		phoneMail.subject = mission.title;
		phoneMail.body = mission.description;
		return phoneMail;
	}

	private void Awake()
	{
		behindText.font = missionText.font;
		behindText.material = missionText.material;
		behindText.material.color = Color.black;
		guitext = string.Empty;
		MissionGUIText.default_font = missionText.font;
		MissionGUIText.default_material = missionText.material;
	}

	private void Start()
	{
		is_setup = false;
		ClearMissions();
		AddAttachedMissions();
		is_setup = true;
	}

	private void ClearMissions()
	{
		all_missions.Clear();
		active_missions.Clear();
		completed_missions.Clear();
	}

	private void AddAttachedMissions()
	{
		MissionObject[] componentsInChildren = base.gameObject.GetComponentsInChildren<MissionObject>();
		foreach (MissionObject missionObject in componentsInChildren)
		{
			if (missionObject.auto_add)
			{
				AddMission(missionObject);
			}
		}
	}

	public static MissionController GetInstance()
	{
		if (!_instance)
		{
			_instance = Object.FindObjectOfType(typeof(MissionController)) as MissionController;
		}
		return _instance;
	}

	public static void SetFocus(string missionid)
	{
		MissionObject missionObject = FindMission(missionid);
		if (missionObject == null)
		{
			Debug.LogWarning("Mission " + missionid + " does not exist...");
		}
		else
		{
			SetFocus(missionObject);
		}
	}

	public static void SetFocus(MissionObject mission)
	{
		if (mission == null)
		{
			Debug.LogWarning("Mission does not exist...");
			return;
		}
		if (mission.status != 1)
		{
			Debug.LogWarning("Why are you trying to do this? This mission is not active but you are still trying to focus on it... are you kidding me? Is this a joke? " + mission.id);
			return;
		}
		focus_mission = mission;
		GetInstance().PointArrowAt(focus_mission);
		focus_mission.is_new = false;
		refresh = true;
	}

	public static void Unfocus()
	{
		focus_mission = null;
		GetInstance().DeactivateArrow();
	}

	public void PointArrowAt(MissionObject mission)
	{
		arrowRef.CheckAndPoint();
	}

	public void DeactivateArrow()
	{
		guitext = string.Empty;
	}

	public static MissionObject FindMission(string missionid)
	{
		if (!all_missions.ContainsKey(missionid))
		{
			Debug.LogWarning("HEY! Mission " + missionid + " does NOT exist!!!");
			return null;
		}
		return all_missions[missionid];
	}

	public static void AddMission(MissionObject missionobj)
	{
		if (all_missions.ContainsKey(missionobj.id))
		{
			return;
		}
		all_missions.Add(missionobj.id, missionobj);
		if (missionobj.status == 1)
		{
			missionobj.status = 0;
			SetActive(missionobj.id);
			return;
		}
		if (missionobj.auto_active && missionobj.status != 2)
		{
			SetActive(missionobj.id);
			return;
		}
		missionobj.gameObject.SetActiveRecursively(false);
		if (missionobj.status == 2)
		{
			completed_missions.Add(missionobj);
		}
	}

	public static MissionObject NewMission(string missionid)
	{
		GameObject gameObject = new GameObject("missionobj_" + missionid);
		return gameObject.AddComponent<MissionObject>();
	}

	public static bool SetActive(string missionid)
	{
		return SetActive(missionid, false);
	}

	public static bool SetActive(string missionid, bool insert)
	{
		MissionObject missionObject = FindMission(missionid);
		if (missionObject == null)
		{
			return false;
		}
		return SetActive(missionObject, insert);
	}

	public static bool SetActive(MissionObject mobj)
	{
		return SetActive(mobj, false);
	}

	private static int MissionCompare(MissionObject m1, MissionObject m2)
	{
		return m1.name.CompareTo(m2.name);
	}

	public static bool SetActive(MissionObject mobj, bool insert)
	{
		if (mobj.status == 1 || active_missions.Contains(mobj) || mobj.status == 2)
		{
			return false;
		}
		mobj.status = 1;
		mobj.gameObject.active = true;
		mobj.StartMission();
		if (insert)
		{
			active_missions.Insert(0, mobj);
		}
		else
		{
			active_missions.Add(mobj);
			if (sort_by_name)
			{
				active_missions.Sort(MissionCompare);
			}
		}
		refresh = true;
		mobj.SaveMyInfo();
		Playtomic.Log.CustomMetric("tMissionActivated " + mobj.id, PlaytomicController.current_group, true);
		return true;
	}

	public static bool SetComplete(string missionid)
	{
		MissionObject missionObject = FindMission(missionid);
		if (missionObject == null)
		{
			return false;
		}
		return SetComplete(missionObject);
	}

	public static bool SetComplete(MissionObject mobj)
	{
		mobj.status = 2;
		mobj.gameObject.SetActiveRecursively(false);
		completed_missions.Add(mobj);
		active_missions.Remove(mobj);
		GetInstance().DeactivateArrow();
		if (mobj == focus_mission)
		{
			focus_mission = null;
		}
		refresh = true;
		Playtomic.Log.CustomMetric("tMissionCompleted " + mobj.id, PlaytomicController.current_group, true);
		mobj.SaveMyInfo();
		if (active_missions.Count > 0)
		{
			if (auto_next_mission && focus_mission == null)
			{
				SetFocus(active_missions[0]);
			}
			else
			{
				Debug.Log(focus_mission);
				Debug.Log(auto_next_mission);
			}
		}
		if (is_setup)
		{
			GetInstance().CheckUnlockCamera();
		}
		return true;
	}

	public bool CheckUnlockCamera()
	{
		if (!unlocked_cam)
		{
			MissionObject[] array = unlock_camera_missions;
			foreach (MissionObject item in array)
			{
				if (!completed_missions.Contains(item))
				{
					return false;
				}
			}
		}
		if (Application.isEditor)
		{
			Debug.Log("completed all cam missions...");
		}
		PhoneInterface.UnlockCamera();
		return true;
	}
}
