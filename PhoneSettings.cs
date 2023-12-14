using UnityEngine;

public class PhoneSettings
{
	public PhoneColorPallete pallete;

	private int _allow_twitter = -1;

	public bool muted;

	private float _master_volume = -1f;

	public float _menu_volume = -1f;

	public float _ring_volume = -1f;

	public float _game_volume = -1f;

	public float _music_volume = -1f;

	public float _vibrate_amount = -1f;

	public Color backgroundColor
	{
		get
		{
			return pallete.back;
		}
	}

	public Color textColor
	{
		get
		{
			return pallete.text;
		}
	}

	public Color selectedTextColor
	{
		get
		{
			return pallete.selected;
		}
	}

	public Color selectableTextColor
	{
		get
		{
			return pallete.selectable;
		}
	}

	public Color particleColor
	{
		get
		{
			if (pallete.particles == new Color(0f, 0f, 0f, 0f))
			{
				return pallete.selected;
			}
			return pallete.particles;
		}
	}
	
	public Color mailColor
	{
		get
		{
			if (pallete.mail == new Color(0f, 0f, 0f, 0f))
			{
				return new Color {r = 0.9764706f, g = 0.9960784f, b = 0.7411765f, a = 1f};
			}
			return pallete.mail;
		}
	}

	public bool allow_twitter
	{
		get
		{
			if (_allow_twitter == -1)
			{
				_allow_twitter = PlayerPrefs.GetInt("allow_twitter", 1);
			}
			return _allow_twitter > 0;
		}
		set
		{
			if (value)
			{
				_allow_twitter = 1;
			}
			else
			{
				_allow_twitter = 0;
			}
			PlayerPrefs.SetInt("allow_twitter", _allow_twitter);
		}
	}

	public float master_volume
	{
		get
		{
			if (_master_volume == -1f)
			{
				_master_volume = PlayerPrefs.GetFloat("volume_master", 0.75f);
				AudioListener.volume = _master_volume;
			}
			return _master_volume;
		}
		set
		{
			_master_volume = value;
			PlayerPrefs.SetFloat("volume_master", _master_volume);
			AudioListener.volume = _master_volume;
		}
	}

	public float menu_volume
	{
		get
		{
			if (_menu_volume == -1f)
			{
				_menu_volume = PlayerPrefs.GetFloat("volume_menu", 1f);
			}
			return _menu_volume;
		}
		set
		{
			_menu_volume = value;
			PlayerPrefs.SetFloat("volume_menu", _menu_volume);
		}
	}

	public float ring_volume
	{
		get
		{
			if (_ring_volume == -1f)
			{
				_ring_volume = PlayerPrefs.GetFloat("volume_ring", 1f);
			}
			return _ring_volume;
		}
		set
		{
			_ring_volume = value;
			PlayerPrefs.SetFloat("volume_ring", _ring_volume);
		}
	}

	public float game_volume
	{
		get
		{
			if (_game_volume == -1f)
			{
				_game_volume = PlayerPrefs.GetFloat("volume_game", 1f);
			}
			return _game_volume;
		}
		set
		{
			_game_volume = value;
			PlayerPrefs.SetFloat("volume_game", _game_volume);
		}
	}

	public float music_volume
	{
		get
		{
			if (_music_volume == -1f)
			{
				_music_volume = PlayerPrefs.GetFloat("volume_music", 0.5f);
			}
			return _music_volume;
		}
		set
		{
			_music_volume = value;
			PlayerPrefs.SetFloat("volume_music", _music_volume);
			MusicManager.base_vol = MusicManager.base_vol;
		}
	}

	public float vibrate_amount
	{
		get
		{
			if (_vibrate_amount == -1f)
			{
				_vibrate_amount = PlayerPrefs.GetFloat("volume_vibrate", 0.75f);
			}
			return _vibrate_amount;
		}
		set
		{
			_vibrate_amount = value;
			PlayerPrefs.SetFloat("volume_vibrate", _vibrate_amount);
		}
	}
}
