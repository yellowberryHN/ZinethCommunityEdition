using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
	public delegate void OnTriggerEnterDelegate(Collider collider);

	public delegate void OnTriggerExitDelegate(Collider collider);

	protected OnTriggerEnterDelegate _triggerEnterDelegate;

	protected OnTriggerExitDelegate _triggerExitDelegate;

	public OnTriggerEnterDelegate TriggerEnterDelegate
	{
		set
		{
			_triggerEnterDelegate = value;
		}
	}

	public OnTriggerExitDelegate TriggerExitDelegate
	{
		set
		{
			_triggerExitDelegate = value;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_triggerEnterDelegate != null)
		{
			_triggerEnterDelegate(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_triggerExitDelegate != null)
		{
			_triggerExitDelegate(other);
		}
	}
}
