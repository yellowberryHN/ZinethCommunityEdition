using System;
using UnityEngine;

[Serializable]
public class PlayerGraphic : MonoBehaviour
{
	public bool isRespawning;

	private Transform player;

	private HawkBehavior hawk;

	public bool onMoon;

	private SpawnPointScript spawn;

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		hawk = GameObject.Find("Hawk").GetComponent<HawkBehavior>();
		spawn = GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>();
	}

	public virtual void Match()
	{
		MonoBehaviour.print("fuck");
		transform.position = player.position;
		if (isRespawning || hawk.targetHeld)
		{
			transform.rotation = player.rotation;
		}
		else
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, Time.fixedDeltaTime * 2f);
		}
	}

	public virtual void DoFixedUpdate()
	{
		if (hawk.targetHeld || onMoon)
		{
			transform.rotation = player.rotation;
			transform.position = player.position;
		}
		else if (isRespawning)
		{
			Quaternion rotation = player.rotation;
			transform.position = player.position;
			if (spawn.GetWallNormal() != Vector3.zero)
			{
				Quaternion quaternion;
				if (!(Vector3.Angle(player.right, spawn.GetWallNormal()) <= 90f))
				{
					transform.position = player.position + player.right;
					quaternion = Quaternion.AngleAxis(45f, player.forward);
				}
				else
				{
					transform.position = player.position - player.right;
					quaternion = Quaternion.AngleAxis(-45f, player.forward);
				}
				transform.rotation = quaternion * rotation;
			}
			else
			{
				transform.rotation = rotation;
			}
		}
		else if (player.GetComponent<move>().wallRiding)
		{
			Quaternion rotation2 = player.rotation;
			Quaternion quaternion;
			if (player.GetComponent<move>().wallRideRight)
			{
				transform.position = player.position - player.right;
				quaternion = Quaternion.AngleAxis(-45f, player.forward);
			}
			else
			{
				transform.position = player.position + player.right;
				quaternion = Quaternion.AngleAxis(45f, player.forward);
			}
			transform.rotation = quaternion * rotation2;
		}
		else
		{
			transform.position = player.position;
			float y = player.rotation.eulerAngles.y;
			Quaternion rotation3 = transform.rotation;
			Vector3 eulerAngles = rotation3.eulerAngles;
			float num = (eulerAngles.y = y);
			Vector3 vector2 = (rotation3.eulerAngles = eulerAngles);
			Quaternion quaternion3 = (transform.rotation = rotation3);
			transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, Time.fixedDeltaTime * 2f);
		}
	}
}
