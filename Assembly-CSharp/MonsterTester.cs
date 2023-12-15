using UnityEngine;

public class MonsterTester : MonoBehaviour
{
	public int level = 1;

	public PhoneMonster monster;

	public bool showgui
	{
		get
		{
			return base.useGUILayout;
		}
		set
		{
			base.useGUILayout = value;
		}
	}

	private void Awake()
	{
		base.useGUILayout = false;
	}

	private void Start()
	{
		monster = PhoneMemory.monsters[0];
	}

	private void Gen(int lev)
	{
		level = lev;
		Gen();
	}

	private void Gen()
	{
		if (monster == null || true)
		{
			monster = new PhoneMonster(level);
			return;
		}
		monster.level = level;
		monster.GenerateStats();
	}

	private void OnGUI()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			Debug.Log(MonsterTraits.Opinions.NewGetPossibleOpinion());
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			TweetComposer.MakeTweet();
		}
		if (!showgui)
		{
			return;
		}
		GUILayout.BeginHorizontal(string.Empty);
		for (int i = 0; i < PhoneMemory.monsters.Count; i++)
		{
			if (PhoneMemory.monsters[i] == monster)
			{
				GUILayout.Box("Slot " + i);
			}
			else if (GUILayout.Button("Slot " + i))
			{
				monster = PhoneMemory.monsters[i];
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(string.Empty);
		if (GUILayout.Button("Save"))
		{
			monster.SaveMonster(PhoneMemory.monsters.IndexOf(monster));
			MonoBehaviour.print("saved?");
		}
		if (GUILayout.Button("Load"))
		{
			monster = PhoneMonster.LoadMonster(PhoneMemory.monsters.IndexOf(monster));
			MonoBehaviour.print("loaded?");
		}
		for (int j = 1; j < 6; j++)
		{
			if (GUILayout.Button("Level " + j))
			{
				Gen(j);
			}
		}
		GUILayout.EndHorizontal();
		DrawStats(monster);
	}

	private void DrawStats(PhoneMonster mon)
	{
		GUILayout.Label(mon.name);
		GUILayout.Label(mon.monsterType);
		string[] array = new string[4] { "Attack", "Defense", "Magic", "Glam" };
		float mult = 4f;
		Rect rect = new Rect(60f, 120f, 0f, 40f);
		for (int i = 0; i < 4; i++)
		{
			GUI.Label(new Rect(10f, rect.y + 10f, rect.x - 10f, rect.height), array[i]);
			DrawStat(mon.stats[i], rect, mult);
			rect.y += rect.height;
		}
	}

	private void DrawStat(MonsterStat stat, Rect rect, float mult)
	{
		rect.width = stat.current * mult;
		if (GUI.Button(rect, " "))
		{
			stat.Grow(1f);
		}
		rect.x = rect.xMax;
		rect.width = stat.potential * mult;
		GUI.Box(rect, " ");
		rect.x = rect.xMax;
		rect.width = stat.locked * mult;
		if (GUI.Button(rect, " "))
		{
			stat.Unlock(1f);
		}
		rect.x = rect.xMax + 8f;
		rect.width = 400f;
		GUI.Label(rect, stat.current + " / " + stat.potential + " / " + stat.locked);
	}
}
