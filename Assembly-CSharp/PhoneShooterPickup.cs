using UnityEngine;

public class PhoneShooterPickup : PhoneElement
{
	public Color color = Color.green;

	public float givehealth = 2f;

	public float size = 1f;

	public bool _inited;

	public Texture2D[] sprites;

	public int sprite_index;

	public bool allow_magnet;

	private float magnet_timer = 1f;

	private static bool weight_gain_enabled;

	private void Awake()
	{
		if (sprites.Length > 0)
		{
			ChooseSprite();
		}
	}

	private void Start()
	{
		if (!_inited)
		{
			ChooseRandom();
			Resize();
		}
		Display();
	}

	public override void OnUpdate()
	{
		if (magnet_timer > 0f)
		{
			magnet_timer -= PhoneElement.deltatime;
			if (magnet_timer <= 0f)
			{
				allow_magnet = true;
			}
		}
	}

	public virtual void Resize(float newamount)
	{
		givehealth = newamount;
		Resize();
	}

	public virtual void Resize(int low, int high)
	{
		givehealth = Random.Range(low, high);
		Resize();
	}

	public virtual void Resize()
	{
		_inited = true;
		Vector3 normalized = base.transform.localScale.normalized;
		size = 2.5f + givehealth / 12f;
		base.transform.localScale = normalized * size;
	}

	public virtual void ChooseSprite()
	{
		sprite_index = Random.Range(0, sprites.Length);
		base.renderer.material.mainTexture = sprites[sprite_index];
		base.name = "food_" + sprites[sprite_index].name;
	}

	public virtual void ChooseRandom()
	{
		givehealth = Random.Range(1, 4);
	}

	public virtual void Randomize()
	{
		givehealth = Random.Range(1f, 4f);
	}

	public virtual void OnUsed(PhoneShooterMonster monster)
	{
		float num = givehealth;
		if ("food_" + monster.monster.bloodtype == base.name)
		{
			num *= 1.5f;
			string stext = "Tasty!";
			monster.ShowText(base.transform.position + Vector3.up * 4f + Vector3.forward * 0.25f, stext, 0.5f, Color.yellow, true);
		}
		string stext2 = "+" + num.ToString("0.0");
		monster.ShowText(base.transform.position + Vector3.up * 4f, stext2, 0.25f, color, true);
		if (weight_gain_enabled)
		{
			Vector2 scale = monster.monster.scale;
			scale.x += num / (scale.x * scale.x) / 500f;
			monster.monster.scale = scale;
			monster.SetScaling(monster.monster.scale);
		}
		monster.Heal(num);
		PhoneAudioController.PlayAudioClip("health_up", SoundType.game);
		Object.Destroy(base.gameObject);
	}

	public virtual void Display()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == base.name)
		{
			Vector3 position = (base.transform.position + other.transform.position) / 2f;
			PhoneShooterPickup component = other.gameObject.GetComponent<PhoneShooterPickup>();
			if (givehealth >= component.givehealth)
			{
				givehealth += component.givehealth * 1.1f;
				Resize();
				Object.Destroy(component.gameObject);
				base.transform.position = position;
			}
			else
			{
				component.givehealth += givehealth * 1.1f;
				component.Resize();
				component.transform.position = position;
				Object.Destroy(base.gameObject);
			}
		}
	}
}
