using UnityEngine;

public class PhoneShooterExp : PhoneElement
{
	public Vector4 exp_vals = Vector4.zero;

	public Color color;

	public float attack_exp
	{
		get
		{
			return exp_vals[0];
		}
		set
		{
			exp_vals[0] = value;
		}
	}

	public float defense_exp
	{
		get
		{
			return exp_vals[1];
		}
		set
		{
			exp_vals[1] = value;
		}
	}

	public float magic_exp
	{
		get
		{
			return exp_vals[2];
		}
		set
		{
			exp_vals[2] = value;
		}
	}

	public float glam_exp
	{
		get
		{
			return exp_vals[3];
		}
		set
		{
			exp_vals[3] = value;
		}
	}

	private void Start()
	{
		ChooseRandom();
	}

	public override void OnUpdate()
	{
		Display();
	}

	public virtual void Resize()
	{
		Vector3 normalized = base.transform.localScale.normalized;
		normalized *= exp_vals.magnitude / 5f;
	}

	public virtual void ChooseRandom()
	{
		exp_vals = Vector4.zero;
		int index = Random.Range(0, 4);
		exp_vals[index] = Random.Range(0.5f, 1f);
	}

	public virtual void Randomize()
	{
		for (int i = 0; i < 4; i++)
		{
			exp_vals[i] = Random.Range(0.25f, 0.75f);
		}
	}

	public virtual void OnUsed(PhoneShooterMonster monster)
	{
		string text = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			text = text + "+" + exp_vals[i].ToString(".0") + "\n";
			monster.monster.stats[i].Grow(exp_vals[i]);
		}
		monster.ShowText(base.transform.position + Vector3.up * 4f, text, 0.25f, color, true);
		Object.Destroy(base.gameObject);
	}

	public virtual void Display()
	{
		Vector4 vector = exp_vals;
		vector = exp_vals.normalized;
		color = new Color(vector[0], vector[1], vector[2], 1f - Random.Range(0f, vector[3]));
		base.renderer.material.color = color;
	}
}
