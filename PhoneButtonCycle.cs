public class PhoneButtonCycle : PhoneButton
{
	public string[] colors;

	private int ind;

	private void Awake()
	{
		Init();
		if (colors.Length == 0)
		{
			colors[0] = "white";
		}
	}

	public override void OnPressed()
	{
		string text = command + " " + colors[ind];
		controller.DoCommand(text);
	}
}
