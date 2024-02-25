using UnityEngine;

public class Quitter : MonoBehaviour
{
	private static Quitter quitter;

	private GUIText _guiText;

	public Color color = Color.red;

	public float requiredHoldTime = 1f;

	private float holdTime;

	public float showGuiTime = 1f;

	private float guiTimer;

	private KeyCode quitKey = KeyCode.Escape;

	public bool drawBar = true;

	public Vector2 barPosition;

	public Vector2 barSize;

	public Texture barTexture;

	private Rect textRect;

	private float progress;

	public bool debug;

	private void Start()
	{
		if (quitter != null)
		{
			Object.Destroy(base.gameObject);
		}
		quitter = this;
		Object.DontDestroyOnLoad(base.gameObject);
		_guiText = base.guiText;
		textRect = _guiText.GetScreenRect();
		_guiText.enabled = false;
		_guiText.material.color = color;
	}

	private void Update()
	{
		if (Input.GetKey(quitKey))
		{
			_guiText.enabled = true;
			guiTimer = showGuiTime;
			holdTime += Time.deltaTime;
			if (holdTime >= requiredHoldTime)
			{
				Quit();
			}
			progress = holdTime / requiredHoldTime;
		}
		else
		{
			_guiText.enabled = guiTimer > 0f;
			guiTimer -= Time.deltaTime;
			holdTime = 0f;
			if (_guiText.enabled)
			{
				progress = Mathf.Lerp(progress, 0f, Time.deltaTime / showGuiTime * 2f);
			}
			else
			{
				progress = 0f;
			}
			holdTime = progress * requiredHoldTime;
		}
	}

	private void OnGUI()
	{
		if (debug)
		{
			progress = 1f;
		}
		if ((_guiText.enabled && drawBar && progress > 0f) || debug)
		{
			Rect position = new Rect(textRect.x + barPosition.x, (float)Screen.height - textRect.y + barPosition.y, textRect.width * progress, barSize.y);
			Color color = GUI.color;
			GUI.color = this.color;
			GUI.DrawTexture(position, barTexture);
			GUI.color = color;
		}
	}

	private void Quit()
	{
		Application.Quit();
	}
}
