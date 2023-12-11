using System;
using UnityEngine;

public class PlaytomicController : MonoBehaviour
{
	private long gameid = 939496L;

	private string guid = "62344ea476bb4220";

	private string apikey = "84f0b740a4ca4d57870d733c145ab5";

	private bool is_enabled = true;

	protected static bool do_pos_logging = true;

	private static bool has_viewed;

	private static bool has_init;

	public Transform player_trans;

	public static Vector3 offset;

	public static Vector2 scaling;

	public static int map_width;

	public static int map_height;

	private static Bounds bounds;

	public static string current_phone_group = "ReleasePhone";

	public static string current_group = "ReleaseMovement";

	private void Awake()
	{
		if (is_enabled)
		{
			if (Application.loadedLevelName != "Loader 1")
			{
				do_pos_logging = false;
			}
			if (!has_init)
			{
				Playtomic.Initialize(Convert.ToInt64(gameid), guid, apikey);
				has_init = true;
			}
			if (Application.loadedLevelName == "Loader 3")
			{
				MainMenuLoad();
			}
			else if (Application.loadedLevelName == "Loader 5")
			{
				TutorialLoad();
			}
			else if (Application.loadedLevelName == "Loader 1")
			{
				GameLoad();
			}
		}
	}

	private void MainMenuLoad()
	{
		if (!has_viewed)
		{
			Playtomic.Log.View();
			has_viewed = true;
			Playtomic.Log.CustomMetric("version_" + PhoneInterface.version);
		}
	}

	private void TutorialLoad()
	{
		Playtomic.Log.Play();
		Playtomic.Log.CustomMetric("tHasLoadedTutorial", "loading", true);
		current_group = "Tutorial_Movement";
	}

	private void GameLoad()
	{
		Playtomic.Log.Play();
		Playtomic.Log.CustomMetric("tHasLoadedGame", "loading", true);
		current_group = "Game_Movement";
	}

	private void Start()
	{
		if (player_trans == null)
		{
			GameObject gameObject = GameObject.Find("Player");
			if ((bool)gameObject)
			{
				player_trans = gameObject.transform;
			}
		}
		if (Application.loadedLevelName == "Loader 1")
		{
			SetOffset();
		}
		if ((bool)player_trans && do_pos_logging)
		{
			InvokeRepeating("LogPlayerPosition", 2f, 3f);
		}
	}

	private void SetOffset()
	{
		Terrain[] array = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
		bounds = new Bounds(array[0].transform.position, Vector3.zero);
		Terrain[] array2 = array;
		foreach (Terrain terrain in array2)
		{
			bounds.Encapsulate(terrain.collider.bounds);
		}
		offset = bounds.center;
		offset.y = -600f;
		int num = 2;
		map_width = 480 * num;
		map_height = 800 * num;
		float num2 = bounds.size.z * (2f / 3f);
		scaling = Vector2.one * ((float)map_height / (num2 * 2f));
	}

	public static string TranslatePlayerPosToGPSString()
	{
		return TranslateGPSToString(TranslatePlayerPosToGPS());
	}

	public static Vector3 TranslateGPSStringToPos(string str)
	{
		return TranslateGPSToPos(TranslateStringToGPS(str));
	}

	public static Vector3 TranslateStringToGPS(string str)
	{
		string[] array = str.Split(' ');
		Vector3 zero = Vector3.zero;
		string empty = string.Empty;
		string text = array[0];
		string s = text.Remove(text.IndexOf(".") + 2);
		zero.x = float.Parse(s);
		s = text.Substring(text.IndexOf(".") + 2);
		empty = empty + s + ".";
		text = array[1];
		s = text.Remove(text.IndexOf(".") + 2);
		zero.z = float.Parse(s);
		s = text.Substring(text.IndexOf(".") + 2);
		empty += s;
		zero.y = float.Parse(empty);
		return zero;
	}

	public static string TranslateGPSToString(Vector3 pos)
	{
		string text = pos.y.ToString("0.00");
		string text2 = pos.x.ToString("0.0");
		string text3 = pos.z.ToString("0.0");
		string[] array = text.Split('.');
		text = array[0];
		while (text.Length < 3)
		{
			text = "0" + text;
		}
		while (text.Length > 0)
		{
			text2 += text.Substring(0, 1);
			text = text.Remove(0, 1);
		}
		text = array[1];
		while (text.Length < 3)
		{
			text = "0" + text;
		}
		while (text.Length > 0)
		{
			text3 += text.Substring(0, 1);
			text = text.Remove(0, 1);
		}
		if (text2.Contains("Infinity"))
		{
			text2 = "0";
		}
		if (text3.Contains("Infinity"))
		{
			text3 = "0";
		}
		if (Application.isEditor)
		{
			Debug.Log(text2 + " , " + text3);
		}
		return text2 + "/" + text3;
	}

	public static Vector3 TranslatePlayerPosToGPS()
	{
		return TranslatePosToGPS(PhoneInterface.player_trans.position);
	}

	public static Vector3 TranslatePosToGPS(Vector3 position)
	{
		Vector3 vector = position - offset;
		Vector3 zero = Vector3.zero;
		float num = 180f / (bounds.size.x * 1.2f);
		float num2 = 360f / (bounds.size.z * 1.2f);
		float num3 = 0.1f;
		zero.x = vector.x * num;
		zero.y = vector.y * num3;
		zero.z = vector.z * num2;
		return zero;
	}

	public static Vector3 TranslateGPSToPos(Vector3 gps)
	{
		float num = 180f / (bounds.size.x * 1.2f);
		float num2 = 360f / (bounds.size.z * 1.2f);
		float num3 = 0.1f;
		Vector3 zero = Vector3.zero;
		zero.x = gps.x / num;
		zero.y = gps.y / num3;
		zero.z = gps.z / num2;
		return zero + offset;
	}

	public static Vector2 TranslatePos(Vector3 position)
	{
		Vector3 vector = position - offset;
		vector.z *= -1f;
		vector.x *= scaling.x;
		vector.z *= scaling.y;
		vector.x += map_width / 2;
		vector.z += map_height / 2;
		return new Vector2(vector.x, vector.z);
	}

	public static void LogPosition(string name, Vector3 position)
	{
		if (do_pos_logging)
		{
			LogPosition(name, current_group, position);
		}
	}

	public static void LogPosition(string name, string groupname, Vector3 position)
	{
		Vector2 vector = TranslatePos(position);
		Playtomic.Log.Heatmap(name, groupname, (long)vector.x, (long)vector.y);
	}

	private void LogPlayerPosition()
	{
		if ((bool)player_trans)
		{
			Vector2 vector = TranslatePos(player_trans.position);
			float magnitude = player_trans.rigidbody.velocity.magnitude;
			if (magnitude > 1f)
			{
				Playtomic.Log.Heatmap("tMoving", current_group, (long)vector.x, (long)vector.y);
			}
			else
			{
				Playtomic.Log.Heatmap("tNot Moving", current_group, (long)vector.x, (long)vector.y);
			}
		}
	}

	private void OnApplicationQuit()
	{
		if ((bool)player_trans)
		{
			LogPosition("tQuit", player_trans.position);
		}
		Playtomic.Log.ForceSend();
	}
}
