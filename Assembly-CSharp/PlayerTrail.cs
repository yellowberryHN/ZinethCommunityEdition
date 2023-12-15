using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
	public float decayTime;

	public Material trailMaterial;

	public float startWidth;

	public float endWidth;

	public List<Transform> trailHolderList = new List<Transform>();

	public List<GameObject> holderObjectList = new List<GameObject>();

	public List<TrailRenderer> trailList = new List<TrailRenderer>();

	public List<Vector3> lastPointList = new List<Vector3>();

	private int trailLength = 1;

	private float currentTime;

	private static bool _set_color;

	private static Color _color;

	public Color color
	{
		get
		{
			Debug.Log("trail color get: "+_color);
			if (!_set_color)
			{
				_color = trailList[0].renderer.material.color;
				_set_color = true;
			}
			return _color;
		}
		set
		{
			Debug.Log("trail color set: "+value);
			value.a = 0.71f;
			_color = value;
			foreach (TrailRenderer trail in trailList)
			{
				trail.renderer.material.color = color;
			}
		}
	}

	private void Awake()
	{
		currentTime = decayTime;
		foreach (Transform trailHolder in trailHolderList)
		{
			GameObject gameObject = new GameObject("trailHolder");
			gameObject.transform.position = trailHolder.position;
			holderObjectList.Add(gameObject);
			gameObject.layer = 2;
			gameObject.AddComponent<TrailRenderer>();
			gameObject.GetComponent<TrailRenderer>().material = trailMaterial;
			gameObject.GetComponent<TrailRenderer>().startWidth = startWidth;
			gameObject.GetComponent<TrailRenderer>().endWidth = endWidth;
			gameObject.GetComponent<TrailRenderer>().time = decayTime;
			gameObject.transform.parent = trailHolder;
			trailList.Add(gameObject.GetComponent<TrailRenderer>());
			lastPointList.Add(trailHolder.position);
		}
		SetColor(color);
	}

	public void SetColor(Color col)
	{
		color = col;
	}
}
