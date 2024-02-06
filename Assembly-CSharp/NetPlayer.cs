using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : MonoBehaviour
{
	protected class State
	{
		public Vector3 p;

		public Quaternion r;

		public double t;

		public bool h;

		public char a;

		public char s;

		public State(Vector3 pp, Quaternion rr, double tt, bool hh, char aa, char ss)
		{
			p = pp;
			r = rr;
			t = tt;
			h = hh;
			a = aa;
			s = ss;
		}
	}

	private Transform _player_trans;

	public TrailRenderer[] trails;

	public Color altColor = Color.blue;

	public string userName = string.Empty;

	public string fakeName = string.Empty;

	public string twitterId = string.Empty;

	public float ping = -1f;

	public string netStatus = string.Empty;

	public float mood = 4f;

	public Animation myAnimation;

	public Texture2D iconTex;

	public NetworkPlayer networkPlayer;

	public Transform camTarget;

	public bool isHawking;

	public GameObject hawk_obj;

	public int pizzaScore;

	private Color _trailColor = Color.white;

	public Renderer[] robotRends;

	private Color _robotColor = Color.white;

	private int stateCount;

	private State[] states = new State[25];

	private int m;

	private Vector3 p;

	private Quaternion r;

	private bool h;

	private char a;

	private char s;

	private Vector3 velocity = Vector3.zero;

	public bool updatePosition = true;

	public double netInterp = 0.15000000596046448;

	public bool isResponding;

	private bool simulatePhysics;

	private State lastState;

	public bool doPrinting;

	public static Dictionary<string, char> anim_clip_dic = new Dictionary<string, char>();

	private static Dictionary<string, float> anim_speed_dic = new Dictionary<string, float>();

	private static string[] anim_clip_list;

	private char _lastchar;

	private static Dictionary<string, char> screen_state_dic;

	private static string[] screen_state_list = new string[8] { "closed", "na", "Unused", "Cool Cam", "GameScreen", "Mail", "Settings", "Radar" };

	public string cur_screenstate = "closed";

	public Texture2D screen_icon;

	private NPCTrainer trainer;

	private Transform player_trans
	{
		get
		{
			if (_player_trans == null)
			{
				if (base.networkView.isMine)
				{
					_player_trans = PhoneInterface.player_trans;
				}
				else
				{
					_player_trans = base.transform;
				}
			}
			return _player_trans;
		}
		set
		{
			_player_trans = value;
		}
	}

	public Color trailColor
	{
		get
		{
			return _trailColor;
		}
		set
		{
			Color color = value;
			color.a = trailColor.a;
			TrailRenderer[] array = trails;
			foreach (TrailRenderer trailRenderer in array)
			{
				trailRenderer.renderer.material.color = color;
			}
			_trailColor = color;
		}
	}

	public Color robotColor
	{
		get
		{
			return _robotColor;
		}
		set
		{
			Color color = value;
			color.a = robotColor.a;
			Renderer[] array = robotRends;
			foreach (Renderer renderer in array)
			{
				renderer.material.color = color;
			}
			_robotColor = color;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		_robotColor.a = PhoneInterface.robotColor.a;
		if (base.networkView.isMine)
		{
			if (player_trans == null)
			{
				player_trans = PhoneInterface.player_trans;
			}
			camTarget = null;
		}
		else
		{
			if (player_trans == null)
			{
				player_trans = base.transform;
			}
			if (camTarget == null)
			{
				camTarget = base.transform.FindChild("Cam Target");
			}
		}
		SetupAnimations();
		SetupScreenStates();
	}

	private void Start()
	{
		base.networkView.observed = this;
	}

	private void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if (base.networkView.isMine)
		{
			player_trans = PhoneInterface.player_trans;
			base.networkView.observed = this;
			myAnimation.enabled = false;
			myAnimation.gameObject.SetActiveRecursively(false);
			myAnimation.gameObject.active = true;
			hawk_obj.SetActiveRecursively(false);
			myAnimation = Networking.instance.player_anim;
			//string currentScreenName = TwitterDemo.instance.GetCurrentScreenName();
			fakeName = MonsterTraits.Name.createFullName();
			/*
			if (currentScreenName == "gamsfest")
			{
				currentScreenName = fakeName;
			}
			*/
			string currentUserId = TwitterDemo.instance.GetCurrentUserId();
			networkPlayer = Network.player;
			int[] array = ColToInt(PhoneInterface.trailColor);
			int[] array2 = ColToInt(PhoneInterface.robotColor);
			base.networkView.RPC("SetInfoColor", RPCMode.All, networkPlayer, fakeName, currentUserId, array[0], array[1], array[2], array2[0], array2[1], array2[2]);
			InvokeRepeating("CheckInfo", 5f + Random.value, 5f);
		}
		else
		{
			base.networkView.observed = this;
			player_trans = base.transform;
		}
	}

	private int[] ColToInt(Color col)
	{
		int[] array = new int[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = Mathf.FloorToInt(col[i] * 255f);
		}
		return array;
	}

	private void CheckInfo()
	{
		if (base.networkView.owner == Network.player && (Network.isClient || Network.isServer))
		{
			/*
			string currentScreenName = TwitterDemo.instance.GetCurrentScreenName();
			if (currentScreenName == "gamsfest")
			{
				currentScreenName = fakeName;
			}
			*/
			if (userName != fakeName || _trailColor != PhoneInterface.trailColor || _robotColor != PhoneInterface.robotColor)
			{
				string text = fakeName;
				string currentUserId = TwitterDemo.instance.GetCurrentUserId();
				networkPlayer = Network.player;
				int[] array = ColToInt(PhoneInterface.trailColor);
				int[] array2 = ColToInt(PhoneInterface.robotColor);
				base.networkView.RPC("SetInfoColor", RPCMode.All, networkPlayer, text, currentUserId, array[0], array[1], array[2], array2[0], array2[1], array2[2]);
			}
		}
	}

	private static bool CheckNaN(Vector3 vec)
	{
		for (int i = 0; i < 3; i++)
		{
			if (float.IsNaN(vec[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool CheckNaN(Quaternion rot)
	{
		for (int i = 0; i < 4; i++)
		{
			if (float.IsNaN(rot[i]))
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		if (base.networkView.isMine || !updatePosition || states[10] == null || Networking.batchMode)
		{
			return;
		}
		simulatePhysics = false;
		ping = Mathf.Lerp(ping, (float)(Network.time - states[0].t), Time.deltaTime * 2f);
		double num = Network.time - netInterp;
		if (states[0].t > num)
		{
			for (int i = 0; i < stateCount; i++)
			{
				if (states[i] != null && (states[i].t <= num || i == stateCount - 1))
				{
					State state = states[Mathf.Max(i - 1, 0)];
					State state2 = states[i];
					double num2 = state.t - state2.t;
					float num3 = 0f;
					if (num2 > 9.999999747378752E-05)
					{
						num3 = (float)((num - state2.t) / num2);
					}
					if ((num3 <= 0f || num3 > 1f) && Application.isEditor)
					{
						Debug.LogWarning("t not clamped... " + num3);
					}
					Vector3 vector = Vector3.Lerp(state2.p, state.p, num3);
					Quaternion quaternion = Quaternion.Slerp(state2.r, state.r, num3);
					if (CheckNaN(vector))
					{
						Debug.LogWarning("pos is NaN!");
						vector = player_trans.position;
					}
					if (CheckNaN(quaternion))
					{
						Debug.LogWarning("rot is NaN!");
						quaternion = player_trans.rotation;
					}
					player_trans.position = vector;
					player_trans.rotation = quaternion;
					velocity = (state.p - states[i + 1].p) / (float)(state.t - states[i + 1].t);
					if (num3 <= 0.5f)
					{
						isHawking = state2.h;
						CheckAnimation(state2.a);
					}
					else
					{
						isHawking = state.h;
						CheckAnimation(state.a);
					}
					CheckScreenState(state.s);
					if (Application.isEditor && doPrinting)
					{
						Debug.Log("i=" + i + ", t=" + num3 + ", l=" + num2);
					}
					isResponding = true;
					netStatus = string.Empty;
					break;
				}
			}
		}
		else
		{
			float num4 = (float)(num - states[0].t);
			if (num4 < 1f && states[0] != null && states[1] != null)
			{
				int num5 = 1;
				for (int j = 1; j < stateCount; j++)
				{
					if (states[j] != null && states[j].t < states[0].t)
					{
						num5 = j;
						break;
					}
				}
				float num6 = (float)(states[0].t - states[1].t);
				Vector3 vector2 = states[0].p - states[1].p;
				if (num6 <= 0f || num6 > 1f)
				{
					num6 = Mathf.Clamp(num6, 0.001f, 1f);
					vector2 = Vector3.zero;
				}
				else
				{
					vector2 /= num6;
				}
				Vector3 vector3 = states[0].p + vector2 * num4;
				if (CheckNaN(vector3))
				{
					Debug.LogWarning("pos is NaN!");
					vector3 = player_trans.position;
				}
				player_trans.position = vector3;
				Quaternion rotation = states[0].r;
				if (CheckNaN(rotation))
				{
					Debug.LogWarning("rot is NaN!");
					rotation = player_trans.rotation;
				}
				player_trans.rotation = rotation;
				isHawking = states[0].h;
				CheckAnimation(states[0].a);
				CheckScreenState(states[0].s);
				isResponding = true;
				if (num4 < 0.5f)
				{
					netStatus = " .";
				}
				else
				{
					netStatus = " ..";
				}
			}
			else
			{
				netStatus = " ...";
				isResponding = false;
			}
		}
		if (isHawking != hawk_obj.renderer.enabled)
		{
			hawk_obj.renderer.enabled = isHawking;
		}
		if ((bool)Networking.newCam && Networking.newCam.tempTarget == camTarget)
		{
			Networking.newCam.NormalMode(Time.deltaTime);
		}
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (base.networkView.isMine)
		{
			p = player_trans.position;
			r = player_trans.rotation;
			m = 0;
			h = (bool)PhoneInterface.hawk && PhoneInterface.hawk.active && PhoneInterface.hawk.targetHeld;
			a = GetAnimationChar();
			s = GetScreenStateChar();
			cur_screenstate = GetScreenState(s);
			stream.Serialize(ref p);
			stream.Serialize(ref r);
			stream.Serialize(ref m);
			stream.Serialize(ref h);
			stream.Serialize(ref a);
			stream.Serialize(ref s);
			return;
		}
		if (stream.isWriting)
		{
			if (stateCount != 0)
			{
				p = lastState.p;
				r = lastState.r;
				m = (int)((Network.time - lastState.t) * 1000.0);
				h = lastState.h;
				a = lastState.a;
				s = lastState.s;
				stream.Serialize(ref p);
				stream.Serialize(ref r);
				stream.Serialize(ref m);
				stream.Serialize(ref h);
				stream.Serialize(ref a);
				stream.Serialize(ref s);
			}
			return;
		}
		stream.Serialize(ref p);
		stream.Serialize(ref r);
		stream.Serialize(ref m);
		stream.Serialize(ref h);
		stream.Serialize(ref a);
		stream.Serialize(ref s);
		lastState = new State(p, r, info.timestamp - ((!((double)m > 0.0)) ? 0.0 : ((double)m / 1000.0)), h, a, s);
		if (stateCount == 0)
		{
			states[0] = lastState;
		}
		else if (lastState.t > states[0].t)
		{
			if (Application.isEditor && doPrinting)
			{
				MonoBehaviour.print("cool time " + lastState.t + " " + states[0].t);
			}
			for (int num = states.Length - 1; num > 0; num--)
			{
				states[num] = states[num - 1];
			}
			states[0] = lastState;
		}
		else
		{
			if (Application.isEditor && doPrinting)
			{
				MonoBehaviour.print("wow skippin " + lastState.t + " " + states[0].t);
			}
			int num2 = 1;
			for (num2 = 1; num2 < stateCount && (states[num2] == null || !(lastState.t >= states[num2].t)); num2++)
			{
			}
			if (num2 < stateCount)
			{
				for (int num3 = states.Length - 1; num3 > num2; num3--)
				{
					states[num3] = states[num3 - 1];
				}
				states[num2] = lastState;
				if (Application.isEditor && doPrinting)
				{
					MonoBehaviour.print("inserting at " + num2);
				}
			}
		}
		stateCount = Mathf.Min(stateCount + 1, states.Length);
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDisable()
	{
		Networking.RemoveNetPlayer(networkPlayer);
	}

	private void SetupAnimations()
	{
		if (anim_clip_list == null)
		{
			anim_clip_list = new string[myAnimation.GetClipCount()];
			int num = 0;
			foreach (AnimationState item in myAnimation)
			{
				anim_clip_list[num] = item.clip.name;
				anim_clip_dic.Add(item.clip.name, (char)num);
				if (anim_speed_dic.ContainsKey(item.name))
				{
					item.speed = anim_speed_dic[item.name];
				}
				else
				{
					item.speed = 1f;
				}
				num++;
			}
		}
		_lastchar = GetAnimationChar();
		if (myAnimation["PhoneArm"] != null)
		{
			AnimationState animationState2 = myAnimation["PhoneArm"];
			animationState2.AddMixingTransform(myAnimation.transform.Find("Joints_GRP/root/hips_upper/chest/R_h_shoulder"));
			animationState2.layer = 2;
			animationState2.blendMode = AnimationBlendMode.Blend;
			animationState2.wrapMode = WrapMode.Once;
			animationState2.enabled = true;
			animationState2.weight = 1f;
			animationState2.speed = 3f;
		}
	}

	private char GetAnimationChar()
	{
		string key = PhoneInterface.player_move.animName;
		if (base.networkView.owner == Network.player && SpawnPointScript.instance.isRespawning)
		{
			key = SpawnPointScript.instance.GetCurrentSpawnPoint().animationName;
		}
		if (anim_clip_dic.ContainsKey(key))
		{
			return anim_clip_dic[key];
		}
		return _lastchar;
	}

	private string GetAnimationClip(char indchar)
	{
		return anim_clip_list[(uint)indchar];
	}

	private void CheckAnimation(char anim_char)
	{
		string animationClip = GetAnimationClip(anim_char);
		if (_lastchar != anim_char)
		{
			myAnimation.CrossFade(animationClip);
			myAnimation[animationClip].time = 0f;
			myAnimation[animationClip].speed = PhoneInterface.player_move.model.animation[animationClip].speed;
		}
		if (animationClip == "Skate")
		{
			float z = player_trans.InverseTransformDirection(velocity).z;
			if (z > 1f)
			{
				myAnimation[animationClip].speed = Mathf.Clamp(z / 5f, 3f, 5f);
			}
			else
			{
				myAnimation[animationClip].speed = 0f;
			}
		}
		_lastchar = anim_char;
	}

	private void SetupScreenStates()
	{
		if (screen_state_dic == null)
		{
			screen_state_dic = new Dictionary<string, char>();
		}
		if (screen_state_dic.Count == 0)
		{
			for (int i = 0; i < screen_state_list.Length; i++)
			{
				screen_state_dic.Add(screen_state_list[i], (char)i);
			}
		}
	}

	private void ScreenOpen()
	{
		if (myAnimation["PhoneArm"] != null)
		{
			myAnimation["PhoneArm"].speed = Mathf.Abs(myAnimation["PhoneArm"].speed);
			myAnimation["PhoneArm"].wrapMode = WrapMode.ClampForever;
			myAnimation["PhoneArm"].weight = 0.5f;
			myAnimation["PhoneArm"].enabled = true;
			myAnimation.CrossFade("PhoneArm");
		}
		if (Application.isEditor)
		{
			Debug.Log("opening...");
		}
	}

	private void ScreenClose()
	{
		if (myAnimation["PhoneArm"] != null)
		{
			myAnimation["PhoneArm"].speed = 0f - Mathf.Abs(myAnimation["PhoneArm"].speed);
			myAnimation["PhoneArm"].wrapMode = WrapMode.Once;
			myAnimation["PhoneArm"].time = myAnimation["PhoneArm"].length;
			myAnimation.CrossFade("PhoneArm");
		}
		if (Application.isEditor)
		{
			Debug.Log("closing...");
		}
	}

	private void CheckScreenState(char c)
	{
		string screenState = GetScreenState(c);
		if (!(screenState != cur_screenstate))
		{
			return;
		}
		if (screenState == "closed")
		{
			ScreenClose();
		}
		else if (cur_screenstate == "closed")
		{
			ScreenOpen();
		}
		cur_screenstate = screenState;
		if (cur_screenstate == "na" || cur_screenstate == "closed")
		{
			screen_icon = null;
		}
		else if (PhoneController.instance.screendict.ContainsKey(cur_screenstate))
		{
			PhoneScreen phoneScreen = PhoneController.instance.screendict[cur_screenstate];
			if (phoneScreen.icon_texture != null)
			{
				screen_icon = phoneScreen.icon_texture;
			}
			else
			{
				screen_icon = null;
			}
		}
		if (Application.isEditor)
		{
			Debug.Log("switch screen... " + cur_screenstate);
		}
	}

	private char GetScreenStateChar()
	{
		string key = string.Empty;
		if ((bool)PhoneController.instance)
		{
			key = "closed";
			if (PhoneController.powerstate == PhoneController.PowerState.open && (bool)PhoneController.instance.curscreen)
			{
				key = PhoneController.instance.curscreen.screenname;
			}
		}
		if (screen_state_dic.ContainsKey(key))
		{
			return screen_state_dic[key];
		}
		return screen_state_dic["na"];
	}

	private string GetScreenState(char s)
	{
		if (s < screen_state_list.Length)
		{
			return screen_state_list[(uint)s];
		}
		return "na";
	}

	private void OnGUI()
	{
		if (!Networking.batchMode && !base.networkView.isMine && iconTex != null)
		{
			Texture2D image = iconTex;
			bool flag;
			int num2;
			if (screen_icon != null)
			{
				flag = Time.time % 1f < 0.5f;
			}
			else
				num2 = 0;
			float num = 48f;
			Vector3 vector = WorldToScreen(player_trans.position + player_trans.up * 3f);
			Rect rect = new Rect(vector.x, vector.y, (int)num, (int)num);
			if (vector.z > 0f)
			{
				rect.x -= rect.width / 2f;
				rect.y -= rect.height;
				rect.y -= 8f;
				rect = ClampToScreen(rect);
				GUI.DrawTexture(rect, image);
			}
		}
	}

	private Vector3 WorldToScreen(Vector3 worldpos)
	{
		Vector3 result = Camera.main.WorldToViewportPoint(worldpos);
		result.y = Mathf.Clamp01(result.y);
		result.x *= Screen.width;
		result.y = (1f - result.y) * (float)Screen.height;
		return result;
	}

	private Rect WorldToScreen(Vector3 worldpos, int width, int height)
	{
		Vector3 vector = WorldToScreen(worldpos);
		Rect result = new Rect(vector.x, vector.y, width, height);
		if (vector.z < 0f)
		{
			result.width *= -1f;
		}
		return result;
	}

	private Rect ClampToScreen(Rect rect)
	{
		rect.x = Mathf.Clamp(rect.x, 0f, (float)Screen.width - rect.width);
		rect.y = Mathf.Clamp(rect.y, 0f, (float)Screen.height - rect.height);
		return rect;
	}

	public void DoAddIcon(string tex)
	{
		Vector3 vector = player_trans.position + Vector3.up * 3f;
		base.networkView.RPC("AddIcon", RPCMode.All, networkPlayer, vector, tex);
	}

	[RPC]
	public void AddIcon(NetworkPlayer player, Vector3 position, string tex)
	{
		if (!Networking.batchMode)
		{
			Texture2D tex2 = Networking.chat_icon_dic[tex];
			NetIcon.AddNetIcon(player, position, tex2, Vector2.one * 64f);
		}
	}

	[RPC]
	public void SetInfoColor(NetworkPlayer player, string twitname, string twitid, int r, int g, int b, int r2, int g2, int b2)
	{
		SetTrailColor(r, g, b);
		SetRobotColor(r2, g2, b2);
		networkPlayer = player;
		userName = twitname;
		twitterId = twitid;
		if (!Networking.netplayer_dic.ContainsKey(networkPlayer))
		{
			Networking.AddNetPlayer(networkPlayer, this);
		}
	}

	public void SetTrailColor(int r, int g, int b)
	{
		Color color = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
		if (base.networkView.owner == Network.player)
		{
			_trailColor = color;
		}
		else if (trailColor != color)
		{
			trailColor = color;
		}
	}

	public void SetRobotColor(int r, int g, int b)
	{
		Color color = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
		if (base.networkView.owner == Network.player)
		{
			_robotColor = color;
		}
		else if (robotColor != color)
		{
			robotColor = color;
		}
	}

	public void DoChatMessage(string text)
	{
		text = string.Format("@{0}: {1}", userName, text);
		DoChatMessageRaw(text);
	}

	public void DoChatMessageRaw(string text)
	{
		base.networkView.RPC("ChatMessage", RPCMode.All, text);
	}

	[RPC]
	public void ChatMessage(string text)
	{
		Networking.AddChatMessage(text);
	}

	public void DoSetMood(int moodind)
	{
		base.networkView.RPC("SetMood", RPCMode.Others, moodind);
	}

	[RPC]
	public void SetMood(int moodind)
	{
		mood = moodind;
	}

	public void DoCallHawk()
	{
		base.networkView.RPC("CallHawk", RPCMode.All);
	}

	[RPC]
	public void CallHawk()
	{
		if ((bool)PhoneInterface.hawk)
		{
			PhoneInterface.SummonHawk();
		}
	}

	public void DoSetPizzaScore(int score)
	{
		pizzaScore = score;
		DoSetPizzaScore();
	}

	public void DoSetPizzaScore()
	{
		base.networkView.RPC("SetPizzaScore", RPCMode.Others, pizzaScore);
	}

	[RPC]
	public void SetPizzaScore(int score)
	{
		pizzaScore = score;
	}

	public void DoMakeTrainer()
	{
		PhoneMonster main_monster = PhoneMemory.main_monster;
		base.networkView.RPC("MakeTrainer", RPCMode.All, player_trans.position, player_trans.rotation, main_monster.monsterType.typeName, main_monster.bloodtype.typename, main_monster.attack, main_monster.defense, main_monster.magic, main_monster.glam, main_monster.scale.x, main_monster.scale.y, main_monster.speed, main_monster.bullet_speed, main_monster.bullet_cooldown, main_monster.bullet_homing);
	}

	[RPC]
	public void MakeTrainer(Vector3 pos, Quaternion rot, string type, string blood, float attack, float defense, float magic, float glam, float scale_x, float scale_y, float speed, float bullet_speed, float bullet_cooldown, float bullet_homing)
	{
		if (trainer == null)
		{
			trainer = Object.Instantiate(Networking.instance.trainer_prefab, pos, rot) as NPCTrainer;
		}
		else
		{
			trainer.transform.position = pos;
			trainer.transform.rotation = rot;
		}
		trainer.npc_name = userName;
		trainer.monster_first_name = userName;
		trainer.monster_last_name = string.Empty;
		trainer.rigidbody.freezeRotation = true;
		trainer.monsterTypeName = type;
		trainer.bloodtype = blood;
		trainer.attack = attack;
		trainer.defense = defense;
		trainer.magic = magic;
		trainer.glam = glam;
		trainer.scale = new Vector2(scale_x, scale_y);
		trainer.speed = speed;
		trainer.bullet_speed = bullet_speed;
		trainer.bullet_cooldown = bullet_cooldown;
		trainer.bullet_homing = bullet_homing;
		trainer.auto_gen_stats = false;
		trainer.InitMonster();
		trainer.waiting_icon = iconTex;
		trainer.battling_icon = iconTex;
		trainer.near_icon = iconTex;
		trainer.SetBubbleTexture(iconTex);
		trainer.can_challenge = true;
		trainer.GiveBadge();
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		int[] array = ColToInt(trailColor);
		int[] array2 = ColToInt(robotColor);
		base.networkView.RPC("SetInfoColor", player, networkPlayer, userName, twitterId, array[0], array[1], array[2], array2[0], array2[1], array2[2]);
		base.networkView.RPC("SetPizzaScore", player, pizzaScore);
		base.networkView.RPC("SetMood", player, Mathf.RoundToInt(mood));
		if ((bool)trainer)
		{
			base.networkView.RPC("MakeTrainer", player, trainer.transform.position, trainer.transform.rotation, trainer.monsterTypeName, trainer.bloodtype, trainer.attack, trainer.defense, trainer.magic, trainer.magic, trainer.scale.x, trainer.scale.y, trainer.speed, trainer.bullet_speed, trainer.bullet_cooldown, trainer.bullet_cooldown);
		}
		if (base.networkView.isMine)
		{
			if (!string.IsNullOrEmpty(Networking.bundlefile) && Application.loadedLevelName == "test")
			{
				base.networkView.RPC("LoadBundleCommand", player, Networking.bundlefile);
			}
			else
			{
				base.networkView.RPC("LoadSceneCommand", player, Application.loadedLevelName);
			}
		}
	}

	public void DoSceneCommand(string nam)
	{
		if (base.networkView.isMine && Network.isServer)
		{
			base.networkView.RPC("LoadSceneCommand", RPCMode.All, nam);
		}
	}

	[RPC]
	public void LoadSceneCommand(string nam)
	{
		if (Application.loadedLevelName != nam)
		{
			Application.LoadLevel(nam);
		}
	}

	public void DoLoadBundle(string nam)
	{
		if (base.networkView.isMine && Network.isServer)
		{
			base.networkView.RPC("LoadBundleCommand", RPCMode.All, nam);
		}
	}

	[RPC]
	public void LoadBundleCommand(string nam)
	{
		Networking.bundlefile = nam;
		if (Application.loadedLevelName != "test")
		{
			Application.LoadLevel("test");
		}
		if (Network.isClient && !nam.StartsWith("http") && !SpawnPointScript.HasBundle(nam))
		{
			Debug.Log("i need to get this bundle... " + nam);
			base.networkView.RPC("RequestBundleBytes", RPCMode.Others, Network.player, nam);
		}
		else
		{
			SpawnPointScript.instance.LoadAndSpawn(nam);
		}
	}

	[RPC]
	public void SendBundleBytes(string nam, byte[] data)
	{
		Debug.Log("i am getting some bundle bytes... " + nam + ", " + data.Length);
		if (nam != Networking.bundlefile)
		{
			Debug.LogWarning("i was expecting " + Networking.bundlefile + " but got " + nam + "...");
		}
		else
		{
			SpawnPointScript.instance.LoadAndSpawn(nam, data);
		}
	}

	[RPC]
	public void RequestBundleBytes(NetworkPlayer player, string nam)
	{
		if (Network.isServer)
		{
			Debug.Log("I am going to send some bundle bytes...");
			StartCoroutine(Co_RequestBundleBytes(player, nam));
		}
	}

	private IEnumerator Co_RequestBundleBytes(NetworkPlayer player, string nam)
	{
		while (!SpawnPointScript.instance.loaded)
		{
			yield return null;
		}
		if (nam != Networking.bundlefile)
		{
			Debug.LogWarning("the client wants " + nam + " but my current bundle is " + Networking.bundlefile + "... sending mine anyway");
		}
		Debug.Log("i am sending some bundle bytes..." + SpawnPointScript.instance.curBundleBytes.Length);
		base.networkView.RPC("SendBundleBytes", player, Networking.bundlefile, SpawnPointScript.instance.curBundleBytes);
	}
}
