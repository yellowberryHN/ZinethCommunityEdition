using System.Collections.Generic;
using UnityEngine;

public class PhoneShooterLevelMenu : PhoneMainMenu
{
	public PhoneLabel namelabel;

	public PhoneLabel lvllabel;

	public PhoneElement texturelabel;

	public PhoneLabel vslabel;

	public PhoneButton nextbut;

	public PhoneButton prevbut;

	public List<PhoneElement> moving_elements = new List<PhoneElement>();

	public string nextscreenname;

	public int levelind
	{
		get
		{
			return PhoneMemory.game_level;
		}
		set
		{
			PhoneMemory.game_level = value;
		}
	}

	public PhoneShooterLevel current_level
	{
		get
		{
			return PhoneMemory.level_obj;
		}
	}

	public Texture2D current_texture
	{
		get
		{
			return current_level.texture;
		}
	}

	public List<PhoneShooterLevel> levels
	{
		get
		{
			List<PhoneShooterLevel> list = new List<PhoneShooterLevel>(PhoneResourceController.phoneshooterlevels);
			if (PhoneMemory.trainer_challenge != null)
			{
				list.Add(PhoneMemory.trainer_challenge.levelobj);
			}
			return list;
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	private void Awake()
	{
		Init();
	}

	public override void UpdateScreen()
	{
		base.UpdateScreen();
		if ((bool)vslabel && current_level.trainer != null)
		{
			vslabel.text = "VS";
			vslabel.textmesh.characterSize = Mathf.Lerp(2.5f, 3.2f, (float)(Time.frameCount % 10) / 10f);
		}
		if (!PhoneMemory.trainer_challenge && levelind >= PhoneResourceController.phoneshooterlevels.Count)
		{
			levelind = 0;
		}
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		if (exit_animating)
		{
			cancel_exit_animate = true;
		}
		if ((bool)PhoneMemory.trainer_challenge)
		{
			levelind = levels.Count - 1;
		}
		menuind = 0;
		UpdateLevelTexture();
		PhoneElement[] array = base.elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		DoArrows();
		UpdateMenuItems();
	}

	public void UpdateLevelTexture()
	{
		if ((bool)texturelabel)
		{
			texturelabel.renderer.material.mainTexture = current_texture;
		}
		if ((bool)namelabel)
		{
			namelabel.text = current_level.name;
		}
		if ((bool)lvllabel)
		{
			lvllabel.text = "LVL " + current_level.difficulty;
		}
		if ((bool)vslabel)
		{
			if (current_level.trainer != null)
			{
				vslabel.text = "VS";
			}
			else
			{
				vslabel.text = string.Empty;
			}
		}
	}

	private void NextLevel()
	{
		levelind++;
		if (levelind >= levels.Count)
		{
			levelind = levels.Count - 1;
		}
		else
		{
			foreach (PhoneElement moving_element in moving_elements)
			{
				if (moving_element.animateOnLoad)
				{
					moving_element.transform.position += Vector3.right * 1f;
				}
				if (moving_element == texturelabel)
				{
					moving_element.transform.position += Vector3.right * 1f;
				}
			}
		}
		UpdateLevelTexture();
		if ((bool)nextbut)
		{
			nextbut.transform.position -= base.transform.right * 0.15f;
		}
		DoArrows();
	}

	private void PreviousLevel()
	{
		levelind--;
		if (levelind < 0)
		{
			levelind = 0;
		}
		else
		{
			foreach (PhoneElement moving_element in moving_elements)
			{
				if (moving_element.animateOnLoad)
				{
					moving_element.transform.position -= Vector3.right * 1f;
				}
				if (moving_element == texturelabel)
				{
					moving_element.transform.position -= Vector3.right * 1f;
				}
			}
		}
		UpdateLevelTexture();
		if ((bool)prevbut)
		{
			prevbut.transform.position += base.transform.right * 0.15f;
		}
		DoArrows();
	}

	private void DoArrows()
	{
		bool flag = false;
		if ((bool)prevbut)
		{
			flag = levelind > 0;
			prevbut.renderer.enabled = flag;
			prevbut.selectable = flag;
			foreach (PhoneButton button in buttons)
			{
				if (flag)
				{
					button.left_button = prevbut;
				}
				else
				{
					button.left_button = null;
				}
			}
		}
		if ((bool)nextbut)
		{
			flag = levelind < levels.Count - 1;
			nextbut.renderer.enabled = flag;
			nextbut.selectable = flag;
			foreach (PhoneButton button2 in buttons)
			{
				if (flag)
				{
					button2.right_button = nextbut;
				}
				else
				{
					button2.right_button = null;
				}
			}
		}
		UpdateButtonSelected();
	}

	public override bool ButtonMessage(PhoneButton button, string command)
	{
		switch (command)
		{
		case "next":
			NextLevel();
			break;
		case "previous":
			PreviousLevel();
			break;
		case "accept":
			controller.LoadScreen(nextscreenname);
			break;
		default:
			return base.ButtonMessage(button, command);
		}
		return true;
	}

	protected override void DoStickControls()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.x >= 0.5f)
		{
			NextLevel();
		}
		else if (controlDirPressed.x <= -0.5f)
		{
			PreviousLevel();
		}
		if (controlDirPressed.y >= 0.5f)
		{
			menuind--;
		}
		if (controlDirPressed.y <= -0.5f)
		{
			menuind++;
		}
		if (controls_wrap)
		{
			if (menuind < 0)
			{
				menuind = Mathf.Max(0, buttons.Count - 1);
			}
			if (menuind >= buttons.Count)
			{
				menuind = 0;
			}
		}
		else
		{
			if (menuind < 0)
			{
				menuind = 0;
			}
			if (menuind >= buttons.Count)
			{
				menuind = buttons.Count - 1;
			}
		}
		while (buttons[menuind] == nextbut || buttons[menuind] == prevbut)
		{
			menuind++;
		}
		if (menuind >= buttons.Count)
		{
			if (controls_wrap)
			{
				menuind = 0;
			}
			else
			{
				menuind = buttons.Count - 1;
			}
		}
		if (PhoneInput.IsPressedDown() && buttons.Count > menuind)
		{
			buttons[menuind].OnPressed();
		}
	}
}
