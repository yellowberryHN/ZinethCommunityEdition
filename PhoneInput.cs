using UnityEngine;

public static class PhoneInput
{
	public enum ControlType
	{
		Mouse,
		TouchScreen,
		Keyboard
	}

	public static PhoneController phonecontroller;

	public static Collider phonescreencollider;

	public static Camera phonescreencamera;

	public static Camera phonescenecamera;

	public static ControlType controltype = ControlType.Keyboard;

	private static Vector3 last_touchpoint = Vector3.one * -1f;

	private static float last_update_time = -1f;

	private static bool dirpressready = true;

	private static int last_dirpress_frame = -1;

	private static Vector2 last_dirpress;

	private static Vector2 _oldmousepos = Vector2.one * -1f;

	private static Vector3 _oldtouchpos = Vector3.one * -1f;

	public static bool invert_stick = false;

	public static Vector2 GetControlDir()
	{
		return Vector2.ClampMagnitude(GetRStickVec(), 1f);
	}

	public static Vector2 GetArrowsVec()
	{
		return Vector2.zero;
	}

	public static Vector2 GetControlDirPressed()
	{
		if (Time.frameCount == last_dirpress_frame)
		{
			return last_dirpress;
		}
		last_dirpress_frame = Time.frameCount;
		Vector2 controlDir = GetControlDir();
		last_dirpress = Vector2.zero;
		if (controlDir.magnitude < 0.6f)
		{
			dirpressready = true;
		}
		else if (dirpressready)
		{
			last_dirpress = controlDir;
			dirpressready = false;
		}
		return last_dirpress;
	}

	public static void DetectControlType()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			controltype = ControlType.Mouse;
		}
		if (GetControlDir().magnitude != 0f || Input.GetButtonDown("CellFire"))
		{
			controltype = ControlType.Keyboard;
			GetControlDirPressed();
		}
		Vector2 vector = Input.mousePosition;
		if (controltype != 0 && _oldmousepos != vector)
		{
			Vector3 touchPoint = GetTouchPoint();
			if (touchPoint != _oldtouchpos)
			{
				controltype = ControlType.Mouse;
			}
			_oldtouchpos = touchPoint;
		}
		_oldmousepos = vector;
	}

	public static bool IsPressedDown()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return true;
		}
		if (controltype == ControlType.Mouse)
		{
			if (GetTouchPoint() == Vector3.one * -1f)
			{
				return false;
			}
			return Input.GetButtonDown("CellClick");
		}
		if (controltype == ControlType.Keyboard)
		{
			return GetRStickButtonDown();
		}
		return false;
	}

	public static bool IsPressed()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return true;
		}
		if (controltype == ControlType.Mouse)
		{
			if (GetTouchPoint() == Vector3.one * -1f)
			{
				return false;
			}
			return Input.GetButton("CellClick");
		}
		if (controltype == ControlType.Keyboard)
		{
			return GetRStickButton();
		}
		return false;
	}

	public static Vector3 GetTouchPoint()
	{
		if (Time.time == last_update_time)
		{
			return last_touchpoint;
		}
		last_update_time = Time.time;
		return last_touchpoint = _GetTouchPoint();
	}

	private static Vector3 _GetTouchPoint()
	{
		if (controltype == ControlType.Mouse || true)
		{
			Vector3 mousePosition = Input.mousePosition;
			if (phonescreencamera == null)
			{
				return new Vector3(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
			}
			Ray ray = phonescreencamera.ScreenPointToRay(mousePosition);
			int layerMask = 1 << LayerMask.NameToLayer("PhoneView");
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 20f, layerMask) && hitInfo.collider == phonescreencollider)
			{
				Vector2 textureCoord = hitInfo.textureCoord;
				return new Vector3(textureCoord.x, textureCoord.y, 0f);
			}
			return Vector3.one * -1f;
		}
		if (controltype == ControlType.TouchScreen)
		{
			return Vector3.one * -1f;
		}
		Debug.LogWarning("Your touch mode is not correct ~_^");
		return Vector3.one * -1f;
	}

	public static Vector3 TransformPoint(Vector3 point)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return point;
		}
		Vector3 vector = new Vector3(point.x, 0f, point.y);
		vector -= new Vector3(1f, 0f, 1f) * 0.5f;
		vector *= phonescenecamera.orthographicSize * 2f;
		vector.x *= phonescreencollider.bounds.size.x / phonescreencollider.bounds.size.y;
		return vector + phonecontroller.transform.position;
	}

	public static Vector3 GetTransformedTouchPoint()
	{
		return TransformPoint(GetTouchPoint());
	}

	public static bool GetTouchRay(out Ray ray)
	{
		Vector3 touchPoint = GetTouchPoint();
		if (touchPoint == Vector3.one * -1f)
		{
			ray = default(Ray);
			return false;
		}
		ray = phonescenecamera.ScreenPointToRay(touchPoint);
		return true;
	}

	private static Vector2 GetRStickVec()
	{
		Vector2 result = new Vector2(Input.GetAxis("RHorizontal"), Input.GetAxis("RVertical"));
		if (invert_stick)
		{
			result.y = 0f - result.y;
		}
		return result;
	}

	private static bool GetRStickButton()
	{
		return Input.GetButton("CellFire");
	}

	private static bool GetRStickButtonDown()
	{
		return Input.GetButtonDown("CellFire");
	}
}
