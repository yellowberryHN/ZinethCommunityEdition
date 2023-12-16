using UnityEngine;

public class PhoneShooterSpawner : PhoneElement
{
	public PhoneShooterController controller;

	public PhoneShooterMonster prefab;

	public PhoneMonster monster;

	public float timerlength = 2f;

	private float timer = 2f;

	public bool repeat;

	public Color color = Color.black;

	public Vector3 offset = Vector3.zero;

	private void Start()
	{
		Reset();
		SetColor(color);
	}

	public void Reset()
	{
		timer = timerlength;
	}

	public void SetColor(Color col)
	{
		color = col;
		base.renderer.material.color = color;
	}

	public override void OnUpdate()
	{
		timer -= PhoneElement.deltatime;
		if (timer <= 0f)
		{
			Spawn();
			if (repeat)
			{
				Reset();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		Display();
	}

	public virtual PhoneShooterMonster Spawn()
	{
		PhoneShooterMonster phoneShooterMonster = Object.Instantiate(prefab, base.transform.position + offset, Quaternion.identity) as PhoneShooterMonster;
		phoneShooterMonster.transform.parent = base.transform.parent;
		phoneShooterMonster.SetMonster(monster);
		phoneShooterMonster.controller = controller;
		PhoneController.EmitParts(base.transform.position, 6 + (int)monster.level);
		return phoneShooterMonster;
	}

	protected virtual void Display()
	{
		float num = timerlength - Mathf.Max(timer, 0.25f);
		if (timer < 0.25f)
		{
			num += timer * 2f;
		}
		num *= 1.5f;
		num = Mathf.Ceil(num * 8f) / 8f;
		base.transform.localScale = base.transform.localScale.normalized * num;
	}
}