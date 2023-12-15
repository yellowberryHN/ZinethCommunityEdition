public class PhoneLabelNewTweets : PhoneLabelNewMail
{
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

	protected override int GetNumber()
	{
		return 0;
	}
}
