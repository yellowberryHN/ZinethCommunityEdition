using System.Collections.Generic;
using UnityEngine;

public class TweetComposer
{
	private static string[] my_name = new string[33]
	{
		"Hi, I'm {0}", "My name is {0}", "I go by {0}", "The name's {0}", "They call me {0}", "Name's {0}", "Call me {0}", "You can call me {0}", "Please think of me as {0}", "My gamer tag is {0}",
		"I only respond to {0}", "My handle is {0}", "Mom calls me {0}", "Changed my name to {0}", "My ID says {0}", "Refer to me as {0}", "Here lies {0}", "My nickname's {0}", "This is {0}", "Bullies know me as {0}",
		"Cops call me {0}", "Remember the name {0}", "I'm known as {0}", "Now I'm {0}", "Don't call me {0}", "Agent {0} speaking", "This is Mr {0}", "{0} is my middle name", "It says {0} on my shirt", "The wife calls me {0}",
		"Address your comments to {0}", "You are now speaking with {0}", "My kids have to call me {0}"
	};

	private static string[] i_likes = new string[39]
	{
		"I like {0}", "I love {0}", "I'm all about {0}", "I live for {0}", "I am excited by {0}", "I can't live without {0}", "I'm a big fan of {0}", "I really relate to {0}", "I'm passionate about {0}", "I long for {0}",
		"I dream about {0}", "I fantasize about {0}", "I really fancy {0}", "{0} is my passion", "{0} is the only thing I know", "{0} is nice", "{0} is my first choice", "I think about {0} all day", "I think about {0} at night", "I talk about {0} at lunch",
		"I dislike {0}", "I hate {0}", "I don't care for {0}", "I despise {0}", "I'm not a fan of {0}", "I could do without {0}", "I don't understand {0}", "I'm puzzled by {0}", "I'm sick of {0}", "I can't deal with {0}",
		"I cry about {0}", "I'm scared of {0}", "I don't care about {0}", "I want to destroy {0}", "{0} is horrible", "{0} is my greatest fear", "when {0} is mentioned I cringe", "I believe {0} must be a joke", "I can't imagine {0}"
	};

	private static string[] even_tho = new string[17]
	{
		" even though ", " yet ", " despite the fact that ", " and ", " because ", " since ", " and ", " and ", ": ", " which is why ",
		" but ", "... ", ". ", " so ", " ;). ", ". Also, ", " and yet, "
	};

	private static readonly string[] before_part = new string[1] { "Listen." };

	private static readonly string[] after_part = new string[2] { "Ok?", "Deal with it." };

	private static string[] obs_start = new string[2] { "Why is it that", "I always notice that" };

	public static string MakeTweet()
	{
		string text = NameTweet() + " " + OpinionTweet();
		if (text.Length > 140)
		{
			Debug.LogWarning("Tweet exceeds 140 chars! Watch out \n" + text);
		}
		else if (Application.isEditor)
		{
			Debug.Log(text);
		}
		return text;
	}

	protected static string NameTweet()
	{
		string arg = MonsterTraits.Name.createFullName();
		string text = string.Format(GetRandomThing(my_name), arg);
		return text + ".";
	}

	protected static string OpinionTweet()
	{
		string text = MonsterTraits.Opinions.NewGetPossibleOpinion().ToLower();
		string text2 = MonsterTraits.Opinions.NewGetPossibleOpinion().ToLower();
		string text3 = GetRandomThing(i_likes);
		if (text.EndsWith("s"))
		{
			text3 = text3.Replace(" is ", " are ");
		}
		text3 = string.Format(text3, text);
		text3 = text3[0].ToString().ToUpper() + text3.Substring(1);
		string text4 = text3;
		text4 += GetRandomThing(even_tho);
		text3 = GetRandomThing(i_likes);
		if (text4.TrimEnd(' ').EndsWith("."))
		{
			text3 = text3[0].ToString().ToUpper() + text3.Substring(1);
		}
		if (text2.EndsWith("s"))
		{
			text3 = text3.Replace(" is ", " are ");
		}
		text3 = string.Format(text3, text2);
		return text4 + text3 + ".";
	}

	protected static string ObservationTweet()
	{
		string text = MonsterTraits.Opinions.GetRandomOpinion().ToLower();
		string text2 = MonsterTraits.Opinions.GetRandomOpinion().ToLower();
		string text3 = GetRandomThing(i_likes);
		if (text.EndsWith("s"))
		{
			text3 = text3.Replace(" is ", " are ");
		}
		text3 = string.Format(text3, text);
		text3 = text3[0].ToString().ToUpper() + text3.Substring(1);
		string text4 = text3;
		text4 += GetRandomThing(even_tho);
		text3 = GetRandomThing(i_likes);
		if (text4.TrimEnd(' ').EndsWith("."))
		{
			text3 = text3[0].ToString().ToUpper() + text3.Substring(1);
		}
		if (text2.EndsWith("s"))
		{
			text3 = text3.Replace(" is ", " are ");
		}
		text3 = string.Format(text3, text2);
		return text4 + text3 + ".";
	}

	protected static string GetRandomLike(MonsterTraits.Opinions opinion)
	{
		return GetRandomThing(opinion.likes, true);
	}

	protected static string GetRandomDislike(MonsterTraits.Opinions opinion)
	{
		return GetRandomThing(opinion.dislikes, true);
	}

	protected static string GetRandomThing(string[] things)
	{
		return things[Random.Range(0, things.Length)];
	}

	protected static string GetRandomThing(List<string> things)
	{
		return GetRandomThing(things, false);
	}

	protected static string GetRandomThing(List<string> things, bool lower)
	{
		string text = things[Random.Range(0, things.Count)];
		if (lower)
		{
			text = text.ToLower();
		}
		return text;
	}
}
