using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Networking : MonoBehaviour
{
	public string ip_address = "localhost";

	public string port = "7777";

	private string server_name = "24/7 Freeze Tag";

	public string password = string.Empty;

	public string max_players = "12";

	private bool use_NAT = true;

	private static readonly string mastergameid = "xNewJSROnlineXDDXXxX";

	public Animation player_anim;

	public Transform other_tran;

	public AudioClip _piz_clip;

	public NetPlayer host_net;

	public NetPlayer client_net;

	private static Networking _instance;

	public NetPlayer player_prefab;

	public NPCTrainer trainer_prefab;

	public static Dictionary<NetworkPlayer, NetPlayer> netplayer_dic = new Dictionary<NetworkPlayer, NetPlayer>();

	private Transform camTarget;

	private static NewCamera _newCam;

	public static bool batchMode = false;

	public bool auto;

	private string connectTestMessage = string.Empty;

	private ConnectionTesterStatus connectTest = ConnectionTesterStatus.Undetermined;

	private bool showgui = true;

	private bool dpad_ready = true;

	private HostData[] serverdats = new HostData[0];

	private bool iswatching;

	private bool show_player_list = true;

	private Rect player_list_rect = new Rect(4f, 80f, 32f, 32f);

	private Vector2 player_list_scroll = Vector2.zero;

	public Texture2D[] moodIcons = new Texture2D[0];

	private List<NetPlayer> gui_expanded_list = new List<NetPlayer>();

	public static Dictionary<string, Texture2D> chat_icon_dic = new Dictionary<string, Texture2D>();

	public Texture2D[] chat_icons = new Texture2D[0];

	private Rect toggrect = new Rect(2f, 0f, 16f, 16f);

	private Rect normal_chat_rect = new Rect(0f, 300f, 360f, 240f);

	private Rect small_chat_rect = new Rect(0f, 300f, 80f, 48f);

	private Rect chat_log_rect = new Rect(200f, 80f, 360f, 240f);

	public GameObject critter_prefab;

	private bool show_icon_list = true;

	private Rect icon_list_rect = new Rect(200f, 8f, 360f, 48f);

	private Rect normal_icon_rect = new Rect(0f, 220f, 404f, 60f);

	private static float new_message_timer = 0f;

	private bool draw_chat_log = true;

	private float maxwidth = 360f;

	private string chat_input = string.Empty;

	private static Vector2 chat_scroll = Vector2.zero;

	private bool drawbundles;

	private string bundleURL = string.Empty;

	private bool drawscenes;

	public static string bundlefile = string.Empty;

	private string[] scenelist = new string[1] { "test" };

	private bool drawsettings;

	private List<string> mute_list = new List<string>();

	private static int max_chat_log_size = 48;

	public static List<string> chat_log = new List<string>();

	public Transform player_tran
	{
		get
		{
			return PhoneInterface.player_trans;
		}
	}

	public static AudioClip piz_clip
	{
		get
		{
			return instance._piz_clip;
		}
	}

	public static NetPlayer my_net_player
	{
		get
		{
			if (Network.isClient)
			{
				return instance.client_net;
			}
			if (Network.isServer)
			{
				return instance.host_net;
			}
			return null;
		}
	}

	public static Networking instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(Networking)) as Networking;
			}
			return _instance;
		}
	}

	public static NewCamera newCam
	{
		get
		{
			if (!_newCam)
			{
				_newCam = UnityEngine.Object.FindObjectOfType(typeof(NewCamera)) as NewCamera;
			}
			return _newCam;
		}
	}

	private void Awake()
	{
		if (instance != this)
		{
			if (!string.IsNullOrEmpty(bundlefile))
			{
				Debug.Log("here...");
				if (my_net_player != null)
				{
					my_net_player.LoadBundleCommand(bundlefile);
				}
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Start()
	{
		LoadLastServer();
		GetServerList();
		if (chat_icon_dic.Count == 0)
		{
			Texture2D[] array = chat_icons;
			foreach (Texture2D texture2D in array)
			{
				chat_icon_dic.Add(texture2D.name, texture2D);
			}
		}
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs.Length > 1)
		{
			string[] array2 = commandLineArgs;
			foreach (string text in array2)
			{
				if (text == "-batchmode")
				{
					batchMode = true;
				}
				else if (text.StartsWith("pass="))
				{
					password = text.Replace("pass=", string.Empty);
				}
				else if (text.StartsWith("port="))
				{
					port = text.Replace("port=", string.Empty);
				}
				else if (text.StartsWith("limit="))
				{
					max_players = text.Replace("limit=", string.Empty);
				}
			}
		}
		StartCoroutine(TestConnection());
		if (batchMode)
		{
			if (Application.isEditor)
			{
				Debug.Log("batch mode...");
			}
			Invoke("StartServer", 0.5f);
			Debug.Log("starting server in .5s ...");
		}
		else if (auto)
		{
			InvokeRepeating("AutoServer", 1f + UnityEngine.Random.value, 5f);
		}
	}

	private void AutoServer()
	{
		if (!Network.isClient && !Network.isServer)
		{
			serverdats = MasterServer.PollHostList();
			if (serverdats.Length > 0)
			{
				Network.Connect(serverdats[0], password);
			}
			else
			{
				StartServer();
			}
		}
	}

	private void LoadLastServer()
	{
		string @string = PlayerPrefs.GetString("prev_server_ip", string.Empty);
		if (!string.IsNullOrEmpty(@string))
		{
			ip_address = @string;
		}
		string string2 = PlayerPrefs.GetString("prev_server_port", string.Empty);
		if (!string.IsNullOrEmpty(string2))
		{
			port = string2;
		}
		string string3 = PlayerPrefs.GetString("prev_server_name", string.Empty);
		if (!string.IsNullOrEmpty(string3))
		{
			server_name = string3;
		}
	}

	private void SaveCurrentServer()
	{
		PlayerPrefs.SetString("prev_server_ip", ip_address);
		PlayerPrefs.SetString("prev_server_port", port);
		PlayerPrefs.SetString("prev_server_name", server_name);
	}

	private IEnumerator TestConnection()
	{
		while (ConnectionTestLoop())
		{
			yield return null;
		}
	}

	private bool ConnectionTestLoop()
	{
		switch (connectTest)
		{
		case ConnectionTesterStatus.Undetermined:
			connectTest = Network.TestConnection();
			connectTestMessage = "testing network...";
			return true;
		case ConnectionTesterStatus.Error:
			connectTestMessage = "error...!";
			break;
		case ConnectionTesterStatus.PublicIPIsConnectable:
			connectTestMessage = "directly connectable!";
			use_NAT = false;
			break;
		case ConnectionTesterStatus.PublicIPPortBlocked:
			connectTestMessage = "NAT testing...";
			connectTest = Network.TestConnectionNAT();
			return true;
		case ConnectionTesterStatus.PublicIPNoServerStarted:
			connectTestMessage = "public ip, but no server!";
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
			connectTestMessage = "limited NAT, port restricted!";
			use_NAT = true;
			break;
		case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
			connectTestMessage = "limited NAT, don't run a server!";
			use_NAT = true;
			break;
		case ConnectionTesterStatus.NATpunchthroughFullCone:
		case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
			connectTestMessage = "great NAT!";
			use_NAT = true;
			break;
		default:
			connectTestMessage = "? " + connectTest;
			break;
		}
		return false;
	}

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		showgui = GUILayout.Toggle(showgui, "gui (net test)");
		if (showgui)
		{
			if (!Network.isClient && !Network.isServer)
			{
				drawsettings = GUILayout.Toggle(drawsettings, "NAT");
			}
			GUILayout.EndHorizontal();
			if (Network.isServer)
			{
				ServerGUI();
			}
			else if (Network.isClient)
			{
				ClientGUI();
			}
			else
			{
				NoneGUI();
			}
		}
	}

	private void OnConnectedToServer()
	{
		if (Application.isEditor)
		{
			Debug.Log("connected to server...");
		}
		Playtomic.Log.CustomMetric("has_joined_server", PlaytomicController.current_group, true);
		client_net = Network.Instantiate(player_prefab, player_tran.position, player_tran.rotation, 0) as NetPlayer;
		client_net.name = "client obj";
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("Disconnected from server: " + info);
		if ((bool)client_net)
		{
			UnityEngine.Object.Destroy(client_net.gameObject);
		}
		CleanUpNetObjs();
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		if (Application.isEditor)
		{
			Debug.Log("Player disconnected");
		}
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		if (netplayer_dic.ContainsKey(player))
		{
			netplayer_dic.Remove(player);
		}
	}

	public void Disconnect()
	{
		if (Network.isServer)
		{
			NetworkPlayer[] connections = Network.connections;
			foreach (NetworkPlayer networkPlayer in connections)
			{
				if (Application.isEditor)
				{
					Debug.Log("cleaning up after " + networkPlayer);
				}
				Network.RemoveRPCs(networkPlayer);
				Network.DestroyPlayerObjects(networkPlayer);
			}
			if (Application.isEditor)
			{
				Debug.Log("cleaing up after host :)");
			}
			Network.RemoveRPCs(Network.player);
			Network.DestroyPlayerObjects(Network.player);
			Network.Disconnect();
		}
		else if (Network.isClient)
		{
			Network.Disconnect();
		}
		CleanUpNetObjs();
	}

	private void CleanUpNetObjs()
	{
		netplayer_dic.Clear();
		gui_expanded_list.Clear();
		chat_log.Clear();
		if ((bool)newCam && newCam.tempTarget != null)
		{
			newCam.tempTarget = null;
		}
	}

	private string MakeServerComment()
	{
		return MakeServerComment(Application.loadedLevelName, bundlefile);
	}

	private string MakeServerComment(string scenename, string bundnam)
	{
		string arg = PhoneInterface.version;
		switch (scenename)
		{
		case "Loader 1":
			arg = "Normal";
			break;
		case "Loader 5":
			arg = "Tutorial";
			break;
		case "test":
			arg = "Custom";
			if (!string.IsNullOrEmpty(bundlefile))
			{
				string text = bundnam;
				if (text.StartsWith("http://") || text.StartsWith("https://"))
				{
					text = text.Substring(text.LastIndexOf("/") + 1);
				}
				arg = text;
			}
			break;
		}
		if (my_net_player != null && my_net_player.userName != null)
		{
			return string.Format("{0} ({1})", arg, my_net_player.userName);
		}
		return string.Format("{0} ({1})", arg, TwitterDemo.instance.GetCurrentScreenName());
	}

	private void StartServer()
	{
		string comment = MakeServerComment();
		Playtomic.Log.CustomMetric("has_started_server", PlaytomicController.current_group, true);
		Network.incomingPassword = password;
		int result;
		if (!int.TryParse(port, out result))
		{
			Debug.LogWarning("could not parse port '" + port + "' as int");
			result = 7777;
			port = result.ToString();
		}
		int result2;
		if (!int.TryParse(max_players, out result2))
		{
			Debug.LogWarning("could not parse max players '" + max_players + "' as int");
			result2 = 8;
			max_players = result2.ToString();
		}
		if (!batchMode)
		{
			result2--;
		}
		if (result2 < 0)
		{
			result2 = 0;
			max_players = 1.ToString();
		}
		Network.InitializeServer(result2, result, use_NAT);
		MasterServer.RegisterHost(mastergameid, server_name, comment);
		if (host_net == null && !batchMode)
		{
			host_net = Network.Instantiate(player_prefab, player_tran.position, player_tran.rotation, 0) as NetPlayer;
			host_net.name = "host obj";
		}
	}

	private void GetServerList()
	{
		MasterServer.RequestHostList(mastergameid);
	}

	private void Update()
	{
		serverdats = MasterServer.PollHostList();
		if (new_message_timer > 0f)
		{
			new_message_timer -= Time.deltaTime;
		}
		if (!batchMode && (Network.isClient || Network.isServer))
		{
			int num = -1;
			Vector2 vector = new Vector2(Input.GetAxisRaw("EffectSwitch"), Input.GetAxisRaw("SpawnDebug"));
			if (vector.magnitude == 0f)
			{
				dpad_ready = true;
			}
			else if (dpad_ready)
			{
				num = ((vector.x > 0f) ? ((vector.y > 0f) ? 1 : ((vector.y < 0f) ? 7 : 0)) : ((vector.x < 0f) ? ((vector.y > 0f) ? 3 : ((!(vector.y < 0f)) ? 4 : 5)) : ((!(vector.y > 0f)) ? 6 : 2)));
			}
			if (num != -1)
			{
				my_net_player.DoAddIcon(chat_icons[num].name);
				dpad_ready = false;
			}
		}
	}

	private void NoneGUI()
	{
		DrawSettings();
		DrawBundleList();
		DrawSceneList();
		GUILayout.BeginVertical("Box");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Name");
		server_name = GUILayout.TextField(server_name);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("ip");
		ip_address = GUILayout.TextField(ip_address);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("port");
		port = GUILayout.TextField(port);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("max players");
		max_players = GUILayout.TextField(max_players);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("password");
		password = GUILayout.TextField(password);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Join server..."))
		{
			int result;
			if (!int.TryParse(port, out result))
			{
				Debug.LogWarning("could not parse port '" + port + "' as int");
				result = 7777;
				port = result.ToString();
			}
			SaveCurrentServer();
			Network.Connect(ip_address, result, password);
		}
		if (GUILayout.Button("Start server..."))
		{
			StartServer();
		}
		GUILayout.EndHorizontal();
		if (GUILayout.Button("refresh server list"))
		{
			GetServerList();
		}
		DrawServerList();
		GUILayout.EndVertical();
	}

	private void DrawServerList()
	{
		HostData[] array = serverdats;
		foreach (HostData hostData in array)
		{
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("{0} ({1}/{2})", hostData.gameName, hostData.connectedPlayers, hostData.playerLimit));
			GUILayout.Space(5f);
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Connect"))
			{
				Network.Connect(hostData, password);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label(hostData.comment);
			if (hostData.useNat)
			{
				Color color = GUI.color;
				GUI.color = Color.yellow;
				GUILayout.Label("NAT");
				GUI.color = color;
			}
			if (hostData.passwordProtected)
			{
				Color color2 = GUI.color;
				if (string.IsNullOrEmpty(password))
				{
					GUI.color = Color.red;
				}
				GUILayout.Label("Password");
				GUI.color = color2;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}

	private void ServerGUI()
	{
		DrawBundleList();
		DrawSceneList();
		GUILayout.Box(string.Format("{0} ({1})", server_name, Network.player.ipAddress));
		DrawChatGUI();
		DrawPlayerList();
		if (GUILayout.Button("Disconnect"))
		{
			Disconnect();
		}
	}

	private void ClientGUI()
	{
		DrawChatGUI();
		DrawPlayerList();
		if (GUILayout.Button("Disconnect"))
		{
			Disconnect();
		}
	}

	private void DrawPlayerList()
	{
		string text = "Pals";
		if (show_player_list)
		{
			text += " ";
		}
		text = ((!Network.isServer) ? (text + string.Format("[{0}]", netplayer_dic.Count.ToString())) : (text + string.Format("[{0}/{1}]", (Network.connections.Length + 1).ToString(), (Network.maxConnections + 1).ToString())));
		Rect screenRect = player_list_rect;
		if (!show_player_list)
		{
			screenRect.width = small_chat_rect.width;
			screenRect.height = small_chat_rect.height;
			screenRect = GUILayout.Window(3, screenRect, DrawPlayerListWindow, text, GUILayout.ExpandWidth(false));
			player_list_rect.x = screenRect.x;
			player_list_rect.y = screenRect.y;
			if (player_list_rect.width != 32f)
			{
				player_list_rect.width = 32f;
				player_list_rect.height = 32f;
			}
		}
		else
		{
			player_list_rect = GUILayout.Window(3, player_list_rect, DrawPlayerListWindow, text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}
	}

	private void DrawPlayerListWindow(int id)
	{
		show_player_list = GUILayout.Toggle(show_player_list, "show");
		if (show_player_list)
		{
			if ((bool)newCam && newCam.tempTarget != null && GUILayout.Button("Stop looking"))
			{
				newCam.tempTarget = null;
			}
			foreach (NetworkPlayer key in netplayer_dic.Keys)
			{
				DrawPlayer(netplayer_dic[key]);
			}
		}
		GUI.DragWindow();
	}

	public Texture2D GetMoodIcon(float index)
	{
		return GetMoodIcon(Mathf.RoundToInt(index));
	}

	public Texture2D GetMoodIcon(int index)
	{
		index = Mathf.Clamp(index, 0, moodIcons.Length - 1);
		return moodIcons[index];
	}

	private void DrawPlayer(NetPlayer player)
	{
		GUILayout.BeginVertical("Box");
		string userName = player.userName;
		userName += player.netStatus;
		int num = Mathf.RoundToInt(player.ping * 1000f);
		if (num <= 0)
		{
			num = -1;
		}
		GUIContent content = new GUIContent(userName, player.iconTex);
		GUILayout.BeginHorizontal();
		bool flag = GUILayout.Button(content, GUILayout.Height(48f));
		GUILayout.FlexibleSpace();
		GUILayout.Label(player.pizzaScore.ToString());
		if (Network.player == player.networkPlayer)
		{
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			float num2 = GUILayout.HorizontalSlider(player.mood, 0f, moodIcons.Length - 1, GUILayout.Width(64f));
			if (num2 != player.mood && Mathf.RoundToInt(num2) != Mathf.RoundToInt(player.mood))
			{
				player.DoSetMood(Mathf.RoundToInt(num2));
			}
			player.mood = num2;
			GUIContent content2 = new GUIContent("mood", GetMoodIcon(player.mood));
			GUILayout.Label(content2);
		}
		else
		{
			GUILayout.Label(GetMoodIcon(player.mood));
		}
		GUILayout.EndHorizontal();
		if (gui_expanded_list.Contains(player))
		{
			if (flag)
			{
				gui_expanded_list.Remove(player);
			}
			GUILayout.BeginVertical("Box");
			if (Network.player != player.networkPlayer && (bool)newCam && (bool)player.camTarget)
			{
				bool flag2 = newCam.tempTarget == player.camTarget;
				bool flag3 = GUILayout.Toggle(flag2, "look");
				if (!flag2 && flag3)
				{
					newCam.tempTarget = player.camTarget;
					iswatching = true;
				}
				else if (flag2 && !flag3)
				{
					newCam.tempTarget = null;
					iswatching = false;
				}
			}
			if (Network.player == player.networkPlayer && GUILayout.Button("Respawn..."))
			{
				SpawnPointScript.instance.SpawnPlayerAtStart();
			}
			if (num != -1)
			{
				GUILayout.Label("ping: " + num);
			}
			if (!string.IsNullOrEmpty(player.twitterId) && GUILayout.Button("Twitter Page"))
			{
				Application.OpenURL(string.Format("https://twitter.com/account/redirect_by_id?id={0}", player.twitterId));
			}
			if (IsDev() && player.networkPlayer != Network.player && GUILayout.Button("Warp->>>"))
			{
				if ((bool)PhoneInterface.hawk && PhoneInterface.hawk.targetHeld)
				{
					PhoneInterface.hawk.Drop();
				}
				PhoneInterface.player_trans.transform.position = player.transform.position;
				PhoneInterface.player_trans.transform.rotation = player.transform.rotation;
			}
			if (player.networkView != null && Network.isServer && Network.player != player.networkPlayer && GUILayout.Button("Kick"))
			{
				Network.CloseConnection(player.networkPlayer, true);
			}
			if (!batchMode && player.networkPlayer == Network.player)
			{
				if (IsDev())
				{
					if ((bool)PhoneInterface.hawk && !PhoneInterface.hawk.canControl && GUILayout.Button("bird powers"))
					{
						PhoneInterface.hawk.canControl = true;
					}
					if (!PhoneInterface.player_move.canDebugBoost && GUILayout.Button("\"it's boost time\""))
					{
						PhoneInterface.player_move.canDebugBoost = true;
					}
				}
				if (IsDev() && GUILayout.Button("ghost trainer"))
				{
					player.DoMakeTrainer();
				}
			}
			GUILayout.EndVertical();
		}
		else if (flag)
		{
			gui_expanded_list.Add(player);
		}
		GUILayout.EndVertical();
	}

	private void DrawChatGUI()
	{
		if (draw_chat_log)
		{
			chat_log_rect.width = normal_chat_rect.width;
			chat_log_rect.height = normal_chat_rect.height;
		}
		else
		{
			chat_log_rect.width = small_chat_rect.width;
			chat_log_rect.height = small_chat_rect.height;
		}
		Color backgroundColor = GUI.backgroundColor;
		if (new_message_timer > 0f)
		{
			GUI.backgroundColor = Color.Lerp(backgroundColor, Color.yellow, new_message_timer);
		}
		chat_log_rect = GUI.Window(0, chat_log_rect, DrawChatLogWindow, "Talk");
		GUI.backgroundColor = backgroundColor;
		if (show_icon_list)
		{
			icon_list_rect.width = normal_icon_rect.width;
			icon_list_rect.height = normal_icon_rect.height;
		}
		else
		{
			icon_list_rect.width = small_chat_rect.width;
			icon_list_rect.height = small_chat_rect.height;
		}
		icon_list_rect = GUI.Window(1, icon_list_rect, DrawChatIconList, "Emote");
	}

	private void DrawChatIconList(int id)
	{
		show_icon_list = GUI.Toggle(toggrect, show_icon_list, string.Empty);
		if (!show_icon_list)
		{
			GUI.DragWindow();
			return;
		}
		float num = 32f;
		GUILayout.BeginHorizontal();
		foreach (string key in chat_icon_dic.Keys)
		{
			if (GUILayout.Button(chat_icon_dic[key], GUILayout.Height(num), GUILayout.Width(num)) && (bool)my_net_player)
			{
				my_net_player.DoAddIcon(key);
			}
		}
		if (!batchMode && IsDev())
		{
			GUILayout.BeginVertical();
			if ((bool)critter_prefab && GUILayout.Button("C"))
			{
				Network.Instantiate(critter_prefab, player_tran.position + player_tran.up, player_tran.rotation, 1);
				SendChatMessageRaw(string.Format("A Critter was unleashed by {0}!", my_net_player.userName));
			}
			if (Application.loadedLevelName != "Loader 5" && GUILayout.Button("H"))
			{
				my_net_player.DoCallHawk();
				SendChatMessageRaw(string.Format("{0} summoned some hawks!", my_net_player.userName));
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		GUI.DragWindow();
	}

	private void DrawChatLogWindow(int id)
	{
		draw_chat_log = GUI.Toggle(toggrect, draw_chat_log, string.Empty);
		if (!draw_chat_log)
		{
			GUI.DragWindow();
			return;
		}
		GUI.SetNextControlName("chatlog");
		chat_scroll = GUILayout.BeginScrollView(chat_scroll);
		foreach (string item in chat_log)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(item);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		if (Event.current.type == EventType.KeyDown && Event.current.character == '\n' && !string.IsNullOrEmpty(chat_input))
		{
			if ((bool)my_net_player)
			{
				my_net_player.DoChatMessage(chat_input);
			}
			chat_input = string.Empty;
		}
		chat_input = GUILayout.TextField(chat_input);
		GUI.DragWindow();
	}

	private void DrawBundleList()
	{
		if (Application.loadedLevelName != "test")
		{
			return;
		}
		GUILayout.BeginVertical("Box");
		bool flag = drawbundles;
		drawbundles = GUILayout.Toggle(drawbundles, "bundles");
		if (!flag && drawbundles)
		{
			SpawnPointScript.GetBundles();
		}
		if (!drawbundles)
		{
			GUILayout.EndVertical();
			return;
		}
		if (SpawnPointScript.bundlenames != null)
		{
			string[] bundlenames = SpawnPointScript.bundlenames;
			foreach (string path in bundlenames)
			{
				string fileName = Path.GetFileName(path);
				if (GUILayout.Button(fileName))
				{
					if ((bool)my_net_player)
					{
						my_net_player.DoLoadBundle(fileName);
					}
					else
					{
						SpawnPointScript.instance.LoadAndSpawn(fileName);
						bundlefile = fileName;
					}
					if (Network.isServer)
					{
						MasterServer.UnregisterHost();
						MasterServer.RegisterHost(mastergameid, server_name, MakeServerComment(Application.loadedLevelName, fileName));
					}
				}
			}
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("URL"))
		{
			if (!bundleURL.StartsWith("http"))
			{
				bundleURL = "http://" + bundleURL;
			}
			if (bundleURL != "http://")
			{
				bundlefile = bundleURL;
				if ((bool)my_net_player)
				{
					my_net_player.DoLoadBundle(bundleURL);
				}
				else
				{
					SpawnPointScript.instance.LoadAndSpawn(bundleURL);
				}
				if (Network.isServer)
				{
					MasterServer.UnregisterHost();
					MasterServer.RegisterHost(mastergameid, server_name, MakeServerComment(Application.loadedLevelName, bundleURL));
				}
			}
		}
		bundleURL = GUILayout.TextField(bundleURL);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	private void DrawSceneList()
	{
		if (Application.loadedLevelName == "test")
		{
			return;
		}
		GUILayout.BeginVertical("Box");
		drawscenes = GUILayout.Toggle(drawscenes, "scenes");
		if (!drawscenes)
		{
			GUILayout.EndVertical();
			return;
		}
		string loadedLevelName = Application.loadedLevelName;
		string[] array = scenelist;
		foreach (string text in array)
		{
			if (text != loadedLevelName && GUILayout.Button(text))
			{
				if (Network.isServer)
				{
					MasterServer.UnregisterHost();
					MasterServer.RegisterHost(mastergameid, server_name, MakeServerComment(text, bundlefile));
				}
				if ((bool)my_net_player)
				{
					my_net_player.DoSceneCommand(text);
				}
				else
				{
					Application.LoadLevel(text);
				}
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawSettings()
	{
		if (drawsettings)
		{
			GUILayout.BeginVertical("Box");
			if (!string.IsNullOrEmpty(connectTestMessage))
			{
				GUILayout.Label(connectTestMessage);
			}
			use_NAT = GUILayout.Toggle(use_NAT, "Use NAT? (don't touch!)");
			GUILayout.EndVertical();
		}
	}

	public static void AddChatMessage(string message)
	{
		new_message_timer = 1f;
		chat_log.Add(message);
		if (chat_log.Count > max_chat_log_size)
		{
			chat_log.RemoveAt(0);
		}
		chat_scroll.y = float.PositiveInfinity;
	}

	public void SendChatMessageRaw(string message)
	{
		if ((bool)my_net_player)
		{
			my_net_player.DoChatMessageRaw(message);
		}
	}

	public void SendChatMessage(string message)
	{
		if ((bool)my_net_player)
		{
			my_net_player.DoChatMessage(message);
		}
	}

	public static void AddNetPlayer(NetworkPlayer networkplayer, NetPlayer netplayer)
	{
		if (!netplayer_dic.ContainsKey(networkplayer))
		{
			netplayer_dic.Add(networkplayer, netplayer);
		}
		else
		{
			netplayer_dic[networkplayer] = netplayer;
		}
	}

	public static void RemoveNetPlayer(NetworkPlayer networkplayer)
	{
		if (netplayer_dic.ContainsKey(networkplayer))
		{
			netplayer_dic.Remove(networkplayer);
		}
	}

	public static bool IsDev()
	{
		if (!TwitterDemo.instance._isConnected || !TwitterDemo.instance._isCustom)
		{
			return false;
		}
		string currentUserId = TwitterDemo.instance.GetCurrentUserId();
		int result;
		switch (currentUserId)
		{
		default:
			result = ((currentUserId == "177965708") ? 1 : 0);
			break;
		case "280379781":
		case "293352325":
		case "751234076":
		case "293795267":
		case "272431331":
			result = 1;
			break;
		}
		return (byte)result != 0;
	}
}
