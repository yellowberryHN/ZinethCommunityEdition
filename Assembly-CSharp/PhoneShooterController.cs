using System.Collections.Generic;
using UnityEngine;

public class PhoneShooterController : PhoneScreen
{
	public PhoneShooterSpawner spawner_prefab;

	public PhoneShooterPlayer player_prefab;

	public PhoneShooterMonster enemy_prefab;

	public static PhoneLabel slabel_prefab;

	public PhoneLabel label_prefab;

	public PhoneLabel wave_label;

	private int attack_tip_count;

	private bool attack_tip_ready = true;

	public PhoneLabel attack_tip_label;

	private PhoneLabel textlabel;

	public Transform background_trans;

	public int enemy_level;

	private int _wave = 1;

	public bool paused;

	public bool battle_mode;

	private float battle_start_timer;

	public List<PhoneElement> stat_elements = new List<PhoneElement>();

	public PhoneShooterMonster player_object;

	public PhoneShooterEnemy battle_enemy_object;

	public PhoneLabel attack_stat_label;

	public PhoneLabel defense_stat_label;

	public PhoneLabel magic_stat_label;

	public PhoneLabel glam_stat_label;

	private float gameovertimer;

	public bool spawn_new;

	private float enemytimer;

	public PhoneShooterLevel current_level
	{
		get
		{
			return PhoneMemory.level_obj;
		}
	}

	public int score
	{
		get
		{
			return wave * 100;
		}
	}

	public int wave
	{
		get
		{
			return _wave;
		}
		set
		{
			_wave = value;
			PhoneMemory.phoneGameScore = score;
		}
	}

	private PhoneMonster battle_enemy_monster
	{
		get
		{
			if ((bool)PhoneMemory.trainer_challenge)
			{
				return PhoneMemory.trainer_challenge.monster;
			}
			return null;
		}
	}

	private PhoneMonster playermonster
	{
		get
		{
			return PhoneMemory.main_monster;
		}
	}

	public override void Init()
	{
		PhoneLabel[] array = new PhoneLabel[6] { attack_stat_label, defense_stat_label, magic_stat_label, glam_stat_label, wave_label, attack_tip_label };
		foreach (PhoneLabel phoneLabel in array)
		{
			if ((bool)phoneLabel)
			{
				stat_elements.Add(phoneLabel);
				if ((bool)phoneLabel.shadow_label)
				{
					stat_elements.Add(phoneLabel.shadow_label);
				}
			}
		}
	}

	public override void OnLoad()
	{
		gameObject.SetActiveRecursively(true);
		battle_mode = current_level.trainer != null;
		enemy_level = current_level.difficulty;
		StartGame();
		foreach (PhoneLabel stat_element in stat_elements)
		{
			stat_element.text = string.Empty;
		}
		if ((bool)attack_tip_label && PlayerPrefs.HasKey("phone_shooter_tut"))
		{
			attack_tip_count = 3;
		}
		if (attack_tip_count < 3)
		{
			attack_tip_count = 0;
		}
	}

	public void StartGame()
	{
		SetBackground();
		player_object = SpawnPlayer();
		enemytimer = 2f;
		slabel_prefab = label_prefab;
		paused = false;
		gameovertimer = 0f;
		wave = 0;
		if (battle_mode)
		{
			battle_start_timer = 2f;
			wave_label.text = string.Empty;
		}
	}

	public override void OnExit()
	{
		PhoneElement[] componentsInChildren = gameObject.GetComponentsInChildren<PhoneElement>();
		foreach (PhoneElement phoneElement in componentsInChildren)
		{
			if (!stat_elements.Contains(phoneElement))
			{
				Destroy(phoneElement.gameObject);
			}
		}
		PhoneMemory.SaveMonsters();
		wave = 0;
		base.OnExit();
	}

	public override void OnPause()
	{
		BeginPause();
	}

	private void BeginPause()
	{
		PhoneMemory.SaveMonsters();
		if (textlabel == null)
		{
			textlabel = Instantiate(label_prefab) as PhoneLabel;
			textlabel.transform.position = transform.position + Vector3.up * 4f;
			textlabel.transform.parent = transform;
		}
		textlabel.textmesh.characterSize = 1f;
		textlabel.textmesh.alignment = TextAlignment.Center;
		textlabel.textmesh.anchor = TextAnchor.MiddleCenter;
		textlabel.text = "Touch to Resume";
		PhoneLabel[] array = new PhoneLabel[4] { attack_stat_label, defense_stat_label, magic_stat_label, glam_stat_label };
		foreach (PhoneLabel phoneLabel in array)
		{
			phoneLabel.gameObject.active = true;
			if ((bool)phoneLabel.shadow_label)
			{
				phoneLabel.shadow_label.gameObject.active = true;
			}
			phoneLabel.CancelInvoke("Hide");
		}
		paused = true;
	}

	private void EndPause()
	{
		if ((bool)textlabel)
		{
			Destroy(textlabel.gameObject);
		}
		PhoneLabel[] array = new PhoneLabel[4] { attack_stat_label, defense_stat_label, magic_stat_label, glam_stat_label };
		foreach (PhoneLabel phoneLabel in array)
		{
			phoneLabel.Invoke("Hide", 1f);
		}
		paused = false;
	}

	public virtual void SetBackground()
	{
		SetBackground(current_level);
	}

	public virtual void SetBackground(PhoneShooterLevel level)
	{
		SetBackground(level.texture);
	}

	public virtual void SetBackground(Texture2D texture)
	{
		if ((bool)background_trans)
		{
			background_trans.renderer.material.mainTexture = texture;
		}
	}

	public bool CheckGameOver()
	{
		return player_object == null;
	}

	public bool CheckWin()
	{
		return battle_mode && battle_enemy_object == null && wave > 0;
	}

	public override void UpdateScreen()
	{
		if (paused)
		{
			if (PhoneInput.IsPressedDown())
			{
				EndPause();
			}
			return;
		}
		if ((bool)attack_tip_label && (bool)player_object)
		{
			if (player_object.attack_timer > 0f && attack_tip_ready)
			{
				attack_tip_count++;
				attack_tip_ready = false;
			}
			else if (player_object.attack_timer <= 0f)
			{
				attack_tip_ready = true;
			}
			if (attack_tip_count >= 3)
			{
				PlayerPrefsX.SetBool("phone_shooter_tut", true);
				PlayerPrefs.Save();
				stat_elements.Remove(attack_tip_label);
				Destroy(attack_tip_label.gameObject);
			}
			else
			{
				attack_tip_label.text = string.Format("Click to Attack ({0}/3)", attack_tip_count.ToString());
			}
		}
		if (battle_mode && battle_start_timer > 0f && !attack_tip_label)
		{
			OnBattleIntro();
			{
				foreach (PhoneElement stat_element in stat_elements)
				{
					stat_element.OnUpdate();
				}
				return;
			}
		}
		GameUpdate();
		UpdateElements();
		CheckStats();
		if ((bool)wave_label && !attack_tip_label)
		{
			if (battle_mode)
			{
				wave_label.text = string.Empty;
			}
			else
			{
				wave_label.text = string.Format("{0:000000}", score);
			}
		}
		if (CheckWin())
		{
			OnGameWin();
		}
		else if (CheckGameOver())
		{
			OnGameOver();
		}
	}

	public virtual void GameUpdate()
	{
		if (battle_mode)
		{
			BattleEnemyUpdate();
		}
		else
		{
			EnemyUpdate();
		}
	}

	public virtual void EnemyUpdate()
	{
		SpawnEnemies();
	}

	public virtual void BattleEnemyUpdate()
	{
	}

	private string StatString(float stat)
	{
		return stat.ToString("0.0");
	}

	private void CheckStats()
	{
		DoShowStat(attack_stat_label, "Atk: ", playermonster.attackStat);
		DoShowStat(defense_stat_label, "Def: ", playermonster.defenseStat);
		DoShowStat(magic_stat_label, "Mag: ", playermonster.magicStat);
		DoShowStat(glam_stat_label, "Glm: ", playermonster.glamStat);
	}

	private void DoShowStat(PhoneLabel label, string txt, MonsterStat statobj)
	{
		string text = txt;
		text += StatString(statobj.current);
		if (statobj.potential <= 0f)
		{
			text += "(MAX)";
		}
		if ((bool)label && (label.text != text || statobj.extra >= 0.1f))
		{
			if (statobj.extra >= 0.1f)
			{
				statobj.extra -= 0.1f;
			}
			label.animateOnLoad = true;
			if (label.animateOnLoad)
			{
				label.transform.position += transform.forward * -0.25f;
			}
			label.gameObject.active = true;
			if ((bool)label.shadow_label)
			{
				label.shadow_label.gameObject.active = true;
			}
			label.text = text;
			label.CancelInvoke("Hide");
			label.Invoke("Hide", 1f);
		}
	}

	private void DoShowStat(PhoneLabel label, string txt, float stat)
	{
		string text = txt + StatString(stat);
		if ((bool)label && label.text != text)
		{
			label.animateOnLoad = true;
			if (label.animateOnLoad)
			{
				label.transform.position += transform.forward * -0.25f;
			}
			label.gameObject.active = true;
			if ((bool)label.shadow_label)
			{
				label.shadow_label.gameObject.active = true;
			}
			label.text = text;
			label.CancelInvoke("Hide");
			label.Invoke("Hide", 1f);
		}
	}

	public virtual void OnBattleIntro()
	{
		if (battle_enemy_object == null)
		{
			Vector3 vector = RandomSpawnPoint();
			while (Vector3.Distance(player_object.transform.position, vector) < 1f)
			{
				vector = RandomSpawnPoint();
			}
			battle_enemy_object = SpawnBoss(battle_enemy_monster, vector);
		}
		battle_start_timer -= deltatime;
		if (textlabel == null)
		{
			textlabel = Instantiate(label_prefab) as PhoneLabel;
			textlabel.transform.position = transform.position + Vector3.up * 4f + Vector3.forward * 1f;
			textlabel.transform.parent = transform;
			textlabel.textmesh.alignment = TextAlignment.Center;
			textlabel.textmesh.anchor = TextAnchor.MiddleCenter;
			textlabel.textmesh.characterSize = 4f;
		}
		string text = Mathf.FloorToInt(battle_start_timer * 1.5f + 1f).ToString();
		if (textlabel.text != text)
		{
			textlabel.text = text;
		}
		if (battle_start_timer <= 0f && (bool)textlabel)
		{
			Destroy(textlabel.gameObject);
		}
	}

	public virtual void OnGameWin()
	{
		if (gameovertimer <= 0f)
		{
			PhoneAudioController.PlayAudioClip("win", SoundType.game);
			if (battle_mode)
			{
				for (int i = 0; i < 4; i++)
				{
					PhoneMemory.main_monster.stats[i].Grow(current_level.trainer.monster.level / 40f);
				}
			}
			if ((bool)current_level.trainer)
			{
				current_level.trainer.OnDefeated();
			}
		}
		gameovertimer += deltatime;
		if (gameovertimer >= 0.25f)
		{
			if (textlabel == null)
			{
				textlabel = Instantiate(label_prefab) as PhoneLabel;
				textlabel.transform.position = transform.position + Vector3.up * 4f;
				textlabel.transform.parent = transform;
			}
			textlabel.textmesh.characterSize = 2.5f;
			string text = textlabel.text;
			textlabel.text = "You";
			if (gameovertimer >= 1f)
			{
				textlabel.text = "You\n\nWin!";
			}
			if (text != textlabel.text)
			{
				PhoneEffects.AddCamShake(0.2f);
			}
			textlabel.textmesh.alignment = TextAlignment.Center;
			textlabel.textmesh.anchor = TextAnchor.MiddleCenter;
		}
		if (gameovertimer > 0.75f && PhoneInput.IsPressedDown())
		{ 
			Destroy(textlabel.gameObject);
			controller.LoadScreen("Game");
		}
	}

	public virtual void OnGameOver()
	{
		if (gameovertimer <= 0f)
		{
			PhoneAudioController.PlayAudioClip("die", SoundType.game);
		}
		gameovertimer += deltatime;
		if (gameovertimer >= 0.25f)
		{
			if (textlabel == null)
			{
				textlabel = Instantiate(label_prefab) as PhoneLabel;
				textlabel.transform.position = transform.position + Vector3.up * 4f;
				textlabel.transform.parent = transform;
			}
			textlabel.textmesh.characterSize = 2.5f;
			string text = textlabel.text;
			textlabel.text = "Game";
			if (gameovertimer >= 1f)
			{
				textlabel.text = "Game\n\nOver!";
			}
			if (text != textlabel.text)
			{
				PhoneEffects.AddCamShake(0.2f);
			}
			textlabel.textmesh.alignment = TextAlignment.Center;
			textlabel.textmesh.anchor = TextAnchor.MiddleCenter;
		}
		if (gameovertimer > 0.5f && PhoneInput.IsPressedDown())
		{
			Destroy(textlabel.gameObject);
			controller.LoadScreen("Game");
		}
	}

	private void SpawnEnemies()
	{
		if ((bool)attack_tip_label)
		{
			return;
		}
		if (spawn_new)
		{
			SpawnEnemy();
		}
		spawn_new = false;
		enemytimer = Mathf.Max(0f, enemytimer - base.deltatime);
		if (enemytimer <= 0f)
		{
			int num = Mathf.Min(1 + (int)Mathf.Pow(wave, 0.25f), 10);
			int num2 = transform.GetComponentsInChildren(typeof(PhoneShooterEnemy)).Length;
			num2 += transform.GetComponentsInChildren(typeof(PhoneShooterSpawner)).Length;
			if (num2 < num)
			{
				SpawnEnemy();
				enemytimer = Random.Range(0.15f, 2f);
			}
			else
			{
				enemytimer = Random.Range(0.15f, 0.75f);
			}
		}
	}

	private void UpdateElements()
	{
		PhoneElement[] componentsInChildren = gameObject.GetComponentsInChildren<PhoneElement>();
		foreach (PhoneElement phoneElement in componentsInChildren)
		{
			phoneElement.OnUpdate();
			if (phoneElement.name.Contains("Enemy") || phoneElement.name.StartsWith("food_") || phoneElement.name.Contains("Bullet") || phoneElement.name.Contains("Player") || phoneElement.name.Contains("Attack"))
			{
				Vector3 localPosition = phoneElement.transform.localPosition;
				localPosition.y = 5f + (transform.position.z - phoneElement.renderer.bounds.min.z) / 100f;
				if (phoneElement.transform.localPosition != localPosition)
				{
					phoneElement.transform.localPosition = localPosition;
				}
			}
		}
	}

	private PhoneMonster GetMonster()
	{
		return GetMonster(enemy_level + wave / 50f);
	}

	private PhoneMonster GetMonster(float level)
	{
		return new PhoneMonster(current_level.RandomMonsterType(), level);
	}

	public Vector3 RandomSpawnPoint()
	{
		Vector3 result = default(Vector3);
		for (int i = 0; i < 3; i++)
		{
			result[i] = Random.Range(collider.bounds.min[i], collider.bounds.max[i]);
		}
		result.y = transform.position.y + 1f;
		return result;
	}

	private void SpawnEnemy()
	{
		SpawnEnemy(GetMonster(), RandomSpawnPoint());
	}

	private void SpawnEnemy(PhoneMonster monster, Vector3 pos)
	{
		Vector3 up = Vector3.up;
		PhoneShooterSpawner phoneShooterSpawner = Instantiate(spawner_prefab, pos - up, Quaternion.identity) as PhoneShooterSpawner;
		phoneShooterSpawner.transform.parent = transform;
		phoneShooterSpawner.monster = monster;
		phoneShooterSpawner.prefab = enemy_prefab;
		phoneShooterSpawner.offset = up;
		phoneShooterSpawner.controller = this;
	}

	private PhoneShooterPlayer SpawnPlayer()
	{
		return SpawnPlayer(transform.position + Vector3.up);
	}

	private PhoneShooterPlayer SpawnPlayer(Vector3 pos)
	{
		return SpawnPlayer(playermonster, pos);
	}

	private PhoneShooterPlayer SpawnPlayer(PhoneMonster monster, Vector3 pos)
	{
		PhoneShooterPlayer phoneShooterPlayer = Instantiate(player_prefab, pos, Quaternion.identity) as PhoneShooterPlayer;
		phoneShooterPlayer.transform.parent = transform;
		phoneShooterPlayer.SetMonster(monster);
		return phoneShooterPlayer;
	}

	private PhoneShooterEnemy SpawnBoss(PhoneMonster monster, Vector3 pos)
	{
		PhoneShooterEnemy phoneShooterEnemy = Instantiate(enemy_prefab, pos, Quaternion.identity) as PhoneShooterEnemy;
		phoneShooterEnemy.transform.parent = transform;
		phoneShooterEnemy.controller = this;
		phoneShooterEnemy.SetMonster(monster);
		return phoneShooterEnemy;
	}
}
