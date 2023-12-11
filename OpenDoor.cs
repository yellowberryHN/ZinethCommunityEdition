using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	public Transform door;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player")
		{
			door.animation.Play();
			Object.Destroy(base.gameObject);
		}
	}
}
