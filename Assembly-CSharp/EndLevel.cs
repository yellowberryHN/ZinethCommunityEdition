using System.Collections;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
	public Transform door;

	private IEnumerator Go()
	{
		yield return new WaitForSeconds(door.animation.clip.length);
		
		// stop the timer after the door closes
		if(SpeedrunTimer.instance != null) SpeedrunTimer.instance.StopTimer();
		
		yield return new WaitForSeconds(2f - door.animation.clip.length);
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
