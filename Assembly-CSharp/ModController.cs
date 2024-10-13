using System.Runtime.InteropServices;
using UnityEngine;

public class ModController : MonoBehaviour
{
    public static ModController instance;
    private System.IntPtr windowHandle;

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
    private static extern bool SetWindowTextW(System.IntPtr hWnd, string lpString);

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
        if (SystemInfo.graphicsDeviceID == 0) // batchmode
        {
            Application.LoadLevel("Loader 1");
            return;
        }
        UpdateTitle();
        SpawnDiscordController();
    }

    private void SpawnDiscordController()
    {
        var dc = new GameObject("DiscordController");
        dc.AddComponent<DiscordController>().enabled = PlayerPrefsX.GetBool("discord_rpc");
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
    }

    private void OnLevelWasLoaded(int level)
    {
#if DEBUG
        Debug.Log(string.Format("Loaded {0} ({1})", Application.loadedLevelName, Application.loadedLevel));
#endif
        if (Application.loadedLevelName == "Loader 1" && !Networking.instance.enabled && SystemInfo.graphicsDeviceID == 0)
        {
            Networking.instance.enabled = true;
        }
    }
    
    
#if DEBUG
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

                var edgeDetect = Camera.mainCamera.GetComponent<EdgeDetectEffect>();
                if (GUILayout.Button(string.Format("toggle edge detect ({0})", edgeDetect.threshold)))
                {
                    edgeDetect.enabled = !edgeDetect.enabled;
                }
                
                edgeDetect.threshold = GUILayout.HorizontalSlider(edgeDetect.threshold, 0.016f, 0.1f);

                if (GUILayout.Button(string.Format("cycle phone theme ({0})", PhoneMemory.current_theme)))
                {
                    PhoneMemory.CycleSelectedTheme();
                    PhoneController.instance.SetPhoneTheme(PhoneMemory.current_theme);
                    PlayerPrefs.SetString("phone_theme", PhoneMemory.current_theme);
                }
                
                if (GUILayout.Button("send test mail"))
                {
                    var newMail = new PhoneMail();
                    newMail.color = new Color(Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f));
                    newMail.id = "coloredMail" + newMail.color;
                    newMail.body = "No way, it's a colored mail!";
                    newMail.sender = "yellowberry";
                    newMail.is_new = true;
                    newMail.subject = "Colored mail!";

                    MailController.SendMail(newMail);
                }

                if (GUILayout.Button("create phone color key"))
                {
                    PlayerPrefsX.SetColor("color_phone", Color.black);
                }

                if (GUILayout.Button("update phone color"))
                {
                    PhoneController.instance.SetPhoneColor(PlayerPrefsX.GetColor("color_phone", Color.white));
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
