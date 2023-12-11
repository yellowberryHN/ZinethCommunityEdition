using System.Collections.Generic;
using UnityEngine;

public class PhoneTweetComposer : PhoneMainMenu
{
	public string message_text = string.Empty;

	public string[] words = new string[28]
	{
		"I", "love", "move", "play", "am", "are", "garbage", "Zineth", "scary", "tweets",
		"lumber", "the", "i", "to", "a", "and", "is", "in", "it", "you",
		"of", "for", "on", "my", "#", "@", "?", ":)"
	};

	public PhoneLabel tweet_text_label;

	private void Start()
	{
		if (hide_background)
		{
			HideBackground();
		}
		List<string> list = new List<string>();
		list.AddRange(words);
		list.AddRange(MonsterTraits.Name.possiblenames);
		list.AddRange(MonsterTraits.Opinions.possibleopinions);
		words = list.ToArray();
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		message_text = string.Empty;
		tweet_text_label.text = string.Empty;
		foreach (PhoneButton auto_button in auto_buttons)
		{
			Object.Destroy(auto_button.gameObject);
			buttons.Remove(auto_button);
		}
		auto_buttons.Clear();
		base.OnLoad();
		for (int i = 0; i < 8; i++)
		{
			CreateWordButton();
		}
	}

	public string RandomWord()
	{
		return words[Random.Range(0, words.Length)];
	}

	private void CreateWordButton()
	{
		PhoneButton phoneButton = Object.Instantiate(button_prefab) as PhoneButton;
		phoneButton.transform.position = base.transform.position;
		phoneButton.transform.parent = base.transform;
		float num = base.renderer.bounds.size.x / 2f;
		float num2 = base.renderer.bounds.size.z / 3f;
		phoneButton.transform.position += Vector3.right * Random.Range(0f - num, num);
		phoneButton.transform.position += Vector3.forward * Random.Range((0f - num2) / 2f, num2);
		buttons.Add(phoneButton);
		auto_buttons.Add(phoneButton);
		phoneButton.screen = this;
		phoneButton.Init();
		if (phoneButton.animateOnLoad)
		{
			phoneButton.transform.position = PhoneController.presspos;
		}
		phoneButton.text = RandomWord();
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		if (message.StartsWith("addword"))
		{
			AddWord(button);
			return true;
		}
		if (message.StartsWith("reply"))
		{
			return PostTweet(message_text);
		}
		return base.ButtonMessage(button, message);
	}

	private void AddWord(PhoneButton button)
	{
		message_text = message_text + button.text + " ";
		if ((bool)tweet_text_label)
		{
			tweet_text_label.text = message_text;
		}
		buttons.Remove(button);
		auto_buttons.Remove(button);
		Object.Destroy(button.gameObject);
		CreateWordButton();
	}

	protected virtual bool PostTweet(string tweet)
	{
		controller.LoadScreen("Twitter");
		return TwitterDemo.PostTweet(tweet.TrimEnd(' '));
	}
}
