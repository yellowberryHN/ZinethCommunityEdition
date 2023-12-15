using UnityEngine;

public class PhoneTwitterAccountMenu : PhoneMainMenu
{
	public PhoneTweetButton accountButton;

	public PhoneButton signInButton;

	public PhoneButton signOutButton;

	public PhoneButton registerButton;

	private string oldImageUrl = string.Empty;

	private string twitterUrl = "http://api.twitter.com/1/users/profile_image/";

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
	}

	public override void UpdateScreen()
	{
		DoProfileStuff();
		base.UpdateScreen();
	}

	public void DoProfileStuff()
	{
		string currentScreenName = TwitterDemo.instance.GetCurrentScreenName();
		if ((bool)accountButton)
		{
			if (accountButton.username_label.text != "@" + currentScreenName)
			{
				accountButton.username_label.text = "@" + currentScreenName;
			}
			string text = twitterUrl + TwitterDemo.instance.GetCurrentUserId();
			if (oldImageUrl != text)
			{
				accountButton.GetImage(text);
				oldImageUrl = text;
			}
		}
		if (!signInButton || !signOutButton || !registerButton)
		{
			return;
		}
		string customScreenName = "";
		if (customScreenName != string.Empty && signInButton.text != "Sign in as\n@" + customScreenName)
		{
			signInButton.text = "Sign in as\n@" + customScreenName;
			signInButton.command = "twitter_login";
			signInButton.OnUnSelected();
			signOutButton.up_button = signInButton;
			if (!buttons.Contains(signInButton))
			{
				buttons.Add(signInButton);
			}
		}
		else if (customScreenName == string.Empty)
		{
			signInButton.text = "You Need to\nRegister First...";
			signInButton.command = string.Empty;
			signInButton.background_box.renderer.material.color = Color.gray;
			signOutButton.up_button = null;
			if (buttons.Contains(signInButton))
			{
				buttons.Remove(signInButton);
			}
		}
	}
}
