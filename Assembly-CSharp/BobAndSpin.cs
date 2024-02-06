using UnityEngine;

public class BobAndSpin : MonoBehaviour
{
	public float bobSpeed = 3f;

	public float maxBob = 2f;

	public float spinSpeed = 50f;

	private bool bobUp = true;

	private float currentBob;

	private void Update()
	{
		BS();
	}

	private void BS()
	{
		transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
		if (bobUp)
		{
			if (currentBob < maxBob)
			{
				currentBob += bobSpeed * Time.deltaTime;
				transform.position += new Vector3(0f, bobSpeed * Time.deltaTime, 0f);
			}
			else
			{
				bobUp = false;
			}
		}
		else if (currentBob > 0f)
		{
			currentBob -= bobSpeed * Time.deltaTime;
			transform.position -= new Vector3(0f, bobSpeed * Time.deltaTime, 0f);
		}
		else
		{
			bobUp = true;
		}
	}
}
