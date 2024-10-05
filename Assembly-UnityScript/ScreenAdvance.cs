using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class ScreenAdvance : MonoBehaviour
{
	public string levelToLoad;

	private bool advance;

	private bool locked;

	public virtual void Start()
	{
		StartCoroutine(Lock());
	}

	public IEnumerator Lock()
	{
		yield return new WaitForSeconds(1f);

		locked = true;

		StartCoroutine(Input2());
	}
	

	public IEnumerator Input2()
	{
		while (true)
		{
			if (advance)
			{
				Application.LoadLevel(levelToLoad);
			}
		
			yield return null;
		}
	}

	public virtual void Update()
	{
		if (Input.anyKeyDown && locked)
		{
			advance = true;
		}
	}
}
