using System;
using UnityEngine;

public class PhoneShooterEnemy : PhoneShooterMonster
{
	private float circleSpeed = (float)Math.PI / 8f;

	protected Vector3 storePos = Vector3.zero;

	private PhoneShooterMonster playerobj
	{
		get
		{
			return controller.player_object;
		}
	}

	public MonsterAI aiType
	{
		get
		{
			return monster.monsterType.monsterAI;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		bounds = base.transform.parent.collider.bounds;
	}

	public override void OnUpdate()
	{
		DoAI();
		DoAnimation();
		healthbar.OnUpdate();
	}

	protected virtual void DoAI()
	{
		if ((bool)playerobj)
		{
			target_trans = playerobj.transform;
		}
		DoMove();
		DoShoot();
	}

	protected virtual void DoMove()
	{
		sprite_player.animation_name = "walk";
		if (monster.flying_animate)
		{
			sprite_player.play_speed = 0.2f;
		}
		else
		{
			sprite_player.play_speed = 0f;
		}
		if (aiType == MonsterAI.Goto)
		{
			MoveToPlayer();
		}
		else if (aiType == MonsterAI.Run)
		{
			MoveAwayFromPlayer();
		}
		else if (aiType == MonsterAI.Circle)
		{
			MoveCircle();
		}
		else if (aiType == MonsterAI.CircleCW)
		{
			MoveCircleCW();
		}
		else if (aiType == MonsterAI.Horizontal)
		{
			MoveHorizontal();
		}
		else if (aiType == MonsterAI.Vertical)
		{
			MoveVertical();
		}
		else if (aiType == MonsterAI.Mirror)
		{
			MoveMirror();
		}
		else if (aiType == MonsterAI.Jitter)
		{
			MoveJitter();
		}
		LimitPos();
	}

	public override void OnDeath()
	{
		controller.wave++;
		base.OnDeath();
	}

	protected virtual void MoveToPlayer()
	{
		if ((bool)playerobj)
		{
			MoveTowardsPoint(playerobj.transform.position);
		}
	}

	protected virtual void MoveAwayFromPlayer()
	{
		if ((bool)playerobj)
		{
			Vector3 position = playerobj.transform.position;
			if (Vector3.Distance(position, base.transform.position) <= 2f)
			{
				MoveFromPoint(position);
			}
		}
	}

	protected virtual void MoveHorizontal()
	{
		if ((bool)playerobj)
		{
			Vector3 position = playerobj.transform.position;
			position.z = base.transform.position.z;
			MoveTowardsPoint(position);
		}
	}

	protected virtual void MoveVertical()
	{
		if ((bool)playerobj)
		{
			Vector3 position = playerobj.transform.position;
			position.x = base.transform.position.x;
			MoveTowardsPoint(position);
		}
	}

	protected virtual void MoveMirror()
	{
		if ((bool)playerobj)
		{
			Vector3 position = playerobj.transform.localPosition * -1f;
			Vector3 vector = playerobj.transform.parent.TransformPoint(position);
			vector.y = base.transform.position.y;
			Debug.DrawLine(base.transform.position, vector);
			MoveTowardsPoint(vector);
		}
	}

	protected virtual void MoveCircle()
	{
		MoveCircle(circleSpeed);
	}

	protected virtual void MoveCircleCW()
	{
		MoveCircleCW(circleSpeed);
	}

	protected virtual void MoveCircleCW(float anglespeed)
	{
		MoveCircle(0f - anglespeed);
	}

	protected virtual void MoveCircle(float anglespeed)
	{
		if ((bool)playerobj)
		{
			Vector3 position = playerobj.transform.position;
			Vector3 pos = base.transform.position - position;
			float magnitude = pos.magnitude;
			magnitude = Mathf.Max(magnitude, 1f);
			float num = Mathf.Atan2(pos.z, pos.x);
			num -= anglespeed;
			pos.x = Mathf.Cos(num) * magnitude;
			pos.z = Mathf.Sin(num) * magnitude;
			pos.y = 0f;
			pos += position;
			MoveTowardsPoint(pos);
		}
	}

	protected virtual void MoveJitter()
	{
		MoveJitter(1f);
	}

	protected Vector3 RandomPosition()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			zero[i] = UnityEngine.Random.Range(bounds.min[i], bounds.max[i]);
		}
		return zero;
	}

	protected virtual void MoveJitter(float amount)
	{
		if (Vector3.Distance(base.transform.position, storePos) <= 0.5f || UnityEngine.Random.value < 0.05f * (PhoneElement.deltatime / (1f / 60f)))
		{
			storePos = Vector3.zero;
		}
		if (storePos == Vector3.zero)
		{
			storePos = RandomPosition();
			storePos.y = base.transform.position.y;
		}
		Vector3 vector = storePos - base.transform.position;
		Vector2 vector2 = new Vector2(vector.x, vector.z);
		vector2 = Vector2.ClampMagnitude(vector2, 1f + monster.level / 2f);
		vector2 += UnityEngine.Random.insideUnitCircle * amount;
		MoveByVec(vector2);
	}

	protected virtual void MoveTowardsPoint(Vector3 pos)
	{
		MoveByVec((pos - base.transform.position).normalized);
	}

	protected virtual void MoveFromPoint(Vector3 pos)
	{
		MoveByVec((base.transform.position - pos).normalized);
	}

	protected virtual void MoveByVec(Vector2 vec)
	{
		MoveByVec(new Vector3(vec.x, 0f, vec.y));
	}

	protected virtual void MoveByVec(Vector3 vec)
	{
		MoveByVecRaw(vec * base.realspeed);
	}

	protected virtual void MoveByVecRaw(Vector3 vec)
	{
		base.transform.position += vec * PhoneElement.deltatime;
		if (monster.flying_animate)
		{
			sprite_player.play_speed = 0.5f + vec.magnitude * 0.5f;
		}
		else
		{
			sprite_player.play_speed = vec.magnitude;
		}
	}

	protected virtual void DoShoot()
	{
		shoot_timer = Mathf.Max(0f, shoot_timer - PhoneElement.deltatime);
		if (shoot_timer <= 0f && ShootAtPlayer())
		{
			ResetShootTimer();
		}
	}

	protected virtual bool ShootAtPlayer()
	{
		if (!playerobj)
		{
			return false;
		}
		Vector3 normalized = (playerobj.transform.position - base.transform.position).normalized;
		Shoot(normalized, base.magic / 2f);
		return true;
	}
}
