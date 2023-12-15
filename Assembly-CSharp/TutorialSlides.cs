using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSlides : MonoBehaviour
{
	[Serializable]
	public class Slideset
	{
		public string set_name;

		public List<Texture2D> slides = new List<Texture2D>();
	}

	public Texture2D[] controller_textures = new Texture2D[0];

	public Texture2D[] keyboard_textures = new Texture2D[0];

	public static Dictionary<Texture2D, Texture2D> slide_dictionary = new Dictionary<Texture2D, Texture2D>();

	protected static TutorialSlides _instance;

	public List<Slideset> _slide_sets = new List<Slideset>();

	public Dictionary<string, Slideset> _slideset_dictionary = new Dictionary<string, Slideset>();

	public static TutorialSlides instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(TutorialSlides)) as TutorialSlides;
			}
			return _instance;
		}
	}

	public static List<Slideset> slide_sets
	{
		get
		{
			return instance._slide_sets;
		}
	}

	public static Dictionary<string, Slideset> slideset_dictionary
	{
		get
		{
			return instance._slideset_dictionary;
		}
	}

	public static Texture2D GetKeyboardSlide(Texture2D slide)
	{
		if (slide_dictionary.ContainsKey(slide))
		{
			return slide_dictionary[slide];
		}
		return slide;
	}

	private void Awake()
	{
		slide_dictionary.Clear();
		slideset_dictionary.Clear();
		for (int i = 0; i < controller_textures.Length && i < keyboard_textures.Length; i++)
		{
			AddSlide(controller_textures[i], keyboard_textures[i]);
		}
		foreach (Slideset slide_set in slide_sets)
		{
			AddSlideSet(slide_set.set_name, slide_set);
		}
	}

	public void AddSlide(Texture2D tex1, Texture2D tex2)
	{
		if (!slide_dictionary.ContainsKey(tex1))
		{
			slide_dictionary.Add(tex1, tex2);
		}
	}

	public void AddSlideSet(string setname, Slideset sset)
	{
		if (!slideset_dictionary.ContainsKey(setname))
		{
			slideset_dictionary.Add(setname, sset);
		}
	}
}
