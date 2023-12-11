using UnityEngine;

public class PhoneShooterPlayer : PhoneShooterMonster
{
	public Vector3 curdir = Vector3.forward;

	public PhoneShooterAttack attackprefab;

	public bool autoshoot = true;

	public override float bullet_homing
	{
		get
		{
			return monster.bullet_homing + (Mathf.Sqrt(1f + base.glam) - 1f);
		}
	}

	public override float maxhealth
	{
		get
		{
			return 12f + monster.defense * 4f;
		}
	}

	private float pullspeed
	{
		get
		{
			return 2f + monster.glam / 5f;
		}
	}

	private void Start()
	{
		bounds = base.transform.parent.collider.bounds;
	}

	public override void OnUpdate()
	{
		if (health <= 0f)
		{
			OnDeath();
			return;
		}
		DoMovement();
		PullExpTowards();
		DoAttacking();
		DoShooting();
		DoAnimation();
		healthbar.OnUpdate();
	}

	private void DoMovement()
	{
		Vector3 vector = Vector3.zero;
		base.rigidbody.drag = 30f;
		if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
		{
			if (PhoneInput.GetTouchPoint() != Vector3.one * -1f)
			{
				Vector3 transformedTouchPoint = PhoneInput.GetTransformedTouchPoint();
				transformedTouchPoint.y = base.transform.position.y;
				transformedTouchPoint = (transformedTouchPoint - base.transform.position) * base.realspeed;
				if (transformedTouchPoint.magnitude > base.realspeed)
				{
					transformedTouchPoint = transformedTouchPoint.normalized * base.realspeed;
				}
				base.transform.position += transformedTouchPoint * PhoneElement.deltatime;
				vector = transformedTouchPoint;
				curdir = transformedTouchPoint;
			}
		}
		else
		{
			Vector2 controlDir = PhoneInput.GetControlDir();
			if (controlDir.magnitude > 0.1f)
			{
				if (controlDir.magnitude > 1f)
				{
					controlDir.Normalize();
				}
				controlDir *= base.realspeed;
				curdir = new Vector3(controlDir.x, 0f, controlDir.y);
				vector = curdir;
				base.transform.position += curdir * PhoneElement.deltatime;
			}
		}
		if (attack_timer > 0f)
		{
			sprite_player.animation_name = "attack";
			sprite_player.play_speed = 1f;
		}
		else
		{
			if (sprite_player.animation_name != "walk")
			{
				sprite_player.PlayAnimation("walk");
			}
			if (monster.flying_animate)
			{
				sprite_player.play_speed = 1f + vector.magnitude * 0.25f;
			}
			else
			{
				sprite_player.play_speed = vector.magnitude;
			}
			if (vector.magnitude == 0f && !monster.flying_animate)
			{
				sprite_player.time = 0f;
			}
		}
		LimitPos();
	}

	private void DoAttacking()
	{
		attack_timer = Mathf.Max(0f, attack_timer - PhoneElement.deltatime);
		if (attack_timer <= 0f && PhoneInput.IsPressedDown())
		{
			Vector2 dir = new Vector2(curdir.x, curdir.z);
			Attack(dir);
			attack_timer = 0.5f;
		}
	}

	private void Attack(Vector2 dir)
	{
		float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		int num2 = Mathf.RoundToInt(num / 45f);
		if (num2 >= 8)
		{
			num2 = 0;
		}
		PhoneShooterAttack phoneShooterAttack = Object.Instantiate(attackprefab) as PhoneShooterAttack;
		phoneShooterAttack.transform.position = base.transform.position + new Vector3(dir.normalized.x, 0f, dir.normalized.y) * 0.5f;
		Vector3 localScale = phoneShooterAttack.transform.localScale;
		localScale.x *= monster.scale.x;
		localScale.z *= monster.scale.y;
		phoneShooterAttack.transform.localScale = localScale;
		phoneShooterAttack.owner = this;
		phoneShooterAttack.velocity = new Vector3(dir.normalized.x, 0f, dir.normalized.y) * 2f;
		phoneShooterAttack.damage = base.attack * 1f;
		phoneShooterAttack.renderer.material.color = Color.white;
		phoneShooterAttack.transform.parent = base.transform.parent;
		phoneShooterAttack.spriteplayer.SetSpriteSet(monster.spriteset);
		phoneShooterAttack.spriteplayer.PlayAnimation("bullet", 0f);
		phoneShooterAttack.spriteplayer.SetFrame(phoneShooterAttack.renderer.material, num2);
		sprite_player.PlayAnimation("attack");
		PhoneAudioController.PlayAudioClip("attack", SoundType.game);
	}

	private void DoShooting()
	{
		shoot_timer = Mathf.Max(0f, shoot_timer - PhoneElement.deltatime);
		ShootAtNearest();
	}

	private void ShootAtNearest()
	{
		if ((!autoshoot && !PhoneInput.IsPressed()) || !(shoot_timer <= 0f) || !(attack_timer <= 0f))
		{
			return;
		}
		PhoneShooterEnemy phoneShooterEnemy = null;
		float num = float.PositiveInfinity;
		PhoneShooterEnemy[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<PhoneShooterEnemy>();
		PhoneShooterEnemy[] array = componentsInChildren;
		foreach (PhoneShooterEnemy phoneShooterEnemy2 in array)
		{
			float num2 = Vector3.Distance(base.transform.position, phoneShooterEnemy2.transform.position);
			if (num2 < num)
			{
				num = num2;
				phoneShooterEnemy = phoneShooterEnemy2;
			}
		}
		if (phoneShooterEnemy != null)
		{
			target_trans = phoneShooterEnemy.transform;
			Vector3 normalized = (phoneShooterEnemy.transform.position - base.transform.position).normalized;
			Shoot(normalized);
			ResetShootTimer();
		}
		else
		{
			target_trans = null;
		}
	}

	private void ShootAtMoveDir()
	{
		if ((autoshoot || PhoneInput.IsPressed()) && shoot_timer <= 0f)
		{
			Vector3 normalized = curdir.normalized;
			Shoot(normalized);
			shoot_timer = shoot_cooldown / monster.bullet_cooldown;
		}
	}

	private float GetPullDistance()
	{
		return 0.5f + monster.glam / 5f;
	}

	private void PullExpTowards()
	{
		Debug.DrawLine(base.transform.position, base.transform.position + Vector3.right * GetPullDistance(), Color.red);
		PhoneShooterPickup[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<PhoneShooterPickup>();
		PhoneShooterPickup[] array = componentsInChildren;
		foreach (PhoneShooterPickup phoneShooterPickup in array)
		{
			Vector3 position = phoneShooterPickup.transform.position;
			position.y = base.transform.position.y;
			float num = Vector3.Distance(base.transform.position, position);
			if (num < 0.2f)
			{
				Collide_Pickup(phoneShooterPickup);
			}
			if (num < GetPullDistance() && phoneShooterPickup.allow_magnet)
			{
				float num2 = pullspeed * (1f - num / GetPullDistance());
				monster.glamStat.Grow(num2 / 200f * PhoneElement.deltatime);
				num2 /= phoneShooterPickup.givehealth / 1f;
				Vector3 normalized = (base.transform.position - phoneShooterPickup.transform.position).normalized;
				normalized *= num2 * PhoneElement.deltatime;
				phoneShooterPickup.transform.position += normalized;
			}
		}
	}

	public override void OnDeath()
	{
		PhoneController.EmitParts(base.transform.position, (int)(maxhealth * 0.75f));
		base.OnDeath();
	}

	private void OnTriggerEnter(Collider other)
	{
		HandleCollision(other);
	}

	public override void Collide_Enemy(PhoneShooterEnemy enemy)
	{
		enemy.health -= base.attack;
		health -= enemy.attack;
		PhoneController.EmitParts(enemy.transform.position, 10);
	}

	public override void Collide_Pickup(PhoneShooterPickup pickup)
	{
		pickup.OnUsed(this);
	}
}
