using UnityEngine;

public class MonsterStat
{
	public float current;

	public float potential;

	public float locked;

	public float max;

	public float extra;

	public float statMod;

	public float remaining
	{
		get
		{
			return max - current - potential - locked;
		}
	}

	public void Grow(float amount)
	{
		amount = 10f / current * amount;
		amount *= 1.5f;
		amount *= statMod;
		float num = Mathf.Min(potential, amount);
		if (num < amount)
		{
			extra += amount - num;
		}
		potential -= num;
		current += num;
	}

	public void Unlock(float amount)
	{
		float num = Mathf.Min(locked, amount);
		locked -= num;
		potential += num;
	}

	public string ToSaveString()
	{
		return current + ";" + potential + ";" + locked;
	}

	public static MonsterStat LoadFromString(string str)
	{
		MonsterStat monsterStat = new MonsterStat();
		string[] array = str.Split(';');
		monsterStat.current = float.Parse(array[0]);
		monsterStat.potential = float.Parse(array[1]);
		monsterStat.locked = float.Parse(array[2]);
		monsterStat.max = monsterStat.current + monsterStat.potential + monsterStat.locked;
		return monsterStat;
	}
}
