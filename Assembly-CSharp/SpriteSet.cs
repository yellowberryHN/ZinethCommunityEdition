using System.Collections.Generic;
using UnityEngine;

public class SpriteSet
{
	public string setname;

	public Texture2D texture;

	public Dictionary<string, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>();

	public SpriteSet(Texture2D tex)
	{
		texture = tex;
	}

	public bool AddAnimation(string name, SpriteAnimation animation)
	{
		animations.Add(name, animation);
		return true;
	}

	public bool AddAnimation(string name, Rect[] rects)
	{
		animations.Add(name, new SpriteAnimation(rects));
		return true;
	}

	public SpriteAnimation GetAnimation(string name)
	{
		return animations[name];
	}
}
