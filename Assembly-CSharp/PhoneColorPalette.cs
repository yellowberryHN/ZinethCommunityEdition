using UnityEngine;

public class PhoneColorPalette
{
	public string name;
	
	public Color back;

	public Color text;

	public Color selectable;

	public Color selected;

	public Color particles = new Color(0f, 0f, 0f, 0f);
	
	public Color mail = new Color(0f, 0f, 0f, 0f);

	public bool dark;

	public PhoneColorPalette()
	{
	}

	public PhoneColorPalette(Color _text, Color _selected, Color _selectable, Color _back)
	{
		text = _text;
		selected = _selected;
		selectable = _selectable;
		back = _back;
		mail = Color.clear;
		particles = selected;
	}
	
	public PhoneColorPalette(Color _text, Color _selected, Color _selectable, Color _back, Color _mail)
	{
		text = _text;
		selected = _selected;
		selectable = _selectable;
		back = _back;
		mail = _mail;
		particles = selected;
	}


	public PhoneColorPalette(Color _text, Color _selected, Color _selectable, Color _back, Color _mail, Color _particles)
	{
		text = _text;
		selected = _selected;
		selectable = _selectable;
		back = _back;
		mail = _mail;
		particles = _particles;
	}
}
