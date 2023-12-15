using System.Collections.Generic;
using UnityEngine;

public class PhoneSlideshow : PhoneMainMenu
{
	public string start_set;

	public List<Texture2D> slides;

	public Renderer slide_renderer;

	public bool use_tutorial_slide_conversion = true;

	public PhoneButton prev_button;

	public PhoneButton next_button;

	protected int _slide_ind;

	public bool wrap_slides;

	private Vector3 button_scale = Vector3.zero;

	public int slide_ind
	{
		get
		{
			return _slide_ind;
		}
		set
		{
			_slide_ind = value;
			if (_slide_ind >= slides.Count)
			{
				if (wrap_slides)
				{
					_slide_ind = 0;
				}
				else
				{
					_slide_ind = slides.Count - 1;
				}
			}
			if (_slide_ind < 0)
			{
				if (wrap_slides)
				{
					_slide_ind = slides.Count - 1;
				}
				else
				{
					_slide_ind = 0;
				}
			}
			if ((bool)next_button && (wrap_slides || _slide_ind < slides.Count - 1))
			{
				controller.menulines[3].end = next_button;
				if (!next_button.selected)
				{
					next_button.textmesh.renderer.material.color = Color.black;
				}
				next_button.selectable = true;
			}
			else if ((bool)next_button)
			{
				controller.menulines[3].end = null;
				if (next_button.selected)
				{
					next_button.selected = false;
				}
				next_button.selectable = false;
				next_button.textmesh.renderer.material.color = Color.gray;
			}
			if ((bool)prev_button && (wrap_slides || _slide_ind > 0))
			{
				controller.menulines[2].end = prev_button;
				if (!prev_button.selected)
				{
					prev_button.textmesh.renderer.material.color = Color.black;
				}
				prev_button.selectable = true;
			}
			else if ((bool)prev_button)
			{
				controller.menulines[2].end = null;
				if (prev_button.selected)
				{
					prev_button.selected = false;
				}
				prev_button.selectable = false;
				prev_button.textmesh.renderer.material.color = Color.gray;
			}
			SetTexture(slides[slide_ind]);
		}
	}

	private bool should_convert
	{
		get
		{
			return use_tutorial_slide_conversion && PhoneInput.controltype != PhoneInput.ControlType.Keyboard;
		}
	}

	protected override void SetMenuLines(PhoneButton button)
	{
	}

	private void SetupMenuLines()
	{
		controller.menulines[0].start = prev_button;
		controller.menulines[0].end = null;
		controller.menulines[1].start = prev_button;
		controller.menulines[1].end = null;
		controller.menulines[2].start = prev_button;
		controller.menulines[3].start = next_button;
	}

	private void SetTexture(Texture2D texture)
	{
		if (should_convert)
		{
			texture = TutorialSlides.GetKeyboardSlide(texture);
		}
		slide_renderer.material.mainTexture = texture;
	}

	private void Update()
	{
		if ((bool)next_button)
		{
			if (!next_button.gameObject.active)
			{
				Debug.LogError("next button is not active!");
				next_button.gameObject.active = true;
			}
			if (!next_button.renderer.enabled)
			{
				Debug.LogError("next button renderer is not enabled!");
				next_button.renderer.enabled = true;
			}
		}
		else
		{
			Debug.LogError("no next button...");
		}
	}

	private void Start()
	{
		if (!slide_renderer)
		{
			slide_renderer = base.renderer;
		}
		slide_ind = slide_ind;
		if ((bool)next_button)
		{
			button_scale = next_button.transform.localScale;
			next_button.text_selected = next_button.curSelectedTextColor;
			next_button.text_normal = Color.black;
			next_button.use_own_color = true;
		}
		if ((bool)prev_button)
		{
			prev_button.text_selected = prev_button.curSelectedTextColor;
			prev_button.text_normal = Color.black;
			prev_button.use_own_color = true;
		}
	}

	public void LoadSlideSet(string setname)
	{
		if (TutorialSlides.slideset_dictionary.ContainsKey(setname))
		{
			LoadSlideSet(TutorialSlides.slideset_dictionary[setname]);
		}
		else
		{
			Debug.LogWarning("slideset " + setname + " does not exist...");
		}
	}

	public void LoadSlideSet(TutorialSlides.Slideset slideset)
	{
		MonoBehaviour.print("loading slideset: " + slideset.set_name);
		slides = slideset.slides;
		slide_ind = 0;
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		if (start_set != string.Empty)
		{
			LoadSlideSet(start_set);
		}
		SetupMenuLines();
		slide_ind = slide_ind;
	}

	public virtual void NextSlide()
	{
		slide_ind++;
	}

	public virtual void PreviousSlide()
	{
		slide_ind--;
	}

	public override void UpdateScreen()
	{
		if (button_scale == Vector3.zero)
		{
			Debug.LogError("button_scale was 0, that was probably the problem. fixin it now...");
			button_scale = next_button.transform.localScale;
		}
		float magnitude = button_scale.magnitude;
		float num = magnitude * 0.9f + Mathf.PingPong(Time.time, magnitude * 0.2f);
		float magnitude2 = button_scale.magnitude;
		if ((bool)next_button)
		{
			float num2 = magnitude2;
			if (next_button.selectable)
			{
				num2 = num;
			}
			next_button.transform.localScale = button_scale.normalized * num2;
		}
		if ((bool)prev_button)
		{
			float num3 = magnitude2;
			if (prev_button.selectable)
			{
				num3 = num;
			}
			prev_button.transform.localScale = button_scale.normalized * num3;
		}
		base.UpdateScreen();
	}

	protected override void DoStickControls()
	{
		Vector2 controlDirPressed = PhoneInput.GetControlDirPressed();
		if (Mathf.Abs(controlDirPressed.y) < 0.3f)
		{
			if (controlDirPressed.x >= 0.5f)
			{
				NextSlide();
			}
			else if (controlDirPressed.x <= -0.5f)
			{
				PreviousSlide();
			}
		}
	}

	public override bool ButtonMessage(PhoneButton button, string message)
	{
		switch (message)
		{
		case "next":
			NextSlide();
			break;
		case "back":
		case "prev":
		case "previous":
			PreviousSlide();
			break;
		default:
			return base.ButtonMessage(button, message);
		}
		return true;
	}
}
