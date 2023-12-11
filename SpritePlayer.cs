using UnityEngine;

public class SpritePlayer
{
	public SpriteSet sprite_set;

	public string animation_name = "walk";

	public float time;

	public int cur_index = -99;

	public float play_speed;

	public Texture2D cur_texture
	{
		get
		{
			return sprite_set.texture;
		}
	}

	public SpriteAnimation cur_animation
	{
		get
		{
			return sprite_set.GetAnimation(animation_name);
		}
	}

	public SpriteFrame cur_frame
	{
		get
		{
			return cur_animation.GetFrame(cur_index);
		}
	}

	public float cur_speed
	{
		get
		{
			return cur_animation.animationSpeed * play_speed;
		}
	}

	public void SetSpriteSet(string name)
	{
		SetSpriteSet(SpriteController.GetSpriteSet(name));
	}

	public void SetSpriteSet(SpriteSet spriteset)
	{
		sprite_set = spriteset;
		cur_index = -1;
	}

	public void SetAnimation(string animname)
	{
		animation_name = animname;
	}

	public void PlayAnimation(string animname, float speed)
	{
		SetAnimation(animname);
		Play(speed);
	}

	public void PlayAnimation(string animname)
	{
		SetAnimation(animname);
		Play();
	}

	public void Play(float speed)
	{
		play_speed = speed;
		cur_index = -1;
	}

	public void Play()
	{
		play_speed = 1f;
		cur_index = -1;
	}

	public void SetFrame(Material mat, int index)
	{
		mat.mainTexture = sprite_set.texture;
		SetMatFrame(mat, cur_animation.frames[index % cur_animation.frames.Count]);
		cur_index = index;
	}

	public bool UpdateMat(Material mat)
	{
		if (mat.mainTexture != sprite_set.texture)
		{
			mat.mainTexture = sprite_set.texture;
		}
		time += cur_speed * Time.deltaTime;
		if ((int)time != cur_index)
		{
			cur_index = (int)time;
			return SetMatFrame(mat, cur_frame);
		}
		return true;
	}

	public bool SetMatFrame(Material mat, SpriteFrame frame)
	{
		Vector2 offset = default(Vector2);
		offset.x = frame.framerect.x / (float)cur_texture.width;
		offset.y = 1f - frame.framerect.yMax / (float)cur_texture.height;
		Vector2 scale = default(Vector2);
		scale.x = frame.framerect.width / (float)cur_texture.width;
		scale.y = frame.framerect.height / (float)cur_texture.height;
		mat.SetTextureOffset("_MainTex", offset);
		mat.SetTextureScale("_MainTex", scale);
		return true;
	}
}
