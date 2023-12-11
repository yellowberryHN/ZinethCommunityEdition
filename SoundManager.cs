using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioClip grind;

	public AudioClip jump;

	public AudioClip skate;

	public AudioClip skateSand;

	public AudioClip wallRide;

	public AudioClip rewind;

	public AudioClip boost;

	public float maxVolume = 1f;

	public string currentSound = "none";

	public bool muted;

	public bool rewindPlaying;

	private new AudioSource audio;

	private void Awake()
	{
		audio = base.gameObject.GetComponent<AudioSource>();
	}

	public void PlayGrind()
	{
		if (!muted)
		{
			rewindPlaying = false;
			audio.clip = grind;
			audio.loop = true;
			audio.volume = maxVolume;
			audio.Play();
			currentSound = "grind";
		}
	}

	public void PlayJump()
	{
		if (!muted)
		{
			AudioSource.PlayClipAtPoint(jump, new Vector3(5f, 1f, 2f));
		}
	}

	public void PlayBoost()
	{
		if (!muted)
		{
			AudioSource.PlayClipAtPoint(boost, new Vector3(5f, 1f, 2f));
		}
	}

	public void PlaySkate()
	{
		if (!muted)
		{
			rewindPlaying = false;
			audio.clip = skate;
			audio.loop = true;
			audio.volume = maxVolume;
			audio.Play();
			currentSound = "skate";
		}
	}

	public void PlaySkateSand()
	{
		if (!muted)
		{
			rewindPlaying = false;
			audio.clip = skateSand;
			audio.loop = true;
			audio.volume = maxVolume - 0.5f;
			audio.Play();
			currentSound = "skateSand";
		}
	}

	public void PlayWallRide()
	{
		if (!muted)
		{
			rewindPlaying = false;
			audio.clip = wallRide;
			audio.loop = true;
			audio.volume = maxVolume;
			audio.Play();
			currentSound = "wallRide";
		}
	}

	public void PlayRewind()
	{
		if (!rewindPlaying)
		{
			rewindPlaying = true;
			audio.clip = rewind;
			audio.loop = true;
			audio.volume = maxVolume;
			audio.Play();
			currentSound = "rewind";
		}
	}

	public void PauseRewind()
	{
		if (rewindPlaying)
		{
			rewindPlaying = false;
			audio.Pause();
		}
	}

	public void StopSound()
	{
		audio.Stop();
		currentSound = "none";
		rewindPlaying = false;
	}
}
