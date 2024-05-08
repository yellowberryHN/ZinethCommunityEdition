using UnityEngine;

public class PhoneViewController : MonoBehaviour
{
	public Transform phoneviewscreen;

	public Transform phoneviewbordertop;

	public Transform phoneviewborderbottom;

	public Transform phoneviewscreenanchor;

	public Transform phoneviewtopanchor;

	public Transform phoneviewstick;

	public Light phoneviewlight;

	public Vector3 closescale;

	public Vector3 openscale;

	public Renderer zinerenderer;

	public bool showing_zine;

	public bool open;

	public float openspeed = 5f;

	public bool is_opening;

	public RenderTexture rendertex;

	public PhoneController phonecontroller;

	private Vector3 normal_zine_pos;

	private Vector3 hidden_zine_pos;

	private Vector3 normal_zine_scale;

	private bool has_resized = true;

	private float light_brightness;

	public float deltatime
	{
		get
		{
			return PhoneController.deltatime;
		}
	}

	private void Start()
	{
		if (phonecontroller == null)
		{
			phonecontroller = Object.FindObjectOfType(typeof(PhoneController)) as PhoneController;
		}
		rendertex = new RenderTexture(480, 800, 0);
		phonecontroller.phonecam.targetTexture = rendertex;
		phoneviewscreen.renderer.material.mainTexture = rendertex;
		phoneviewscreen.renderer.material.mainTextureScale = Vector2.one;
		phoneviewscreen.renderer.material.mainTextureOffset = Vector2.zero;
		phonecontroller.phonecam.orthographicSize = 4f;
		Color color = phoneviewscreen.renderer.material.color;
		color.a = 0.9f;
		phoneviewscreen.renderer.material.color = color;
		phoneviewlight.transform.parent.renderer.material.color = Color.black;
		if ((bool)zinerenderer)
		{
			normal_zine_pos = zinerenderer.transform.position;
			hidden_zine_pos = normal_zine_pos + Vector3.down * 10f;
			normal_zine_scale = zinerenderer.transform.localScale;
		}
		HideZine();
	}

	private void Update()
	{
		if (!PhoneController._use_fixed_update)
		{
			HandleControls();
			HandleTransition();
			HandleZine();
			HandleLight();
			HandleStick();
		}
	}

	public bool ShowZine(Texture2D texture)
	{
		return ShowZine(texture, false);
	}

	public bool ShowZine(Texture2D texture, bool resize)
	{
		showing_zine = true;
		if (!zinerenderer)
		{
			return false;
		}
		zinerenderer.gameObject.active = true;
		zinerenderer.material.mainTexture = texture;
		zinerenderer.transform.position = hidden_zine_pos;
		if (resize)
		{
			has_resized = false;
		}
		else
		{
			has_resized = true;
			zinerenderer.transform.localScale = normal_zine_scale;
		}
		return true;
	}

	public bool HideZine()
	{
		showing_zine = false;
		return true;
	}

	private void HandleZine()
	{
		if (!zinerenderer)
		{
			return;
		}
		if (!has_resized)
		{
			ResizeZine();
		}
		Vector3 vector = hidden_zine_pos;
		if (showing_zine)
		{
			vector = normal_zine_pos;
		}
		if (zinerenderer.transform.position != vector)
		{
			zinerenderer.transform.position = Vector3.Lerp(zinerenderer.transform.position, vector, deltatime * 10f);
		}
		if (Vector3.Distance(zinerenderer.transform.position, vector) < 0.01f)
		{
			zinerenderer.transform.position = vector;
			if (!showing_zine)
			{
				zinerenderer.gameObject.active = false;
			}
		}
	}

	private void ResizeZine()
	{
		if (!has_resized)
		{
			Vector3 localScale = zinerenderer.transform.localScale;
			Texture mainTexture = zinerenderer.material.mainTexture;
			if (mainTexture != null)
			{
				Vector2 vector = new Vector2(mainTexture.width, mainTexture.height);
				float magnitude = new Vector2(localScale.x, localScale.z).magnitude;
				vector = vector.normalized * magnitude;
				zinerenderer.transform.localScale = new Vector3(vector.x, localScale.y, vector.y);
				has_resized = true;
			}
		}
	}

	private void Flicker()
	{
		Color color = phoneviewscreen.renderer.material.color;
		color.a = Random.Range(0.86f, 0.92f);
		phoneviewscreen.renderer.material.color = color;
	}

	private void HandleControls()
	{
		if (Input.GetButtonDown("CellOpen"))
		{
			ToggleOpen();
		}
		if (Input.GetButtonDown("CellHome"))
		{
			OnHomeButton();
		}
	}

	public void OnHomeButton()
	{
		OnHomeButton(false);
	}

	public void OnHomeButton(bool force)
	{
		phonecontroller.OnHomePressed(force);
	}

	public void ToggleOpen()
	{
		SetOpen(!open);
	}

	public void SetOpen(bool isopen)
	{
		SetOpen(isopen, false);
	}

	public void SetOpen(bool isopen, bool force)
	{
		if ((phonecontroller.allow_open || force) && open != isopen)
		{
			open = isopen;
			if (open)
			{
				phonecontroller.OnScreenOpen(force);
			}
			else
			{
				phonecontroller.OnScreenClose(force);
			}
		}
	}

	private void HandleTransition()
	{
		Vector3 vector;
		if (open)
		{
			float b = 0.025f;
			vector = Vector3.Lerp(phoneviewscreenanchor.transform.localScale, openscale, Mathf.Min(deltatime, b) * openspeed);
		}
		else
		{
			vector = Vector3.Lerp(phoneviewscreenanchor.transform.localScale, closescale, Mathf.Min(deltatime, 1f) * openspeed);
		}
		is_opening = vector != phoneviewscreenanchor.localScale;
		if (is_opening)
		{
			phoneviewscreenanchor.localScale = vector;
		}
		if (phoneviewbordertop.position != phoneviewtopanchor.position)
		{
			phoneviewbordertop.position = phoneviewtopanchor.position;
		}
		if (phoneviewbordertop.rotation != phoneviewtopanchor.rotation)
		{
			phoneviewbordertop.rotation = phoneviewtopanchor.rotation;
		}
	}

	public void HandleLight()
	{
		if (light_brightness <= 0f)
		{
			phoneviewlight.gameObject.active = false;
			phoneviewlight.light.intensity = 0f;
			phoneviewlight.transform.parent.renderer.material.color = Color.black;
		}
		else
		{
			phoneviewlight.gameObject.active = true;
			phoneviewlight.light.intensity = light_brightness;
			phoneviewlight.transform.parent.renderer.material.color = Color.Lerp(Color.black, phoneviewlight.color, light_brightness / 4f);
		}
	}

	public void SetLightBrightness(float amount)
	{
		light_brightness = amount;
	}

	public void SetLightColor(Color color)
	{
		phoneviewlight.color = color;
	}

	public void HandleStick()
	{
		if ((bool)phoneviewstick)
		{
			Vector2 controlDir = PhoneInput.GetControlDir();
			Vector3 localPosition = new Vector3(controlDir.x, 0f, controlDir.y) * 0.08f;
			if (PhoneInput.IsPressed())
			{
				localPosition.y = -0.045f;
				phoneviewstick.renderer.material.color = new Color(0.11764706f, 0f, 0.101960786f);
			}
			else
			{
				phoneviewstick.renderer.material.color = Color.black;
			}
			phoneviewstick.localPosition = localPosition;
		}
	}
}
