using UnityEngine;

public class PhoneLoaderMenu : PhoneMainMenu
{
	private bool force_tutorial = true;

	public PhoneButton tutorialButton;

	public PhoneButton continueButton;

	public PhoneButton newGameButton;

	public PhoneButton newGamePlusButton;

	private bool has_tried_tutorial
	{
		get
		{
			return PlayerPrefs.HasKey("tried_tutorial");
		}
		set
		{
			if (value)
			{
				PlayerPrefs.SetInt("tried_tutorial", 1);
			}
			else
			{
				PlayerPrefs.DeleteKey("tried_tutorial");
			}
		}
	}

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		bool flag = CheckForGameData();
		bool flag2 = has_tried_tutorial;
		if ((bool)continueButton)
		{
			if (flag)
			{
				continueButton.selectable = true;
			}
			else
			{
				continueButton.selectable = false;
				continueButton.gameObject.SetActiveRecursively(false);
			}
		}
		if ((bool)newGamePlusButton)
		{
			if (flag)
			{
				newGamePlusButton.selectable = true;
			}
			else
			{
				newGamePlusButton.selectable = false;
				newGamePlusButton.gameObject.SetActiveRecursively(false);
			}
		}
		if ((bool)tutorialButton && force_tutorial)
		{
			if (has_tried_tutorial)
			{
				tutorialButton.selectable = true;
			}
			else
			{
				tutorialButton.selectable = false;
				tutorialButton.gameObject.SetActiveRecursively(false);
			}
		}
		foreach (PhoneButton button in buttons)
		{
			if (button.gameObject.active)
			{
				button.down_button = CheckDown(button.down_button);
				button.up_button = CheckUp(button.up_button);
			}
		}
		if (menuind < 0 && PhoneInput.controltype == PhoneInput.ControlType.Keyboard)
		{
			menuind = 0;
		}
		if (menuind >= 0 && !buttons[menuind].selectable)
		{
			while (!buttons[menuind].selectable && menuind < buttons.Count)
			{
				menuind++;
			}
			UpdateButtonSelected();
		}
	}

	public static void CleanUp()
	{
		Capsule.all_list.Clear();
		Capsule.collected_list.Clear();
		SecretObject.all_list.Clear();
		SecretObject.collected_list.Clear();
		SecretObject.uncollected_list.Clear();
		NPCTrainer.all_list.Clear();
		NPCTrainer.defeated_list.Clear();
		PhoneMemory.unlocked_zines.Clear();
		PhoneMemory.ResetCapsulePoints();
	}

	public PhoneButton CheckDown(PhoneButton button)
	{
		if (button == null)
		{
			return null;
		}
		if (button.gameObject.active)
		{
			return button;
		}
		return CheckDown(button.down_button);
	}

	public PhoneButton CheckUp(PhoneButton button)
	{
		if (button == null)
		{
			return null;
		}
		if (button.gameObject.active)
		{
			return button;
		}
		return CheckUp(button.up_button);
	}

	public void LoadTutorial()
	{
		CleanUp();
		has_tried_tutorial = true;
		Application.LoadLevel("loader 5");
	}

	public void NewGame()
	{
		if (force_tutorial && !has_tried_tutorial)
		{
			LoadTutorial();
			return;
		}
		PhoneInterface.ClearGameData();
		ContinueGame();
	}

	public void NewGamePlus()
	{
		PhoneInterface.ClearGameData(true);
		ContinueGame();
	}

	public void ContinueGame()
	{
		if (force_tutorial && !has_tried_tutorial)
		{
			LoadTutorial();
			return;
		}
		CleanUp();
		Application.LoadLevel("loader 1");
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		switch (message)
		{
		case "tutorial":
			LoadTutorial();
			break;
		case "newgame":
			NewGame();
			break;
		case "newgameplus":
			NewGamePlus();
			break;
		case "continue":
			ContinueGame();
			break;
		default:
			return base.ButtonMessage(button, message);
		}
		return true;
	}

	private bool CheckForGameData()
	{
		return PlayerPrefs.HasKey("times_file_played");
	}
}
