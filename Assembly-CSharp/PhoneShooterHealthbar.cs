using UnityEngine;

public class PhoneShooterHealthbar : MonoBehaviour
{
	public PhoneShooterMonster monster;

	private Vector3 backscale;

	public Transform backbar;

	private float old_health = float.NegativeInfinity;

	private float back_timer;

	private void Start()
	{
		if ((bool)backbar)
		{
			backbar.renderer.material.color = Color.white;
			backscale = backbar.localScale;
		}
		backscale = transform.localScale;
	}

	public void OnUpdate()
	{
		if (old_health == float.NegativeInfinity)
		{
			old_health = monster.health;
		}
		else if (old_health != monster.health)
		{
			if (back_timer <= 0f)
			{
				backscale.x = transform.localScale.x;
			}
			back_timer = 2f;
			old_health = monster.health;
		}
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Max(0f, monster.health / monster.maxhealth);
		if (transform.localScale != localScale)
		{
			transform.localScale = localScale;
		}
		if ((bool)backbar)
		{
			Vector3 localScale2 = backbar.localScale;
			localScale2.x = localScale.x;
			localScale2.z *= 0.5f;
			if (localScale2 != backbar.localScale)
			{
				backbar.localScale = Vector3.Lerp(localScale2, backscale, Mathf.Min(back_timer, 1f));
			}
		}
		back_timer = Mathf.Max(back_timer - Time.deltaTime * 5f, 0f);
	}
}
