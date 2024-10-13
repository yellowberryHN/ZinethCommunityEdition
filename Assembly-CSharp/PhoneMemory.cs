using System;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMemory : MonoBehaviour
{
	private static PhoneMemory _instance;

	public static int phoneGameScore = 0;

	public static NPCTrainer trainer_challenge;

	public static bool menus_updated = false;

	public List<string> _unlocked_menus = new List<string>();

	private static PhoneZineMenu _zine_menu;

	public List<Texture2D> _unlocked_zines;

	public static bool mail_updated;

	public static PhoneSettings _settings;

	public List<PhoneMonster> _monsters = new List<PhoneMonster>();

	public int _monster_ind;

	public int _game_level;

	public List<PhoneMail> _messages = new List<PhoneMail>();

	public List<PhoneMail> _deleted_messages = new List<PhoneMail>();

	private float __capsule_points = float.NegativeInfinity;

	public static bool initialized = false;

	private static List<PhoneMail> extra_mail_add = new List<PhoneMail>();

	private static bool _setting_up = false;

	public static int max_num_monsters = 2;

	public PhoneMail[] mail_auto_list = new PhoneMail[0];

	public Dictionary<string, PhoneColorPalette> colorthemes = new Dictionary<string, PhoneColorPalette>();
	
	private List<string> theme_keys = new List<string>();
	
	private int theme_idx = -1;

	public static PhoneMemory main
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(PhoneMemory)) as PhoneMemory;
			}
			if ((bool)_instance && !initialized)
			{
				_instance.Initialize();
			}
			return _instance;
		}
	}

	public static PhoneSettings settings
	{
		get
		{
			return _settings;
		}
	}

	public static bool isingame
	{
		get
		{
			return PhoneController.instance.curscreen.screenname == "GameScreen";
		}
	}

	public static bool isbattling
	{
		get
		{
			if (isingame)
			{
				PhoneShooterController phoneShooterController = PhoneController.instance.curscreen as PhoneShooterController;
				return phoneShooterController.battle_mode;
			}
			return false;
		}
	}

	public static List<PhoneMonster> monsters
	{
		get
		{
			return main._monsters;
		}
	}

	public static int monster_ind
	{
		get
		{
			return main._monster_ind;
		}
		set
		{
			main._monster_ind = value;
		}
	}

	public static PhoneMonster main_monster
	{
		get
		{
			return monsters[monster_ind];
		}
		set
		{
			monster_ind = monsters.IndexOf(value);
		}
	}

	public static PhoneShooterLevel level_obj
	{
		get
		{
			if (game_level >= PhoneResourceController.phoneshooterlevels.Count)
			{
				if ((bool)trainer_challenge)
				{
					return trainer_challenge.levelobj;
				}
				game_level = 0;
			}
			return PhoneResourceController.phoneshooterlevels[game_level];
		}
	}

	public static int game_level
	{
		get
		{
			return main._game_level;
		}
		set
		{
			main._game_level = value;
		}
	}

	public static List<string> unlocked_menus
	{
		get
		{
			return main._unlocked_menus;
		}
	}

	private static PhoneZineMenu zine_menu
	{
		get
		{
			if (_zine_menu == null)
			{
				_zine_menu = UnityEngine.Object.FindObjectOfType(typeof(PhoneZineMenu)) as PhoneZineMenu;
			}
			return _zine_menu;
		}
	}

	public static List<Texture2D> unlocked_zines
	{
		get
		{
			return main._unlocked_zines;
		}
		set
		{
			main._unlocked_zines = value;
		}
	}

	public static int new_mail
	{
		get
		{
			int num = 0;
			foreach (PhoneMail message in messages)
			{
				if (message.is_new)
				{
					num++;
				}
			}
			return num;
		}
	}

	public static List<PhoneMail> messages
	{
		get
		{
			return main._messages;
		}
	}

	public static List<PhoneMail> deleted_messages
	{
		get
		{
			return main._deleted_messages;
		}
	}

	public static float capsule_points
	{
		get
		{
			return main._capsule_points;
		}
		set
		{
			main._capsule_points = value;
		}
	}

	public float _capsule_points
	{
		get
		{
			if (__capsule_points == float.NegativeInfinity)
			{
				__capsule_points = PlayerPrefs.GetFloat("cash", 0f);
			}
			return __capsule_points;
		}
		set
		{
			__capsule_points = value;
			if (__capsule_points != float.NegativeInfinity)
			{
				PlayerPrefs.SetFloat("cash", value);
			}
		}
	}

	public static bool IsBattlingTrainer(NPCTrainer trainer)
	{
		return isbattling && trainer_challenge == trainer;
	}

	public static bool MonsterChallenge(NPCTrainer trainer)
	{
		if (trainer_challenge == null)
		{
			trainer_challenge = trainer;
			game_level = PhoneResourceController.phoneshooterlevels.Count;
			return true;
		}
		return false;
	}

	public static bool WithdrawChallenge(NPCTrainer trainer)
	{
		if (trainer == trainer_challenge)
		{
			trainer_challenge = null;
			return true;
		}
		return false;
	}

	public static bool SaveMonsters()
	{
		bool flag = true;
		for (int i = 0; i < monsters.Count; i++)
		{
			flag = flag && SaveMonster(i);
		}
		return flag;
	}

	public static bool SaveMonster(PhoneMonster monster)
	{
		return SaveMonster(monsters.IndexOf(monster));
	}

	public static bool SaveMonster(int index)
	{
		return monsters[index].SaveMonster(index);
	}

	public static bool IsMenuUnlocked(string menuname)
	{
		return unlocked_menus.Contains(menuname);
	}

	public static void LockMenu(PhoneScreen screen)
	{
		LockMenu(screen.screenname);
	}

	public static void LockMenu(string menu)
	{
		if (unlocked_menus.Contains(menu))
		{
			menus_updated = true;
			unlocked_menus.Remove(menu);
		}
	}

	public static void UnlockMenu(PhoneScreen screen)
	{
		UnlockMenu(screen.screenname);
	}

	public static void UnlockMenu(string menu)
	{
		if (!unlocked_menus.Contains(menu))
		{
			UnlockMenuQuiet(menu);
			PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_new_app, SoundType.menu);
		}
	}

	public static void UnlockMenuQuiet(string menu)
	{
		if (!unlocked_menus.Contains(menu))
		{
			menus_updated = true;
			unlocked_menus.Add(menu);
		}
	}

	private static int CompareZines(Texture2D a, Texture2D b)
	{
		return string.Compare(a.name, b.name);
	}

	public static void UnlockZine(int index)
	{
		if (index >= PhoneResourceController.zine_images.Count)
		{
			Debug.LogWarning("zine index is too big!");
		}
		else
		{
			UnlockZine(PhoneResourceController.zine_images[index]);
		}
	}

	public static void UnlockZine(Texture2D tex)
	{
		if (!unlocked_zines.Contains(tex))
		{
			unlocked_zines.Add(tex);
			unlocked_zines.Sort(CompareZines);
			zine_menu.zine_ind = unlocked_zines.IndexOf(tex);
		}
	}

	public static void OnZineUnlock(int index)
	{
	}

	public static void SendMail(string id)
	{
		PhoneMail phoneMail = MailController.FindMail(id);
		if (phoneMail == null)
		{
			Debug.LogWarning("Mail id does not exist: " + id);
		}
		else
		{
			SendMail(phoneMail);
		}
	}

	public static void SendMail(PhoneMail mail)
	{
		PhoneController.instance.OnNewMessage(1);
		SendMailQuiet(mail);
	}

	public static void SendMailQuiet(string id)
	{
		PhoneMail phoneMail = MailController.FindMail(id);
		if (phoneMail == null)
		{
			Debug.LogWarning("Mail id does not exist: " + id);
		}
		else
		{
			SendMailQuiet(phoneMail);
		}
	}

	public static void SendMailQuiet(PhoneMail mail)
	{
		if (mail.time.Ticks == 0L)
		{
			mail.time = DateTime.Now;
		}
		if (!messages.Contains(mail))
		{
			messages.Insert(0, mail);
			mail_updated = true;
			if (!_setting_up)
			{
				SaveMail();
			}
		}
	}

	public static PhoneMail GetMail(string id)
	{
		PhoneMail phoneMail = MailController.FindMail(id);
		if (messages.Contains(phoneMail))
		{
			return phoneMail;
		}
		return null;
	}

	public static PhoneMail GetMail(int ind)
	{
		return messages[ind];
	}

	public static bool DeleteMail(PhoneMail mailobj)
	{
		deleted_messages.Add(mailobj);
		messages.Remove(mailobj);
		mail_updated = true;
		return true;
	}

	public static bool DeleteMail(int ind)
	{
		deleted_messages.Add(messages[ind]);
		messages.RemoveAt(ind);
		mail_updated = true;
		return true;
	}

	public static void AddCapsulePoints(float amount)
	{
		capsule_points += amount;
	}

	public static void ResetCapsulePoints()
	{
		capsule_points = float.NegativeInfinity;
	}

	private void Awake()
	{
		if (!initialized)
		{
			Initialize();
		}
		if (Application.loadedLevelName == "Loader 1" || Application.loadedLevelName == "test")
		{
			BeginGame();
		}
		PhoneMail[] array = mail_auto_list;
		foreach (PhoneMail mailobj in array)
		{
			MailController.AddMail(mailobj);
		}
	}

	public void Initialize()
	{
		initialized = true;
		_settings = new PhoneSettings();
		// TODO: this isn't a great spot for this, consider placing elsewhere
		PhoneMapController.player_radar = PlayerPrefsX.GetBool("player_radar", true);
		float master_volume = settings.master_volume;
		SetupMail();
		SetupColors();
		SetupScreens();
	}

	private void BeginGame()
	{
		PlayerPrefs.SetString("version", PhoneInterface.version);
		PlayerPrefs.SetInt("times_file_played", PlayerPrefs.GetInt("times_file_played", 0) + 1);
		SetupColors();
		SetupMonsters();
		SetupZines();
		MailController.active_mail.Clear();
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("mail_string", string.Empty)))
		{
			LoadMail();
		}
		else
		{
			foreach (PhoneMail value in MailController.all_mail.Values)
			{
				value.is_new = true;
			}
			MailController.FindMail("jobOffer").is_new = false;
			MailController.FindMail("bank").is_new = false;
			MailController.SendMail("bank");
			MailController.SendMail("jobOffer");
			MailController.SendMail("intro_1");
		}
		foreach (PhoneMail item in extra_mail_add)
		{
			MailController.SendMail(item.id);
		}
		if (PlayerPrefsX.GetBool("cool_cam", false))
		{
			UnlockMenuQuiet("Cool Cam");
		}
	}

	public static void LoadMail()
	{
		string @string = PlayerPrefs.GetString("mail_string", string.Empty);
		string[] array = @string.Split('|');
		_setting_up = true;
		for (int num = array.Length - 1; num >= 0; num--)
		{
			string text = array[num];
			bool is_new = false;
			if (text.StartsWith("!"))
			{
				is_new = true;
				text = text.TrimStart('!');
			}
			PhoneMail phoneMail = MailController.FindMail(text);
			if (phoneMail != null)
			{
				phoneMail.is_new = is_new;
				SendMailQuiet(phoneMail);
			}
		}
		_setting_up = false;
	}

	public static void SaveMail()
	{
		string text = string.Empty;
		foreach (PhoneMail message in messages)
		{
			if (!message.id.StartsWith("npc_"))
			{
				string text2 = string.Format("{0}|", message.id);
				if (message.is_new)
				{
					text2 = string.Format("!{0}", text2);
				}
				text += text2;
			}
		}
		text = text.TrimEnd('|');
		PlayerPrefs.SetString("mail_string", text);
	}

	private void SetupMonsters()
	{
		for (int i = 0; i < max_num_monsters; i++)
		{
			InitMonster(i);
		}
	}

	private void InitMonster(int index)
	{
		PhoneMonster phoneMonster;
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("monster" + index + "_namef", string.Empty)))
		{
			phoneMonster = PhoneMonster.LoadMonster(index);
		}
		else
		{
			phoneMonster = new PhoneMonster(1f);
			phoneMonster.SaveMonster(index);
		}
		while (monsters.Count <= index)
		{
			monsters.Add(null);
		}
		monsters[index] = phoneMonster;
	}

	public static void ResetMonsters()
	{
		while (monsters.Count < max_num_monsters)
		{
			monsters.Add(null);
		}
		for (int i = 0; i < max_num_monsters; i++)
		{
			PhoneMonster phoneMonster = new PhoneMonster(1f);
			if (i >= monsters.Count)
			{
				monsters.Add(phoneMonster);
			}
			else
			{
				monsters[i] = phoneMonster;
			}
		}
		SaveMonsters();
	}

	private void DebugResetMonsters()
	{
		for (int i = 1; i < 6; i++)
		{
			_monsters.Add(new PhoneMonster(i));
		}
	}

	private void SetupZines()
	{
		if (unlocked_zines.Count <= 0)
		{
			unlocked_zines = new List<Texture2D>();
			if (PhoneResourceController.zine_images.Count > 0)
			{
				UnlockZine(0);
			}
			if (PhoneResourceController.zine_images.Count > 1)
			{
				UnlockZine(1);
			}
		}
	}

	private void SetupScreens()
	{
		if (unlocked_menus.Count == 0)
		{
			unlocked_menus.Add("Mail");
		}
	}

	private void SetupMail()
	{
		PhoneMail[] array = mail_auto_list;
		foreach (PhoneMail mailobj in array)
		{
			MailController.AddMail(mailobj);
		}
		PhoneMail phoneMail = new PhoneMail();
		phoneMail.id = "bank";
		phoneMail.subject = "Loan Approved";
		phoneMail.sender = "BigBank";
		phoneMail.body = "Dear sir,                 We are writing to let you know that your loan of $180,000,000,000 desert dollars has been approved.  Please remember to send all monthly payments in on time, and thank you for choosing BigBank for all your big banking needs.";
		phoneMail.is_new = false;
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "jobOffer";
		phoneMail.subject = "Re: Help Wanted";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "The job's yours!  You can start tomorrow at 9AM sharp.  The pay's based on  how well the zine sells.  That robot suit of yours should make the job much easier.                  Welcome aboard!";
		phoneMail.is_new = false;
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "intro_1";
		phoneMail.subject = "Where are you?";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "What's going on? You need to get over here now!";
		phoneMail.is_new = true;
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "intro_2";
		phoneMail.subject = "Lost?";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "Did you get lost? I sent you my position. Please just get here.";
		phoneMail.open_command = "mission_activate Intro_1|mission_focus Intro_1";
		phoneMail.accept_command = "load_screen Map";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "CactusMissionMail";
		phoneMail.subject = "Spikey Job";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "I've got a new job for you. We need to make some cactus paper for the next batch of zines, but I'm really scared of needles. Break open 10 cacti and bring me back the chunks.";
		phoneMail.open_command = "mission_activate CactusBreak";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "LostPages";
		phoneMail.subject = "Missed you";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "Had to jet out.  I accidentally dropped a few pages meant for the new zine.  The wind had its way with them, but they should all be nearby.  Grab them for me please!";
		phoneMail.open_command = "mission_activate LostPages|mission_focus LostPages";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "DeliverZines";
		phoneMail.subject = "Delivery";
		phoneMail.sender = "Zine Boss";
		phoneMail.body = "You finished picking up those zine pages yet?  I need you to go towards the city and hand them out to anyone and everyone.";
		phoneMail.open_command = "mission_activate DeliverZines|mission_focus DeliverZines";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "tut_capsule";
		phoneMail.subject = "Capsule Quest!";
		phoneMail.sender = "Catco";
		phoneMail.body = "Thank you for entering Capsule Quest! Find capsules to gain capsule points! Redeem them to upgrade your Catco monsters. Your game is waiting!";
		MailController.AddMail(phoneMail);
		MailController.AddMail(new PhoneMail("test_test", "Mail Test", "This text is designed to mimic an actual message. It contains little content.", "Tester"));
		MailController.AddMail(new PhoneMail("test_noodles", "RE:NOODLES", "Alert! New Flavors of NOODLESOOP have been spotted in the area. Please keep your eyes open!", "Noodle Club"));
		MailController.AddMail(new PhoneMail("test_meeting", "Hi", "Hey, how are you doing? Please meet me in Pizza Town tomorrow at 2:00PM.", "Pep"));
		phoneMail = new PhoneMail();
		phoneMail.id = "test_job";
		phoneMail.subject = "Job for you!";
		phoneMail.sender = "Job Dude";
		phoneMail.body = "I have a job for you. I want you to go to a few checkpoints. Do you think you can handle it?";
		phoneMail.can_reply = true;
		phoneMail.accept_command = "load_screen Missions|mission_activate Test";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "test_points";
		phoneMail.subject = "Free Points";
		phoneMail.sender = "Gamedave";
		phoneMail.body = "Here are some free points to make the testing process easier.";
		phoneMail.can_reply = true;
		phoneMail.accept_command = "capsule_points_add 999";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "test_trap";
		phoneMail.subject = "Free Cash";
		phoneMail.sender = "Secret";
		phoneMail.body = "Don't spend it all in one place!";
		phoneMail.can_reply = true;
		phoneMail.accept_command = "capsule_points_add 999|mail_send test_arrest";
		MailController.AddMail(phoneMail);
		phoneMail = new PhoneMail();
		phoneMail.id = "test_arrest";
		phoneMail.subject = "Busted!";
		phoneMail.sender = "Cops";
		phoneMail.body = "You are under arrest for accepting stolen cash! Nice try, buddy.";
		phoneMail.can_reply = true;
		phoneMail.accept_command = "load_screen PhoneJail";
		phoneMail.close_command = "load_screen PhoneJail";
		MailController.AddMail(phoneMail);
		DateTime now = DateTime.Now;
		if (now.Month == 10 && now.Day == 31)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_halloween";
			phoneMail.subject = "Warning...";
			phoneMail.body = "This message is FILLED with Occult/Supernatural content... it's too late! You are already haunted...";
			phoneMail.sender = "???";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 1 && now.Day == 1)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_newyears";
			phoneMail.subject = "Happy New Years";
			phoneMail.body = "It's a great day for everyone";
			phoneMail.sender = "Mr Time";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 7 && now.Day == 4)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_independence";
			phoneMail.subject = "Are you alone?";
			phoneMail.body = "It's a good day to be independent... haha (joke)";
			phoneMail.sender = "???";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 12 && now.Day == 25)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_xmas";
			phoneMail.subject = "Cool.. wow";
			phoneMail.body = "I hope you're happy...";
			phoneMail.sender = "-Signed: XXX";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 2 && now.Day == 2)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_groundhog";
			phoneMail.subject = "Fatal warning...";
			phoneMail.body = "This is a very dangerous day. Please stay indoors at all times.";
			phoneMail.sender = "PSA";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 2 && now.Day == 29)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_leapday";
			phoneMail.subject = "I can't believe it!";
			phoneMail.body = "You've broken the clock again! How irritating!";
			phoneMail.sender = "Time Frog";
			extra_mail_add.Add(phoneMail);
		}
		if (now.Month == 4 && now.Day == 1)
		{
			phoneMail = new PhoneMail();
			phoneMail.id = "npc_s_fools";
			phoneMail.subject = "The joke's on you...";
			phoneMail.body = "That's It";
			phoneMail.sender = "The Joke Master";
			extra_mail_add.Add(phoneMail);
		}
		foreach (PhoneMail item in extra_mail_add)
		{
			MailController.AddMail(item);
		}
	}

	public void AddTheme(string name, PhoneColorPalette palette)
	{
		colorthemes.Add(name, palette);
		theme_keys.Add(name);
	}

	private void SetupColors()
	{
		PhoneColorPalette phoneColorPalette = new PhoneColorPalette
		{
			text = new Color32(10, 18, 10),
			selectable = new Color32(161, 76, 161),
			selected = new Color32(255, 255, 255),
			back = new Color32(154, 245, 184),
			mail = new Color32(245, 215,245)
		};
		AddTheme("old", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			text = new Color32(252, 235, 182),
			selectable = new Color32(240, 120, 24),
			selected = new Color32(240, 168, 48),
			back = new Color32(94, 65, 47),
			mail = new Color32(137, 82, 48)
		};
		AddTheme("papua", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			text = new Color32(77, 156, 77),
			selectable = new Color32(139, 172, 15),
			selected = new Color32(155, 188, 15),
			back = new Color32(15, 56, 15),
			mail = new Color32(195,226,195),
			dark = true
		};
		AddTheme("gameboy", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			text = new Color32(60, 50, 81),
			selectable = new Color32(53, 150, 104),
			selected = new Color32(190, 240, 125),
			back = new Color32(255, 237, 144)
		};
		AddTheme("moment", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(153, 170, 128),
			text = new Color32(195, 255, 104),
			selectable = new Color32(45, 50, 60),
			selected = new Color32(126, 208, 214),
			mail = new Color32(76, 85, 61)
		};
		AddTheme("frogs", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(233, 233, 233),
			text = new Color32(66, 66, 66),
			selectable = new Color32(30, 30, 30),
			selected = new Color32(255, 153, 0)
		};
		AddTheme("gamebookers", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(152, 127, 105),
			text = new Color32(253, 241, 204),
			selectable = new Color32(227, 173, 64),
			selected = new Color32(252, 208, 54),
			mail = new Color32(68,60,53)
		};
		phoneColorPalette.particles = phoneColorPalette.selected;
		AddTheme("honey", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(0, 0, 0),
			text = new Color32(255, 255, 255),
			selectable = new Color32(231, 80, 80),
			selected = new Color32(255, 20, 20),
			mail = Color.gray,
			dark = true
		};
		phoneColorPalette.particles = phoneColorPalette.selected;
		AddTheme("black", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(255, 255, 255),
			text = new Color32(0, 0, 0),
			selectable = new Color32(42, 42, 84),
			selected = new Color32(50, 50, 140)
		};
		phoneColorPalette.particles = phoneColorPalette.selected;
		AddTheme("white", phoneColorPalette);
		
		phoneColorPalette = new PhoneColorPalette
		{
			back = new Color32(53, 49, 48),
			text = new Color32(203, 207, 180),
			selectable = new Color32(171, 106, 110),
			selected = new Color32(247, 52, 91),
			mail = new Color32(26,26,26),
			dark = true
		};
		phoneColorPalette.particles = phoneColorPalette.selected;
		AddTheme("killer", phoneColorPalette);
		
		var selected_theme = PlayerPrefs.GetString("phone_theme", "white");
		settings.Palette = colorthemes[colorthemes.ContainsKey(selected_theme) ? selected_theme : "white"];
		theme_idx = theme_keys.IndexOf(selected_theme);
	}
	
	public static void CycleSelectedTheme()
	{
		main.theme_idx = (main.theme_idx + 1) % main.theme_keys.Count;
	}

	public static string current_theme
	{
		get
		{
			return main.theme_keys[main.theme_idx];
		}
	}

	internal void SetTheme(string theme)
	{
		settings.Palette = colorthemes[theme];
	}

	private PhoneColorPalette QuickColorAdder(string name, int r1, int g1, int b1, int r2, int g2, int b2, int r3, int g3, int b3)
	{
		PhoneColorPalette phoneColorPalette = new PhoneColorPalette(new Color32(r1, g1, b1), new Color32(r2, g2, b2), new Color32(r2, g2, b2), new Color32(r3, g3, b3));
		AddTheme(name, phoneColorPalette);
		return phoneColorPalette;
	}
}