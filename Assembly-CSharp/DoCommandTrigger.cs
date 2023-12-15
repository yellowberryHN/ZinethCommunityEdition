using UnityEngine;

public class DoCommandTrigger : MonoBehaviour
{
	public string command_string = string.Empty;

	public bool destroy_on_activate = true;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void Activate()
	{
		if (Application.isEditor)
		{
			MonoBehaviour.print("activated trigger " + base.gameObject.name);
		}
		if (!string.IsNullOrEmpty(command_string))
		{
			PhoneController.DoPhoneCommand(command_string);
		}
		if ((bool)base.gameObject.GetComponent<SecretObject>())
		{
			SecretObject component = base.gameObject.GetComponent<SecretObject>();
			component.Found();
		}
		if (destroy_on_activate)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Player")
		{
			Activate();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player")
		{
			Activate();
		}
	}
}
