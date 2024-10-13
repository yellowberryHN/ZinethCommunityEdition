using UnityEngine;

public class PhoneShooterMonster : PhoneElement
{
	public PhoneShooterController controller;

	public SpritePlayer sprite_player = new SpritePlayer();

	public bool isplayer;

	public float health = 2f;

	public float speed = 2f;

	public float shoot_cooldown = 1f / 3f;

	protected float shoot_timer;

	public float attack_timer;

	public Transform sprite_obj;

	public PhoneShooterPickup deathdrop_prefab;

	public PhoneShooterHealthbar healthbar;

	public Color color = Color.red;

	public PhoneShooterBullet bulletprefab;

	public PhoneMonster monster;

	public Bounds bounds;

	private Vector3 startscale = Vector3.zero;

	private float damage_timer;

	public Transform target_trans;

	protected float realspeed
	{
		get
		{
			return speed * (monster.speed / monster.scale.x);
		}
	}

	public virtual float bullet_homing
	{
		get
		{
			return (Mathf.Sqrt(1f + glam * 1.5f) - 1f) * monster.bullet_homing;
		}
	}

	public virtual float bullet_speed
	{
		get
		{
			return monster.bullet_speed * 2f;
		}
	}

	public virtual float maxhealth
	{
		get
		{
			return monster.defense * 4f * monster.scale.x;
		}
	}

	public float attack
	{
		get
		{
			return monster.attack;
		}
	}

	public float magic
	{
		get
		{
			return monster.magic;
		}
	}

	public float glam
	{
		get
		{
			return monster.glam;
		}
	}

	public Color sprite_color
	{
		get
		{
			return sprite_obj.renderer.material.color;
		}
		set
		{
			sprite_obj.renderer.material.color = value;
		}
	}

	public float exp_value
	{
		get
		{
			return maxhealth;
		}
	}

	public override void OnLoad()
	{
	}

	private void Start()
	{
		bounds = transform.parent.collider.bounds;
	}

	private void Awake()
	{
		Init();
	}

	public override void Init()
	{
		if (healthbar == null)
		{
			healthbar = GetComponentInChildren<PhoneShooterHealthbar>();
		}
		healthbar.monster = this;
		healthbar.renderer.material.color = color;
	}

	public virtual void SetMonster(PhoneMonster monstero)
	{
		monster = monstero;
		health = maxhealth;
		SetSpriteSet(monster.spriteset);
		SetScaling(monster.scale);
		DoAnimation();
	}

	public virtual void SetScaling(Vector2 scale)
	{
		if (startscale == Vector3.zero)
		{
			startscale = transform.localScale;
		}
		Vector3 localScale = startscale;
		localScale.x *= scale.x;
		localScale.z *= scale.y;
		transform.localScale = localScale;
	}

	public virtual void SetSpriteSet(SpriteSet spriteset)
	{
		sprite_player.SetSpriteSet(spriteset);
	}

	public virtual void SetImage(Texture2D image)
	{
		sprite_obj.renderer.material.mainTexture = image;
	}

	public override void OnUpdate()
	{
		healthbar.OnUpdate();
		DoAnimation();
	}

	public virtual void DoAnimation()
	{
		sprite_player.UpdateMat(sprite_obj.renderer.material);
		DoColor();
	}

	public virtual void DoColor()
	{
		if (sprite_color.a < 1f)
		{
			Color color = sprite_color;
			color.a = Mathf.Lerp(1f, 0f, Mathf.Min(damage_timer, 1f));
			sprite_color = color;
		}
		damage_timer = Mathf.Max(0f, damage_timer - Time.deltaTime * 15f);
	}

	public virtual void LimitPos()
	{
		LimitPos(bounds);
	}

	public virtual void LimitPos(Bounds boundslimit)
	{
		base.transform.position = LimitPos(base.transform.position, boundslimit);
	}

	public virtual Vector3 LimitPos(Vector3 pos, Bounds boundslimit)
	{
		for (int i = 0; i < 3; i += 2)
		{
			pos[i] = Mathf.Max(boundslimit.min[i], Mathf.Min(pos[i], boundslimit.max[i]));
		}
		return pos;
	}

	public virtual void Heal(float amount)
	{
		health = Mathf.Min(health + amount, maxhealth);
	}

	public virtual PhoneShooterBullet Shoot(Vector3 direction)
	{
		return Shoot(direction, magic / 2f);
	}

	public virtual PhoneShooterBullet Shoot(Vector3 direction, float damage)
	{
		PhoneShooterBullet phoneShooterBullet = Instantiate(bulletprefab, transform.position, Quaternion.identity) as PhoneShooterBullet;
		phoneShooterBullet.owner = this;
		phoneShooterBullet.velocity = direction * bullet_speed;
		phoneShooterBullet.damage = damage;
		phoneShooterBullet.homing = bullet_homing;
		phoneShooterBullet.renderer.material.color = color;
		phoneShooterBullet.transform.parent = transform.parent;
		if ((bool)target_trans)
		{
			phoneShooterBullet.target = target_trans;
		}
		return phoneShooterBullet;
	}

	public virtual void ResetShootTimer()
	{
		shoot_timer = (shoot_cooldown - 0.95f * shoot_cooldown * (magic / 750f)) / monster.bullet_cooldown;
	}

	public virtual void Damage(PhoneShooterBullet bullet)
	{
		float damage = bullet.damage;
		health -= damage;
		sprite_color = new Color(1f, 1f, 1f, 0f);
		damage_timer = 2f;
		if (health <= 0f)
		{
			OnDeath(bullet);
		}
		else
		{
			monster.defenseStat.Grow(damage / 160f);
		}
		PhoneAudioController.PlayAudioClip("hit", SoundType.game);
	}

	public virtual PhoneLabel ShowText(Vector3 vec, string stext, float time, Color tcolor, bool outline)
	{
		PhoneLabel phoneLabel = Instantiate(PhoneShooterController.slabel_prefab) as PhoneLabel;
		phoneLabel.transform.position = vec;
		phoneLabel.transform.parent = base.transform.parent;
		phoneLabel.textmesh.text = stext;
		phoneLabel.textmesh.characterSize = 1f;
		phoneLabel.overrideColor = true;
		phoneLabel.color = tcolor;
		phoneLabel.velocity = Vector3.forward;
		Destroy(phoneLabel.gameObject, time);
		if (outline)
		{
			Vector3 vec2 = vec + new Vector3(0.02f, -0.02f, -0.02f);
			ShowText(vec2, stext, time, Color.black, false);
		}
		return phoneLabel;
	}

	public virtual void OnDeath(PhoneShooterBullet bullet)
	{
		if (bullet.GetType() == typeof(PhoneShooterAttack))
		{
			bullet.owner.monster.attackStat.Grow(exp_value / 400f);
		}
		else
		{
			bullet.owner.monster.magicStat.Grow(exp_value / 500f);
		}
		OnDeath();
	}

	public virtual void OnDeath()
	{
		PhoneController.EmitParts(transform.position, (int)(2f + Mathf.Ceil(monster.level / 4f)));
		PhoneEffects.AddCamShake(monster.level / 20f);
		PhoneAudioController.PlayAudioClip("enemy_die", SoundType.game);
		if ((bool)deathdrop_prefab && !controller.battle_mode)
		{
			PhoneShooterPickup phoneShooterPickup = Instantiate(deathdrop_prefab) as PhoneShooterPickup;
			phoneShooterPickup.transform.position = transform.position + Vector3.down * 0.5f;
			phoneShooterPickup.transform.parent = transform.parent;
			phoneShooterPickup.Resize(1, (int)Mathf.Max(Mathf.Pow(monster.level, 1.25f), 3f));
		}
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		HandleCollision(other);
	}

	private void OnTriggerStay(Collider other)
	{
		PhoneShooterBullet component = other.gameObject.GetComponent<PhoneShooterBullet>();
		if (component != null)
		{
			Collide_Bullet(component);
		}
	}

	public virtual void HandleCollision(Collider other)
	{
		PhoneShooterPickup component = other.gameObject.GetComponent<PhoneShooterPickup>();
		if (component != null)
		{
			Collide_Pickup(component);
			return;
		}
		PhoneShooterEnemy component2 = other.gameObject.GetComponent<PhoneShooterEnemy>();
		if (component2 != null)
		{
			Collide_Enemy(component2);
		}
	}

	public virtual void Collide_Bullet(PhoneShooterBullet obullet)
	{
		if (obullet.owner.isplayer != isplayer)
		{
			obullet.OnHit(this);
		}
	}

	public virtual void Collide_Pickup(PhoneShooterPickup pickup)
	{
	}

	public virtual void Collide_Enemy(PhoneShooterEnemy enemy)
	{
	}
}
