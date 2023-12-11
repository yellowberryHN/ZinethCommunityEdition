using System.Collections.Generic;
using UnityEngine;

public class PhoneMonsterMenu : PhoneMainMenu
{
	public PhoneMonsterStatsDisplay statsdisplayer;

	public PhoneLabel namelabel;

	public PhoneButton nextbut;

	public PhoneButton prevbut;

	public PhoneLabel capsulepoints_label;

	public string nextscreenname;

	public PhoneButton acceptbut;

	private float last_points = -99f;

	public List<PhoneButton> monsterbuttons = new List<PhoneButton>();

	public PhoneElement moveElement;

	public PhoneLabel newmail_prefab;

	public int monsterind
	{
		get
		{
			return PhoneMemory.monster_ind;
		}
		set
		{
			PhoneMemory.monster_ind = value;
		}
	}

	public PhoneMonster current_monster
	{
		get
		{
			return PhoneMemory.monsters[monsterind];
		}
		set
		{
			PhoneMemory.monsters[monsterind] = value;
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
		base.gameObject.SetActiveRecursively(true);
		if (exit_animating)
		{
			cancel_exit_animate = true;
		}
		menuind = 0;
		UpdateStatsDisplayer();
		PhoneElement[] array = base.elements;
		foreach (PhoneElement phoneElement in array)
		{
			phoneElement.OnLoad();
		}
		UpdateMenuItems();
		UpdateButtonSelected();
		DoArrows();
	}

	public override void UpdateScreen()
	{
		base.UpdateScreen();
		statsdisplayer.OnUpdate();
		if ((bool)capsulepoints_label && PhoneMemory.capsule_points != last_points)
		{
			last_points = PhoneMemory.capsule_points;
			capsulepoints_label.text = "$" + last_points;
			capsulepoints_label.transform.position += Vector3.forward * -0.25f;
			PhoneMonsterStatbar[] bars = statsdisplayer.bars;
			foreach (PhoneMonsterStatbar phoneMonsterStatbar in bars)
			{
				phoneMonsterStatbar.DoPriceLabels();
			}
		}
	}

	public void UpdateStatsDisplayer()
	{
		if ((bool)namelabel)
		{
			namelabel.textmesh.text = current_monster.name;
		}
		statsdisplayer.SetMonster(current_monster);
	}

	private void NextMonster()
	{
		monsterind++;
		if (monsterind >= PhoneMemory.monsters.Count)
		{
			monsterind = PhoneMemory.monsters.Count - 1;
			return;
		}
		UpdateStatsDisplayer();
		statsdisplayer.MoveBarsRelative(Vector3.right * 4f);
		if ((bool)nextbut)
		{
			nextbut.transform.position -= base.transform.right * 0.15f;
		}
		if ((bool)moveElement)
		{
			moveElement.transform.position += base.transform.right * 2f;
		}
		DoArrows();
	}

	private void PreviousMonster()
	{
		monsterind--;
		if (monsterind < 0)
		{
			monsterind = 0;
			return;
		}
		UpdateStatsDisplayer();
		statsdisplayer.MoveBarsRelative(-Vector3.right * 4f);
		if ((bool)prevbut)
		{
			prevbut.transform.position += base.transform.right * 0.15f;
		}
		if ((bool)moveElement)
		{
			moveElement.transform.position -= base.transform.right * 2f;
		}
		DoArrows();
	}

	private void DoArrows()
	{
		bool flag = false;
		if ((bool)prevbut)
		{
			flag = monsterind > 0;
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
			flag = monsterind < PhoneMemory.monsters.Count - 1;
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

	private void UnlockStat(PhoneButton button)
	{
		PhoneMonsterStatbar phoneMonsterStatbar = button as PhoneMonsterStatbar;
		float num = Mathf.Min(PhoneMemory.capsule_points, 1f);
		if (!(num <= 0f) && !(phoneMonsterStatbar.stat.locked <= 0f))
		{
			PhoneMemory.AddCapsulePoints(0f - num);
			if (PhoneMemory.capsule_points < 0f)
			{
				PhoneMemory.AddCapsulePoints(0f - PhoneMemory.capsule_points);
			}
			phoneMonsterStatbar.stat.Unlock(num * 2f);
			phoneMonsterStatbar.DoRealPressedParticles();
			UpdateStatsDisplayer();
			PhoneMemory.SaveMonster(current_monster);
			Playtomic.Log.CustomMetric("tUnlockedStat", "tPhone", true);
			Playtomic.Log.CustomMetric("tStatsUnlocked", "tPhone", false);
		}
	}

	public override bool ButtonMessage(PhoneButton button, string command)
	{
		switch (command)
		{
		case "unlockstat":
			UnlockStat(button);
			break;
		case "next":
			NextMonster();
			break;
		case "previous":
			PreviousMonster();
			break;
		case "accept":
			controller.LoadScreen(nextscreenname);
			break;
		default:
			return base.ButtonMessage(button, command);
		}
		return true;
	}

	protected override void StickControls_Vertical()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (controlDirPressed.x >= 0.5f)
		{
			NextMonster();
		}
		else if (controlDirPressed.x <= -0.5f)
		{
			PreviousMonster();
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
				menuind = Mathf.Max(0, buttons.Count - 1);
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

	public override PhoneButton Button_To(PhoneButton button)
	{
		if (!newmail_prefab)
		{
			return button;
		}
		PhoneLabel phoneLabel = Object.Instantiate(newmail_prefab) as PhoneLabel;
		phoneLabel.transform.position = button.transform.position + new Vector3(0f, 0.25f, 0f);
		phoneLabel.transform.parent = button.button_icon.transform;
		phoneLabel.wantedpos = phoneLabel.transform.localPosition;
		phoneLabel.wantedrot = phoneLabel.transform.localRotation;
		return button;
	}
}
