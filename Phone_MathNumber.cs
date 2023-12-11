using UnityEngine;

public class Phone_MathNumber : PhoneMainMenu
{
	private string answer;

	public PhoneLabel num1_label;

	public PhoneLabel num2_label;

	public PhoneLabel win_label;

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		SetupGame();
	}

	private string NumToString(float num)
	{
		return num.ToString("0.0");
	}

	private void SetupGame()
	{
		float num = Random.Range(-5f, 5f);
		float num2 = Random.Range(-5f, 5f);
		float num3 = num * num2;
		win_label.text = string.Empty;
		win_label.overrideColor = true;
		answer = NumToString(num3);
		if ((bool)num1_label)
		{
			num1_label.text = NumToString(num);
		}
		if ((bool)num2_label)
		{
			num2_label.text = NumToString(num2);
		}
		foreach (PhoneButton auto_button in auto_buttons)
		{
			auto_button.text = NumToString(Random.Range(-28f, 28f));
			auto_button.command = ".accept";
		}
		auto_buttons[Random.Range(0, auto_buttons.Count)].text = "Recycle";
		auto_buttons[Random.Range(0, auto_buttons.Count)].text = answer;
	}

	private bool CheckAnswer(string text)
	{
		if (text == answer)
		{
			OnWin();
		}
		else
		{
			OnLose();
		}
		return true;
	}

	private void OnWin()
	{
		if ((bool)win_label)
		{
			win_label.text = "CORRECT!!";
			win_label.color = Color.green;
			win_label.renderer.material.color = Color.green;
		}
		Invoke("SetupGame", 1f);
	}

	private void OnLose()
	{
		if ((bool)win_label)
		{
			win_label.text = "WRONG!!";
			win_label.color = Color.red;
			win_label.renderer.material.color = Color.red;
		}
		Invoke("SetupGame", 1f);
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message.StartsWith("accept"))
		{
			CheckAnswer(button.text);
			return true;
		}
		return base.ButtonMessage(button, message);
	}
}
