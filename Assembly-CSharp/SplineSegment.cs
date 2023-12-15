public class SplineSegment
{
	private readonly Spline parentSpline;

	private readonly SplineNode startNode;

	private readonly SplineNode endNode;

	public Spline ParentSpline
	{
		get
		{
			return parentSpline;
		}
	}

	public SplineNode StartNode
	{
		get
		{
			return startNode;
		}
	}

	public SplineNode EndNode
	{
		get
		{
			return endNode;
		}
	}

	public float Length
	{
		get
		{
			return startNode.length * parentSpline.Length;
		}
	}

	public float NormalizedLength
	{
		get
		{
			return startNode.length;
		}
	}

	public SplineSegment(Spline pSpline, SplineNode sNode, SplineNode eNode)
	{
		if ((sNode.NextNode0 == eNode || sNode.NextNode2 == eNode) && pSpline != null)
		{
			parentSpline = pSpline;
			startNode = sNode;
			endNode = eNode;
		}
		else
		{
			parentSpline = null;
			startNode = null;
			endNode = null;
		}
	}

	public float ConvertSegmentToSplineParamter(float param)
	{
		return startNode.posInSpline + param * startNode.length;
	}

	public float ConvertSplineToSegmentParamter(float param)
	{
		if (param < startNode.posInSpline)
		{
			return 0f;
		}
		if (param >= endNode.posInSpline)
		{
			return 1f;
		}
		return (param - startNode.posInSpline) / startNode.length;
	}

	public float ClampParameterToSegment(float param)
	{
		if (param < startNode.posInSpline)
		{
			return startNode.posInSpline;
		}
		if (param >= endNode.posInSpline)
		{
			return endNode.posInSpline;
		}
		return param;
	}
}
