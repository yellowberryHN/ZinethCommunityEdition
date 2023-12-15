using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneParser : MonoBehaviour
{
	private delegate bool CommandDelegate(string[] args);

	private PhoneController controller;

	public List<string> commandlist = new List<string>();

	private Dictionary<string, CommandDelegate> commandMethods = new Dictionary<string, CommandDelegate>();

	public void Init(PhoneController control)
	{
		controller = control;
		SetupCommandMethods();
	}

	public bool ParseNode(List<string> lines)
	{
		foreach (string line in lines)
		{
			commandlist.Add(line);
		}
		DoNextCommand();
		return true;
	}

	public void DoNextCommand()
	{
		while (commandlist.Count > 0)
		{
			string command = commandlist[0];
			commandlist.RemoveAt(0);
			_DoStringCommand(command);
		}
	}

	public bool DoStringCommand(string command)
	{
		string[] array = command.Split('|');
		string[] array2 = array;
		foreach (string item in array2)
		{
			commandlist.Add(item);
		}
		DoNextCommand();
		return true;
	}

	private void SetupCommandMethods()
	{
		commandMethods.Add("back", BackCommand);
		commandMethods.Add("load_screen", LoadScreenCommand);
		commandMethods.Add("set_bgcolor", SetColorCommand);
		commandMethods.Add("mission_focus", SetMissionFocus);
		commandMethods.Add("mission_activate", ActivateMission);
		commandMethods.Add("mission_activate_insert", ActivateInsertMission);
		commandMethods.Add("set_mute", SetMuteCommand);
		commandMethods.Add("mail_send", SendMail);
		commandMethods.Add("mail_send_quiet", SendMailQuiet);
		commandMethods.Add("capsule_points_add", AddCapsulePoints);
		commandMethods.Add("player_freeze", FreezePlayer);
		commandMethods.Add("player_unfreeze", UnfreezePlayer);
		commandMethods.Add("menu_unlock", UnlockMenu);
		commandMethods.Add("menu_lock", LockMenu);
		commandMethods.Add("monster_save_all", SaveMonsters);
		commandMethods.Add("save_monsters", SaveMonsters);
		commandMethods.Add("monster_reset_all", ResetMonsters);
		commandMethods.Add("reset_monsters", ResetMonsters);
		commandMethods.Add("enable_hawk_control", EnableHawkControl);
		commandMethods.Add("unlock_zine", UnlockZine);
		commandMethods.Add("print_zine", PrintZine);
		commandMethods.Add("open_phone", OpenPhone);
		commandMethods.Add("close_phone", ClosePhone);
		commandMethods.Add("open_mail", OpenMail);
		commandMethods.Add("summon_hawk", SummonHawk);
		commandMethods.Add("hawk_whistle", SummonHawk);
		commandMethods.Add("reset_prefs", ResetPlayerPrefs);
		commandMethods.Add("delete_prefs", ResetPlayerPrefs);
		commandMethods.Add("tut_slide", SetSlides);
		commandMethods.Add("load_scene", LoadScene);
		commandMethods.Add("open_url", OpenURL);
		commandMethods.Add("unlock_cam", UnlockCamera);
	}

	public bool _DoStringCommand(string command)
	{
		command.Trim();
		if (command.StartsWith("//") || command == string.Empty)
		{
			return true;
		}
		string[] array = command.Split(' ');
		if (array.Length <= 0)
		{
			Debug.LogWarning("empty command");
			return false;
		}
		if (commandMethods.ContainsKey(array[0]))
		{
			return commandMethods[array[0]](array);
		}
		Debug.LogWarning("Command not valid: " + array[0]);
		return false;
	}

	public bool BackCommand(string[] args)
	{
		return controller.LoadPrevious();
	}

	public bool LoadScreenCommand(string[] args)
	{
		if (args.Length >= 2)
		{
			return controller.LoadScreen(ArgsToString(args, 1));
		}
		Debug.LogWarning("Command " + args[0] + "needs a node name.");
		return false;
	}

	public bool SetColorCommand(string[] args)
	{
		if (args.Length >= 2)
		{
			controller.SetBackColor(PhoneTextController.GetColor(ArgsToString(args, 1)));
			return true;
		}
		Debug.LogWarning("Command 'color' needs a color name.");
		return false;
	}

	public bool SetMuteCommand(string[] args)
	{
		if (args.Length >= 2)
		{
			if (args[1] == "0" || args[1] == "off" || args[1] == "false")
			{
				PhoneMemory.settings.muted = false;
			}
			else
			{
				PhoneMemory.settings.muted = true;
			}
		}
		else
		{
			PhoneMemory.settings.muted = !PhoneMemory.settings.muted;
		}
		return true;
	}

	public bool ActivateInsertMission(string[] args)
	{
		if (args.Length >= 2)
		{
			return MissionController.SetActive(ArgsToString(args, 1), true);
		}
		Debug.LogWarning("Command 'mission_activate' needs a mission id.");
		return false;
	}

	public bool ActivateMission(string[] args)
	{
		if (args.Length >= 2)
		{
			return MissionController.SetActive(ArgsToString(args, 1));
		}
		Debug.LogWarning("Command 'mission_activate' needs a mission id.");
		return false;
	}

	public bool SetMissionFocus(string[] args)
	{
		if (args.Length >= 2)
		{
			MissionController.SetFocus(ArgsToString(args, 1));
			return true;
		}
		Debug.LogWarning("Command 'mission_focus' needs a node name.");
		return false;
	}

	public bool SendMail(string[] args)
	{
		if (args.Length >= 2)
		{
			return MailController.SendMail(ArgsToString(args, 1));
		}
		Debug.LogWarning("Command " + args[0] + " needs a mail id.");
		return false;
	}

	public bool SendMailQuiet(string[] args)
	{
		if (args.Length >= 2)
		{
			return MailController.SendMailQuiet(ArgsToString(args, 1));
		}
		Debug.LogWarning("Command " + args[0] + " needs a mail id.");
		return false;
	}

	public bool AddCapsulePoints(string[] args)
	{
		if (args.Length >= 2)
		{
			float result;
			if (float.TryParse(args[1], out result))
			{
				PhoneInterface.AddCapsulePoints(result);
				return true;
			}
			Debug.LogWarning("Command " + args[0] + " needs a float value.");
			return false;
		}
		Debug.LogWarning("Command " + args[0] + " needs an amount.");
		return false;
	}

	public bool PlayAudioClip(string[] args)
	{
		if (args.Length >= 2)
		{
			return PhoneAudioController.PlayAudioClip(ArgsToString(args, 1)) != null;
		}
		Debug.LogWarning("Command " + args[0] + " needs a sound name.");
		return false;
	}

	public bool FreezePlayer(string[] args)
	{
		Transform transform = GameObject.Find("Player").transform;
		transform.GetComponent<move>().freezeControls = true;
		transform.rigidbody.velocity = Vector3.zero;
		return true;
	}

	public bool UnfreezePlayer(string[] args)
	{
		GameObject.Find("Player").GetComponent<move>().freezeControls = false;
		return true;
	}

	public bool UnlockMenu(string[] args)
	{
		if (args.Length > 1)
		{
			PhoneMemory.UnlockMenu(ArgsToString(args, 1));
			return true;
		}
		Debug.LogWarning("Command " + args[0] + " needs a screen id.");
		return false;
	}

	public bool LockMenu(string[] args)
	{
		if (args.Length > 1)
		{
			PhoneMemory.LockMenu(ArgsToString(args, 1));
			return true;
		}
		Debug.LogWarning("Command " + args[0] + " needs a screen id.");
		return false;
	}

	public bool SaveMonsters(string[] args)
	{
		return PhoneMemory.SaveMonsters();
	}

	public bool ResetMonsters(string[] args)
	{
		PhoneMemory.ResetMonsters();
		return true;
	}

	public bool EnableHawkControl(string[] args)
	{
		HawkBehavior hawkBehavior = Object.FindObjectOfType(typeof(HawkBehavior)) as HawkBehavior;
		hawkBehavior.canControl = true;
		return true;
	}

	public bool UnlockZine(string[] args)
	{
		if (args.Length >= 2)
		{
			int result;
			if (!int.TryParse(args[1], out result))
			{
				return false;
			}
			PhoneMemory.UnlockZine(result);
		}
		Debug.LogWarning("Not enough args in " + args[0]);
		return false;
	}

	public bool PrintZine(string[] args)
	{
		Debug.LogWarning("Trying to print...");
		return true;
	}

	public bool OpenPhone(string[] args)
	{
		StartCoroutine("DoOpenPhone", args);
		return true;
	}

	public IEnumerator DoOpenPhone(string[] args)
	{
		if (args.Length > 1)
		{
			controller.LoadScreen(ArgsToString(args, 1));
		}
		yield return null;
		PhoneInterface.view_controller.SetOpen(true);
	}

	public bool ClosePhone(string[] args)
	{
		PhoneInterface.view_controller.SetOpen(false);
		return true;
	}

	public bool OpenMail(string[] args)
	{
		if (args.Length > 1)
		{
			StartCoroutine("DoOpenMail", args);
			return true;
		}
		Debug.LogWarning("Not enough args in " + args[0]);
		return false;
	}

	public IEnumerator DoOpenMail(string[] args)
	{
		if (PhoneController.powerstate == PhoneController.PowerState.closed)
		{
			OpenPhone(new string[2]
			{
				string.Empty,
				"Mail"
			});
			yield return !PhoneInterface.view_controller.is_opening;
		}
		if (PhoneController.instance.curscreen.screenname != "Mail")
		{
			PhoneController.instance.LoadScreen("Mail");
		}
		PhoneController.instance.curscreen.SendMessage("OpenMail", ArgsToString(args, 1));
	}
	
	public bool SummonHawk(string[] args)
	{
		return PhoneInterface.SummonHawk();
	}

	public bool ResetPlayerPrefs(string[] args)
	{
		PlayerPrefs.DeleteAll();
		return true;
	}

	public bool SetSlides(string[] args)
	{
		if (args.Length > 1)
		{
			StartCoroutine(DoSetSlides(args));
			return true;
		}
		Debug.LogWarning("Command " + args[0] + " needs a Slideset name.");
		return false;
	}

	public IEnumerator DoSetSlides(string[] args)
	{
		if (PhoneController.powerstate == PhoneController.PowerState.closed)
		{
			OpenPhone(new string[2]
			{
				string.Empty,
				"Slideshow1"
			});
			yield return !PhoneInterface.view_controller.is_opening;
		}
		if (!controller.curscreen.name.Contains("Slideshow"))
		{
			if (controller.screendict.ContainsKey("Slideshow1"))
			{
				controller.LoadScreen("Slideshow1");
			}
			else
			{
				Debug.LogError("Tried to load a slideset but couldn't find the slideshow screen. Looked for \"Slideshow1\"");
			}
		}
		PhoneSlideshow slideshow = controller.curscreen as PhoneSlideshow;
		slideshow.LoadSlideSet(ArgsToString(args, 1));
	}

	public bool LoadScene(string[] args)
	{
		if (args.Length > 1)
		{
			Application.LoadLevel(ArgsToString(args, 1));
			return true;
		}
		Debug.LogWarning("Command " + args[0] + " needs a scene name.");
		return false;
	}

	public bool OpenURL(string[] args)
	{
		if (args.Length > 1)
		{
			Application.OpenURL(ArgsToString(args, 1));
			return true;
		}
		Debug.LogWarning("Command " + args[0] + " needs a url.");
		return false;
	}

	public bool UnlockCamera(string[] args)
	{
		PhoneMemory.UnlockMenu("Cool Cam");
		return true;
	}

	private string ArgsToString(string[] args, int startInd)
	{
		return ArgsToString(args, startInd, args.Length);
	}

	private string ArgsToString(string[] args, int startInd, int endInd)
	{
		string text = string.Empty;
		for (int i = startInd; i < endInd; i++)
		{
			text = text + args[i] + " ";
		}
		return text.TrimEnd(' ');
	}

	private int FindArg(string[] args, string str)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] == str)
			{
				return i;
			}
		}
		return -1;
	}
}
