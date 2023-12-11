using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation
{
	public List<SpriteFrame> frames = new List<SpriteFrame>();

	public float animationSpeed = 6f;

	public SpriteAnimation()
	{
	}

	public SpriteAnimation(Rect[] rects)
	{
		foreach (Rect rectangle in rects)
		{
			frames.Add(new SpriteFrame(rectangle));
		}
	}

	public void AddFrame(Rect rect)
	{
		frames.Add(new SpriteFrame(rect));
	}

	public void AddFrame(SpriteFrame frame)
	{
		frames.Add(frame);
	}

	public SpriteFrame GetFrame(int ind)
	{
		return frames[ind % frames.Count];
	}
}
