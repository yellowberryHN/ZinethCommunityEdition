using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ModController : MonoBehaviour
{
    private void OnLevelWasLoaded(int level)
    {
        Debug.Log(string.Format("loaded {0} ({1})", Application.loadedLevelName, Application.loadedLevel));
        /*
        if (level == 1)
        {
            GameObject.Find("GUI Text").guiText.material.color = Color.red;
        }
        */
    }

    public static ModController instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Object.Destroy(this);
            return;
        }
        instance = this;
        Object.DontDestroyOnLoad(base.gameObject);
        Debug.Log("Mod controller is active!");
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F5))
        {
            Application.LoadLevel("Loader 3");
        }
    }

    private bool debugMenuActive = false;

    private string command_to_run = "";
    
    private void OnGUI()
    {
        if (Application.loadedLevelName == "Loader 1") 
        {
            
            debugMenuActive = GUILayout.Toggle(debugMenuActive, "menu");
            if(debugMenuActive)
            {
                if (GUILayout.Button("monster tester"))
                {
                    debugMenuActive = false;
                    var mt = base.gameObject.AddComponent<MonsterTester>();
                    mt.showgui = true;
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
}
