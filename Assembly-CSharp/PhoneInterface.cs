using System.Collections.Generic;
using UnityEngine;

public class PhoneInterface : MonoBehaviour
{
	public static string version = "v0_27";

	private static PhoneViewController _view_controller;

	private static SpawnPointScript _spawn_point_script;

	public Transform _player_trans;

	private static move _player_move;

	private static Material[] _robot_materials;

	private static Material _robot_material;

	private static PhoneInterface _instance;

	private static HawkBehavior _hawk;

	private static PlayerTrail _playerTrail;

	private static Dictionary<string, int> int_dic = new Dictionary<string, int>();

	private static Dictionary<string, float> float_dic = new Dictionary<string, float>();

	private static Dictionary<string, string> string_dic = new Dictionary<string, string>();

	public static PhoneViewController view_controller
	{
		get
		{
			if (_view_controller == null)
			{
				_view_controller = Object.FindObjectOfType(typeof(PhoneViewController)) as PhoneViewController;
			}
			return _view_controller;
		}
	}

	public static SpawnPointScript spawn_point_script
	{
		get
		{
			if (!_spawn_point_script)
			{
				_spawn_point_script = Object.FindObjectOfType(typeof(SpawnPointScript)) as SpawnPointScript;
			}
			return _spawn_point_script;
		}
	}

	public static Transform player_trans
	{
		get
		{
			if (instance._player_trans == null)
			{
				GameObject gameObject = GameObject.Find("Player");
				if (!gameObject)
				{
					return null;
				}
				instance._player_trans = gameObject.transform;
			}
			return instance._player_trans;
		}
	}

	public static move player_move
	{
		get
		{
			if (_player_move == null)
			{
				_player_move = Object.FindObjectOfType(typeof(move)) as move;
			}
			return _player_move;
		}
	}

	private static Material[] robot_materials
	{
		get
		{
			if (_robot_materials == null)
			{
				FindRobotMaterials();
			}
			return _robot_materials;
		}
	}

	public static Material robot_material
	{
		get
		{
			if (_robot_material == null)
			{
				_robot_material = GameObject.Find("l_robo_arm").renderer.sharedMaterial;
			}
			return _robot_material;
		}
	}

	public static Color robotColor
	{
		get
		{
			return robot_materials[0].color;
		}
		set
		{
			Material[] array = robot_materials;
			foreach (Material material in array)
			{
				material.color = value;
			}
			PlayerPrefsX.SetColor("color_robot", value);
		}
	}

	public static PhoneInterface instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneInterface)) as PhoneInterface;
			}
			return _instance;
		}
	}

	public static HawkBehavior hawk
	{
		get
		{
			if (!_hawk)
			{
				_hawk = GameObject.Find("Hawk").GetComponent<HawkBehavior>();
			}
			return _hawk;
		}
	}

	public static PlayerTrail playerTrail
	{
		get
		{
			if (_playerTrail == null)
			{
				_playerTrail = player_trans.GetComponent<PlayerTrail>();
			}
			return _playerTrail;
		}
	}

	public static Color trailColor
	{
		get
		{
			return playerTrail.color;
		}
		set
		{
			playerTrail.color = value;
			PlayerPrefsX.SetColor("color_trail", value);
		}
	}
	
	private static void LoadSavedColors()
	{
		robotColor = PlayerPrefsX.GetColor("color_robot", 1893465975);
		trailColor = PlayerPrefsX.GetColor("color_trail", -16750923);
	}

	private static void FindRobotMaterials()
	{
		List<Material> list = new List<Material>();
		GameObject gameObject = GameObject.Find("Holder");
		if (gameObject == null) return;
		Renderer[] componentsInChildren = gameObject.transform.FindChild("main_character").FindChild("geometry_GRP").gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.material.name.StartsWith("mechcolors"))
			{
				list.Add(renderer.material);
			}
		}
		_robot_materials = new Material[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			_robot_materials[j] = list[j];
		}
		
		LoadSavedColors();
	}

	public static void AddPhoneMail(PhoneMail mail)
	{
		PhoneMemory.SendMail(mail);
	}

	public static bool SendPhoneCommand(string command)
	{
		return PhoneController.DoPhoneCommand(command);
	}

	public static void AddCapsulePoints(float amount)
	{
		PhoneMemory.AddCapsulePoints(amount);
	}

	public static bool SummonHawk()
	{
		if (!hawk)
		{
			return false;
		}
		hawk.inBounds = true;
		hawk.active = true;
		hawk.timeFollowed = hawk.maxTimeFollowed - 0.1f;
		PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_ring_long);
		return true;
	}

	public static bool IsZineVisible()
	{
		return view_controller.showing_zine;
	}

	public static bool HideZine()
	{
		return view_controller.HideZine();
	}

	public static bool ShowZine(int index)
	{
		return ShowZine(PhoneResourceController.zine_images[index]);
	}

	public static bool ShowZine(Texture2D texture)
	{
		return view_controller.ShowZine(texture);
	}

	public static bool ShowZine(Texture2D texture, bool resize)
	{
		return view_controller.ShowZine(texture, resize);
	}

	public static List<MissionObject> GetActiveMissions()
	{
		return MissionController.active_missions;
	}

	public static int GetPhoneScore()
	{
		return PhoneMemory.phoneGameScore;
	}

	public static float GetPlayerSpeed()
	{
		return player_trans.InverseTransformDirection(player_trans.rigidbody.velocity).z;
	}

	public static float GetStat(string stat)
	{
		if (stat == "phone_score")
		{
			return GetPhoneScore();
		}
		if (stat == "player_speed")
		{
			return GetPlayerSpeed();
		}
		Debug.LogWarning("unknown stat: " + stat);
		return -1f;
	}

	public static void ClearAllData()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void ClearGameData()
	{
		ClearGameData(false);
	}

	public static void ClearGameData(bool keepMonsters)
	{
		int_dic.Clear();
		float_dic.Clear();
		string_dic.Clear();
		StoreInfo("version", version);
		StoreInfoInt("tried_tutorial");
		StoreInfo("volume_master", 0.75f);
		StoreInfo("volume_menu", 1f);
		StoreInfo("volume_ring", 1f);
		StoreInfo("volume_game", 1f);
		StoreInfo("volume_music", 0.5f);
		StoreInfo("volume_vibrate", 0.75f);
		List<PhoneMonster> list = new List<PhoneMonster>();
		if (keepMonsters)
		{
			for (int i = 0; i < 10; i++)
			{
				if (PhoneMonster.SaveDataExists(i))
				{
					list.Add(PhoneMonster.LoadMonster(i));
				}
			}
			StoreInfo("cash", 0f);
			StoreInfoInt("debug_boost");
			StoreInfoInt("cool_cam");
			StoreInfoInt("hover_time");
		}
		ClearAllData();
		foreach (string key in int_dic.Keys)
		{
			PlayerPrefs.SetInt(key, int_dic[key]);
		}
		foreach (string key2 in float_dic.Keys)
		{
			PlayerPrefs.SetFloat(key2, float_dic[key2]);
		}
		foreach (string key3 in string_dic.Keys)
		{
			PlayerPrefs.SetString(key3, string_dic[key3]);
		}
		if (keepMonsters)
		{
			for (int j = 0; j < list.Count; j++)
			{
				list[j].SaveMonster(j);
			}
		}
	}

	private static void StoreInfoInt(string pref_name)
	{
		if (PlayerPrefs.HasKey(pref_name))
		{
			int_dic.Add(pref_name, PlayerPrefs.GetInt(pref_name));
		}
	}

	private static void StoreInfo(string pref_name, int default_val)
	{
		int_dic.Add(pref_name, PlayerPrefs.GetInt(pref_name, default_val));
	}

	private static void StoreInfoFloat(string pref_name)
	{
		if (PlayerPrefs.HasKey(pref_name))
		{
			float_dic.Add(pref_name, PlayerPrefs.GetFloat(pref_name));
		}
	}

	private static void StoreInfo(string pref_name, float default_val)
	{
		float_dic.Add(pref_name, PlayerPrefs.GetFloat(pref_name, default_val));
	}

	private static void StoreInfoString(string pref_name)
	{
		if (PlayerPrefs.HasKey(pref_name))
		{
			string_dic.Add(pref_name, PlayerPrefs.GetString(pref_name));
		}
	}

	private static void StoreInfo(string pref_name, string default_val)
	{
		string_dic.Add(pref_name, PlayerPrefs.GetString(pref_name, default_val));
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		if ((bool)player_trans)
		{
			FindRobotMaterials();
		}
	}

	public static void UnlockCamera()
	{
		if (!PlayerPrefsX.GetBool("cool_cam", false))
		{
			PlayerPrefsX.SetBool("cool_cam", true);
		}
		PhoneMemory.SendMail("cool_cam_mail");
	}
}
