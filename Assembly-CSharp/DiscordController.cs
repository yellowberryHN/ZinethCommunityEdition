using Discord;
using UnityEngine;

public class DiscordController: MonoBehaviour
{
    private static DiscordController _instance;
    
    public static DiscordController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(DiscordController)) as DiscordController;
            }
            return _instance;
        }
    }
    
    private static Discord.Discord discord;
    private ActivityManager _activityManager;
    private UserManager _userManager;

    public User user;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Discord controller initialized.");
    }

    private void OnEnable()
    {
        if (discord == null) InitDiscord();
        EnableDiscordRPC();
    }

    private void OnDisable()
    {
        DisableDiscordRPC();
    }

    void Update()
    {
        if (discord == null) return;
        
        try
        {
            discord.RunCallbacks();
        }
        catch (ResultException)
        {
            CancelInvoke("UpdateDiscord");
            enabled = false;
        }
    }
    
    private void InitDiscord()
    {
        try
        {
            discord = new Discord.Discord(1233330405092888660, (ulong)CreateFlags.NoRequireDiscord);
            discord.SetLogHook(LogLevel.Debug, ForwardDiscordLog);
            
            _activityManager = discord.GetActivityManager();
            _userManager = discord.GetUserManager();
            
            _userManager.OnCurrentUserUpdate += () =>
            {
                user = _userManager.GetCurrentUser();
                Debug.Log("Stored Discord user information");
            };
        }
        catch (ResultException)
        {
            Debug.LogWarning("Failed to initialize Discord integration, might not be running.");
            enabled = false;
        }
    }

    public void ForwardDiscordLog(LogLevel level, string message)
    {
        Debug.Log(string.Format("[Discord - {0}] {1}", level, message));
    }
    
    void EnableDiscordRPC()
    {
        if (discord == null) return;
        CancelInvoke("UpdateDiscord");
        Invoke("UpdateDiscord", 0f);
        InvokeRepeating("UpdateDiscord", 5f, 5f);
    }

    void DisableDiscordRPC()
    {
        if (discord == null) return;
        CancelInvoke("UpdateDiscord");
        
        discord.GetActivityManager().ClearActivity(result =>
        {
            if(result != Result.Ok) Debug.Log("Clearing Discord Activity: Failed!");
        });
    }
    
    public void UpdateDiscord()
    {
        if (discord == null) return;

        var details = string.Empty;
        var state = string.Empty;
        
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
                
                if (SpeedrunTimer.instance != null && (SpeedrunTimer.instance.enabled || SpeedrunTimer.instance.finalTime != null))
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

        var activity = new Activity
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
                activity.Party.Size.MaxSize = 0;
            }
        }
        else if (SpeedrunTimer.instance != null && SpeedrunTimer.instance.enabled)
        {
            activity.Timestamps.Start = SpeedrunTimer.instance.startTimestamp;
        }

        _activityManager.UpdateActivity(activity, (result) =>
        {
            if (result != Result.Ok)
            {
                Debug.LogWarning("Failed to update Discord Rich Presence");
                if (result == Result.NotRunning)
                {
                    Debug.LogWarning("Discord appears to have been closed, disabling...");
                    CancelInvoke("UpdateDiscord");
                    enabled = false;
                }
            }
        });
    }

    public static long GetDiscordID()
    {
        return instance.user.Id;
    }
}