public class MonsterType
{
	public string typeName;

	public SpriteSet spriteSet;

	public MonsterAI monsterAI;

	public float[] statMods = new float[4] { 1f, 1f, 1f, 1f };

	public float speedMod = 1f;

	public float scaleMod = 1f;

	public bool flyingAnimate;

	public MonsterType(string typename, SpriteSet spriteset)
	{
		typeName = typename;
		spriteSet = spriteset;
	}

	public MonsterType(string typename, SpriteSet spriteset, MonsterAI monsterai)
	{
		typeName = typename;
		spriteSet = spriteset;
		monsterAI = monsterai;
	}

	public MonsterType(string typename, string spriteset)
	{
		typeName = typename;
		spriteSet = PhoneResourceController.GetSpriteSet(spriteset);
	}

	public MonsterType(string typename, string spriteset, MonsterAI monsterai)
	{
		typeName = typename;
		spriteSet = PhoneResourceController.GetSpriteSet(spriteset);
		monsterAI = monsterai;
	}

	public MonsterType(string typename, string spriteset, float speed)
	{
		typeName = typename;
		spriteSet = PhoneResourceController.GetSpriteSet(spriteset);
		speedMod = speed;
	}

	public MonsterType(string typename, string spriteset, MonsterAI monsterai, float speed)
	{
		typeName = typename;
		spriteSet = PhoneResourceController.GetSpriteSet(spriteset);
		monsterAI = monsterai;
		speedMod = speed;
	}

	public static implicit operator string(MonsterType monstertype)
	{
		return monstertype.typeName;
	}
}
