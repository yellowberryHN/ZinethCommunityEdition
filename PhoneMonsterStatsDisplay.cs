using UnityEngine;

public class PhoneMonsterStatsDisplay : MonoBehaviour
{
	public PhoneMonsterStatbar attackbar;

	public PhoneMonsterStatbar defensebar;

	public PhoneMonsterStatbar magicbar;

	public PhoneMonsterStatbar glambar;

	public PhoneLabel namelabel;

	public PhoneLabel levellabel;

	public PhoneLabel bloodlabel;

	public PhoneElement spritedisplay;

	public GUIText namegui;

	public GUIText levelgui;

	public GUIText bloodgui;

	private PhoneMonster curmonster;

	public SpritePlayer spriteplayer = new SpritePlayer();

	public float scalefactor = 0.5f;

	private Vector3 sprite_normalscale = Vector3.zero;

	public PhoneMonsterStatbar[] bars
	{
		get
		{
			return new PhoneMonsterStatbar[4] { attackbar, defensebar, magicbar, glambar };
		}
	}

	private void Start()
	{
	}

	public void OnUpdate()
	{
		if ((bool)spritedisplay)
		{
			spriteplayer.UpdateMat(spritedisplay.renderer.material);
		}
	}

	public void MoveBarsRelative(Vector3 pos)
	{
		PhoneMonsterStatbar[] array = bars;
		foreach (PhoneMonsterStatbar phoneMonsterStatbar in array)
		{
			if (phoneMonsterStatbar.animateOnLoad)
			{
				phoneMonsterStatbar.transform.position += pos;
			}
		}
		if ((bool)spritedisplay && spritedisplay.animateOnLoad)
		{
			spritedisplay.transform.position += pos;
		}
	}

	public void SetMonster(PhoneMonster monster)
	{
		curmonster = monster;
		if ((bool)namelabel)
		{
			namelabel.text = monster.name;
		}
		else if ((bool)namegui)
		{
			namegui.text = monster.name;
		}
		if ((bool)levellabel)
		{
			levellabel.text = "Tier " + monster.level;
		}
		else if ((bool)levelgui)
		{
			levelgui.text = "Tier " + monster.level;
		}
		if ((bool)bloodlabel)
		{
			bloodlabel.text = "Blood: " + monster.bloodtype;
		}
		else if ((bool)bloodgui)
		{
			bloodgui.text = "Blood: " + monster.bloodtype;
		}
		if ((bool)spritedisplay)
		{
			if (sprite_normalscale == Vector3.zero)
			{
				sprite_normalscale = spritedisplay.transform.localScale;
			}
			Vector3 vector = sprite_normalscale;
			vector.x *= monster.scale.x;
			vector.z *= monster.scale.y;
			if (spritedisplay.transform.localScale != vector)
			{
				spritedisplay.transform.localScale = vector;
			}
			spriteplayer.SetSpriteSet(monster.spriteset);
			spriteplayer.Play();
		}
		SetScales(scalefactor);
		UpdateStats();
	}

	public void UpdateStats()
	{
		attackbar.stat = curmonster.attackStat;
		defensebar.stat = curmonster.defenseStat;
		magicbar.stat = curmonster.magicStat;
		glambar.stat = curmonster.glamStat;
	}

	public void SetScales()
	{
		SetScales(scalefactor);
	}

	public void SetScales(float amount)
	{
		attackbar.scalefactor = scalefactor;
		defensebar.scalefactor = scalefactor;
		magicbar.scalefactor = scalefactor;
		glambar.scalefactor = scalefactor;
	}
}
