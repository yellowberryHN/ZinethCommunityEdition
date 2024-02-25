using System.Collections.Generic;
using UnityEngine;

public class MonsterTraits
{
	public class Name
	{
		public static string[] possiblenames = {
			"Cave", "Robby", "Grape", "Isabel", "Pink", "Flip", "George", "Blue", "Berry", "Dust",
			"Pup", "Cave", "Rock", "Mobile", "Sand", "Tree", "Tim", "Trombone", "Drum", "Hour",
			"Sand", "Stick", "Burn", "Despair", "Carp", "Physics", "Blood", "H20", "Bubble", "Baby",
			"Crawl", "Knee", "Puddle", "Tussle", "Vanilla", "Salt", "Float", "Blood", "Fate", "Bones",
			"Under", "Drip", "Teeth", "Adil", "Adult", "Brick", "Scab", "Bucket", "Dunk", "Gems",
			"Crystal", "Rare", "Dirt", "Pet", "Diary", "Dusty", "Tea", "Hell", "Home", "Fish",
			"Dairy", "Rockin'", "Fall", "Legs", "Coat", "Boiled", "Fizzy", "King", "Dog", "Whiskey",
			"Cop", "Speed", "Hair", "Rolling", "Pod", "Champ", "Master", "Straw", "Wood", "Jr",
			"Sr", "Land", "Air", "Sea", "Jungle", "Dragon", "Chase", "Ocean", "Time", "Baby",
			"Swamp", "Bog", "Mud", "Dirt", "Grass", "Flower", "Food", "Soda", "Pop", "Milk",
			"Spike", "Sharp", "House", "Metal", "Evil", "Fresh", "Dribble", "Hill", "Quiz", "Justice",
			"Book", "Hook", "Spill", "Foot", "Shirt", "Red", "Eye", "Shock", "Scoop", "Jazz",
			"Bark", "Moss", "Net", "Slam", "Coyote", "Rash", "Lion", "Tiger", "Gym", "File",
			"Law", "Seal", "Flood", "Skull", "Wiz", "Double", "Wow", "Hanger", "Mist", "Pond",
			"Dude", "Lady", "Tent", "Ring", "Jelly", "Magic", "Glue", "Cat", "Pooch", "Jam",
			"Yell", "Junk", "Mold",
			
			// new names
			
			"Pixel", "Stone", "Rat", "Soul", "Head", "Nail", "Real", "Novice", "Winner", "Extra",
			"Extreme", "Fruit", "Spell", "Hole", "Slam", "Trash", "Balloon", "Big", "Small", "Bowl",
			"Igloo", "Chime", "Ad", "Time", "Skill", "Yard", "Winter", "Key", "Song", "Base", "Bass",
			"Guy", "Gal", "Bro", "Fake", "Burger", "Pizza", "Fries", "Cake", "Cupcake", "Hotdog",
			"Juice", "Tank", "Car", "Sun", "Rad", "Cheese", "Bread", "Clean", "Clear", "Horse",
			"Zest", "Fluff", "Puff", "Munch", "Zing", "Squish"
		};

		public string firstname;

		public string lastname;

		public string fullname
		{
			get
			{
				return firstname + " " + lastname;
			}
		}

		public Name()
		{
			firstname = createFirstName();
			lastname = createLastName();
		}

		public static string createFirstName()
		{
			return possiblenames[Random.Range(0, possiblenames.Length)];
		}

		public static string createLastName()
		{
			return createFirstName();
		}

		public static string createFullName()
		{
			return createFirstName() + " " + createLastName();
		}

		public static implicit operator string(Name name)
		{
			return name.fullname;
		}
	}

	public class Opinions
	{
		public static readonly string[] subjects = new string[200]
		{
			"Bugs", "Homework", "Taxes", "Ice Cream", "Ice", "Cream", "Sunsets", "Pups", "Itchy Skin", "Skin",
			"Summer Savings", "Savings", "Cleaning", "Fussin'", "Cheese", "Light", "Punk Music", "Punks", "Music", "Fast Cars",
			"Cars", "Red Objects", "Objects", "Shaking Hands", "Hands", "Mutual Respect", "Respect", "Healthy Diet", "Diet", "Underdogs",
			"Pipes", "Synergy", "Jungling", "Dinner Plans", "Plans", "Musical Tunes", "Tunes", "Trash", "Transparency", "Traveling",
			"Vacation", "Thanksgiving", "Heat", "Mondays", "TV Shows", "Shows", "Soft Drinks", "Drinks", "Fate", "Blue Eyes",
			"Eyes", "Ethnic Slurs", "Slurs", "Hard Work", "Work", "Charity", "Moon Beams", "Beams", "Yogurt", "Rocking Chairs",
			"Chairs", "Backup Plans", "Plans", "Emergency Exits", "Exits", "Threats", "Veggies", "Apples", "Oranges", "Bugs",
			"Defeat", "Slam Dunks", "Dunks", "Art", "Individuality", "Dining", "Math", "Spells", "Sticky Fingers", "Fingers",
			"Garbage", "Noses", "Glass Eyes", "Fancy Clothes", "Clothes", "Horror Movies", "Movies", "Folk Tales", "Tales", "Theory",
			"Swimming", "Shopping", "Scraping", "Spelunking", "Tiny Clues", "Clues", "Big Dogs", "Dogs", "Gamers", "Silence",
			"Depression", "Despair", "Pizza", "Rigs", "Chores", "Passion", "Illusions", "Rashes", "Warm Milk", "Milk",
			"Slides", "Kids", "Retirement", "Childhood", "Teens", "Parents", "Pop Quizes", "Quizes", "Grinding", "Drifting",
			"Leaves", "House Parties", "Parties", "Wigs", "Involuntary Solitude", "Solitude", "Bigshots", "Pooches", "Fun", "Holiday Memories",
			"Memories", "Tubs", "Youth", "Rad Tricks", "Tricks", "Elbow Grease", "Grease", "Pranks", "Silly Gags", "Gags",
			"Hijinks", "Lumber Yards", "Yards", "Graphic Novels", "Novels", "Video Games", "Games", "Videos", "Joyful Reunions", "Reunions",
			"Moolah", "Petting Zoos", "Zoos", "Spirited Debates", "Debates", "Ghosts", "Ancient Tech", "Tech", "Human Dogs", "Dogs",
			"Dirt", "Rural Myths", "Myths", "Aspect Ratios", "Ratios", "Outdoor Flames", "Flames", "Fires", "Tropical Sunsets", "Sunsets",
			"Risky Trails", "Trails", "Undiscovered Flora", "Flora", "Movie Magic", "Magic", "Howling Tundras", "Tundras", "Regret", "Ruined Birthdays",
			"Birthdays", "Anarchy", "Anarchy", "Chaos", "Rivers And Mud", "Friendships", "Reptialian Monarchies", "Monarchies", "Cats", "Aggressive Jokes",
			"Jokes", "Cooking", "Phantoms", "Dealings", "Sleuthing", "Exotic Pets", "Pets", "Space Movies", "Movies", "Docs"
		};

		public static readonly string[] adjectives = new string[72]
		{
			"Ice", "Light", "Fast", "Red", "Shaking", "Healthy", "Musical", "TV", "Soft", "Blue",
			"Ethnic", "Hard", "Windy", "Rocking", "Backup", "Very Small", "Slam", "Ancient", "Sticky", "Glass",
			"Fancy", "Horror", "Folk", "Itchy", "Tiny", "Big", "Fantastic", "Warm", "Autumn", "House",
			"Involuntary", "Fun", "Fun Loving", "Loving", "Holiday", "Rad", "Great", "Joke", "Prank", "Silly",
			"Wacky", "Lumber", "Eternal", "Graphic", "Novel", "Video", "Joyful", "Pet", "Spirited", "Ancient",
			"Human", "Rural", "Urban", "Outdoor", "Big", "Tropical", "Risky", "Movie", "Howling", "Ruined",
			"Total", "Absolute", "Reptilian", "Aggressive", "Shady", "Exotic", "Space", "Dreary", "Animal", "Internal",
			"Dank", "Moldy"
		};

		public static readonly string[] phrases = new string[19]
		{
			"Losin' the {subject}", "Growing Up {adjective}", "Typin' Up {adjective} Drafts", "Rolling in {subject}", "Buttoning Up", "Crying Alone", "Rollin' Around", "The {adjective} Autumn Breeze", "The {adjective} Breeze", "The Big Bucks",
			"Diggin' Up {subject}", "Gasping For Breath", "The Spirit Of Anarchy", "The Spirit Of {subject}", "Canceling {adjective} Friendships", "The {subject} Man", "The {adjective} Man", "Fun Loving Pooches", "Sleuthing Around"
		};

		public static readonly string[] possibleopinions = new string[152]
		{
			"Bugs", "Homework", "Taxes", "Ice Cream", "Sunsets", "Pups", "Itchy Skin", "Summer Savings", "Spring Cleaning", "Fussin'",
			"String Cheese", "Light", "Punk Music", "Fast Cars", "Red Objects", "Shaking Hands", "Mutual Respect", "Healthy Diet", "Underdogs", "Pipes",
			"Synergy", "Jungling", "Dinner Plans", "Losin' the Keys", "Musical Tunes", "Trash", "Transparency", "Traveling", "Vacation", "Thanksgiving",
			"Buttoning Up", "Heat", "Mondays", "TV Shows", "Soft Drink", "Fate", "Blue Eyes", "Ethnic Slurs", "Crying Alone", "Hard Work",
			"Charity", "Windy Conditions", "Moon Beams", "Yogurt", "Rocking Chairs", "Backup Plans", "Emergency Exits", "Threats", "Veggies", "Apples",
			"Oranges", "Very Small Bugs", "Losing", "Slam Dunks", "Art", "Individuality", "Rollin' Around", "Dining Alone", "Math", "Ancient Spells",
			"Sticky Fingers", "Garbage", "Noses", "Glass Eyes", "Fancy Clothes", "Horror Movies", "Folk Tales", "Color Theory", "Swimming", "Shopping",
			"Scraping", "Spelunking", "Tiny Clues", "Big Dogs", "Gamers", "Silence", "Depression", "Despair", "Pizza", "Gaming Rigs",
			"Chores", "Passion", "Fantastic Illusions", "Rashes", "Warm Milk", "Slides", "Kids", "Growing Up", "Retirement", "Middle Age",
			"Childhood", "Teens", "Parents", "Pop Quizes", "Grinding", "Drifting", "Typin' Up Drafts", "Rolling in Leaves", "The Autumn Breeze", "House Parties",
			"Wigs", "Involuntary Solitude", "Bigshots", "Fun Loving Pooches", "Holiday Memories", "Tubs", "Youth", "Rad Tricks", "Elbow Grease", "Great Pranks",
			"Silly Gags", "Wacky Hijinks", "Lumber Yards", "Graphic Novels", "Video Games", "Joyful Reunions", "Moolah", "The Big Bucks", "Petting Zoos", "Spirited Debates",
			"Ghosts", "Ancient Tech", "Human Dogs", "Diggin' Up Dirt", "Rural Myths", "Aspect Ratios", "Outdoor Flames", "Big Fires", "Tropical Sunsets", "Risky Trails",
			"Undiscovered Flora", "Movie Magic", "Howling Tundras", "Regret", "Gasping For Breath", "Ruined Birthdays", "Total Anarchy", "Absolute Chaos", "Rivers And Mud", "The Spirit Of Anarchy",
			"Canceling Friendships", "Reptialian Monarchies", "Cats", "The Bird Man", "Aggressive Jokes", "Cooking", "Phantoms", "Shady Dealings", "Sleuthing Around", "Exotic Pets",
			"Space Movies", "Dreary Docs"
		};

		public List<string> likes;

		public List<string> dislikes;

		public Opinions()
		{
			likes = createLikes();
			dislikes = createLikes(likes);
		}

		public static string GetRandomSubject()
		{
			return subjects[Random.Range(0, subjects.Length)];
		}

		public static string GetRandomAdjective()
		{
			return adjectives[Random.Range(0, adjectives.Length)];
		}

		public static string GetRandomPhrase()
		{
			string text = phrases[Random.Range(0, phrases.Length)];
			return text.Replace("{subject}", GetRandomSubject()).Replace("{adjective}", GetRandomAdjective());
		}

		public static string NewGetPossibleOpinion()
		{
			float num = (float)phrases.Length / (float)subjects.Length * 2f;
			if (Random.value <= num)
			{
				return GetRandomPhrase();
			}
			return GetRandomAdjective() + " " + GetRandomSubject();
		}

		public static string GetRandomOpinion()
		{
			return possibleopinions[Random.Range(0, possibleopinions.Length)];
		}

		public static List<string> createLikes()
		{
			return createLikes(new List<string>());
		}

		public static List<string> createLikes(List<string> ignorelist)
		{
			List<string> list = new List<string>();
			int num = Random.Range(1, 4);
			for (int i = 0; i < num; i++)
			{
				string item = possibleopinions[Random.Range(0, possibleopinions.Length)];
				if (!ignorelist.Contains(item))
				{
					list.Add(item);
					ignorelist.Add(item);
				}
			}
			return list;
		}
	}

	public class BloodType
	{
		private static string[] bloodtypes = new string[6] { "burger", "cake", "cupcake", "hotdog", "pizza", "soda" };

		public string typename;

		public BloodType()
		{
			typename = Generate();
		}

		private static string Generate()
		{
			return bloodtypes[Random.Range(0, bloodtypes.Length)];
		}

		public static implicit operator string(BloodType bloodtype)
		{
			return bloodtype.typename;
		}
	}

	public Name name;

	public BloodType bloodtype;

	public MonsterTraits()
	{
		name = new Name();
		bloodtype = new BloodType();
	}
}
