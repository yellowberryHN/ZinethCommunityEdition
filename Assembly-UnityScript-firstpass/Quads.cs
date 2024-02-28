using System;
using UnityEngine;

[Serializable]
public class Quads : MonoBehaviour
{
	[NonSerialized]
	public static Mesh[] meshes;

	[NonSerialized]
	public static int currentQuads;

	public static bool HasMeshes()
	{
		int result;
		if (meshes == null)
		{
			result = 0;
		}
		else
		{
			int num = 0;
			Mesh[] array = meshes;
			int length = array.Length;
			while (true)
			{
				if (num < length)
				{
					if (null == array[num])
					{
						result = 0;
						break;
					}
					num++;
					continue;
				}
				result = 1;
				break;
			}
		}
		return (byte)result != 0;
	}

	public static void Cleanup()
	{
		if (meshes == null)
		{
			return;
		}
		int i = 0;
		Mesh[] array = meshes;
		for (int length = array.Length; i < length; i++)
		{
			if (null != array[i])
			{
				UnityEngine.Object.DestroyImmediate(array[i]);
				array[i] = null;
			}
		}
		meshes = null;
	}

	public static Mesh[] GetMeshes(int totalWidth, int totalHeight)
	{
		Mesh[] result;
		if (HasMeshes() && currentQuads == totalWidth * totalHeight)
		{
			result = meshes;
		}
		else
		{
			int num = 10833;
			int num2 = (currentQuads = totalWidth * totalHeight);
			int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
			meshes = new Mesh[num3];
			int num4 = 0;
			int num5 = 0;
			for (num4 = 0; num4 < num2; num4 += num)
			{
				int triCount = Mathf.FloorToInt(Mathf.Clamp(num2 - num4, 0, num));
				meshes[num5] = GetMesh(triCount, num4, totalWidth, totalHeight);
				num5++;
			}
			result = meshes;
		}
		return result;
	}

	public static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
	{
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		Vector3[] array = new Vector3[triCount * 4];
		Vector2[] array2 = new Vector2[triCount * 4];
		Vector2[] array3 = new Vector2[triCount * 4];
		int[] array4 = new int[triCount * 6];
		float num = 0.0075f;
		for (int i = 0; i < triCount; i++)
		{
			int num2 = i * 4;
			int num3 = i * 6;
			int num4 = triOffset + i;
			float num5 = Mathf.Floor(num4 % totalWidth) / (float)totalWidth;
			float num6 = Mathf.Floor(num4 / totalWidth) / (float)totalHeight;
			Vector3 vector = new Vector3(num5 * 2f - 1f, num6 * 2f - 1f, 1f);
			array[num2 + 0] = vector;
			array[num2 + 1] = vector;
			array[num2 + 2] = vector;
			array[num2 + 3] = vector;
			array2[num2 + 0] = new Vector2(0f, 0f);
			array2[num2 + 1] = new Vector2(1f, 0f);
			array2[num2 + 2] = new Vector2(0f, 1f);
			array2[num2 + 3] = new Vector2(1f, 1f);
			array3[num2 + 0] = new Vector2(num5, num6);
			array3[num2 + 1] = new Vector2(num5, num6);
			array3[num2 + 2] = new Vector2(num5, num6);
			array3[num2 + 3] = new Vector2(num5, num6);
			array4[num3 + 0] = num2 + 0;
			array4[num3 + 1] = num2 + 1;
			array4[num3 + 2] = num2 + 2;
			array4[num3 + 3] = num2 + 1;
			array4[num3 + 4] = num2 + 2;
			array4[num3 + 5] = num2 + 3;
		}
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.uv = array2;
		mesh.uv2 = array3;
		return mesh;
	}
}
