using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
	public Transform leftDoor;

	public Transform rightDoor;

	private int doorIncrement;

	private Transform player;

	public bool wallJump;

	private int timesJumped;

	private bool wallJumped;

	private void Start()
	{
		player = GameObject.Find("Player").transform;
	}

	private void Open2()
	{
		MonoBehaviour.print("sup");
		StartCoroutine("Open");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (wallJump)
		{
			StartCoroutine("WallJump");
		}
		else
		{
			StartCoroutine("Open");
		}
	}

	private IEnumerator Open()
	{
		while (doorIncrement < 25)
		{
			leftDoor.position += leftDoor.right / 4f;
			rightDoor.position += leftDoor.right / -4f;
			doorIncrement++;
			yield return null;
		}
		while (doorIncrement > 21)
		{
			leftDoor.position -= leftDoor.right / 4f;
			rightDoor.position -= leftDoor.right / -4f;
			doorIncrement--;
			yield return null;
		}
		while (doorIncrement < 25)
		{
			leftDoor.position += leftDoor.right / 4f;
			rightDoor.position += leftDoor.right / -4f;
			doorIncrement++;
			yield return null;
		}
	}

	private IEnumerator WallJump()
	{
		while (timesJumped < 6)
		{
			bool temp = player.GetComponent<move>().wallRiding;
			if (temp != wallJumped)
			{
				wallJumped = temp;
				timesJumped++;
			}
			yield return null;
		}
		StartCoroutine("Open");
	}
}
