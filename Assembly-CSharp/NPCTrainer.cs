using System.Collections.Generic;
using UnityEngine;

public class NPCTrainer : NPCBehavior
{
	public static List<NPCTrainer> all_list = new List<NPCTrainer>();

	public static List<NPCTrainer> defeated_list = new List<NPCTrainer>();

	public PhoneMonster monster;

	public bool auto_gen_traits = true;

	public string monster_first_name = string.Empty;

	public string monster_last_name = string.Empty;

	public float level = 1f;

	public float speed = 1f;

	public float bullet_speed = 1f;

	public float bullet_cooldown = 1f;

	public float bullet_homing;

	public string bloodtype = string.Empty;

	public string spriteset = string.Empty;

	public string monsterTypeName = string.Empty;

	public Vector2 scale = Vector2.one;

	public bool auto_gen_stats = true;

	public float attack;

	public float defense;

	public float magic;

	public float glam;

	public Texture2D level_bg;

	public string win_command = string.Empty;

	public bool auto_battle = true;

	public bool include_in_lists = true;

	public bool enable_saving = true;

	public PhoneShooterLevel levelobj;

	private bool _can_challege = true;

	public bool defeated;

	public float max_distance = 16f;

	public Texture2D waiting_icon;

	public Texture2D near_icon;

	public Texture2D battling_icon;

	public bool reinitmonster = true;

	private float challengeTimer = 1f;

	public static Transform player_trans
	{
		get
		{
			return PhoneInterface.player_trans;
		}
	}

	public bool can_challenge
	{
		get
		{
			return _can_challege && !defeated;
		}
		set
		{
			_can_challege = value;
		}
	}

	public virtual void InitMonster()
	{
		if (monsterTypeName != string.Empty && PhoneResourceController.instance.monsterTypeDic.ContainsKey(monsterTypeName))
		{
			MonsterType monsterType = PhoneResourceController.GetMonsterType(monsterTypeName);
			monster = new PhoneMonster(monsterType, level);
		}
		else
		{
			monster = new PhoneMonster(level);
		}
		if (monster_first_name != string.Empty)
		{
			monster.name.firstname = monster_first_name;
		}
		if (monster_last_name != string.Empty)
		{
			monster.name.lastname = monster_last_name;
		}
		if (!auto_gen_traits)
		{
			monster.scale = scale;
			monster.speed = speed;
			monster.bullet_speed = bullet_speed;
			monster.bullet_cooldown = bullet_cooldown;
			monster.bullet_homing = bullet_homing;
			if (bloodtype != string.Empty)
			{
				monster.traits.bloodtype.typename = bloodtype;
			}
		}
		if (!auto_gen_stats)
		{
			monster.attackStat.current = attack;
			monster.defenseStat.current = defense;
			monster.magicStat.current = magic;
			monster.glamStat.current = glam;
		}
		levelobj = new PhoneShooterLevel(this);
	}

	private void Awake()
	{
		if (include_in_lists && !all_list.Contains(this))
		{
			all_list.Add(this);
		}
	}

	private void Start()
	{
		Init();
		InitMonster();
		challengeTimer = 1f + Random.value;
		LoadMyInfo();
	}

	public virtual void ChallengeUpdate()
	{
		if (CheckBattling())
		{
			SetBubbleTexture(battling_icon);
			icon_bubble.renderer.material.SetVector("_BounceSpeed", new Vector4(0f, 0f, 1.5f, 0f));
		}
		else if (CheckChallenging())
		{
			icon_bubble.renderer.material.SetVector("_BounceSpeed", new Vector4(0f, 0f, 3f, 0f));
			Challenge();
		}
		else
		{
			icon_bubble.renderer.material.SetVector("_BounceSpeed", new Vector4(0f, 0f, 1f, 0f));
			UnChallenge();
		}
	}

	public virtual bool CheckBattling()
	{
		return PhoneMemory.IsBattlingTrainer(this);
	}

	public virtual bool CheckChallenging()
	{
		if (!can_challenge)
		{
			return false;
		}
		if ((bool)player_trans && Vector3.Distance(base.transform.position, player_trans.position) <= max_distance)
		{
			return true;
		}
		return false;
	}

	public virtual void Challenge()
	{
		if (PhoneMemory.MonsterChallenge(this) && auto_battle)
		{
			PhoneInterface.SendPhoneCommand("open_phone GameScreen");
		}
		if (PhoneMemory.trainer_challenge == this)
		{
			SetBubbleTexture(near_icon);
		}
	}

	public virtual void UnChallenge()
	{
		if (PhoneMemory.WithdrawChallenge(this))
		{
			SetBubbleTexture(waiting_icon);
		}
	}

	private void Update()
	{
		if (can_challenge)
		{
			if (challengeTimer <= 0f)
			{
				ChallengeUpdate();
				challengeTimer += 0.25f;
			}
			challengeTimer -= Time.deltaTime;
		}
		else
		{
			HideBubble();
		}
		if (reinitmonster)
		{
			InitMonster();
			reinitmonster = false;
		}
	}

	public void OnDefeated()
	{
		if (!string.IsNullOrEmpty(win_command))
		{
			PhoneController.DoPhoneCommand(win_command);
		}
		PhoneMemory.AddCapsulePoints(3f);
		RemoveBadge();
		SaveMyInfo();
	}

	public void GiveBadge()
	{
		can_challenge = true;
		defeated = false;
		ShowBubble();
		if (include_in_lists && defeated_list.Contains(this))
		{
			defeated_list.Remove(this);
		}
	}

	public void RemoveBadge()
	{
		can_challenge = false;
		defeated = true;
		HideBubble();
		PhoneMemory.WithdrawChallenge(this);
		if (include_in_lists && !defeated_list.Contains(this))
		{
			defeated_list.Add(this);
		}
	}

	public string GetSaveName()
	{
		return string.Format("npc_{0}", base.name.Replace("NPC_Trainer_", string.Empty));
	}

	public string GetSaveString()
	{
		return monster.name;
	}

	public void SaveMyInfo()
	{
		if (enable_saving)
		{
			PlayerPrefs.SetString(GetSaveName(), GetSaveString());
			PlayerPrefs.Save();
		}
	}

	public void LoadMyInfo()
	{
		if (enable_saving)
		{
			string @string = PlayerPrefs.GetString(GetSaveName(), string.Empty);
			if (@string != string.Empty)
			{
				RemoveBadge();
			}
		}
	}
}
