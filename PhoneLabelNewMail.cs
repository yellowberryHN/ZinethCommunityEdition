using UnityEngine;

public class PhoneLabelNewMail : PhoneLabel
{
	private float counter;

	private bool waschecked;

	private int old_num = -1;

	public bool show_text = true;

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

	protected virtual int GetNumber()
	{
		return PhoneMemory.new_mail;
	}

	protected void SetText()
	{
		counter += Time.deltaTime;
		if (counter % 0.5f > 0.35f)
		{
			base.renderer.enabled = false;
			if ((bool)icon)
			{
				icon.renderer.enabled = false;
			}
			waschecked = false;
		}
		else
		{
			if (waschecked)
			{
				return;
			}
			waschecked = true;
			int number = GetNumber();
			if (number <= 0)
			{
				base.renderer.enabled = false;
				if ((bool)icon)
				{
					icon.renderer.enabled = false;
				}
			}
			else
			{
				base.renderer.enabled = true;
				if ((bool)icon)
				{
					icon.renderer.enabled = true;
				}
				if (old_num != number)
				{
					SetText(number.ToString());
				}
			}
			if (!show_text)
			{
				textmesh.renderer.enabled = false;
			}
			old_num = number;
		}
	}
}
