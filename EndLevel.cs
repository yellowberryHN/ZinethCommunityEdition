using System.Collections;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
	public Transform door;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private IEnumerator Go()
	{
		yield return new WaitForSeconds(2f);
		Application.LoadLevel("Loader 1");
	}

	private IEnumerator OnTriggerEnter(Collider other)
	{
		GameObject.Find("Camera Holder").GetComponent<NewCamera>().pauseCamera = true;
		GameObject.Find("Player").GetComponent<move>().freezeControls = true;
		door.animation.Play();
		yield return StartCoroutine("Go");
	}
}
