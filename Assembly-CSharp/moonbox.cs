using UnityEngine;

public class moonbox : MonoBehaviour
{
	public PhoneMail mail;

	private bool once = true;

	private void Start()
	{
	}

	private void OnCollisionEnter(Collision obj)
	{
		if (obj.gameObject.name == "Player" && once)
		{
			once = false;
			MailController.SendMail(mail);
		}
	}
}
