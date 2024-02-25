using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    private float elapsedTime;
    
    private GUIText _guiText;
    
    public GUIText behindText;
    
    private static SpeedrunTimer _instance;

    // sinful code
    public void Setup(GUIText text)
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
}
