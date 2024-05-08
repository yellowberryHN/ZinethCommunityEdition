public class PhoneButtonCycle : PhoneButton
{
	public string[] colors;

	private int ind;

	private void Awake()
	{
		Init();
		if (colors == null)
		{
			colors = new[] { "white" };
		}
	}

	public override void OnPressed()
	{
		string text = command + " " + colors[ind];
		controller.DoCommand(text);
	}
}
