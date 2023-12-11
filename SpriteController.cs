using System.Collections.Generic;
using UnityEngine;

public static class SpriteController
{
	public static Dictionary<string, SpriteSet> spritesets = new Dictionary<string, SpriteSet>();

	public static Dictionary<string, SpriteFrame> frames = new Dictionary<string, SpriteFrame>();

	public static void Init()
	{
	}

	public static void AddSpriteSet(string name, SpriteSet sprset)
	{
		if (spritesets.ContainsKey(name))
		{
			spritesets[name] = sprset;
		}
		else
		{
			spritesets.Add(name, sprset);
		}
		sprset.setname = name;
	}

	public static SpriteSet CreateSpriteSet(string name, Texture2D tex)
	{
		SpriteSet spriteSet = new SpriteSet(tex);
		AddSpriteSet(name, spriteSet);
		return spriteSet;
	}

	public static SpriteSet GetSpriteSet(string name)
	{
		return spritesets[name];
	}

	public static SpriteAnimation AutoGenAnimation(Vector2 size, Rect framerect)
	{
		SpriteAnimation spriteAnimation = new SpriteAnimation();
		for (float num = framerect.y; num < framerect.yMax; num += 1f)
		{
			for (float num2 = framerect.x; num2 < framerect.xMax; num2 += 1f)
			{
				spriteAnimation.AddFrame(new Rect(num2 * size.x, num * size.y, size.x, size.y));
			}
		}
		return spriteAnimation;
	}
}
