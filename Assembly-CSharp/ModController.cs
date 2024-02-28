using UnityEngine;

public class ModController : MonoBehaviour
{
    public static ModController instance;

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
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            if (Application.loadedLevelName != "Loader 3")
            {
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
    
    private string command_to_run = "";
    
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
                command_to_run = GUILayout.TextField(command_to_run);
                if (GUILayout.Button("run command"))
                {
                    PhoneController.DoPhoneCommand(command_to_run);
                }
                GUILayout.EndHorizontal();
            }
            
        }
    }
#endif
}
