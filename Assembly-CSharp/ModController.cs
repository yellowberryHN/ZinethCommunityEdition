using System.Runtime.InteropServices;
using UnityEngine;

public class ModController : MonoBehaviour
{
    public static ModController instance;
    private System.IntPtr windowHandle;
    
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);
    
    [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
    public static extern bool SetWindowTextW(System.IntPtr hWnd, string lpString);

    public static Discord.Discord discord;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Mod controller initialized.");
        UpdateTitle();
        if (PlayerPrefsX.GetBool("discord_rpc", true)) // discord rich presence enableda
        {
            try
            {
                discord = new Discord.Discord(1233330405092888660, (ulong)Discord.CreateFlags.NoRequireDiscord);
                InvokeRepeating("UpdateDiscord", 5f, 5f);
            }
            catch (Discord.ResultException)
            {
                Debug.LogWarning("Failed to initialize Discord integration, might not be running.");
                discord = null;
                CancelInvoke("UpdateDiscord");
            }
        }
    }
    
    // TODO: I really quite dislike this solution, find a better way to do this
    private void UpdateTitle()
    {
        try
        {
            if (windowHandle == System.IntPtr.Zero)
            {
                windowHandle = FindWindow(null, "Zineth");
            }

            SetWindowTextW(windowHandle, "Zineth Community Edition");
        }
        catch(System.EntryPointNotFoundException)
        {
            Debug.LogWarning("Failed to update window title, probably not running on Windows.");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            if (Application.loadedLevelName != "Loader 3")
            {
                Networking.instance.enabled = false;
                Application.LoadLevel("Loader 3"); 
            }
        }
        else if (SpeedrunTimer.instance != null)
        {
            if (Input.GetKey(KeyCode.F5))
            {
                SpeedrunTimer.instance.StopTimer();
            }
            else if (Input.GetKey(KeyCode.F8))
            {
                PhoneInterface.ClearGameData();
                PhoneLoaderMenu.CleanUp();
                Application.LoadLevel("Loader 1");
            }
        }

        if (discord != null)
        {
            try
            {
                discord.RunCallbacks();
            }
            catch (Discord.ResultException)
            {
                Debug.LogWarning("Failed to run Discord callbacks, disabling...");
                discord = null;
                CancelInvoke("UpdateDiscord");
            }
        }
    }

    public void UpdateDiscord()
    {
        var details = string.Empty;
        var state = string.Empty;
        
        details = "Main Menu";
        switch (Application.loadedLevelName)
        {
            case "test": // Custom Map
            case "Loader 1": // Game
                details = "Skating";
                MissionObject mission = MissionController.focus_mission;
                if (mission != null)
                {
                    details = "On Mission: " + mission.title;
                }
                
                if (SpeedrunTimer.instance.enabled || SpeedrunTimer.instance.finalTime != null)
                {
                    state = "Speedrunning";
                    if (SpeedrunTimer.instance.finalTime != null)
                    {
                        state += " (⏱ " + SpeedrunTimer.instance.finalTime + ")";
                    }
                }
                else
                {
                    state = "Playing alone";
                }
                
                break;
            case "Loader 5": // Tutorial
                details = "Tutorial";
                break;
            default:
                details = "Main Menu";
                break;
        }

        var activity = new Discord.Activity
        {
            Details = details,
            State = state,
            Assets =
            {
                LargeImage = "zineth-ce",
                LargeText = "Zineth CE"
            }
        };

        if (Application.loadedLevelName == "test")
        {
            activity.Details += " (Custom Map)";
        }
        if (Networking.instance != null && Networking.instance.enabled)
        {
            activity.State = "Playing together";
            if (Network.isServer)
            {
                activity.Party.Size.CurrentSize = Network.connections.Length + 1;
                activity.Party.Size.MaxSize = Network.maxConnections + 1;
            }
            else if (Network.isClient)
            {
                activity.Party.Size.CurrentSize = Networking.netplayer_dic.Count;
                // TODO: max players not sent to all players
            }
        }
        else if (SpeedrunTimer.instance != null && SpeedrunTimer.instance.enabled)
        {
            activity.Timestamps.Start = SpeedrunTimer.instance.startTimestamp;
        }

        discord.GetActivityManager().UpdateActivity(activity, (result) =>
        {
            if (result != Discord.Result.Ok)
            {
                Debug.LogWarning("Failed to update Discord Rich Presence");
                if (result == Discord.Result.NotRunning)
                {
                    Debug.LogWarning("Discord appears to have been closed, disabling...");
                    discord = null;
                    CancelInvoke("UpdateDiscord");
                }
            }
        });
    }

#if DEBUG
    private void OnLevelWasLoaded(int level)
    {
        Debug.Log(string.Format("Loaded {0} ({1})", Application.loadedLevelName, Application.loadedLevel));
    }
    
    private bool debugMenuActive;
    
    private string commandToRun = "";
    
    private void OnGUI()
    {
        if (Application.loadedLevelName == "Loader 1" && !Networking.instance.enabled) 
        {
            debugMenuActive = GUILayout.Toggle(debugMenuActive, "debug menu");
            if(debugMenuActive)
            {
                if (GUILayout.Button("monster tester"))
                {
                    debugMenuActive = false;
                    var mt = PhoneController.instance.GetComponent<MonsterTester>();
                    mt.enabled = true;
                    mt.showgui = true;
                }
                if (GUILayout.Button("enable hawk control"))
                {
                    PhoneController.DoPhoneCommand("enable_hawk_control");
                }
                if (GUILayout.Button("show test menu"))
                {
                    PhoneController.DoPhoneCommand("open_phone|load_screen testMenu");
                }
                if (GUILayout.Button("generate monster name"))
                {
                    Debug.Log(MonsterTraits.Name.createFullName());
                }
                if (GUILayout.Button("toggle edge detect"))
                {
                    Camera.mainCamera.GetComponent<EdgeDetectEffect>().enabled = 
                        !Camera.mainCamera.GetComponent<EdgeDetectEffect>().enabled;
                }
                GUILayout.BeginHorizontal();
                commandToRun = GUILayout.TextField(commandToRun);
                if (GUILayout.Button("run command"))
                {
                    PhoneController.DoPhoneCommand(commandToRun);
                }
                GUILayout.EndHorizontal();
            }
            
        }
    }
#endif
}
