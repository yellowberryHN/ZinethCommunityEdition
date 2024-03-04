using UnityEngine;

public class PhoneLoaderMenu : PhoneMainMenu
{
	private bool force_tutorial = true;

	public PhoneButton tutorialButton;

	public PhoneButton continueButton;

	public PhoneButton newGameButton;

	public PhoneButton newGamePlusButton;

	public PhoneButton customMapButton;

	private bool custom_maps_enabled;

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
				PlayerPrefsX.SetBool("tried_tutorial", true);
			}
			else
			{
				PlayerPrefs.DeleteKey("tried_tutorial");
			}
		}
	}

	private void Start()
	{
		custom_maps_enabled = PlayerPrefsX.GetBool("custom_maps_loader", false);
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		var has_game_data = CheckForGameData();
		if ((bool)continueButton)
		{
			if (has_game_data)
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
			if (has_game_data)
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
		if (!(bool)customMapButton && has_game_data && custom_maps_enabled)
		{
			var customMapButtonObj = (GameObject)Instantiate(newGamePlusButton.gameObject);
            	customMapButtonObj.transform.parent = transform;
            	customMapButtonObj.transform.localPosition = new Vector3(0.45f, 5.0f, -0.3f);
            	customMapButtonObj.transform.localScale = newGamePlusButton.transform.localScale;
            	customMapButtonObj.transform.localRotation = newGamePlusButton.transform.localRotation;
            	customMapButton = customMapButtonObj.GetComponent<PhoneButton>();
            	customMapButton.wantedpos = new Vector3(0.45f, 5.0f, -0.4f);
            	customMapButton.text = "custom map";
            	customMapButton.command = ".custom";
                customMapButton.up_button = newGamePlusButton;
                customMapButton.selectable = true;
                newGamePlusButton.down_button = customMapButton;
            	buttons.Add(customMapButton);
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

	public void CustomMap()
	{
		CleanUp();
		Application.LoadLevel("test");
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
		case "custom":
			CustomMap();
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
