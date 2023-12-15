using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneResourceController : MonoBehaviour
{
	public delegate void TextureReturn(Texture2D tex);

	public Texture2D _monstersheet;

	public Texture2D[] _monsterimages;

	public Texture2D[] _levelbackgrounds;

	public static List<PhoneShooterLevel> phoneshooterlevels = new List<PhoneShooterLevel>();

	public List<Texture2D> _zine_images = new List<Texture2D>();

	private static bool has_init = false;

	private static PhoneResourceController _instance;

	public List<MonsterType> monsterTypes = new List<MonsterType>();

	public Dictionary<string, MonsterType> monsterTypeDic = new Dictionary<string, MonsterType>();

	public Transform infoBooth;

	public Camera spriteCamera;

	public PhoneMonsterStatsDisplay statDisplay;

	public Renderer backRend;

	public Texture2D[] infoBGs = new Texture2D[0];

	public static Texture2D[] monsterimages
	{
		get
		{
			return instance._monsterimages;
		}
	}

	public static Texture2D[] levelbackgrounds
	{
		get
		{
			return instance._levelbackgrounds;
		}
	}

	public static List<Texture2D> zine_images
	{
		get
		{
			return instance._zine_images;
		}
	}

	public static PhoneResourceController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneResourceController)) as PhoneResourceController;
				Init();
			}
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public static void Init()
	{
		has_init = true;
		SpriteController.Init();
		instance.SetupSprites();
		instance.SetupMonsterTypes();
		instance.SetupPhoneShooterLevels();
	}

	public static Texture2D RandomMonsterImage()
	{
		int num = Random.Range(0, monsterimages.Length);
		return monsterimages[num];
	}

	public static SpriteSet GetSpriteSet(string name)
	{
		return SpriteController.GetSpriteSet(name);
	}

	public static SpriteSet RandomMonsterSpriteSet()
	{
		if (instance == null)
		{
			Init();
		}
		List<string> list = new List<string>(SpriteController.spritesets.Keys);
		int index = Random.Range(0, list.Count);
		while (list[index] == "hawk")
		{
			index = Random.Range(0, list.Count);
		}
		return SpriteController.GetSpriteSet(list[index]);
	}

	private void Awake()
	{
		if (instance == null)
		{
			Init();
		}
	}

	private void Start()
	{
	}

	private void SetupSprites()
	{
		SetupMonster("fly", 1f, new Vector2(5f, 7f));
		SetupMonster("devil", 3f, new Vector2(5f, 7f));
		SetupMonster("mouse", 5f, new Vector2(5f, 7f));
		SetupMonster("pink", 7f, new Vector2(5f, 7f));
		SetupMonster("slime", 9f, new Vector2(5f, 7f));
		SetupMonster("red", 1f, 11f);
		SetupMonster("jack", 1f, 13f);
		SetupMonster("white", 5f, 11f);
		SetupMonster("black", 5f, 13f);
		SetupMonster("hawk", 9f, 11f);
	}

	private void SetupBullets(SpriteSet sprset)
	{
		sprset.AddAnimation("bullet", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(11f, 5f, 1f, 1f)));
	}

	private void SetupBullets(SpriteSet sprset, Vector2 pos)
	{
		sprset.AddAnimation("bullet", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(11f, 5f, 1f, 1f)));
	}

	private void SetupMonster(string name, float xoffset, float yoffset)
	{
		SpriteSet spriteSet = SpriteController.CreateSpriteSet(name, _monstersheet);
		spriteSet.AddAnimation("attack", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(xoffset + 1f, yoffset + 1f, 1f, 1f)));
		spriteSet.AddAnimation("walk", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(xoffset, yoffset, 4f, 1f)));
		SetupBullets(spriteSet);
	}

	private void SetupMonster(string name, float yoffset, Vector2 bulletpos)
	{
		SpriteSet sprset = SetupMonster(name, yoffset);
		SetupBullets(sprset, bulletpos);
	}

	private SpriteSet SetupMonster(string name, float yoffset)
	{
		SpriteSet spriteSet = SpriteController.CreateSpriteSet(name, _monstersheet);
		spriteSet.AddAnimation("attack", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(2f, yoffset + 1f, 1f, 1f)));
		spriteSet.AddAnimation("walk", SpriteController.AutoGenAnimation(new Vector2(16f, 16f), new Rect(1f, yoffset, 4f, 1f)));
		return spriteSet;
	}

	private void SetupPhoneShooterLevels()
	{
		phoneshooterlevels.Clear();
		phoneshooterlevels.Add(new PhoneShooterLevel("Grassy Farm", 1, 0));
		phoneshooterlevels.Add(new PhoneShooterLevel("Damp Desert", 5, 1));
		phoneshooterlevels.Add(new PhoneShooterLevel("Scrappy Seas", 10, 2));
		phoneshooterlevels.Add(new PhoneShooterLevel("Cloud Sky", 15, 3));
		phoneshooterlevels.Add(new PhoneShooterLevel("Space Galaxy", 20, 4));
	}

	private void SetupMonsterTypes()
	{
		monsterTypes.Clear();
		MonsterType monsterType = new MonsterType("Bug", "fly", MonsterAI.Jitter);
		monsterType.statMods = new float[4] { 0.75f, 0.8f, 0.9f, 1f };
		monsterType.speedMod = 1.5f;
		monsterType.scaleMod = 0.75f;
		monsterType.flyingAnimate = true;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Demon", "devil", MonsterAI.Mirror);
		monsterType.statMods = new float[4] { 1.25f, 0.9f, 1f, 0.75f };
		monsterType.speedMod = 1f;
		monsterType.flyingAnimate = true;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Cat", "mouse", MonsterAI.Circle);
		monsterType.statMods = new float[4] { 1.25f, 0.8f, 0.8f, 1.25f };
		monsterType.speedMod = 1.1f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Rabbit", "pink", MonsterAI.Run);
		monsterType.statMods = new float[4] { 0.75f, 1.5f, 0.75f, 1.5f };
		monsterType.speedMod = 0.9f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Ghost", "slime", MonsterAI.CircleCW);
		monsterType.statMods = new float[4] { 0.4f, 0.5f, 2f, 2f };
		monsterType.speedMod = 0.9f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Candle", "red", MonsterAI.Mirror);
		monsterType.statMods = new float[4] { 2f, 0.5f, 2f, 0.5f };
		monsterType.speedMod = 1f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Pumk", "jack", MonsterAI.Horizontal);
		monsterType.statMods = new float[4] { 0.8f, 1f, 1.5f, 1.5f };
		monsterType.speedMod = 0.75f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Pup", "white", MonsterAI.Goto);
		monsterType.statMods = new float[4] { 1.2f, 1f, 0.75f, 0.75f };
		monsterType.speedMod = 1.25f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Ink", "black", MonsterAI.Vertical);
		monsterType.statMods = new float[4] { 0.8f, 0.8f, 2f, 0.8f };
		monsterType.speedMod = 0.9f;
		monsterTypes.Add(monsterType);
		monsterType = new MonsterType("Hawk", "hawk", MonsterAI.Circle);
		monsterType.flyingAnimate = true;
		monsterTypes.Add(monsterType);
		foreach (MonsterType monsterType2 in monsterTypes)
		{
			monsterTypeDic.Add(monsterType2.typeName, monsterType2);
		}
	}

	public static MonsterType GetMonsterType(string typename)
	{
		if (instance.monsterTypeDic.ContainsKey(typename))
		{
			return instance.monsterTypeDic[typename];
		}
		Debug.LogWarning("Oh No! The monster type " + typename + " is not in the monstertype dictionary... dang...");
		return instance.monsterTypes[0];
	}

	public static MonsterType RandomMonsterType()
	{
		int index = Random.Range(0, instance.monsterTypes.Count - 1);
		return instance.monsterTypes[index];
	}

	public static void SaveMonsterInfoCard(PhoneMonster monster, TextureReturn texReturn)
	{
		instance.StartCoroutine(instance.MakeMonsterInfoTexture(monster, texReturn));
	}

	public IEnumerator MakeMonsterInfoTexture(PhoneMonster monster, TextureReturn texReturn)
	{
		int width = 240;
		int height = 135;
		if ((bool)backRend && infoBGs.Length > 0)
		{
			backRend.material.mainTexture = infoBGs[Random.Range(0, infoBGs.Length)];
		}
		spriteCamera.enabled = true;
		infoBooth.gameObject.SetActiveRecursively(true);
		statDisplay.SetMonster(monster);
		string anim = "walk";
		if (Random.value > 0.75f)
		{
			anim = "attack";
		}
		statDisplay.spriteplayer.SetAnimation(anim);
		statDisplay.OnUpdate();
		yield return null;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		RenderTexture rt = new RenderTexture(width, height, 24)
		{
			filterMode = FilterMode.Point
		};
		spriteCamera.targetTexture = rt;
		yield return new WaitForEndOfFrame();
		spriteCamera.Render();
		RenderTexture.active = rt;
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		spriteCamera.targetTexture = null;
		RenderTexture.active = null;
		Object.Destroy(rt);
		yield return null;
		spriteCamera.enabled = false;
		infoBooth.gameObject.SetActiveRecursively(false);
		texReturn(tex);
	}
}
