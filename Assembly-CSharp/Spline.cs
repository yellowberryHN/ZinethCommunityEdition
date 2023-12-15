using System;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
	public enum ControlNodeMode
	{
		UseChildren,
		UseArray
	}

	public enum TangentMode
	{
		UseNormalizedTangents,
		UseTangents,
		UseNodeForwardVector
	}

	public enum RotationMode
	{
		None,
		Node,
		Tangent
	}

	public enum InterpolationMode
	{
		Hermite,
		Bezier,
		BSpline
	}

	public enum UpdateMode
	{
		DontUpdate,
		EveryFrame,
		EveryXFrames,
		EveryXSeconds
	}

	public InterpolationMode interpolationMode;

	public ControlNodeMode nodeMode;

	public RotationMode rotationMode = RotationMode.Tangent;

	public TangentMode tangentMode = TangentMode.UseTangents;

	public UpdateMode updateMode = UpdateMode.EveryFrame;

	public float deltaSeconds = 0.1f;

	public int deltaFrames = 2;

	public Vector3 tanUpVector = Vector3.up;

	public float tension = 0.5f;

	public bool autoClose;

	public int interpolationAccuracy = 1;

	public Transform[] splineNodesTransform;

	public float[] subSegmentLength;

	public float[] subSegmentPosition;

	public float splineLength;

	private float passedTime;

	public SplineNode[] splineNodes;

	public SplineNode[] SplineNodes
	{
		get
		{
			return splineNodes;
		}
	}

	public SplineNode this[int idx]
	{
		get
		{
			return splineNodes[idx];
		}
		set
		{
			if (value != null)
			{
				splineNodes[idx] = value;
			}
		}
	}

	public float Length
	{
		get
		{
			return splineLength;
		}
	}

	public bool AutoClose
	{
		get
		{
			return autoClose && interpolationMode != InterpolationMode.Bezier;
		}
	}

	public int SegmentCount
	{
		get
		{
			if (interpolationMode != InterpolationMode.Bezier)
			{
				if (AutoClose)
				{
					return splineNodes.Length;
				}
				return splineNodes.Length - 1;
			}
			return (splineNodes.Length - 1) / 3;
		}
	}

	public int ControlSegmentCount
	{
		get
		{
			if (AutoClose)
			{
				return splineNodes.Length;
			}
			return splineNodes.Length - 1;
		}
	}

	public Transform[] SplineNodeTransforms
	{
		get
		{
			if (nodeMode != ControlNodeMode.UseArray)
			{
				List<Transform> list = new List<Transform>();
				SplineControlNode[] componentsInChildren = GetComponentsInChildren<SplineControlNode>();
				foreach (SplineControlNode splineControlNode in componentsInChildren)
				{
					list.Add(splineControlNode.GetTransform);
				}
				list.Remove(base.transform);
				list.Sort((Transform a, Transform b) => a.name.CompareTo(b.name));
				return list.ToArray();
			}
			return splineNodesTransform;
		}
	}

	public SplineSegment[] SplineSegments
	{
		get
		{
			SplineSegment[] array = new SplineSegment[SegmentCount];
			if (interpolationMode != InterpolationMode.Bezier)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new SplineSegment(this, splineNodes[i], splineNodes[i].NextNode0);
				}
			}
			else
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = new SplineSegment(this, splineNodes[j * 3], splineNodes[j * 3].NextNode0);
				}
			}
			return array;
		}
	}

	private bool IsBezier
	{
		get
		{
			return interpolationMode == InterpolationMode.Bezier;
		}
	}

	private int Step
	{
		get
		{
			if (interpolationMode == InterpolationMode.Bezier)
			{
				return 3;
			}
			return 1;
		}
	}

	private void OnEnable()
	{
		UpdateSplineNodes();
	}

	private void LateUpdate()
	{
		switch (updateMode)
		{
		case UpdateMode.EveryFrame:
			UpdateSplineNodes();
			break;
		case UpdateMode.EveryXFrames:
			if (deltaFrames <= 0)
			{
				deltaFrames = 1;
			}
			if (Time.frameCount % deltaFrames == 0)
			{
				UpdateSplineNodes();
			}
			break;
		case UpdateMode.EveryXSeconds:
			passedTime += Time.deltaTime;
			if (passedTime >= deltaSeconds)
			{
				UpdateSplineNodes();
				passedTime = 0f;
			}
			break;
		}
	}

	public void UpdateSplineNodes()
	{
		SetupSplineNodes(SplineNodeTransforms);
	}

	public Vector3 GetPositionOnSpline(float param)
	{
		if (splineNodes == null)
		{
			return Vector3.zero;
		}
		int normalizedIndex;
		float normalizedParam;
		RecalculateParameter(param, out normalizedIndex, out normalizedParam);
		return GetPositionInternal(normalizedIndex, normalizedParam);
	}

	public Vector3 GetTangentToSpline(float param)
	{
		if (splineNodes == null)
		{
			return Vector3.zero;
		}
		int normalizedIndex;
		float normalizedParam;
		RecalculateParameter(param, out normalizedIndex, out normalizedParam);
		return GetTangentInternal(normalizedIndex, normalizedParam);
	}

	public Quaternion GetOrientationOnSpline(float param)
	{
		if (splineNodes == null)
		{
			return Quaternion.identity;
		}
		switch (rotationMode)
		{
		case RotationMode.Tangent:
		{
			Vector3 tangentToSpline = GetTangentToSpline(param);
			if (tangentToSpline.x == 0f && ((tangentToSpline.y == 0f) & (tangentToSpline.z == 0f)))
			{
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(tangentToSpline, tanUpVector);
		}
		case RotationMode.Node:
		{
			int normalizedIndex;
			float normalizedParam;
			RecalculateParameter(param, out normalizedIndex, out normalizedParam);
			return GetRotationInternal(normalizedIndex, normalizedParam);
		}
		default:
			return Quaternion.identity;
		}
	}

	public SplineSegment GetSplineSegment(float param)
	{
		if (interpolationMode == InterpolationMode.Bezier)
		{
			param = Mathf.Clamp(param, 0f, 1f);
			if (param == 1f)
			{
				return new SplineSegment(this, splineNodes[ControlSegmentCount - 1], splineNodes[ControlSegmentCount - 1].NextNode0);
			}
			for (int i = 0; i < ControlSegmentCount; i += 3)
			{
				if (param - splineNodes[i].posInSpline < splineNodes[i].length)
				{
					return new SplineSegment(this, splineNodes[i], splineNodes[i].NextNode2);
				}
			}
		}
		else
		{
			param = ((!AutoClose) ? Mathf.Clamp(param, 0f, 1f) : Mathf.Repeat(param, 1f));
			if (param == 1f)
			{
				return new SplineSegment(this, splineNodes[ControlSegmentCount - 1], splineNodes[ControlSegmentCount - 1].NextNode0);
			}
			for (int j = 0; j < ControlSegmentCount; j++)
			{
				if (param - splineNodes[j].posInSpline < splineNodes[j].length)
				{
					return new SplineSegment(this, splineNodes[j], splineNodes[j].NextNode0);
				}
			}
		}
		return null;
	}

	public float ConvertNormalizedParameterToDistance(float param)
	{
		return splineLength * param;
	}

	public float ConvertDistanceToNormalizedParameter(float param)
	{
		if (splineLength <= 0f || param <= 0f)
		{
			return 0f;
		}
		if (param > splineLength)
		{
			return 1f;
		}
		return param / splineLength;
	}

	private void RecalculateParameter(float param, out int normalizedIndex, out float normalizedParam)
	{
		param = Mathf.Clamp01(param);
		normalizedIndex = 0;
		normalizedParam = 0f;
		if (param == 0f)
		{
			return;
		}
		if (param == 1f)
		{
			if (interpolationMode == InterpolationMode.Bezier)
			{
				normalizedIndex = ControlSegmentCount - 3;
			}
			else
			{
				normalizedIndex = ControlSegmentCount - 1;
			}
			normalizedParam = 1f;
			return;
		}
		float num = 1f / (float)interpolationAccuracy;
		for (int num2 = subSegmentPosition.Length - 1; num2 >= 0; num2--)
		{
			if (subSegmentPosition[num2] < param)
			{
				int num3 = num2 - num2 % interpolationAccuracy;
				normalizedIndex = num3 * Step / interpolationAccuracy;
				normalizedParam = num * ((float)(num2 - num3) + (param - subSegmentPosition[num2]) / subSegmentLength[num2]);
				if (normalizedIndex >= ControlSegmentCount)
				{
					if (interpolationMode == InterpolationMode.Bezier)
					{
						normalizedIndex = ControlSegmentCount - 3;
					}
					else
					{
						normalizedIndex = ControlSegmentCount - 1;
					}
					normalizedParam = 1f;
				}
				break;
			}
		}
	}

	private void SetupSplineNodes(Transform[] transformNodes)
	{
		splineNodes = null;
		if (transformNodes.Length <= 0)
		{
			return;
		}
		int num = transformNodes.Length;
		if (interpolationMode == InterpolationMode.Bezier)
		{
			num = ((transformNodes.Length >= 7) ? (num - (transformNodes.Length - 4) % 3) : (num - transformNodes.Length % 4));
			if (num < 4)
			{
				return;
			}
		}
		else if (num < 2)
		{
			return;
		}
		SplineNode[] array = new SplineNode[num];
		for (int i = 0; i < num; i++)
		{
			if (transformNodes[i] == null)
			{
				return;
			}
			array[i] = new SplineNode(transformNodes[i]);
		}
		for (int j = 0; j < num; j++)
		{
			if (transformNodes[j] == null)
			{
				return;
			}
			int num2 = j - 1;
			int num3 = j + 1;
			int num4 = j + 2;
			int num5 = j + 3;
			if (AutoClose)
			{
				if (num2 < 0)
				{
					num2 = num - 1;
				}
				num3 %= num;
				num4 %= num;
				num5 %= num;
			}
			else
			{
				num2 = Mathf.Max(num2, 0);
				num3 = Mathf.Min(num3, num - 1);
				num4 = Mathf.Min(num4, num - 1);
				num5 = Mathf.Min(num5, num - 1);
			}
			array[j][0] = array[num2];
			array[j][1] = array[num3];
			array[j][2] = array[num4];
			array[j][3] = array[num5];
		}
		splineNodes = array;
		ReparametrizeCurve();
	}

	private void ReparametrizeCurve()
	{
		splineLength = 0f;
		subSegmentLength = new float[SegmentCount * interpolationAccuracy];
		subSegmentPosition = new float[SegmentCount * interpolationAccuracy];
		for (int i = 0; i < SegmentCount * interpolationAccuracy; i++)
		{
			subSegmentLength[i] = 0f;
			subSegmentPosition[i] = 0f;
		}
		if (splineNodes == null)
		{
			return;
		}
		for (int j = 0; j < SegmentCount; j++)
		{
			for (int k = 1; k <= interpolationAccuracy; k++)
			{
				int num = j * interpolationAccuracy + k - 1;
				float num2 = 1f / (float)interpolationAccuracy;
				subSegmentLength[num] = GetSegmentLengthInternal(j * Step, num2 * (float)(k - 1), num2 * (float)k, 0.2f * num2);
				splineLength += subSegmentLength[num];
			}
		}
		for (int l = 0; l < SegmentCount; l++)
		{
			for (int m = 1; m <= interpolationAccuracy; m++)
			{
				int num3 = l * interpolationAccuracy + m;
				subSegmentLength[num3 - 1] /= splineLength;
				if (num3 == subSegmentPosition.Length)
				{
					break;
				}
				subSegmentPosition[num3] = subSegmentPosition[num3 - 1] + subSegmentLength[num3 - 1];
			}
		}
		for (int n = 0; n < subSegmentLength.Length; n++)
		{
			splineNodes[(n - n % interpolationAccuracy) / interpolationAccuracy * Step].length += subSegmentLength[n];
		}
		for (int num4 = 0; num4 < splineNodes.Length - Step; num4 += Step)
		{
			splineNodes[num4 + Step].posInSpline = splineNodes[num4].posInSpline + splineNodes[num4].length;
		}
		if (IsBezier)
		{
			for (int num5 = 0; num5 < splineNodes.Length - Step; num5 += Step)
			{
				splineNodes[num5 + 1].posInSpline = splineNodes[num5].posInSpline;
				splineNodes[num5 + 2].posInSpline = splineNodes[num5].posInSpline;
			}
		}
		if (!AutoClose)
		{
			splineNodes[splineNodes.Length - 1].posInSpline = 1f;
		}
	}

	public float GetClosestPoint(Vector3 p, int iterations)
	{
		float num = float.PositiveInfinity;
		float num2 = 0f;
		iterations = Mathf.Clamp(iterations, 0, 5);
		for (float num3 = 0f; num3 <= 1f; num3 += 0.01f)
		{
			float sqrMagnitude = (GetPositionOnSpline(num3) - p).sqrMagnitude;
			if (num > sqrMagnitude)
			{
				num = sqrMagnitude;
				num2 = num3;
			}
		}
		for (int i = 0; i < iterations; i++)
		{
			float num4 = 0.01f * Mathf.Pow(10f, 0f - (float)i);
			float num5 = num4 * 0.1f;
			for (float num6 = Mathf.Clamp01(num2 - num4); num6 <= Mathf.Clamp01(num2 + num4); num6 += num5)
			{
				float sqrMagnitude2 = (GetPositionOnSpline(num6) - p).sqrMagnitude;
				if (num > sqrMagnitude2)
				{
					num = sqrMagnitude2;
					num2 = num6;
				}
			}
		}
		return num2;
	}

	public float GetClosestPoint(Vector3 p, int iterations, float lastParam, float diff)
	{
		float num = float.PositiveInfinity;
		float num2 = 0f;
		iterations = Mathf.Clamp(iterations, 0, 5);
		for (float num3 = 0f; num3 <= 1f; num3 += 0.01f)
		{
			float magnitude = (GetPositionOnSpline(num3) - p).magnitude;
			if (num > magnitude && Mathf.Abs(num3 - lastParam) < diff)
			{
				num = magnitude;
				num2 = num3;
			}
		}
		for (int i = 0; i < iterations; i++)
		{
			float num4 = 0.01f / Mathf.Pow(10f, i);
			float num5 = num4 * 0.1f;
			for (float num6 = Mathf.Clamp01(num2 - num4); num6 <= Mathf.Clamp01(num2 + num4); num6 += num5)
			{
				float magnitude2 = (GetPositionOnSpline(num6) - p).magnitude;
				if (num > magnitude2 && Mathf.Abs(num6 - lastParam) < diff)
				{
					num = magnitude2;
					num2 = num6;
				}
			}
		}
		return num2;
	}

	public Vector3 GetShortestConnection(Vector3 p, int iterations)
	{
		return GetPositionOnSpline(GetClosestPoint(p, iterations)) - p;
	}

	private void OnDrawGizmos()
	{
		UpdateSplineNodes();
		if (splineNodes == null)
		{
			return;
		}
		DrawSplineGizmo(new Color(0.5f, 0.5f, 0.5f, 0.5f));
		Plane plane = default(Plane);
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		plane.SetNormalAndPosition(Camera.current.transform.forward, Camera.current.transform.position);
		SplineNode[] array = splineNodes;
		foreach (SplineNode splineNode in array)
		{
			float enter = 0f;
			if (Camera.current.orthographic)
			{
				enter = Camera.current.orthographicSize * 2.5f;
			}
			else
			{
				plane.Raycast(new Ray(splineNode.Position, Camera.current.transform.forward), out enter);
			}
			Gizmos.DrawSphere(splineNode.Position, enter * 0.015f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		UpdateSplineNodes();
		if (splineNodes == null)
		{
			return;
		}
		DrawSplineGizmo(new Color(1f, 0.5f, 0f, 1f));
		Plane plane = default(Plane);
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.75f);
		plane.SetNormalAndPosition(Camera.current.transform.forward, Camera.current.transform.position);
		SplineNode[] array = splineNodes;
		foreach (SplineNode splineNode in array)
		{
			float enter = 0f;
			if (Camera.current.orthographic)
			{
				enter = Camera.current.orthographicSize * 2.5f;
			}
			else
			{
				plane.Raycast(new Ray(splineNode.Position, Camera.current.transform.forward), out enter);
			}
			Gizmos.DrawSphere(splineNode.Position, enter * 0.0075f);
		}
	}

	private void DrawSplineGizmo(Color curveColor)
	{
		int num = 1;
		switch (interpolationMode)
		{
		case InterpolationMode.BSpline:
		{
			Gizmos.color = new Color(curveColor.r, curveColor.g, curveColor.b, curveColor.a * 0.25f);
			for (int j = 0; j < ControlSegmentCount; j++)
			{
				Gizmos.DrawLine(splineNodes[j].Position, splineNodes[j].NextNode0.Position);
			}
			break;
		}
		case InterpolationMode.Bezier:
		{
			Gizmos.color = new Color(curveColor.r, curveColor.g, curveColor.b, curveColor.a * 0.25f);
			for (int i = 0; i < ControlSegmentCount; i++)
			{
				Gizmos.DrawLine(splineNodes[i].Position, splineNodes[i].NextNode0.Position);
			}
			num = 3;
			break;
		}
		}
		Gizmos.color = curveColor;
		for (int k = 0; k < ControlSegmentCount; k += num)
		{
			Vector3 from = GetPositionInternal(k, 0f);
			for (float num2 = 0.05f; num2 < 1.05f; num2 += 0.05f)
			{
				Vector3 positionInternal = GetPositionInternal(k, num2);
				Gizmos.DrawLine(from, positionInternal);
				from = positionInternal;
			}
		}
	}

	private Vector3 GetPositionInternal(int idxFirstPoint, float t)
	{
		if (!splineNodes[idxFirstPoint].CheckReferences())
		{
			return Vector3.zero;
		}
		Vector3 P = splineNodes[idxFirstPoint].nodeTransform.position;
		Vector3 P2 = splineNodes[idxFirstPoint].NextNode0.nodeTransform.position;
		switch (interpolationMode)
		{
		case InterpolationMode.Hermite:
		{
			Vector3 T;
			Vector3 T2;
			GetCatMullTangentsInternal(splineNodes[idxFirstPoint], ref P, ref P2, out T, out T2);
			return InterpolatePosition(t, ref P, ref P2, ref T, ref T2);
		}
		case InterpolationMode.Bezier:
		{
			Vector3 P5 = splineNodes[idxFirstPoint].NextNode1.nodeTransform.position;
			Vector3 P6 = splineNodes[idxFirstPoint].NextNode2.nodeTransform.position;
			return InterpolatePosition(t, ref P, ref P2, ref P5, ref P6);
		}
		default:
		{
			Vector3 P3 = splineNodes[idxFirstPoint].PrevNode0.nodeTransform.position;
			Vector3 P4 = splineNodes[idxFirstPoint].NextNode1.nodeTransform.position;
			return InterpolatePosition(t, ref P3, ref P, ref P2, ref P4);
		}
		}
	}

	private Vector3 GetTangentInternal(int idxFirstPoint, float t)
	{
		if (!splineNodes[idxFirstPoint].CheckReferences())
		{
			return Vector3.zero;
		}
		Vector3 P = splineNodes[idxFirstPoint].nodeTransform.position;
		Vector3 P2 = splineNodes[idxFirstPoint].NextNode0.nodeTransform.position;
		switch (interpolationMode)
		{
		case InterpolationMode.Hermite:
		{
			Vector3 T;
			Vector3 T2;
			GetCatMullTangentsInternal(splineNodes[idxFirstPoint], ref P, ref P2, out T, out T2);
			return InterpolateTangent(t, ref P, ref P2, ref T, ref T2);
		}
		case InterpolationMode.Bezier:
		{
			Vector3 P5 = splineNodes[idxFirstPoint].NextNode1.nodeTransform.position;
			Vector3 P6 = splineNodes[idxFirstPoint].NextNode2.nodeTransform.position;
			return InterpolateTangent(t, ref P, ref P2, ref P5, ref P6);
		}
		default:
		{
			Vector3 P3 = splineNodes[idxFirstPoint].PrevNode0.nodeTransform.position;
			Vector3 P4 = splineNodes[idxFirstPoint].NextNode1.nodeTransform.position;
			return InterpolateTangent(t, ref P3, ref P, ref P2, ref P4);
		}
		}
	}

	private Quaternion GetRotationInternal(int idxFirstPoint, float t)
	{
		if (!splineNodes[idxFirstPoint].CheckReferences())
		{
			return Quaternion.identity;
		}
		Quaternion rotation = splineNodes[idxFirstPoint].PrevNode0.nodeTransform.rotation;
		Quaternion rotation2 = splineNodes[idxFirstPoint].nodeTransform.rotation;
		Quaternion rotation3 = splineNodes[idxFirstPoint].NextNode0.nodeTransform.rotation;
		Quaternion rotation4 = splineNodes[idxFirstPoint].NextNode1.nodeTransform.rotation;
		Quaternion squadIntermediate = GetSquadIntermediate(rotation, rotation2, rotation3);
		Quaternion squadIntermediate2 = GetSquadIntermediate(rotation2, rotation3, rotation4);
		return GetQuatSquad(t, rotation2, rotation3, squadIntermediate, squadIntermediate2);
	}

	private Vector3 InterpolatePosition(float t, ref Vector3 P1, ref Vector3 P2, ref Vector3 P3, ref Vector3 P4)
	{
		float num = t * t;
		float num2 = num * t;
		float num3;
		float num4;
		float num5;
		float num6;
		switch (interpolationMode)
		{
		default:
			num3 = 2f * num2 - 3f * num + 0f * t + 1f;
			num4 = -2f * num2 + 3f * num + 0f * t;
			num5 = 1f * num2 - 2f * num + 1f * t;
			num6 = 1f * num2 - 1f * num + 0f * t;
			break;
		case InterpolationMode.Bezier:
			num3 = -1f * num2 + 3f * num - 3f * t + 1f;
			num4 = 3f * num2 - 6f * num + 3f * t;
			num5 = -3f * num2 + 3f * num + 0f * t;
			num6 = 1f * num2 - 0f * num + 0f * t;
			break;
		case InterpolationMode.BSpline:
			num3 = -1f / 6f * num2 + 0.5f * num - 0.5f * t + 1f / 6f;
			num4 = 0.5f * num2 - 1f * num + 0f * t + 2f / 3f;
			num5 = -0.5f * num2 + 0.5f * num + 0.5f * t + 1f / 6f;
			num6 = 1f / 6f * num2 + 0f * num + 0f * t;
			break;
		}
		return new Vector3(num3 * P1.x + num4 * P2.x + num5 * P3.x + num6 * P4.x, num3 * P1.y + num4 * P2.y + num5 * P3.y + num6 * P4.y, num3 * P1.z + num4 * P2.z + num5 * P3.z + num6 * P4.z);
	}

	private Vector3 InterpolateTangent(float t, ref Vector3 P1, ref Vector3 P2, ref Vector3 P3, ref Vector3 P4)
	{
		float num = t * t;
		float num2;
		float num3;
		float num4;
		float num5;
		switch (interpolationMode)
		{
		default:
			num2 = 6f * num - 6f * t;
			num3 = -6f * num + 6f * t;
			num4 = 3f * num - 4f * t + 1f;
			num5 = 3f * num - 2f * t;
			break;
		case InterpolationMode.Bezier:
			num2 = -3f * num + 6f * t - 3f;
			num3 = 9f * num - 12f * t + 3f;
			num4 = -9f * num + 6f * t;
			num5 = 3f * num - 0f * t;
			break;
		case InterpolationMode.BSpline:
			num2 = -0.5f * num + 1f * t - 0.5f;
			num3 = 1.5f * num - 2f * t;
			num4 = -1.5f * num + 1f * t + 0.5f;
			num5 = 0.5f * num + 0f * t;
			break;
		}
		return new Vector3(num2 * P1.x + num3 * P2.x + num4 * P3.x + num5 * P4.x, num2 * P1.y + num3 * P2.y + num4 * P3.y + num5 * P4.y, num2 * P1.z + num3 * P2.z + num4 * P3.z + num5 * P4.z);
	}

	private void GetCatMullTangentsInternal(SplineNode firstNode, ref Vector3 P1, ref Vector3 P2, out Vector3 T1, out Vector3 T2)
	{
		switch (tangentMode)
		{
		case TangentMode.UseTangents:
			T1 = firstNode.PrevNode0.nodeTransform.position;
			T2 = firstNode.NextNode1.nodeTransform.position;
			T1.x = (P2.x - T1.x) * tension;
			T1.y = (P2.y - T1.y) * tension;
			T1.z = (P2.z - T1.z) * tension;
			T2.x = (T2.x - P1.x) * tension;
			T2.y = (T2.y - P1.y) * tension;
			T2.z = (T2.z - P1.z) * tension;
			break;
		case TangentMode.UseNodeForwardVector:
			T1 = firstNode.nodeTransform.forward * tension;
			T2 = firstNode.NextNode0.nodeTransform.forward * tension;
			break;
		default:
			T1 = firstNode.PrevNode0.nodeTransform.position;
			T2 = firstNode.NextNode1.nodeTransform.position;
			T1.x = P2.x - T1.x;
			T1.y = P2.y - T1.y;
			T1.z = P2.z - T1.z;
			T2.x -= P1.x;
			T2.y -= P1.y;
			T2.z -= P1.z;
			T1.Normalize();
			T2.Normalize();
			T1.x *= tension;
			T1.y *= tension;
			T1.z *= tension;
			T2.x *= tension;
			T2.y *= tension;
			T2.z *= tension;
			break;
		}
	}

	private float GetSegmentLengthInternal(int idxFirstPoint)
	{
		return GetSegmentLengthInternal(idxFirstPoint, 0f, 1f, 0.2f);
	}

	private float GetSegmentLengthInternal(int idxFirstPoint, float startValue, float endValue, float step)
	{
		float num = 0f;
		Vector3 a = GetPositionInternal(idxFirstPoint, startValue);
		float num2 = endValue + step * 0.5f;
		for (float num3 = startValue + step; num3 < num2; num3 += step)
		{
			Vector3 positionInternal = GetPositionInternal(idxFirstPoint, num3);
			num += Vector3.Distance(a, positionInternal);
			a = positionInternal;
		}
		return num;
	}

	private static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
	{
		float t2 = 2f * t * (1f - t);
		Quaternion p = QuatSlerp(q0, q1, t);
		Quaternion q2 = QuatSlerp(a0, a1, t);
		return QuatSlerp(p, q2, t2);
	}

	private static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
	{
		Quaternion quatConjugate = GetQuatConjugate(q1);
		Quaternion quatLog = GetQuatLog(quatConjugate * q0);
		Quaternion quatLog2 = GetQuatLog(quatConjugate * q2);
		Quaternion q3 = new Quaternion(-0.25f * (quatLog.x + quatLog2.x), -0.25f * (quatLog.y + quatLog2.y), -0.25f * (quatLog.z + quatLog2.z), -0.25f * (quatLog.w + quatLog2.w));
		return q1 * GetQuatExp(q3);
	}

	private static Quaternion QuatSlerp(Quaternion p, Quaternion q, float t)
	{
		float num = Quaternion.Dot(p, q);
		Quaternion result = default(Quaternion);
		if ((double)(1f + num) > 1E-05)
		{
			float num4;
			float num5;
			if ((double)(1f - num) > 1E-05)
			{
				float num2 = Mathf.Acos(num);
				float num3 = 1f / Mathf.Sin(num2);
				num4 = Mathf.Sin((1f - t) * num2) * num3;
				num5 = Mathf.Sin(t * num2) * num3;
			}
			else
			{
				num4 = 1f - t;
				num5 = t;
			}
			result.x = num4 * p.x + num5 * q.x;
			result.y = num4 * p.y + num5 * q.y;
			result.z = num4 * p.z + num5 * q.z;
			result.w = num4 * p.w + num5 * q.w;
		}
		else
		{
			float num6 = Mathf.Sin((1f - t) * (float)Math.PI * 0.5f);
			float num7 = Mathf.Sin(t * (float)Math.PI * 0.5f);
			result.x = num6 * p.x - num7 * p.y;
			result.y = num6 * p.y + num7 * p.x;
			result.z = num6 * p.z - num7 * p.w;
			result.w = p.z;
		}
		return result;
	}

	private static Quaternion GetQuatLog(Quaternion q)
	{
		Quaternion result = q;
		result.w = 0f;
		if (Mathf.Abs(q.w) < 1f)
		{
			float num = Mathf.Acos(q.w);
			float num2 = Mathf.Sin(num);
			if (Mathf.Abs(num2) > 0.0001f)
			{
				float num3 = num / num2;
				result.x = q.x * num3;
				result.y = q.y * num3;
				result.z = q.z * num3;
			}
		}
		return result;
	}

	private static Quaternion GetQuatExp(Quaternion q)
	{
		Quaternion result = q;
		float num = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
		float num2 = Mathf.Sin(num);
		result.w = Mathf.Cos(num);
		if (Mathf.Abs(num2) > 0.0001f)
		{
			float num3 = num2 / num;
			result.x = num3 * q.x;
			result.y = num3 * q.y;
			result.z = num3 * q.z;
		}
		return result;
	}

	private static Quaternion GetQuatConjugate(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}
}
