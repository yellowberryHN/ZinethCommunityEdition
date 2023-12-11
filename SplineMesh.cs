using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class SplineMesh : MonoBehaviour
{
	public enum UVMode
	{
		Normal,
		Swap,
		DontInterpolate
	}

	public Spline spline;

	public Spline.UpdateMode uMode;

	public float deltaSeconds = 0.1f;

	public int deltaFrames = 2;

	public Mesh baseMesh;

	public int segmentCount = 100;

	public Vector2 xyScale = Vector2.one;

	public Vector2 uvScale = Vector2.one;

	public UVMode uvMode;

	public int splineSegment = -1;

	private Mesh bentMesh;

	private float passedTime;

	public Mesh BentMesh
	{
		get
		{
			return bentMesh;
		}
	}

	private void Start()
	{
		if (!(spline == null))
		{
			spline.UpdateSplineNodes();
			UpdateMesh();
		}
	}

	private void OnEnable()
	{
		if (!(spline == null))
		{
			spline.UpdateSplineNodes();
			UpdateMesh();
		}
	}

	private void LateUpdate()
	{
		switch (uMode)
		{
		case Spline.UpdateMode.EveryFrame:
			UpdateMesh();
			break;
		case Spline.UpdateMode.EveryXFrames:
			if (deltaFrames <= 0)
			{
				deltaFrames = 1;
			}
			if (Time.frameCount % deltaFrames == 0)
			{
				UpdateMesh();
			}
			break;
		case Spline.UpdateMode.EveryXSeconds:
			passedTime += Time.deltaTime;
			if (passedTime >= deltaSeconds)
			{
				UpdateMesh();
				passedTime = 0f;
			}
			break;
		}
	}

	public void UpdateMesh()
	{
		Setup();
		if ((bool)BentMesh)
		{
			BentMesh.Clear();
		}
		if (baseMesh == null || spline == null || segmentCount <= 0)
		{
			return;
		}
		Vector3[] vertices = baseMesh.vertices;
		Vector3[] normals = baseMesh.normals;
		Vector4[] tangents = baseMesh.tangents;
		Vector2[] uv = baseMesh.uv;
		int[] triangles = baseMesh.triangles;
		Vector3[] array = new Vector3[vertices.Length * segmentCount];
		Vector3[] array2 = new Vector3[normals.Length * segmentCount];
		Vector4[] array3 = new Vector4[tangents.Length * segmentCount];
		Vector2[] array4 = new Vector2[uv.Length * segmentCount];
		int[] array5 = new int[triangles.Length * segmentCount];
		if (this.splineSegment >= 0 && this.splineSegment < spline.SegmentCount)
		{
			SplineSegment splineSegment = spline.SplineSegments[this.splineSegment];
			int vIndex = 0;
			for (int i = 0; i < segmentCount; i++)
			{
				float param = (float)i / (float)segmentCount;
				float num = (float)(i + 1) / (float)segmentCount;
				if (num == 1f)
				{
					num -= 1E-05f;
				}
				param = splineSegment.ConvertSegmentToSplineParamter(param);
				num = splineSegment.ConvertSegmentToSplineParamter(num);
				CalculateBentMeshSub(ref vIndex, param, num, vertices, normals, tangents, uv, array, array2, array3, array4);
				for (int j = 0; j < triangles.Length; j++)
				{
					array5[j + i * triangles.Length] = triangles[j] + vertices.Length * i;
				}
			}
			BentMesh.vertices = array;
			BentMesh.uv = array4;
			if (normals.Length > 0)
			{
				BentMesh.normals = array2;
			}
			if (tangents.Length > 0)
			{
				BentMesh.tangents = array3;
			}
			BentMesh.triangles = array5;
			return;
		}
		int vIndex2 = 0;
		for (int k = 0; k < segmentCount; k++)
		{
			float param2 = (float)k / (float)segmentCount;
			float num2 = (float)(k + 1) / (float)segmentCount;
			if (num2 == 1f)
			{
				num2 -= 1E-05f;
			}
			CalculateBentMeshSub(ref vIndex2, param2, num2, vertices, normals, tangents, uv, array, array2, array3, array4);
			for (int l = 0; l < triangles.Length; l++)
			{
				array5[l + k * triangles.Length] = triangles[l] + vertices.Length * k;
			}
		}
		BentMesh.vertices = array;
		BentMesh.uv = array4;
		if (normals.Length > 0)
		{
			BentMesh.normals = array2;
		}
		if (tangents.Length > 0)
		{
			BentMesh.tangents = array3;
		}
		BentMesh.triangles = array5;
	}

	private void Setup()
	{
		if (!(spline == null))
		{
			if (bentMesh == null)
			{
				bentMesh = new Mesh();
				bentMesh.name = "BentMesh";
				bentMesh.hideFlags = HideFlags.HideAndDontSave;
			}
			MeshFilter component = GetComponent<MeshFilter>();
			if (component.sharedMesh != BentMesh)
			{
				component.sharedMesh = BentMesh;
			}
			MeshCollider component2 = GetComponent<MeshCollider>();
			if (component2 != null)
			{
				component2.sharedMesh = null;
				component2.sharedMesh = BentMesh;
			}
		}
	}

	private void CalculateBentMeshSub(ref int vIndex, float param0, float param1, Vector3[] verticesBase, Vector3[] normalsBase, Vector4[] tangentsBase, Vector2[] uvBase, Vector3[] verticesNew, Vector3[] normalsNew, Vector4[] tangentsNew, Vector2[] uvNew)
	{
		Vector3 from = spline.transform.InverseTransformPoint(spline.GetPositionOnSpline(param0));
		Vector3 to = spline.transform.InverseTransformPoint(spline.GetPositionOnSpline(param1));
		Quaternion from2 = spline.GetOrientationOnSpline(param0) * Quaternion.Inverse(spline.transform.localRotation);
		Quaternion to2 = spline.GetOrientationOnSpline(param1) * Quaternion.Inverse(spline.transform.localRotation);
		for (int i = 0; i < verticesBase.Length; i++)
		{
			Vector3 vector = verticesBase[i];
			Vector3 vector2 = uvBase[i];
			Vector3 vector3 = ((normalsBase.Length <= 0) ? Vector3.zero : normalsBase[i]);
			Vector3 vector4 = ((tangentsBase.Length <= 0) ? Vector3.zero : ((Vector3)tangentsBase[i]));
			float t = vector.z + 0.5f;
			vector.z = 0f;
			vector.Scale(new Vector3(xyScale[0], xyScale[1], 1f));
			vector = Quaternion.Lerp(from2, to2, t) * vector;
			vector += Vector3.Lerp(from, to, t);
			vector3 = Quaternion.Lerp(from2, to2, t) * vector3;
			vector4 = Quaternion.Lerp(from2, to2, t) * vector4;
			switch (uvMode)
			{
			case UVMode.Normal:
				vector2.y = Mathf.Lerp(param0, param1, t);
				break;
			case UVMode.Swap:
				vector2.x = Mathf.Lerp(param0, param1, t);
				break;
			}
			verticesNew[vIndex] = vector;
			uvNew[vIndex] = Vector2.Scale(vector2, uvScale);
			if (normalsBase.Length > 0)
			{
				normalsNew[vIndex] = vector3;
			}
			if (tangentsBase.Length > 0)
			{
				tangentsNew[vIndex] = vector4;
			}
			vIndex++;
		}
	}
}
