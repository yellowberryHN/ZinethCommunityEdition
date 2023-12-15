using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public AudioClip[] tracks;

	public AudioClip[] tracksReversed;

	private static int tracksLength;

	private int[] originalPositions;

	public int currentTrack;

	public float maxVolume = 1f;

	public float reversePitch = 1.2f;

	public float normalPitch = 1f;

	private bool hasStarted;

	private bool forward = true;

	private bool isPaused;

	private new static AudioSource audio;

	private float upwardsAirMovementLength;

	private float startFadeTime;

	private float fadeOutAmount = 0.005f;

	private float fadeInAmount = 0.01f;

	private float fadeHeight = 300f;

	private bool startedFadeBack;

	private float minVolume = 0.3f;

	private AudioSource windAudio;

	private static MusicManager _instance;

	public static float override_vol = 1f;

	private static float _base_vol;

	private SpawnPointScript spScript
	{
		get
		{
			return PhoneInterface.spawn_point_script;
		}
	}

	public static MusicManager instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(MusicManager)) as MusicManager;
			}
			return _instance;
		}
	}

	public static float base_vol
	{
		get
		{
			return _base_vol;
		}
		set
		{
			_base_vol = value;
			float num = 1f;
			if (PhoneMemory.initialized)
			{
				num = PhoneMemory.settings.music_volume;
			}
			audio.volume = _base_vol * num * override_vol;
		}
	}

	public static bool show_debug_gui
	{
		get
		{
			return (bool)instance && instance.useGUILayout;
		}
		set
		{
			if ((bool)instance)
			{
				instance.useGUILayout = value;
			}
		}
	}

	private void Awake()
	{
		if (Application.loadedLevelName != "test")
		{
			override_vol = 1f;
		}
		windAudio = GameObject.Find("WindSource").GetComponent<AudioSource>();
		audio = base.gameObject.GetComponent<AudioSource>();
		tracksLength = tracks.Length;
		originalPositions = new int[tracksLength];
		for (int i = 0; i < tracks.Length; i++)
		{
			originalPositions[i] = i;
		}
		RandomizeMusic();
		base_vol = base_vol;
		base.useGUILayout = false;
	}

	private void FixedUpdate()
	{
		if ((bool)PhoneInterface.hawk && PhoneInterface.hawk.active && PhoneInterface.hawk.targetHeld)
		{
			float num = maxVolume;
			num = ((!PhoneInterface.hawk.canControl) ? 800f : PhoneInterface.hawk.spd);
			windAudio.volume = maxVolume * Mathf.Clamp(num / 800f, 0.1f, 0.25f);
		}
		else if (!spScript.isRespawning)
		{
			AirFade();
		}
		if (!hasStarted)
		{
			hasStarted = true;
			forward = true;
			base_vol = maxVolume;
			if (tracks.Length > 0)
			{
				audio.clip = tracks[currentTrack];
				audio.Play();
			}
		}
		if ((bool)audio.clip && audio.time >= audio.clip.length - 0.25f)
		{
			PlayNext();
		}
	}

	private void RandomizeMusic()
	{
		for (int i = 0; i < tracks.Length; i++)
		{
			AudioClip audioClip = tracks[i];
			AudioClip audioClip2 = tracksReversed[i];
			int num = originalPositions[i];
			int num2 = Random.Range(i, tracks.Length);
			tracks[i] = tracks[num2];
			tracks[num2] = audioClip;
			tracksReversed[i] = tracksReversed[num2];
			tracksReversed[num2] = audioClip2;
			originalPositions[i] = originalPositions[num2];
			originalPositions[num2] = num;
		}
	}

	public void PlayNext()
	{
		currentTrack++;
		PlayTrack(currentTrack);
	}

	public void PlayPrevious()
	{
		currentTrack--;
		PlayTrack(currentTrack);
	}

	public void PlayTrack(int trackPos)
	{
		if (tracks.Length != 0)
		{
			if (trackPos < 0)
			{
				trackPos = tracks.Length - 1;
			}
			else if (trackPos >= tracks.Length)
			{
				trackPos = 0;
			}
			currentTrack = trackPos;
			audio.clip = tracks[currentTrack];
			audio.time = 0f;
			audio.loop = false;
			audio.Play();
		}
	}

	public void PlaySpecificTrack(int trackNo)
	{
		if (tracks.Length != 0)
		{
			if (trackNo < 0)
			{
				trackNo = tracks.Length - 1;
			}
			else if (trackNo >= tracks.Length)
			{
				trackNo = 0;
			}
			currentTrack = originalPositions[trackNo];
			audio.clip = tracks[currentTrack];
			audio.time = 0f;
			audio.loop = false;
			audio.Play();
		}
	}

	public void Pause()
	{
		isPaused = true;
		audio.Pause();
	}

	public void PlayForward()
	{
		if (tracks.Length != 0)
		{
			if (!forward)
			{
				isPaused = false;
				forward = true;
				float time = audio.time;
				audio.clip = tracks[currentTrack];
				audio.time = audio.clip.length - time;
				audio.pitch = normalPitch;
				audio.Play();
			}
			else if (isPaused)
			{
				isPaused = false;
				audio.Play();
			}
		}
	}

	public void PlayReversed()
	{
		if (tracks.Length != 0)
		{
			if (forward)
			{
				isPaused = false;
				forward = false;
				float time = audio.time;
				audio.clip = tracksReversed[currentTrack];
				audio.time = audio.clip.length - time;
				audio.pitch = reversePitch;
				audio.Play();
			}
			else if (isPaused)
			{
				isPaused = false;
				audio.Play();
			}
		}
	}

	private void AirFade()
	{
		bool grounded = base.gameObject.GetComponent<move>().grounded;
		bool wallRiding = base.gameObject.GetComponent<move>().wallRiding;
		bool isGrinding = base.gameObject.GetComponent<move>().isGrinding;
		if (!grounded && !wallRiding && !isGrinding && !startedFadeBack)
		{
			upwardsAirMovementLength += Time.deltaTime;
			RaycastHit hitInfo;
			if (!Physics.Raycast(base.transform.position, -Vector3.up, out hitInfo))
			{
				return;
			}
			float distance = hitInfo.distance;
			if (distance >= fadeHeight)
			{
				if (upwardsAirMovementLength >= startFadeTime && base_vol > minVolume)
				{
					FadeOut();
				}
			}
			else if (base_vol < 1f)
			{
				startedFadeBack = true;
			}
			return;
		}
		upwardsAirMovementLength = 0f;
		if (base_vol < 1f)
		{
			if (!grounded)
			{
				FadeIn();
				return;
			}
			base_vol = 1f;
			ControlWind();
		}
		else
		{
			startedFadeBack = false;
		}
	}

	private void FadeOut()
	{
		base_vol -= fadeOutAmount;
		ControlWind();
	}

	private void FadeIn()
	{
		startedFadeBack = true;
		base_vol += fadeInAmount;
		ControlWind();
	}

	private void ControlWind()
	{
		if (!PhoneInterface.hawk || !PhoneInterface.hawk.active || !PhoneInterface.hawk.targetHeld)
		{
			windAudio.volume = maxVolume - base_vol;
		}
	}

	public static void PauseMusic()
	{
		instance.Pause();
	}

	public static void PlayMusic()
	{
		instance.PlayForward();
	}

	public static void PlayMusicTrack(int ind)
	{
		instance.PlayTrack(ind);
	}

	public static void PlayNextTrack()
	{
		instance.PlayNext();
	}

	public static void PlayPreviousTrack()
	{
		instance.PlayPrevious();
	}

	public static void ShuffleTracks()
	{
		instance.RandomizeMusic();
	}

	private void OnGUI()
	{
		if (!show_debug_gui)
		{
			return;
		}
		float num = GUILayout.HorizontalSlider(audio.time, 0f, audio.clip.length, GUILayout.Width(audio.clip.length));
		if (Mathf.Abs(num - audio.time) > 2f)
		{
			MonoBehaviour.print("playing track at " + (int)num);
			audio.time = num;
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("|<"))
		{
			PlayPrevious();
		}
		if (GUILayout.Button(">|"))
		{
			PlayNext();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		for (int i = 0; i < tracks.Length; i++)
		{
			GUILayout.BeginHorizontal();
			string text = tracks[i].name;
			if (currentTrack == i)
			{
				text = "->" + text;
			}
			if (GUILayout.Button(text))
			{
				PlayTrack(i);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
