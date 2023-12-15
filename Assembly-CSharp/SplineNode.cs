using UnityEngine;

public class SplineNode
{
	public Transform nodeTransform;

	public float posInSpline;

	public float length;

	public SplineNode[] adjacentNodes;

	public SplineNode this[int idx]
	{
		get
		{
			return adjacentNodes[idx];
		}
		set
		{
			if (value != null)
			{
				adjacentNodes[idx] = value;
			}
		}
	}

	public SplineNode PrevNode0
	{
		get
		{
			return adjacentNodes[0];
		}
		set
		{
			if (value != null)
			{
				adjacentNodes[0] = value;
			}
		}
	}

	public SplineNode NextNode0
	{
		get
		{
			return adjacentNodes[1];
		}
		set
		{
			if (value != null)
			{
				adjacentNodes[1] = value;
			}
		}
	}

	public SplineNode NextNode1
	{
		get
		{
			return adjacentNodes[2];
		}
		set
		{
			if (value != null)
			{
				adjacentNodes[2] = value;
			}
		}
	}

	public SplineNode NextNode2
	{
		get
		{
			return adjacentNodes[3];
		}
		set
		{
			if (value != null)
			{
				adjacentNodes[3] = value;
			}
		}
	}

	public Vector3 Position
	{
		get
		{
			return nodeTransform.position;
		}
		set
		{
			nodeTransform.position = value;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return nodeTransform.rotation;
		}
		set
		{
			nodeTransform.rotation = value;
		}
	}

	public SplineNode(Transform controlPoint)
	{
		adjacentNodes = new SplineNode[4];
		nodeTransform = controlPoint;
	}

	public bool CheckReferences()
	{
		return nodeTransform != null && adjacentNodes[0] != null && adjacentNodes[1] != null && adjacentNodes[2] != null && adjacentNodes[3] != null;
	}
}
