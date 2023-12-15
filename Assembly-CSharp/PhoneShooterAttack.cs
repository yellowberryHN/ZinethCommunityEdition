using UnityEngine;

public class PhoneShooterAttack : PhoneShooterBullet
{
	public SpritePlayer spriteplayer = new SpritePlayer();

	private void Awake()
	{
		Init();
	}

	public override void OnUpdate()
	{
		lifetime -= PhoneElement.deltatime;
		if (lifetime <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
		base.transform.position += velocity * PhoneElement.deltatime;
		spriteplayer.UpdateMat(base.renderer.material);
	}

	private void OnTriggerEnter(Collider other)
	{
		PhoneShooterBullet component = other.gameObject.GetComponent<PhoneShooterBullet>();
		if ((bool)component && component.owner != owner)
		{
			component.owner.monster.attackStat.Grow(component.damage / 20f);
			Object.Destroy(component.gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.name.StartsWith("food_"))
		{
			PhoneShooterPickup component = other.gameObject.GetComponent<PhoneShooterPickup>();
			if ((bool)component && knockback != 0f)
			{
				float num = knockback / (0.5f + component.givehealth / 3f);
				component.transform.position += velocity.normalized * num * PhoneElement.deltatime;
				component.allow_magnet = false;
			}
		}
	}
}
