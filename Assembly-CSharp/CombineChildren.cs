using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour
{
	public bool generateTriangleStrips = true;

	public bool deleteCollidier;

	public bool city;

	private void Start()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)componentsInChildren[i];
			Renderer renderer = componentsInChildren[i].renderer;
			MeshCombineUtility.MeshInstance meshInstance = default(MeshCombineUtility.MeshInstance);
			meshInstance.mesh = meshFilter.sharedMesh;
			if (!(renderer != null) || !renderer.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)hashtable[sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
					continue;
				}
				arrayList = new ArrayList();
				arrayList.Add(meshInstance);
				hashtable.Add(sharedMaterials[j], arrayList);
			}
			renderer.enabled = false;
		}
		foreach (DictionaryEntry item in hashtable)
		{
			ArrayList arrayList2 = (ArrayList)item.Value;
			MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[])arrayList2.ToArray(typeof(MeshCombineUtility.MeshInstance));
			if (hashtable.Count == 1)
			{
				if (GetComponent<MeshFilter>() == null)
				{
					base.gameObject.AddComponent<MeshFilter>();
				}
				if (GetComponent<MeshRenderer>())
				{
					base.gameObject.AddComponent<MeshRenderer>();
				}
				var meshFilter2 = GetComponent<MeshFilter>();
				meshFilter2.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
				base.renderer.material = (Material)item.Key;
				base.renderer.enabled = true;
			}
			else
			{
				var gameObject = new GameObject("Combined mesh");
				gameObject.transform.parent = base.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent<MeshFilter>();
				gameObject.AddComponent<MeshRenderer>();
				gameObject.renderer.material = (Material)item.Key;
				var meshFilter3 = gameObject.GetComponent<MeshFilter>();
				meshFilter3.mesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
		}
		if (deleteCollidier)
		{
			foreach (Transform item2 in base.transform)
			{
				if (item2.name != "Combined mesh")
				{
					UnityEngine.Object.Destroy(item2.gameObject);
				}
			}
		}
		if (!city)
		{
			return;
		}
		foreach (Transform item3 in base.transform)
		{
			if (item3.name == "Combined mesh")
			{
				item3.gameObject.layer = LayerMask.NameToLayer("City");
			}
		}
	}
}
