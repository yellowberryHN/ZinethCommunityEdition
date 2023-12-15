using System.Collections.Generic;
using UnityEngine;

public class PhoneLabel : PhoneElement
{
	public TextMesh textmesh;

	public PhoneLabel shadow_label;

	public bool overrideColor;

	public Color color;

	public bool wraptext;

	public float wrapwidth = 4f;

	public bool cutz;

	public float cutzheight = 4f;

	public Transform icon;

	private string _text = string.Empty;

	public bool wrapdebug;

	private string _prevtext = string.Empty;

	private Vector2 _prevsize = Vector2.zero;

	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			SetText(value);
		}
	}

	private string _meshtext
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

	private void Awake()
	{
		Init();
	}

	public override void Init()
	{
		if (textmesh == null)
		{
			textmesh = GetComponent<TextMesh>();
		}
		wantedpos = base.transform.localPosition;
		wantedrot = base.transform.localRotation;
		if (!textmesh)
		{
			Debug.LogWarning("no textmesh: " + base.name);
		}
		if (text != string.Empty)
		{
			SetText(text);
		}
		else if (_meshtext != string.Empty)
		{
			SetText(_meshtext);
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		if (overrideColor)
		{
			textmesh.renderer.material.color = color;
		}
		else
		{
			textmesh.renderer.material.color = PhoneMemory.settings.textColor;
		}
	}

	public void SetColor(Color col)
	{
		textmesh.renderer.material.color = col;
	}

	private void Update()
	{
		if (wrapdebug)
		{
			SetText(text);
		}
		wrapdebug = false;
	}

	private void Start()
	{
		if (overrideColor)
		{
			textmesh.renderer.material.color = color;
		}
		else
		{
			textmesh.renderer.material.color = PhoneMemory.settings.textColor;
		}
	}

	public void Hide()
	{
		base.gameObject.active = false;
		if ((bool)shadow_label)
		{
			shadow_label.Hide();
		}
		if ((bool)icon)
		{
			icon.gameObject.active = false;
		}
	}

	public virtual void SetText(string newtext)
	{
		_text = newtext;
		if (wraptext)
		{
			WrapText();
		}
		else
		{
			_meshtext = _text;
		}
		if ((bool)shadow_label)
		{
			shadow_label.SetText(_text);
		}
	}

	protected virtual void WrapText()
	{
		Transform parent = textmesh.transform.parent;
		if (parent != null)
		{
			textmesh.transform.parent = null;
		}
		InnerWrapText();
		if (parent != null)
		{
			textmesh.transform.parent = parent;
		}
	}

	protected virtual Vector2 GetTextSize(string text)
	{
		if (text == _prevtext)
		{
			return _prevsize;
		}
		_prevtext = text;
		_prevsize = PhoneTextController.GetTextMeshSize(text, textmesh);
		return _prevsize;
	}

	protected virtual void InnerWrapText()
	{
		bool flag = true;
		int num = 0;
		string[] collection = _text.Split(' ');
		List<string> list = new List<string>(collection);
		string empty = string.Empty;
		string text = string.Empty;
		int num2 = 0;
		while (list.Count > 0)
		{
			num++;
			if (num >= 1024)
			{
				Debug.Log(text);
				Debug.LogError("oh no there are a fuck load of loops");
				_meshtext = text;
				return;
			}
			string text2 = text;
			if (!flag)
			{
				text += " ";
			}
			flag = false;
			text += list[0];
			if (GetTextSize(text).x > wrapwidth)
			{
				text = ((text2.Length <= 0) ? (text2 + list[0]) : (text2 + "\n" + list[0]));
				if (GetTextSize(text).x > wrapwidth)
				{
					string text3 = list[0];
					string text4 = text2;
					if (text2.Length > 0)
					{
						text4 += "\n";
					}
					text = text4;
					int num3 = 0;
					while (GetTextSize(text).x <= wrapwidth && num3 < text3.Length)
					{
						text = text4 + text3.Substring(0, num3);
						num++;
						if (num >= 512)
						{
							Debug.LogError("wow just look at these loops\n" + text2 + ", " + text3 + ", " + text3.Substring(0, num3));
							_meshtext = text;
							return;
						}
						num3++;
					}
					num3 -= 2;
					if (num3 >= 0)
					{
						text = text4 + text3.Substring(0, num3);
					}
					num3 = Mathf.Clamp(num3, 0, text3.Length - 2);
					if (text3.Length >= 1)
					{
						list.Insert(1, text3.Remove(0, num3));
					}
				}
			}
			num2++;
			list.RemoveAt(0);
			if (cutz && GetTextSize(text).y > cutzheight)
			{
				text = text2 + "...";
				_meshtext = text;
				return;
			}
		}
		_meshtext = text;
	}
}
