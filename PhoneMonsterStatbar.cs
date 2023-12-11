using UnityEngine;

public class PhoneMonsterStatbar : PhoneButton
{
	public Transform currentbar;

	public Transform potentialbar;

	public Transform lockedbar;

	public PhoneLabel numberlabel;

	public PhoneLabel pricelabel;

	public GUIText numbergui;

	private MonsterStat _stat;

	private bool showLockedBar = true;

	private Renderer lockedRend;

	public float scalefactor = 1f;

	public MonsterStat stat
	{
		get
		{
			return _stat;
		}
		set
		{
			_stat = value;
			UpdateStat();
		}
	}

	public float unlockAmount
	{
		get
		{
			return 2f;
		}
	}

	public float unlockPrice
	{
		get
		{
			return Mathf.Ceil((stat.current + stat.potential) / 10f);
		}
	}

	private void Start()
	{
		currentbar.GetChild(0).renderer.material.color = Color.blue;
		potentialbar.GetChild(0).renderer.material.color = Color.Lerp(Color.blue, Color.white, 0.75f);
		lockedRend = lockedbar.GetChild(0).renderer;
		lockedRend.material.color = Color.red;
		showLockedBar = false;
	}

	public override void OnLoad()
	{
		base.OnLoad();
		numberlabel.overrideColor = true;
		numberlabel.color = PhoneMemory.settings.selectableTextColor;
		numberlabel.textmesh.renderer.material.color = PhoneMemory.settings.selectableTextColor;
	}

	public override void Init()
	{
		textmesh = numberlabel.textmesh;
		base.Init();
	}

	public override bool ContainsPoint(Vector3 pos)
	{
		return base.ContainsPoint(pos);
	}

	private void Update()
	{
		if (stat != null && lockedbar.gameObject.active)
		{
			string format = "{0}/{1}";
			float current = stat.current;
			float num = stat.current + stat.potential;
			if (showLockedBar)
			{
				lockedRend.enabled = Mathf.Repeat(Time.time * 3f, 1f) >= 0.25f;
				num += Mathf.Min(unlockAmount, stat.locked);
				numberlabel.textmesh.renderer.material.color = Color.red;
			}
			else
			{
				lockedRend.enabled = false;
			}
			format = string.Format(format, current.ToString("0.0"), num.ToString("0.0"));
			if (numberlabel.text != format)
			{
				numberlabel.text = format;
			}
			if ((bool)numbergui)
			{
				numbergui.text = format;
			}
		}
	}

	private void UpdateStat()
	{
		if (stat == null)
		{
			ScaleBar(currentbar, 0f);
			ScaleBar(potentialbar, 0f);
			ScaleBar(lockedbar, 0f);
			PositionLabel(numberlabel);
			numberlabel.textmesh.text = "?/?";
			return;
		}
		ScaleBar(currentbar, stat.current);
		ScaleBar(potentialbar, stat.current + stat.potential);
		DoLockedBar();
		PositionLabel(numberlabel);
		DoPriceLabels();
		string text = stat.current.ToString("0.0") + "/" + (stat.current + stat.potential).ToString("0.0");
		if (!(stat.locked > 0f) || PhoneMemory.capsule_points > 0f)
		{
		}
		numberlabel.textmesh.text = text;
		if ((bool)numbergui)
		{
			numbergui.text = text;
		}
	}

	public void DoPriceLabels()
	{
		if ((bool)pricelabel)
		{
			pricelabel.overrideColor = true;
			if (stat.locked > 0f)
			{
				pricelabel.text = "$" + unlockPrice.ToString("0");
			}
			else
			{
				pricelabel.text = string.Empty;
			}
			if (PhoneMemory.capsule_points >= unlockPrice)
			{
				pricelabel.color = Color.black;
			}
			else
			{
				pricelabel.color = Color.red;
			}
			pricelabel.OnLoad();
		}
	}

	public void DoLockedBar()
	{
		if (selected)
		{
			float val = stat.current + stat.potential + Mathf.Min(stat.locked, unlockAmount);
			ScaleBar(lockedbar, val);
			showLockedBar = true;
		}
		else
		{
			showLockedBar = false;
		}
	}

	public override void OnSelected()
	{
		base.OnSelected();
		DoLockedBar();
	}

	public override void OnUnSelected()
	{
		base.OnUnSelected();
		showLockedBar = false;
	}

	public override void DoPressedParticles()
	{
	}

	public virtual void DoRealPressedParticles()
	{
		PhoneController.EmitParts(GetPressPos(), 10, Color.red);
	}

	public override Vector3 GetPressPos()
	{
		Vector3 max = potentialbar.GetChild(0).renderer.bounds.max;
		max.y += 0.1f;
		max.z = potentialbar.GetChild(0).renderer.bounds.center.z;
		return max;
	}

	public bool UnlockStat()
	{
		if (stat.locked <= 0f)
		{
			return false;
		}
		if (PhoneMemory.capsule_points < unlockPrice)
		{
			return false;
		}
		PhoneMemory.AddCapsulePoints(0f - unlockPrice);
		stat.Unlock(unlockAmount);
		DoRealPressedParticles();
		PhoneMemory.SaveMonsters();
		UpdateStat();
		Playtomic.Log.CustomMetric("tUnlockedStat", "tPhone", true);
		Playtomic.Log.CustomMetric("tStatsUnlocked", "tPhone", false);
		return true;
	}

	public override bool RunCommand(string stringcommand)
	{
		if (stringcommand == ".unlockstat")
		{
			return UnlockStat();
		}
		return base.RunCommand(stringcommand);
	}

	private void ScaleBar(Transform bar, float val)
	{
		Vector3 localScale = new Vector3(scalefactor * val, 1f, bar.transform.localScale.z);
		bar.localScale = localScale;
	}

	private void PositionLabel(PhoneLabel label)
	{
		Vector3 position = potentialbar.transform.position;
		position.x = potentialbar.GetChild(0).renderer.bounds.max.x + 0.1f;
		position += Vector3.up * 0.5f;
		float num = label.textmesh.renderer.bounds.max.x - label.transform.position.x;
		if (position.x + num >= background_box.renderer.bounds.max.x - 0.15f)
		{
			position.x = background_box.renderer.bounds.max.x - 0.05f - num;
		}
		if (label.animateOnLoad)
		{
			label.wantedpos = position;
		}
		else
		{
			label.transform.position = position;
		}
	}
}
