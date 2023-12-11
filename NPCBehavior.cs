using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
	public string npc_name;

	public Transform icon_bubble;

	public bool sendMessageOnFall2 = true;

	public PhoneMail newmail;

	private float resetDistance = 500f;

	private float rotationMin = 0.5f;

	private bool fallenOver;

	private bool messageSent;

	private bool justFell;

	private Vector3 originalPosition = new Vector3(0f, 0f, 0f);

	private Quaternion originalRotation = new Quaternion(0f, 0f, 0f, 0f);

	private static readonly string[] fallmail = new string[13]
	{
		"Hey!|You jerk! Watch where you're going!", "CHEATER!!|trying to mess up my game by knocking me over? might work if I wasn't SUPER PRO at this anyway SCREW YOU", "wtf???|Are you the one with that weird skating machine? Learn to steer it before you ride it around normal people!", "please be careful!|I think you knocked me down! could you maybe not do that? :(", "HEY JERKASS|NICE DRIVING, ALSO THAT WAS SARCASM", "whoa man|watch it man like slow down jeez", "excuse me??|If your gonna bump into me like that you could at least apologize???", "so cool!|hey u prolly don't remember me but you sort of ran me over with your mech? but its cool i never saw a machine like that before!! ^^", "RUDE|hitting people with your robot: rude!", "hit and run|I should report you for running into me! but who wants to deal with filing reports... you're getting lucky here!",
		"crazy machine|What the hell is that thing you're driving around and do you need a license to drive it? cause if you do YOU SHOULD HAVE YOURS REVOKED hint hint", "{l}*** YOU|I JUST GOT A NEW PHONE AND YOU WRECKED THE SCREEN WITH YOUR STUPID ROBOT i hope you CRASH >:(", "warning|no one told me a big mech was going to be rolling around and knocking people over  : /"
	};

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		originalPosition = base.transform.position;
		originalRotation = base.transform.rotation;
		if (npc_name == string.Empty)
		{
			if (base.name.StartsWith("NPC_Trainer_"))
			{
				npc_name = base.name.Replace("NPC_Trainer_", RandomNamePart() + " ");
			}
			else
			{
				npc_name = RandomName();
			}
		}
		if (sendMessageOnFall2)
		{
			if (newmail.body == string.Empty)
			{
				newmail = RandomFallMail(newmail);
			}
			if (newmail.id == string.Empty)
			{
				newmail.id = "npc_" + GetInstanceID();
			}
			if (newmail.sender == string.Empty)
			{
				newmail.sender = npc_name;
			}
			MailController.AddMail(newmail);
		}
		if (!base.animation)
		{
			return;
		}
		foreach (AnimationState item in base.animation)
		{
			item.speed = 0f;
		}
	}

	public static PhoneMail RandomFallMail(PhoneMail mail)
	{
		string text = fallmail[Random.Range(0, fallmail.Length)];
		text = text.Replace("{l}", ((char)(97 + Random.Range(0, 26))).ToString());
		string[] array = text.Split('|');
		mail.subject = array[0];
		mail.body = array[1];
		return mail;
	}

	public static string RandomNamePart()
	{
		return MonsterTraits.Name.createFirstName();
	}

	public static string RandomName()
	{
		return MonsterTraits.Name.createFullName();
	}

	public void ShowBubble()
	{
		SetBubbleVisible(true);
	}

	public void HideBubble()
	{
		SetBubbleVisible(false);
	}

	public void SetBubbleVisible(bool isvis)
	{
		if ((bool)icon_bubble && icon_bubble.gameObject.active != isvis)
		{
			icon_bubble.gameObject.active = isvis;
		}
	}

	public virtual void SetBubbleTexture(Texture2D tex)
	{
		if ((bool)icon_bubble)
		{
			icon_bubble.renderer.material.mainTexture = tex;
		}
	}

	private void FixedUpdate()
	{
		float num = Mathf.Abs(base.transform.up.y);
		if (num <= rotationMin)
		{
			fallenOver = true;
		}
		if (fallenOver)
		{
			float num2 = Vector3.Distance(base.transform.position, GameObject.Find("Player").transform.position);
			if (num2 >= resetDistance)
			{
				Reset();
			}
			if (!messageSent)
			{
				SendFallMail(Random.Range(0.5f, 4f));
			}
			if (!justFell)
			{
				JustFell();
			}
		}
	}

	private void Reset()
	{
		base.transform.position = originalPosition;
		base.transform.rotation = originalRotation;
		fallenOver = false;
		justFell = false;
	}

	private void JustFell()
	{
		justFell = true;
	}

	protected void SendFallMail()
	{
		messageSent = true;
		if (sendMessageOnFall2)
		{
			MailController.SendMail(newmail);
		}
	}

	private void SendFallMail(float time)
	{
		messageSent = true;
		Invoke("SendFallMail", time);
	}
}
