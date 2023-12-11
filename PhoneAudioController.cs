using UnityEngine;

public class PhoneAudioController : MonoBehaviour
{
	private static AudioListener _listener;

	public AudioClip clip_accept;

	public AudioClip clip_back;

	public AudioClip clip_open;

	public AudioClip clip_close;

	public AudioClip clip_ring_short;

	public AudioClip clip_ring_long;

	public AudioClip clip_bad;

	public static AudioSource gobj_ring;

	public AudioClip clip_game_win;

	public AudioClip clip_game_lose;

	public AudioClip clip_game_attack;

	public AudioClip clip_game_hit;

	public AudioClip clip_game_enemy_die;

	public AudioClip clip_game_health_up;

	public AudioClip clip_new_app;

	private static PhoneAudioController _instance;

	public static AudioListener listener
	{
		get
		{
			if (!_listener)
			{
				_listener = Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
			}
			return _listener;
		}
	}

	public static PhoneAudioController instance
	{
		get
		{
			return audcon;
		}
	}

	public static PhoneAudioController audcon
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneAudioController)) as PhoneAudioController;
			}
			return _instance;
		}
	}

	private void Start()
	{
	}

	public static float GetTypeVolume(SoundType type)
	{
		return SubGetTypeVolume(type);
	}

	private static float SubGetTypeVolume(SoundType type)
	{
		switch (type)
		{
		case SoundType.menu:
			return PhoneMemory.settings.menu_volume;
		case SoundType.ring:
			return PhoneMemory.settings.ring_volume;
		case SoundType.game:
			return PhoneMemory.settings.game_volume;
		case SoundType.music:
			return PhoneMemory.settings.music_volume;
		case SoundType.other:
			return 1f;
		default:
			Debug.LogWarning("Unknown soundtype: " + type);
			return 0f;
		}
	}

	public static AudioClip LoadClip(string name)
	{
		return audcon.LoadClipLocal(name);
	}

	public AudioClip LoadClipLocal(string name)
	{
		switch (name)
		{
		case "attack":
			if ((bool)clip_game_attack)
			{
				return clip_game_attack;
			}
			break;
		case "die":
		case "lose":
			if ((bool)clip_game_lose)
			{
				return clip_game_lose;
			}
			break;
		case "win":
			if ((bool)clip_game_win)
			{
				return clip_game_win;
			}
			break;
		case "hit":
			if ((bool)clip_game_hit)
			{
				return clip_game_hit;
			}
			break;
		case "enemy_die":
			if ((bool)clip_game_enemy_die)
			{
				return clip_game_enemy_die;
			}
			break;
		case "health_up":
			if ((bool)clip_game_health_up)
			{
				return clip_game_health_up;
			}
			break;
		case "click":
		case "accept":
			if ((bool)clip_accept)
			{
				return clip_accept;
			}
			break;
		case "back":
			if ((bool)clip_back)
			{
				return clip_back;
			}
			break;
		case "open":
			if ((bool)clip_open)
			{
				return clip_open;
			}
			break;
		case "new_app":
			if ((bool)clip_new_app)
			{
				return clip_new_app;
			}
			break;
		}
		return Resources.Load("Phone/" + name) as AudioClip;
	}

	public static AudioSource PlayAudioClip(string name)
	{
		return PlayAudioClip(LoadClip(name));
	}

	public static AudioSource PlayAudioClip(AudioClip clip)
	{
		return PlayAudioClip(clip, SoundType.other);
	}

	public static AudioSource PlayAudioClip(string name, SoundType type)
	{
		return PlayAudioClip(LoadClip(name), type);
	}

	public static AudioSource PlayAudioClip(AudioClip clip, SoundType type)
	{
		float typeVolume = GetTypeVolume(type);
		return PlayAudioClip(clip, typeVolume);
	}

	public static AudioSource PlayAudioClip(AudioClip clip, float volume)
	{
		return PlayAudioClip(clip, Vector3.zero, volume);
	}

	public static AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume)
	{
		if (clip == null)
		{
			Debug.LogWarning("audio clip is null...");
			return null;
		}
		GameObject gameObject = new GameObject("Audio-" + clip.name);
		gameObject.transform.parent = listener.transform;
		gameObject.transform.localPosition = position;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.Play();
		Object.Destroy(gameObject, clip.length);
		return audioSource;
	}

	public static bool StartRinging()
	{
		if (gobj_ring == null)
		{
			gobj_ring = PlayAudioClip(audcon.clip_ring_short, SoundType.ring);
			return true;
		}
		if (!gobj_ring.isPlaying)
		{
			gobj_ring.Play();
			return true;
		}
		return false;
	}
}
