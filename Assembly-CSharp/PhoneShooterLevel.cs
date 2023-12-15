using System.Collections.Generic;
using UnityEngine;

public class PhoneShooterLevel
{
	public string name = "Name";

	public int difficulty;

	public Texture2D texture;

	public NPCTrainer trainer;

	public List<MonsterType> monsterTypes = new List<MonsterType>();

	public PhoneShooterLevel(NPCTrainer npctrainer)
	{
		trainer = npctrainer;
		name = npctrainer.monster.name;
		difficulty = (int)npctrainer.level;
		if (npctrainer.level_bg != null)
		{
			texture = trainer.level_bg;
		}
		else
		{
			texture = PhoneResourceController.levelbackgrounds[0];
		}
	}

	public PhoneShooterLevel(string lvl_name, int lvl_difficulty, int lvl_texture)
	{
		name = lvl_name;
		difficulty = lvl_difficulty;
		if (PhoneResourceController.levelbackgrounds.Length > lvl_texture)
		{
			texture = PhoneResourceController.levelbackgrounds[lvl_texture];
		}
	}

	public PhoneShooterLevel(string lvl_name, int lvl_difficulty, Texture2D lvl_texture)
	{
		name = lvl_name;
		difficulty = lvl_difficulty;
		texture = lvl_texture;
	}

	public MonsterType RandomMonsterType()
	{
		if (monsterTypes.Count == 0)
		{
			return PhoneResourceController.RandomMonsterType();
		}
		return monsterTypes[Random.Range(0, monsterTypes.Count)];
	}
}
