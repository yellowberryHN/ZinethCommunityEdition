using UnityEngine;

public class MissionGUIText : MonoBehaviour
{
	public bool shadow = true;

	public GUIText shadow_text;

	public Vector2 shadow_offset = new Vector2(2f, -2f);

	public Vector2 shake = Vector2.zero;

	public Vector3 velocity = Vector3.zero;

	public float stopAfter = 1f;

	public bool decay = true;

	public float lifeTime = 1f;

	public Color startColor = Color.black;

	public Color endColor = Color.black;

	public static Font default_font;

	public static Material default_material;

	public string text
	{
		get
		{
			return base.guiText.text;
		}
		set
		{
			base.guiText.text = value;
			if ((bool)shadow_text)
			{
				shadow_text.text = value;
			}
		}
	}

	public Material material
	{
		get
		{
			return base.guiText.material;
		}
		set
		{
			base.guiText.material = value;
		}
	}

	public Color color
	{
		get
		{
			return base.guiText.material.color;
		}
		set
		{
			base.guiText.material.color = value;
		}
	}

	public Vector2 pixelOffset
	{
		get
		{
			return base.guiText.pixelOffset;
		}
		set
		{
			base.guiText.pixelOffset = value;
			if ((bool)shadow_text)
			{
				shadow_text.pixelOffset = value + shadow_offset;
			}
		}
	}

	public Font font
	{
		get
		{
			return base.guiText.font;
		}
		set
		{
			base.guiText.font = value;
			if ((bool)shadow_text)
			{
				shadow_text.font = value;
			}
		}
	}

	private void Awake()
	{
		if (!base.guiText)
		{
			base.gameObject.AddComponent<GUIText>();
		}
		if (shadow)
		{
			AddShadow();
		}
		color = startColor;
	}

	private GUIText AddShadow()
	{
		if (shadow_text == null)
		{
			GameObject gameObject = new GameObject("MissionGUI_Shadow");
			shadow_text = gameObject.AddComponent<GUIText>();
			shadow_text.transform.parent = base.transform;
			shadow_text.transform.localScale = Vector3.zero;
			shadow_text.transform.localPosition = Vector3.zero;
			shadow_text.transform.localRotation = Quaternion.identity;
		}
		shadow_text.text = text;
		shadow_text.font = font;
		shadow_text.material = material;
		shadow_text.material.color = Color.black;
		shadow_text.pixelOffset = pixelOffset + shadow_offset;
		shadow_text.anchor = base.guiText.anchor;
		shadow_text.alignment = base.guiText.alignment;
		return shadow_text;
	}

	private void Update()
	{
		if (shake.magnitude > 0f)
		{
			base.guiText.pixelOffset = new Vector2(Random.Range(0f - shake.x, shake.x), Random.Range(0f - shake.y, shake.y));
		}
		if (velocity.magnitude > 0f && stopAfter > 0f)
		{
			base.transform.position += velocity * Time.deltaTime;
		}
		stopAfter -= Time.deltaTime;
		if (decay)
		{
			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0f)
			{
				Kill();
			}
		}
	}

	public void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	public static MissionGUIText Create(string text, Vector3 position, Vector3 scale)
	{
		MissionGUIText missionGUIText = Object.Instantiate(MissionController.GetInstance().missionGUIPrefab) as MissionGUIText;
		missionGUIText.transform.position = position;
		missionGUIText.transform.localScale = scale;
		missionGUIText.text = text;
		return missionGUIText;
	}
}
