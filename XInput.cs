using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class XInput : MonoBehaviour
{
	private static bool playerIndexSet = false;

	private static PlayerIndex playerIndex;

	public static GamePadState state;

	private static GamePadState prevState;

	private static List<VibrateForce> vibrationlist = new List<VibrateForce>();

	public bool _allow_vibrate = true;

	public static bool allow_vibrate = true;

	private float wallridepower = 0.4f;

	private float landpower = 0.75f;

	private static float grindpower = 0.25f;

	private static Vector2 curvib;

	private static Vector2 phonevib;

	public static float GetTriggerR()
	{
		return state.Triggers.Right;
	}

	public static float GetTriggerL()
	{
		return state.Triggers.Left;
	}

	public static void SetVibration(Vector2 vib)
	{
		SetVibration(vib.x, vib.y);
	}

	public static void SetVibration(float left, float right)
	{
		if (playerIndexSet)
		{
			if (allow_vibrate)
			{
				GamePad.SetVibration(playerIndex, left, right);
			}
			else
			{
				GamePad.SetVibration(playerIndex, 0f, 0f);
			}
		}
	}

	public static void AddPhoneVibrateForce(float left, float right, float time)
	{
		VibrateForce vibrateForce = new VibrateForce(left, right, time);
		vibrateForce.is_phone = true;
		vibrationlist.Add(vibrateForce);
	}

	public static void AddVibrateForce(float left, float right, float time, bool decay)
	{
		vibrationlist.Add(new VibrateForce(left, right, time, decay));
	}

	public static void AddVibrateForce(float left, float right, float time)
	{
		vibrationlist.Add(new VibrateForce(left, right, time));
	}

	public static Vector2 GetVibrateForce()
	{
		return curvib;
	}

	public static Vector2 GetPhoneVibrateForce()
	{
		return phonevib;
	}

	private void Update()
	{
		if (Application.isWebPlayer)
		{
			return;
		}
		bool flag = true;
		if (!prevState.IsConnected || !playerIndexSet)
		{
			flag = false;
			string[] joystickNames = Input.GetJoystickNames();
			foreach (string text in joystickNames)
			{
				if (text == "Controller (XBOX 360 For Windows)" || text == "Controller (Xbox 360 Wireless Receiver for Windows)")
				{
					flag = true;
				}
			}
			if (flag)
			{
				FindPlayerIndex();
			}
		}
		allow_vibrate = _allow_vibrate;
		if (playerIndexSet)
		{
			state = GamePad.GetState(playerIndex);
		}
		HandleVibrate();
		prevState = state;
	}

	private void FindPlayerIndex()
	{
		for (int i = 0; i < 1; i++)
		{
			PlayerIndex playerIndex = (PlayerIndex)i;
			if (GamePad.GetState(playerIndex).IsConnected)
			{
				XInput.playerIndex = playerIndex;
				playerIndexSet = true;
			}
		}
	}

	public void LandVibrate()
	{
		vibrationlist.Add(new VibrateForce(landpower, landpower, 0.4f, true));
	}

	public void LandVibrateForce(float force)
	{
		vibrationlist.Add(new VibrateForce(force, 0f, 0.25f, true));
	}

	public void WallRideL()
	{
		WallRideBoth();
	}

	public void WallRideR()
	{
		WallRideBoth();
	}

	public void WallRideBoth()
	{
		vibrationlist.Add(new VibrateForce(0.1f, wallridepower, Time.deltaTime, false));
	}

	public static void GrindingVibrate()
	{
		vibrationlist.Add(new VibrateForce(grindpower, grindpower, Time.deltaTime, false));
	}

	public void Grinding()
	{
		GrindingVibrate();
	}

	public void Skate()
	{
	}

	private void HandleVibrate()
	{
		curvib = Vector2.zero;
		phonevib = Vector2.zero;
		for (int i = 0; i < vibrationlist.Count; i++)
		{
			Vector2 vector = vibrationlist[i].OnUpdate();
			if (vibrationlist[i].is_phone)
			{
				phonevib += vector;
			}
			vector *= PhoneMemory.settings.vibrate_amount;
			curvib += vector;
			if (vibrationlist[i].life <= 0f)
			{
				vibrationlist.RemoveAt(i);
				i--;
			}
		}
		SetVibration(curvib);
	}

	private void OnApplicationFocus(bool focus)
	{
		SetVibration(Vector2.zero);
	}

	private void OnApplicationPause()
	{
		SetVibration(Vector2.zero);
	}

	private void OnApplicationQuit()
	{
		SetVibration(Vector2.zero);
	}
}
