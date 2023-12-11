using UnityEngine;

public class SplineAnimatorClosestPoint : MonoBehaviour
{
	public Spline spline;

	public WrapMode wMode = WrapMode.Once;

	public Transform target;

	public int iterations = 5;

	public float diff = 0.5f;

	public float offset;

	private Transform thisTransform;

	private float lastParam;

	private void Start()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		if (!(target == null) && !(spline == null))
		{
			float param = WrapValue(spline.GetClosestPoint(target.position, iterations, lastParam, diff) + offset, 0f, 1f, wMode);
			thisTransform.position = spline.GetPositionOnSpline(param);
			thisTransform.rotation = spline.GetOrientationOnSpline(param);
			lastParam = param;
		}
	}

	private float WrapValue(float v, float start, float end, WrapMode wMode)
	{
		switch (wMode)
		{
		case WrapMode.Once:
		case WrapMode.ClampForever:
			return Mathf.Clamp(v, start, end);
		case WrapMode.Default:
		case WrapMode.Loop:
			return Mathf.Repeat(v, end - start) + start;
		case WrapMode.PingPong:
			return Mathf.PingPong(v, end - start) + start;
		default:
			return v;
		}
	}
}
