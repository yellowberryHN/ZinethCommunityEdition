using UnityEngine;

internal class VibrateForce
{
	public float life = 0.1f;

	protected float startlife = 0.1f;

	public float lpower = 0.1f;

	public float rpower = 0.1f;

	public bool decay = true;

	public bool is_phone;

	public VibrateForce(float force, float time)
	{
		lpower = force;
		rpower = force;
		life = time;
		startlife = life;
	}

	public VibrateForce(float force, float time, bool decaying)
	{
		lpower = force;
		rpower = force;
		life = time;
		startlife = life;
		decay = decaying;
	}

	public VibrateForce(float left, float right, float time)
	{
		lpower = left;
		rpower = right;
		life = time;
		startlife = life;
	}

	public VibrateForce(float left, float right, float time, bool decaying)
	{
		lpower = left;
		rpower = right;
		life = time;
		startlife = life;
		decay = decaying;
	}

	public virtual Vector2 OnUpdate()
	{
		if (life <= 0f)
		{
			return Vector2.zero;
		}
		float x = lpower;
		float y = rpower;
		if (decay)
		{
			x = Mathf.Lerp(0f, lpower, life / startlife);
			y = Mathf.Lerp(0f, rpower, life / startlife);
		}
		life -= Time.deltaTime;
		return new Vector2(x, y);
	}
}
