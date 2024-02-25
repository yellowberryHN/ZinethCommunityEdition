using System;
using UnityEngine;

[Serializable]
public class UpdateZone : MonoBehaviour
{
	public virtual void Awake()
	{
		ZoneLoader.SetPlayerTransform(GameObject.Find("Player").transform);
	}
}
