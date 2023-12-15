using UnityEngine;

public class PhoneScreen : MonoBehaviour
{
	public PhoneController controller;

	public string screenname = "Default";

	public bool keepactive;

	public bool bgscreen;

	public bool clearparticles;

	public Texture2D icon_texture;

	public AudioClip clip;

	public bool do_exit_effect = true;

	public bool use_other_exit_dir;

	public Vector3 exit_dir = new Vector3(5f, -3f, 0f);

	public float deltatime
	{
		get
		{
			return PhoneController.deltatime;
		}
	}

	public virtual void Init()
	{
	}

	public virtual void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
	}

	public virtual void OnHomeLoad()
	{
	}

	public virtual void OnExit()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public virtual void OnPause()
	{
	}

	public virtual void OnResume()
	{
	}

	public virtual void UpdateScreen()
	{
	}

	public virtual bool ButtonMessage(PhoneButton button, string message)
	{
		MonoBehaviour.print("empty message receiver (" + message + ")");
		return false;
	}

	public virtual PhoneButton Button_To(PhoneButton button)
	{
		return button;
	}

	public virtual float GetSliderVar(string message)
	{
		if (message.StartsWith("."))
		{
			message = message.Remove(0, 1);
		}
		if (message.StartsWith("trailcolor_"))
		{
			if (message.StartsWith("trailcolor_r"))
			{
				return PhoneInterface.trailColor.r;
			}
			if (message.StartsWith("trailcolor_g"))
			{
				return PhoneInterface.trailColor.g;
			}
			if (message.StartsWith("trailcolor_b"))
			{
				return PhoneInterface.trailColor.b;
			}
		}
		else if (message.StartsWith("robotcolor_"))
		{
			if (message.StartsWith("robotcolor_r"))
			{
				return PhoneInterface.robotColor.r;
			}
			if (message.StartsWith("robotcolor_g"))
			{
				return PhoneInterface.robotColor.g;
			}
			if (message.StartsWith("robotcolor_b"))
			{
				return PhoneInterface.robotColor.b;
			}
		}
		else if (message.StartsWith("bgcolor_"))
		{
			if (message.StartsWith("bgcolor_r"))
			{
				return PhoneMemory.settings.backgroundColor.r;
			}
			if (message.StartsWith("bgcolor_g"))
			{
				return PhoneMemory.settings.backgroundColor.g;
			}
			if (message.StartsWith("bgcolor_b"))
			{
				return PhoneMemory.settings.backgroundColor.b;
			}
		}
		else if (message.StartsWith("volume_"))
		{
			if (message.StartsWith("volume_menu"))
			{
				return PhoneMemory.settings.menu_volume;
			}
			if (message.StartsWith("volume_game"))
			{
				return PhoneMemory.settings.game_volume;
			}
			if (message.StartsWith("volume_music"))
			{
				return PhoneMemory.settings.music_volume;
			}
			if (message.StartsWith("volume_ring"))
			{
				return PhoneMemory.settings.ring_volume;
			}
			if (message.StartsWith("volume_master"))
			{
				return PhoneMemory.settings.master_volume;
			}
			if (message.StartsWith("volume_vibrate"))
			{
				return PhoneMemory.settings.vibrate_amount;
			}
		}
		Debug.LogWarning("Unknown slider command: " + message);
		return -1f;
	}
}
