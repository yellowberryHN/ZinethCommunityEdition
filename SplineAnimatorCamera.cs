using UnityEngine;

public class SplineAnimatorCamera : MonoBehaviour
{
	public Spline spline;

	public float speed = 1f;

	public float targetOffSet;

	public WrapMode wrapMode = WrapMode.Once;

	public float passedTime;

	private void Update()
	{
		passedTime += Time.deltaTime * speed;
		base.transform.position = spline.GetPositionOnSpline(WrapValue(passedTime, 0f, 1f, wrapMode)) + Vector3.up;
		base.transform.rotation = Quaternion.LookRotation(spline.GetPositionOnSpline(WrapValue(passedTime + targetOffSet, 0f, 1f, wrapMode)) - base.transform.position);
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
