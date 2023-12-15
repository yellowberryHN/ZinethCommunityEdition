using UnityEngine;

public class PhoneButtonSlider : PhoneButton
{
	private float _slider_pos = 0.5f;

	public float min_val;

	public float max_val = 1f;

	public string slide_command = string.Empty;

	public float slide_scale = 0.5f;

	private bool last_selected;

	public virtual float slider_pos
	{
		get
		{
			return _slider_pos;
		}
		set
		{
			_slider_pos = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public float val
	{
		get
		{
			return Mathf.Lerp(min_val, max_val, slider_pos);
		}
		set
		{
			slider_pos = Mathf.InverseLerp(min_val, max_val, value);
		}
	}

	private void Awake()
	{
		Init();
	}

	public override void Init()
	{
		base.Init();
		left_button = this;
		right_button = this;
		UpdateSliderPos();
	}

	public override void OnLoad()
	{
		base.OnLoad();
		InitSliderPos();
		last_selected = false;
	}

	public override void OnUpdate()
	{
		if ((selected || last_selected) && (!animateOnLoad || base.transform.localPosition == wantedpos))
		{
			SlideControls();
		}
		last_selected = selected;
		base.OnUpdate();
	}

	protected virtual void SlideControls()
	{
		Vector2 controlDir = PhoneInput.GetControlDir();
		if (Mathf.Abs(controlDir.x) > 0.15f && Mathf.Abs(controlDir.y) < 0.25f)
		{
			ShiftSlider((controlDir.x - 0.15f) * slide_scale * PhoneElement.deltatime);
		}
		if (PhoneInput.controltype == PhoneInput.ControlType.Mouse && PhoneInput.IsPressed())
		{
			Vector3 transformedTouchPoint = PhoneInput.GetTransformedTouchPoint();
			Bounds bounds = GetBounds();
			transformedTouchPoint.y = bounds.center.y;
			transformedTouchPoint.x = Mathf.Clamp(transformedTouchPoint.x, bounds.min.x, bounds.max.x);
			if (bounds.Contains(transformedTouchPoint))
			{
				SetSlider(transformedTouchPoint);
			}
		}
	}

	private void InitSliderPos()
	{
		float sliderVar = screen.GetSliderVar(slide_command);
		val = sliderVar;
		UpdateSliderPos();
	}

	public override void ShiftSlider(float amount)
	{
		SetSlider(slider_pos + amount);
	}

	public virtual void SetSlider(Vector3 pos)
	{
		Bounds slideBounds = GetSlideBounds();
		float slider = Mathf.InverseLerp(slideBounds.min.x, slideBounds.max.x, pos.x);
		SetSlider(slider);
	}

	public virtual void SetSlider(float amount)
	{
		float num = slider_pos;
		slider_pos = amount;
		if (num != slider_pos)
		{
			DoSliderCommand();
			UpdateSliderPos();
		}
	}

	public void UpdateSliderPos()
	{
		if ((bool)button_icon)
		{
			Bounds bounds = GetBounds();
			Vector3 from = new Vector3(bounds.min.x, button_icon.position.y, bounds.center.z);
			Vector3 to = new Vector3(bounds.max.x, button_icon.position.y, bounds.center.z);
			Vector3 vector = Vector3.Lerp(from, to, slider_pos);
			if (button_icon.transform.position != vector)
			{
				button_icon.transform.position = vector;
			}
		}
	}

	public virtual bool DoSliderCommand()
	{
		string format = slide_command;
		format = string.Format(format, val);
		return RunCommand(format);
	}

	public virtual Bounds GetSlideBounds()
	{
		Bounds bounds = base.GetBounds();
		if ((bool)button_icon)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			min.z = button_icon.renderer.bounds.min.z;
			max.z = button_icon.renderer.bounds.max.z;
			bounds.SetMinMax(min, max);
		}
		return bounds;
	}

	public override Bounds GetBounds()
	{
		return GetSlideBounds();
	}
}
