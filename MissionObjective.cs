using UnityEngine;

public class MissionObjective : MonoBehaviour
{
	public bool completed;

	public bool failed;

	public float timeLimit = -1f;

	public bool use_position = true;

	public bool block_next = true;

	public bool play_sound = true;

	private AudioClip sound_clip;

	public bool throw_zine;

	public ThrownZine zine_prefab;

	public string objectiveName;

	public static Transform player;

	public bool requireTrigger;

	public bool triggered;

	public Collider triggerCollider;

	public bool requireGrounded;

	public bool requireNotGrounded;

	public bool clearRewindOnStart;

	public bool clearRewindOnComplete;

	public bool show_guitext;

	public bool skipAsCurrent;

	public MissionObjective[] completeCondition;

	public static move _playerMove;

	private int condition_progress;

	public Transform[] unparent_on_end;

	public Transform[] destroy_on_end;

	private bool _has_left;

	public Vector3 objectivePosition
	{
		get
		{
			return base.transform.position;
		}
	}

	private void Awake()
	{
		Setup();
	}

	public virtual void Setup()
	{
		if (!player)
		{
			player = PhoneInterface.player_trans;
		}
		if (requireTrigger && !triggerCollider)
		{
			triggerCollider = GetComponentInChildren<Collider>();
		}
		if ((bool)triggerCollider)
		{
			CollisionChecker collisionChecker = triggerCollider.gameObject.AddComponent<CollisionChecker>();
			collisionChecker.TriggerEnterDelegate = TriggerEnter;
			collisionChecker.TriggerExitDelegate = TriggerExit;
		}
		if (!_playerMove && (bool)player)
		{
			_playerMove = player.GetComponent<move>();
		}
	}

	public virtual bool CheckCompleted()
	{
		if (show_guitext)
		{
			DoGUIText();
		}
		return CheckTriggered() && CheckConditions() && CheckGrounded();
	}

	public virtual bool CheckConditions()
	{
		condition_progress = 0;
		MissionObjective[] array = completeCondition;
		foreach (MissionObjective missionObjective in array)
		{
			if (!missionObjective.completed)
			{
				return false;
			}
			condition_progress++;
		}
		return true;
	}

	public virtual bool CheckTriggered()
	{
		bool result = !requireTrigger || triggered;
		if (_has_left)
		{
			_has_left = false;
			triggered = false;
		}
		return result;
	}

	public virtual bool CheckGrounded()
	{
		if (requireGrounded)
		{
			return _playerMove.grounded;
		}
		if (requireNotGrounded)
		{
			return !_playerMove.grounded;
		}
		return true;
	}

	public virtual void OnBegin()
	{
		triggered = false;
		completed = false;
		failed = false;
		base.gameObject.SetActiveRecursively(true);
		if (clearRewindOnStart)
		{
			SpawnPointScript.instance.ClearSpawns();
		}
	}

	public virtual void OnCompleted()
	{
		if (play_sound)
		{
			PlayCompletedSound();
		}
		if (throw_zine)
		{
			for (int i = 0; i < 8; i++)
			{
				ThrowZine();
			}
		}
		Transform[] array = destroy_on_end;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActiveRecursively(false);
		}
		base.gameObject.active = false;
		if (clearRewindOnComplete)
		{
			SpawnPointScript.instance.ClearSpawns();
		}
	}

	public virtual void OnEnd()
	{
		Transform[] array = unparent_on_end;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				transform.parent = null;
			}
		}
		Transform[] array2 = destroy_on_end;
		foreach (Transform transform2 in array2)
		{
			if ((bool)transform2)
			{
				Object.Destroy(transform2.gameObject);
			}
		}
		base.gameObject.active = false;
	}

	public virtual string GetText()
	{
		return ParseGUIString(objectiveName);
	}

	public virtual string ParseGUIString(string guistring)
	{
		guistring = guistring.Replace("{cond_progress}", condition_progress.ToString());
		guistring = guistring.Replace("{cond_progress+}", (condition_progress + 1).ToString());
		guistring = guistring.Replace("{cond_total}", completeCondition.Length.ToString());
		return guistring;
	}

	public virtual void DoGUIText()
	{
		MissionController.guitext = MissionController.guitext + GetText() + "\n";
	}

	public virtual void ThrowZine()
	{
		ThrownZine original = ((!zine_prefab) ? MissionController.thrown_zine_prefab : zine_prefab);
		Vector3 position = player.position + player.up * 0.8f + player.forward * 1f;
		ThrownZine thrownZine = Object.Instantiate(original, position, player.rotation) as ThrownZine;
		thrownZine.Init(player.rigidbody.velocity);
	}

	public virtual void PlayCompletedSound()
	{
		AudioClip audioClip = ((!sound_clip) ? MissionController.checkpoint_sound : sound_clip);
		if (!(audioClip == null))
		{
			AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
		}
	}

	protected virtual void TriggerEnter(Collider other)
	{
		if (other.gameObject.name == "Player")
		{
			triggered = true;
		}
	}

	protected virtual void TriggerExit(Collider other)
	{
		if (other.gameObject.name == "Player")
		{
			_has_left = true;
		}
	}
}
