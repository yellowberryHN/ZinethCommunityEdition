using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Movie_Start : MonoBehaviour
{
	public float videoTimer;

	public void Start()
	{
		Debug.Log("Movie_Start");
	}
	
	public virtual void Update()
	{
		if (!((MovieTexture)renderer.material.mainTexture).isPlaying || !(videoTimer <= 13.3f))
		{
			((MovieTexture)renderer.material.mainTexture).Play();
			((MovieTexture)renderer.material.mainTexture).loop = true;
			videoTimer = 0f;
		}
		videoTimer += Time.deltaTime;
	}

	public virtual void Main()
	{
	}
}
