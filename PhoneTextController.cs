using UnityEngine;

public static class PhoneTextController
{
	public static string filePath = "dialog/";

	public static PhoneButton buttonprefab;

	private static GUIStyle guistyle;

	public static Font GetFont(string fontName)
	{
		if (fontName == "none")
		{
			return null;
		}
		string text = "fonts/" + fontName;
		MonoBehaviour.print("loading font from " + text);
		return Resources.Load(text) as Font;
	}

	public static bool LoadFont(string fontName, TextMesh mesh)
	{
		Font font = GetFont(fontName);
		mesh.renderer.material.mainTexture = font.material.mainTexture;
		mesh.font = font;
		return true;
	}

	public static Color GetColor(string colorName)
	{
		switch (colorName)
		{
		case "red":
			return Color.red;
		case "blue":
			return Color.blue;
		case "green":
			return Color.green;
		case "white":
			return Color.white;
		case "black":
			return Color.black;
		case "yellow":
			return Color.yellow;
		default:
			MonoBehaviour.print("unknown color: <" + colorName + ">");
			return Color.white;
		}
	}

	public static bool LoadColor(string colorName, TextMesh mesh)
	{
		Color color = GetColor(colorName);
		mesh.renderer.material.color = color;
		return true;
	}

	public static string WrapText(string text, int charwidth)
	{
		string text2 = string.Empty;
		string[] array = text.Split(' ');
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text5 = array[i].Trim();
			string text6 = text4;
			if (i == 0)
			{
				text4 = array[0];
				text2 = text3 + text4;
			}
			if (i > 0)
			{
				text4 = text4 + " " + text5;
				text2 = text3 + text4;
			}
			if (text4.Length > charwidth)
			{
				text3 = text3 + text6 + "\n";
				text2 = text3;
				text4 = text5;
			}
			else
			{
				text5 = string.Empty;
			}
		}
		if (text5 != string.Empty)
		{
			text2 += text5;
		}
		return text2;
	}

	public static Vector2 GetTextMeshSize(string text, Font font, int fontSize, FontStyle fontStyle, TextAnchor alignment)
	{
		if (guistyle == null)
		{
			guistyle = new GUIStyle();
		}
		guistyle.font = font;
		guistyle.fontSize = fontSize;
		guistyle.fontStyle = fontStyle;
		guistyle.alignment = alignment;
		return guistyle.CalcSize(new GUIContent(text)) / 10f;
	}

	public static Vector2 GetTextMeshSize(string text, TextMesh mesh)
	{
		Vector2 textMeshSize = GetTextMeshSize(text, mesh.font, mesh.fontSize, mesh.fontStyle, mesh.anchor);
		textMeshSize.x *= mesh.transform.localScale.x * mesh.characterSize;
		if (textMeshSize.x == 0f)
		{
			textMeshSize.y = 0f;
		}
		else
		{
			textMeshSize.y *= mesh.transform.localScale.y * mesh.characterSize;
		}
		return textMeshSize;
	}
}
