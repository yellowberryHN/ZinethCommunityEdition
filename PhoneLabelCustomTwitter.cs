public class PhoneLabelCustomTwitter : PhoneLabel
{
	public bool only_once;

	private bool _has_trigged;

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
			textmesh.renderer.material.color = PhoneMemory.settings.textColor;
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		if (only_once && _has_trigged)
		{
			base.renderer.enabled = false;
		}
		else if (TwitterDemo.instance._isCustom)
		{
			base.renderer.enabled = false;
			_has_trigged = true;
		}
		else
		{
			_has_trigged = false;
			base.renderer.enabled = true;
		}
	}
}
