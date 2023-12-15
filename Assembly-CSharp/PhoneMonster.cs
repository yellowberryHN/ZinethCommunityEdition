using UnityEngine;

public class PhoneMonster
{
	public float level = 1f;

	public Vector2 _scale = Vector2.one;

	public float _speed = 1f;

	public MonsterType monsterType;

	public float bullet_speed = 1f;

	public float bullet_cooldown = 1f;

	public float bullet_homing = 1f;

	public MonsterTraits traits;

	public MonsterStat[] stats = new MonsterStat[4];

	public Vector2 scale
	{
		get
		{
			return monsterType.scaleMod * _scale;
		}
		set
		{
			_scale = value;
		}
	}

	public float speed
	{
		get
		{
			return monsterType.speedMod * _speed;
		}
		set
		{
			_speed = value;
		}
	}

	public SpriteSet spriteset
	{
		get
		{
			return monsterType.spriteSet;
		}
	}

	public bool flying_animate
	{
		get
		{
			return monsterType.flyingAnimate;
		}
	}

	public MonsterTraits.Name name
	{
		get
		{
			return traits.name;
		}
	}

	public MonsterTraits.BloodType bloodtype
	{
		get
		{
			return traits.bloodtype;
		}
	}

	public MonsterStat attackStat
	{
		get
		{
			return stats[0];
		}
		set
		{
			stats[0] = value;
		}
	}

	public MonsterStat defenseStat
	{
		get
		{
			return stats[1];
		}
		set
		{
			stats[1] = value;
		}
	}

	public MonsterStat magicStat
	{
		get
		{
			return stats[2];
		}
		set
		{
			stats[2] = value;
		}
	}

	public MonsterStat glamStat
	{
		get
		{
			return stats[3];
		}
		set
		{
			stats[3] = value;
		}
	}

	public float attack
	{
		get
		{
			return attackStat.current;
		}
	}

	public float defense
	{
		get
		{
			return defenseStat.current;
		}
	}

	public float magic
	{
		get
		{
			return magicStat.current;
		}
	}

	public float glam
	{
		get
		{
			return glamStat.current;
		}
	}

	public PhoneMonster()
	{
		GenerateStats();
	}

	public PhoneMonster(float lev)
	{
		level = lev;
		GenerateStats();
	}

	public PhoneMonster(MonsterType montype)
	{
		monsterType = montype;
		GenerateStats();
	}

	public PhoneMonster(MonsterType montype, float lev)
	{
		level = lev;
		monsterType = montype;
		GenerateStats();
	}

	public void GenerateType()
	{
		monsterType = PhoneResourceController.RandomMonsterType();
	}

	public void GenerateTraits()
	{
		traits = new MonsterTraits();
	}

	public void GenerateImage()
	{
	}

	public void GenerateStats()
	{
		if (monsterType == null)
		{
			GenerateType();
		}
		for (int i = 0; i < 4; i++)
		{
			float num = monsterType.statMods[i];
			MonsterStat monsterStat = new MonsterStat();
			stats[i] = monsterStat;
			monsterStat.statMod = num;
			monsterStat.max = 25f + Mathf.Pow(level, 1.5f) * Random.Range(9f, 11f) * num;
			monsterStat.current = monsterStat.remaining * Random.Range(0.05f, 0.16f);
			monsterStat.potential = monsterStat.remaining * Random.Range(0.25f, 0.45f);
			monsterStat.max = Mathf.Clamp(monsterStat.max * Random.Range(1.5f, 1.6f), 35f, 100f);
			monsterStat.locked = monsterStat.remaining;
		}
		GenerateTraits();
	}

	public void GenerateScale()
	{
		float num = 0.1f;
		scale = Vector2.right * (1f + Random.Range(0f - num, num)) + Vector2.up * (1f + Random.Range(0f - num, num));
	}

	public static string GetSavePrefix(int index)
	{
		return string.Format("monster{0}_", index.ToString());
	}

	public static bool SaveDataExists(int index)
	{
		return PlayerPrefs.HasKey(string.Format("{0}namef", GetSavePrefix(index)));
	}

	public bool SaveMonster(int index)
	{
		string savePrefix = GetSavePrefix(index);
		PlayerPrefs.SetString(savePrefix + "version", PhoneInterface.version);
		PlayerPrefs.SetString(savePrefix + "namef", traits.name.firstname);
		PlayerPrefs.SetString(savePrefix + "namel", traits.name.lastname);
		PlayerPrefs.SetString(savePrefix + "monster_type", monsterType.typeName);
		PlayerPrefs.SetFloat(savePrefix + "level", level);
		PlayerPrefs.SetString(savePrefix + "blood", traits.bloodtype);
		PlayerPrefs.SetString(savePrefix + "attack_stat", attackStat.ToSaveString());
		PlayerPrefs.SetString(savePrefix + "defense_stat", defenseStat.ToSaveString());
		PlayerPrefs.SetString(savePrefix + "magic_stat", magicStat.ToSaveString());
		PlayerPrefs.SetString(savePrefix + "glam_stat", glamStat.ToSaveString());
		PlayerPrefs.SetFloat(savePrefix + "scale_x", _scale.x);
		PlayerPrefs.SetFloat(savePrefix + "scale_y", _scale.y);
		PlayerPrefs.SetFloat(savePrefix + "speed", _speed);
		PlayerPrefs.SetFloat(savePrefix + "bullet_speed", bullet_speed);
		PlayerPrefs.SetFloat(savePrefix + "bullet_cooldown", bullet_cooldown);
		PlayerPrefs.SetFloat(savePrefix + "bullet_homing", bullet_homing);
		return true;
	}

	public static PhoneMonster LoadMonster(int index)
	{
		string savePrefix = GetSavePrefix(index);
		PhoneMonster phoneMonster = new PhoneMonster();
		phoneMonster.traits.name.firstname = PlayerPrefs.GetString(savePrefix + "namef");
		phoneMonster.traits.name.lastname = PlayerPrefs.GetString(savePrefix + "namel");
		string @string = PlayerPrefs.GetString(savePrefix + "monster_type", string.Empty);
		if (@string == string.Empty)
		{
			phoneMonster.monsterType = PhoneResourceController.RandomMonsterType();
		}
		else
		{
			phoneMonster.monsterType = PhoneResourceController.GetMonsterType(@string);
		}
		phoneMonster.level = PlayerPrefs.GetFloat(savePrefix + "level");
		phoneMonster.bloodtype.typename = PlayerPrefs.GetString(savePrefix + "blood");
		phoneMonster.attackStat = MonsterStat.LoadFromString(PlayerPrefs.GetString(savePrefix + "attack_stat"));
		phoneMonster.defenseStat = MonsterStat.LoadFromString(PlayerPrefs.GetString(savePrefix + "defense_stat"));
		phoneMonster.magicStat = MonsterStat.LoadFromString(PlayerPrefs.GetString(savePrefix + "magic_stat"));
		phoneMonster.glamStat = MonsterStat.LoadFromString(PlayerPrefs.GetString(savePrefix + "glam_stat"));
		for (int i = 0; i < 4; i++)
		{
			phoneMonster.stats[i].statMod = phoneMonster.monsterType.statMods[i];
		}
		Vector2 vector = new Vector2(PlayerPrefs.GetFloat(savePrefix + "scale_x", 1f), PlayerPrefs.GetFloat(savePrefix + "scale_y", 1f));
		phoneMonster.scale = vector;
		phoneMonster.speed = PlayerPrefs.GetFloat(savePrefix + "speed", 1f);
		phoneMonster.bullet_speed = PlayerPrefs.GetFloat(savePrefix + "bullet_speed", 1f);
		phoneMonster.bullet_cooldown = PlayerPrefs.GetFloat(savePrefix + "bullet_cooldown", 1f);
		phoneMonster.bullet_homing = PlayerPrefs.GetFloat(savePrefix + "bullet_homing", 1f);
		return phoneMonster;
	}
}
