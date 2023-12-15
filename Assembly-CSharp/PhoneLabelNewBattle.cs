using UnityEngine;

public class PhoneLabelNewBattle : PhoneLabel
{
	private float counter;

	private bool waschecked;

	private string old_text = "ddd";

	public override void OnLoad()
	{
		base.OnLoad();
		if (overrideColor)
		{
			textmesh.renderer.material.color = color;
		}
		else
		{
			textmesh.renderer.material.color = PhoneMemory.settings.selectedTextColor;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		if (overrideColor)
		{
			textmesh.renderer.material.color = color;
		}
		else
		{
			textmesh.renderer.material.color = PhoneMemory.settings.selectedTextColor;
		}
	}

	private void Update()
	{
		SetText();
	}

	protected virtual string GetText()
	{
		if (PhoneMemory.trainer_challenge == null)
		{
			return string.Empty;
		}
		return "VS";
	}

	protected void SetText()
	{
		counter += Time.deltaTime;
		if (counter % 0.5f > 0.35f)
		{
			base.renderer.enabled = false;
			waschecked = false;
		}
		else
		{
			if (waschecked)
			{
				return;
			}
			waschecked = true;
			string text = GetText();
			if (text == string.Empty)
			{
				base.renderer.enabled = false;
			}
			else
			{
				base.renderer.enabled = true;
				if (old_text != text)
				{
					SetText(text);
				}
			}
			old_text = text;
		}
	}
}
