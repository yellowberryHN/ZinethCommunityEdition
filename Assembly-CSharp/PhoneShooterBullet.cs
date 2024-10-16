using System.Collections.Generic;
using UnityEngine;

public class PhoneShooterBullet : PhoneElement
{
	public PhoneShooterMonster owner;

	public float damage = 1f;

	public float knockback;

	public float lifetime = 30f;

	public float homing;

	public Transform target;

	public bool destroyonhit = true;

	public List<PhoneShooterMonster> ignorelist = new List<PhoneShooterMonster>();

	public override void OnLoad()
	{
	}

	private void Awake()
	{
		Init();
	}

	public override void OnUpdate()
	{
		lifetime -= deltatime;
		if (lifetime <= 0f)
		{
			Destroy(gameObject);
		}
		if ((bool)target && homing != 0f)
		{
			DoHoming(target.position);
		}
		base.transform.position += velocity * deltatime;
	}

	protected virtual void DoHoming(Vector3 pos)
	{
		Vector3 vector = pos - base.transform.position;
		vector.y = 0f;
		float magnitude = velocity.magnitude;
		velocity += Mathf.Sign(homing) * vector.normalized * (1f + Mathf.Sqrt(Mathf.Abs(homing)) - 1f) * deltatime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "PhoneGameWall")
		{
			Destroy(gameObject);
		}
	}

	public virtual void OnHit(PhoneShooterMonster monster)
	{
		if (knockback != 0f)
		{
			float num = Mathf.Max(monster.monster.scale.x * monster.monster.scale.y, 1f);
			float num2 = knockback / num;
			monster.transform.position += velocity.normalized * num2 * deltatime;
			monster.LimitPos();
		}
		if (!ignorelist.Contains(monster))
		{
			monster.ShowText(transform.position + Vector3.up * 4f, damage.ToString("0.0"), 0.25f, Color.red, true);
			monster.Damage(this);
			if (destroyonhit)
			{
				Destroy(gameObject);
			}
			else
			{
				ignorelist.Add(monster);
			}
		}
	}
}
