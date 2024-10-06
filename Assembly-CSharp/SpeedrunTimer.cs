using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    private static SpeedrunTimer _instance;
    
    private float elapsedTime;
    
    private GUIText behindText;

    public RunTypes runType;
    
    public long startTimestamp; // used by Discord

    public string finalTime { get; private set; }

    // sinful code
    public void Setup()
    {
        transform.Translate(new Vector3(0.0075f,0.075f,0f));
        gameObject.AddComponent<GUIText>();
        guiText.font = GameObject.Find("Reminder").guiText.font;
        guiText.material.color = Color.white;
        guiText.text = "00:00.000";

        behindText = new GameObject("BackText").AddComponent<GUIText>().guiText;
        behindText.transform.parent = transform;
        behindText.transform.localPosition = Vector3.zero;
        behindText.font = guiText.font;
        behindText.material.color = Color.black;
        behindText.text = "00:00.000";
        behindText.pixelOffset = new Vector2(2f, -2f);
    }
    
    private void Awake()
    {
        elapsedTime = 0f;
        switch (Application.loadedLevelName)
        {
            case "Loader 1": // Main Game
                runType = PlayerPrefsX.GetEnum("speedrun_type", RunTypes.Manual);
                break;
            default:
                runType = RunTypes.Manual;
                break;
        }
    }

    private void Start()
    {
        var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        startTimestamp = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
    }
    
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        UpdateTimer();
    }

    void UpdateTimer()
    {
        SetText(string.Format("{0:00}:{1:00}.{2:000}", Mathf.Floor(elapsedTime / 60), elapsedTime % 60, (elapsedTime * 1000) % 1000));
    }
    
    public void SetText(string text)
    {
        guiText.text = text;
        behindText.text = text;
    }

    public void StopTimer()
    {
        guiText.material.color = Color.red;
        enabled = false;
        finalTime = string.Format("{0:00}:{1:00}.{2:000}", Mathf.Floor(elapsedTime / 60), elapsedTime % 60, (elapsedTime * 1000) % 1000);
    }
    
    public static SpeedrunTimer instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(SpeedrunTimer)) as SpeedrunTimer;
            }
            return _instance;
        }
    }

    public RunTypes CycleRunType()
    {
        runType = (RunTypes)(((int)runType + 1) % System.Enum.GetValues(typeof(RunTypes)).Length);
        return runType;
    }

    public enum RunTypes
    {
        Manual,
        Moon,
        Story,
        Mission,
        Capsule
    }
}
