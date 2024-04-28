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

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Mod controller is active, quick keys enabled.");
        UpdateTitle();
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
                Application.LoadLevel("loader 1");
            }
        }
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
