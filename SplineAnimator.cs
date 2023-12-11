using UnityEngine;

public class SplineAnimator : MonoBehaviour
{
	public Spline spline;

	public float speed = 1f;

	public float offSet;

	public WrapMode wrapMode = WrapMode.Once;

	public float passedTime;

	private void FixedUpdate()
	{
		passedTime += Time.deltaTime * speed;
		base.transform.position = spline.GetPositionOnSpline(WrapValue(passedTime + offSet, 0f, 1f, wrapMode));
		base.transform.rotation = spline.GetOrientationOnSpline(WrapValue(passedTime + offSet, 0f, 1f, wrapMode));
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
