using System.Collections;
using UnityEngine;

public class PyramidScript : MonoBehaviour
{
	public stateEnum state;

	public int counter = 3;

	public float animationSpeed = 0.1f;

	private int animationCounter = 1;

	private Action[] queuedAction;

	private int queuedIndex;

	private float offset;

	private float timeRemaining;

	public Texture2D[] alertSlides = new Texture2D[0];

	public Texture2D[] blinkSlides = new Texture2D[0];

	public Texture2D[] excitedSlides = new Texture2D[0];

	public Texture2D[] frownSlides = new Texture2D[0];

	public Texture2D[] happySlides = new Texture2D[0];

	public Texture2D[] lovesSlides = new Texture2D[0];

	public Texture2D[] smileSlides = new Texture2D[0];

	public Texture2D[] talkSlides = new Texture2D[0];

	public Texture2D[][] anis;

	public Transform eye;

	public Vector3[] alertPos = new Vector3[0];

	public Vector3[] blinkPos = new Vector3[0];

	public Vector3[] excitedPos = new Vector3[0];

	public Vector3[] frownPos = new Vector3[0];

	public Vector3[] happyPos = new Vector3[0];

	public Vector3[] lovesPos = new Vector3[0];

	public Vector3[] smilePos = new Vector3[0];

	public Vector3[] talkPos = new Vector3[0];

	public Vector3[][] pos;

	public void SwitchAction(Action[] newAction)
	{
		queuedAction = newAction;
		queuedIndex = 0;
		SwitchAnimation(queuedAction[queuedIndex].mood);
		timeRemaining = queuedAction[queuedIndex].length;
	}

	public void SwitchAnimation(stateEnum newState)
	{
		state = newState;
		animationCounter = 0;
		if (state == stateEnum.alert)
		{
			counter = 0;
		}
		else if (state == stateEnum.blink)
		{
			counter = 1;
		}
		else if (state == stateEnum.excited)
		{
			counter = 2;
		}
		else if (state == stateEnum.frown)
		{
			counter = 3;
		}
		else if (state == stateEnum.happy)
		{
			counter = 4;
		}
		else if (state == stateEnum.love)
		{
			counter = 5;
		}
		else if (state == stateEnum.smile)
		{
			counter = 6;
		}
		else if (state == stateEnum.talk)
		{
			counter = 7;
		}
	}

	public void Start()
	{
		anis = new Texture2D[8][];
		anis[0] = alertSlides;
		anis[1] = blinkSlides;
		anis[2] = excitedSlides;
		anis[3] = frownSlides;
		anis[4] = happySlides;
		anis[5] = lovesSlides;
		anis[6] = smileSlides;
		anis[7] = talkSlides;
		pos = new Vector3[8][];
		pos[0] = alertPos;
		pos[1] = blinkPos;
		pos[2] = excitedPos;
		pos[3] = frownPos;
		pos[4] = happyPos;
		pos[5] = lovesPos;
		pos[6] = smilePos;
		pos[7] = talkPos;
		StartCoroutine("Animate");
		Action[] array = new Action[2]
		{
			new Action(stateEnum.alert, 2.3f, 0f),
			new Action(stateEnum.love, 4.3f, 0.4f)
		};
	}

	public void Update()
	{
		offset += Time.deltaTime;
		eye.renderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
	}

	public IEnumerator Animate()
	{
		while (true)
		{
			float pauseTime = 0f;
			if (queuedAction != null)
			{
				base.renderer.material.SetTexture("_MainTex", anis[counter][animationCounter]);
				eye.localPosition = pos[counter][animationCounter];
				animationCounter++;
				if (animationCounter >= anis[counter].Length)
				{
					pauseTime += queuedAction[queuedIndex].pause;
					animationCounter = 0;
				}
				timeRemaining -= animationSpeed;
				if (timeRemaining < 0f)
				{
					queuedIndex++;
					if (queuedIndex >= queuedAction.Length)
					{
						queuedIndex = 0;
					}
					SwitchAnimation(queuedAction[queuedIndex].mood);
					timeRemaining = queuedAction[queuedIndex].length;
				}
			}
			yield return new WaitForSeconds(animationSpeed + pauseTime);
		}
	}
}
