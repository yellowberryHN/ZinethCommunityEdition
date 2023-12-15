using UnityEngine;

public class DetailZone : MonoBehaviour
{
	public GameObject[] objects = new GameObject[0];

	public bool active_inside = true;

	public bool starting_state;

	public Collider on_collider;

	public Collider off_collider;

	private void Start()
	{
		if ((bool)on_collider)
		{
			CollisionChecker collisionChecker = on_collider.GetComponent<CollisionChecker>();
			if (collisionChecker == null)
			{
				collisionChecker = on_collider.gameObject.AddComponent<CollisionChecker>();
			}
			collisionChecker.TriggerEnterDelegate = OnTrigger;
		}
		if ((bool)off_collider)
		{
			CollisionChecker collisionChecker2 = off_collider.GetComponent<CollisionChecker>();
			if (collisionChecker2 == null)
			{
				collisionChecker2 = off_collider.gameObject.AddComponent<CollisionChecker>();
			}
			collisionChecker2.TriggerExitDelegate = OffTrigger;
		}
		SetState(starting_state);
	}

	public void TurnOn()
	{
		SetState(true);
	}

	public void TurnOff()
	{
		SetState(false);
	}

	public void SetState(bool state)
	{
		GameObject[] array = objects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(state);
		}
	}

	public void OnTrigger(Collider other)
	{
		if (other.name == "Player")
		{
			SetState(active_inside);
		}
	}

	public void OffTrigger(Collider other)
	{
		if (other.name == "Player")
		{
			SetState(!active_inside);
		}
	}
}
