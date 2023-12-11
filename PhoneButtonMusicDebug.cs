using UnityEngine;

public class PhoneButtonMusicDebug : PhoneButton
{
	private bool is_on
	{
		get
		{
			return MusicManager.show_debug_gui;
		}
	}

	private void Awake()
	{
		if (textmesh == null)
		{
			textmesh = base.gameObject.GetComponent<TextMesh>();
		}
		if (controller == null)
		{
			controller = Object.FindObjectOfType(typeof(PhoneController)) as PhoneController;
		}
		if (is_on)
		{
			textmesh.text = "Music Debug(on)";
		}
		else
		{
			textmesh.text = "Music Debug(off)";
		}
		Init();
	}

	private void Start()
	{
	}

	public override void OnPressed()
	{
		MusicManager.show_debug_gui = !is_on;
		if (is_on)
		{
			textmesh.text = "Music Debug(on)";
		}
		else
		{
			textmesh.text = "Music Debug(off)";
		}
	}
}
