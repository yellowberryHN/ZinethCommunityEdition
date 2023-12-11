using UnityEngine;

public class PhoneButton : PhoneElement
{
	public Transform button_icon;

	private Vector3 icon_scale;

	protected float text_size;

	public bool pop_open;

	public bool selectable = true;

	public string id_info = string.Empty;

	public Transform background_box;

	public bool use_background_border_box;

	public bool always_use_background_border;

	private Transform background_border;

	private Vector3 background_border_newscale;

	private Vector3 background_border_activescale;

	public Vector3 background_box_selected_offset = Vector3.zero;

	private Vector3 background_box_normal_offset;

	public PhoneScreen screen;

	public string command = string.Empty;

	public PhoneButton relaybutton;

	public TextMesh textmesh;

	public PhoneController controller;

	private bool _selected;

	public bool claim_point = true;

	public bool pressed_particles = true;

	public bool play_sound = true;

	public Color back_normal_color = Color.gray;

	public Color back_selected_color = Color.white;

	public bool use_own_color;

	public Color text_normal = Color.white;

	public Color text_selected = Color.black;

	public bool horizontal_slider;

	public PhoneButton up_button;

	public PhoneButton down_button;

	public PhoneButton left_button;

	public PhoneButton right_button;

	public bool force_mouse_menulines;

	protected Vector3 normal_scale;

	public bool expand_on_select;

	public float expand_size = 1.1f;

	private Vector3 specialscale = Vector3.zero;

	protected float textscale = -1f;

	private static readonly string messagesignal = ".";

	public virtual string button_name
	{
		get
		{
			return textmesh.text;
		}
		set
		{
			textmesh.text = value;
		}
	}

	public virtual string text
	{
		get
		{
			return textmesh.text;
		}
		set
		{
			textmesh.text = value;
		}
	}

	public virtual bool selected
	{
		get
		{
			return _selected;
		}
		set
		{
			if (value && !_selected)
			{
				OnSelected();
			}
			else if (!value && _selected)
			{
				OnUnSelected();
			}
			_selected = value;
		}
	}

	public Color curNormalTextColor
	{
		get
		{
			if (use_own_color)
			{
				return text_normal;
			}
			return PhoneMemory.settings.selectableTextColor;
		}
	}

	public Color curSelectedTextColor
	{
		get
		{
			if (use_own_color)
			{
				return text_selected;
			}
			return PhoneMemory.settings.selectedTextColor;
		}
	}

	private void Awake()
	{
		Init();
	}

	public override void OnLoad()
	{
		base.OnLoad();
		if (pop_open && (bool)button_icon)
		{
			button_icon.localScale = Vector3.zero;
		}
		textmesh.renderer.material.color = PhoneMemory.settings.selectableTextColor;
		if (use_own_color)
		{
			textmesh.renderer.material.color = text_normal;
		}
		SetBorderActive(always_use_background_border);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (specialscale != Vector3.zero && (bool)button_icon && button_icon.transform.localScale != specialscale)
		{
			button_icon.transform.localScale = Vector3.Lerp(button_icon.transform.localScale, specialscale, PhoneElement.deltatime * 12f);
		}
		if (textscale >= 0f && (bool)textmesh && textmesh.characterSize != textscale)
		{
			textmesh.characterSize = Mathf.Lerp(textmesh.characterSize, textscale, PhoneElement.deltatime * 12f);
		}
		if ((bool)background_border && use_background_border_box && background_border_newscale != background_border.localScale)
		{
			background_border.localScale = Vector3.Lerp(background_border.localScale, background_border_newscale, Time.deltaTime * 20f);
		}
	}

	public override void Init()
	{
		if (!textmesh)
		{
			textmesh = base.gameObject.GetComponent<TextMesh>();
		}
		if (!controller)
		{
			controller = PhoneController.instance;
		}
		wantedpos = base.transform.localPosition;
		wantedrot = base.transform.localRotation;
		if (screen == null && (bool)base.transform.parent)
		{
			PhoneScreen component = base.transform.parent.GetComponent<PhoneScreen>();
			if ((bool)component)
			{
				screen = component;
			}
		}
		if ((bool)button_icon)
		{
			icon_scale = button_icon.localScale;
			specialscale = icon_scale;
		}
		if ((bool)textmesh)
		{
			text_size = textmesh.characterSize;
			textscale = text_size;
			if (textscale == 0f)
			{
				MonoBehaviour.print(base.name + " (0 textscale)");
			}
		}
		if (back_normal_color == new Color(0f, 0f, 0f, 0f))
		{
			if ((bool)background_box)
			{
				back_normal_color = background_box.renderer.material.color;
			}
			else
			{
				back_normal_color = Color.gray;
			}
		}
		if (back_selected_color == new Color(0f, 0f, 0f, 0f))
		{
			back_selected_color = Color.Lerp(back_normal_color, Color.white, 0.6f);
		}
		if (use_background_border_box && background_border == null && background_box != null)
		{
			background_box_normal_offset = background_box.transform.localPosition;
			if ((bool)PhoneController.buttonBackPrefab)
			{
				background_border = (Object.Instantiate(PhoneController.buttonBackPrefab) as GameObject).transform;
			}
			else
			{
				Debug.LogWarning("PhoneController.buttonBackPrefab not set, reverting to bad ol' cube primitive...");
				background_border = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				Object.Destroy(background_border.collider);
				background_border.gameObject.layer = background_box.gameObject.layer;
			}
			background_border.transform.parent = background_box;
			background_border.rotation = background_box.rotation;
			background_border.position = background_box.position + new Vector3(0.05f, -0.1f, -0.05f);
			background_border.localScale = Vector3.one;
			background_border.parent = null;
			Vector3 localScale = background_border.localScale;
			background_border.localScale = localScale;
			background_border.parent = background_box;
			background_border_activescale = background_border.localScale;
			background_border.renderer.material = background_box.renderer.material;
			background_border.renderer.material.color = new Color(0f, 0f, 0f, 0.5f);
			background_border.gameObject.active = false;
		}
		SetBackColor(back_normal_color);
		SetBorderActive(always_use_background_border && base.gameObject.active);
		normal_scale = base.transform.localScale;
		wantedscale = normal_scale;
	}

	private void CheckHover()
	{
		Vector3 touchPoint = PhoneInput.GetTouchPoint();
		if (touchPoint != Vector3.zero * -1f)
		{
			Vector3 point = PhoneInput.TransformPoint(touchPoint);
			point.y = base.transform.position.y;
			if (ContainsPoint(point))
			{
				OnSelected();
			}
		}
	}

	public virtual bool ContainsPoint(Vector3 point)
	{
		if (GetBounds().Contains(point))
		{
			return true;
		}
		return false;
	}

	public virtual Bounds GetBounds()
	{
		if ((bool)background_box)
		{
			return background_box.renderer.bounds;
		}
		if ((bool)button_icon)
		{
			return button_icon.renderer.bounds;
		}
		if ((bool)textmesh)
		{
			return textmesh.renderer.bounds;
		}
		if ((bool)base.collider)
		{
			return base.collider.bounds;
		}
		return base.renderer.bounds;
	}

	protected virtual void SetBackColor(Color col)
	{
		if ((bool)background_box)
		{
			background_box.renderer.material.color = col;
		}
	}

	protected virtual void SetBorderActive(bool val)
	{
		if ((bool)background_border)
		{
			background_border.gameObject.active = val && base.gameObject.active;
			if (val)
			{
				background_border_newscale = background_border_activescale;
			}
			else
			{
				background_border_newscale = Vector3.one * 0.6f;
			}
		}
	}

	public virtual void OnSelected()
	{
		if ((bool)textmesh)
		{
			textmesh.renderer.material.color = curSelectedTextColor;
			textscale = text_size + Mathf.Min(text_size * 0.2f, 0.1f);
		}
		if ((bool)button_icon)
		{
			specialscale = icon_scale * 1.2f;
			button_icon.renderer.material.color = Color.gray;
		}
		if ((bool)background_box && background_box_selected_offset.magnitude != 0f)
		{
			background_box.localPosition = background_box_normal_offset + background_box_selected_offset;
		}
		SetBackColor(back_selected_color);
		SetBorderActive(true);
		if (expand_on_select)
		{
			wantedscale = normal_scale * expand_size;
		}
	}

	public virtual void OnUnSelected()
	{
		if ((bool)textmesh)
		{
			textmesh.renderer.material.color = curNormalTextColor;
			textscale = text_size;
		}
		if ((bool)button_icon)
		{
			specialscale = icon_scale;
			button_icon.renderer.material.color = Color.white;
		}
		if ((bool)background_box && background_box_selected_offset.magnitude != 0f)
		{
			background_box.localPosition = background_box_normal_offset;
		}
		SetBackColor(back_normal_color);
		SetBorderActive(always_use_background_border);
		if (expand_on_select)
		{
			wantedscale = normal_scale;
		}
	}

	public override Vector3 GetCenter()
	{
		return GetBounds().center;
	}

	public virtual Vector3 GetPressPos()
	{
		return GetCenter();
	}

	public virtual void DoPressedParticles()
	{
		PhoneController.EmitPartsMenu(GetPressPos() + Vector3.up * 0.1f, 30);
	}

	public virtual bool RelayPress(PhoneButton button)
	{
		if (button != relaybutton)
		{
			OnPressed();
			return true;
		}
		return false;
	}

	public virtual void OnPressed()
	{
		if (claim_point)
		{
			PhoneController.presspos = GetPressPos();
		}
		if (pressed_particles)
		{
			DoPressedParticles();
		}
		if (play_sound)
		{
			PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_accept, SoundType.menu);
		}
		if (expand_on_select)
		{
			base.transform.localScale = normal_scale;
		}
		if (relaybutton != null)
		{
			relaybutton.RelayPress(this);
		}
		else
		{
			RunCommand(command);
		}
	}

	public virtual bool RunCommand(string stringcommand)
	{
		if (stringcommand.StartsWith(messagesignal))
		{
			if (screen != null)
			{
				return screen.ButtonMessage(this, stringcommand.Remove(0, messagesignal.Length));
			}
			Debug.LogWarning("no screen for button:" + base.name + " (" + button_name + "), command: " + stringcommand);
			return false;
		}
		return controller.DoCommand(stringcommand);
	}

	public virtual void ShiftSlider(float amount)
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		if ((bool)down_button)
		{
			Gizmos.DrawLine(base.transform.position, down_button.transform.position);
		}
		if ((bool)up_button)
		{
			Gizmos.DrawLine(base.transform.position, up_button.transform.position);
		}
		if ((bool)left_button)
		{
			Gizmos.DrawLine(base.transform.position, left_button.transform.position);
		}
		if ((bool)right_button)
		{
			Gizmos.DrawLine(base.transform.position, right_button.transform.position);
		}
	}
}
