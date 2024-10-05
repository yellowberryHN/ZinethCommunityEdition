using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhoneCamControl : PhoneScreen
{
	private static PhoneCamControl _instance;

	public Camera cam;

	public AudioClip snapSound;

	public string[] effectNames = new string[0];

	public List<Component> imageEffects = new List<Component>();

	public RenderTexture rendtex;

	public Material rendmat;

	private Material oldmat;

	public Camera oldcam;

	public GUITexture gui_texture;

	public GUIText gui_text;

	private Vector2 input = Vector2.zero;

	private float hold_timer;

	private float lastclick;

	private Vector2 lastpos = Vector2.zero;

	private bool lastpressed;

	private bool waitingforrelease = true;

	private bool canswitch = true;

	private int effectIndex = -1;

	private bool taking_pic;

	public static PhoneCamControl instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(PhoneCamControl)) as PhoneCamControl;
			}
			return _instance;
		}
	}

	private string normal_message
	{
		get
		{
			if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
			{
				return "Drag & Click to snap a pic";
			}
			return "Click to take a pic";
		}
	}

	private void Awake()
	{
		if (!cam)
		{
			cam = GameObject.Find("SubCamera").camera;
		}
		oldcam = controller.phonecam;
		rendtex = new RenderTexture(480, 800, 1);
		rendmat.mainTexture = rendtex;
		cam.targetTexture = rendtex;
		if (gui_text == null)
		{
			gui_text = GameObject.Find("SubCameraGUI").guiText;
		}
		if (gui_texture == null)
		{
			gui_texture = GameObject.Find("SubCameraGUI").guiTexture;
		}
		string[] array = effectNames;
		foreach (string type in array)
		{
			Component component = cam.GetComponent(type);
			if (component != null)
			{
				imageEffects.Add(component);
			}
		}
		foreach (Component imageEffect in imageEffects)
		{
			(imageEffect as MonoBehaviour).enabled = false;
		}
		SetTextureColor(Color.green);
		ResetMessage();
	}

	public override void UpdateScreen()
	{
		if (!taking_pic)
		{
			DoEffects();
		}
		if (cam.backgroundColor != Camera.main.backgroundColor)
		{
			cam.backgroundColor = Camera.main.backgroundColor;
		}
		Vector3 euler = default(Vector3);
		bool flag = false;
		if (PhoneInput.controltype == PhoneInput.ControlType.Mouse)
		{
			Vector2 vector = Vector2.zero;
			Vector3 touchPoint = PhoneInput.GetTouchPoint();
			if (touchPoint == Vector3.one * -1f)
			{
				hold_timer = 0f;
				vector = Vector2.zero;
			}
			else if (!PhoneInput.IsPressed())
			{
				if (hold_timer == 0f)
				{
					hold_timer = 0.5f;
				}
			}
			else
			{
				hold_timer = 0f;
				vector.x = touchPoint.x - 0.5f;
				vector.y = touchPoint.y - 0.5f;
				vector = Vector2.ClampMagnitude(vector * 4f, 1f);
			}
			if (!taking_pic)
			{
				hold_timer -= Time.deltaTime;
			}
			if (hold_timer <= 0f)
			{
				input = vector;
				hold_timer = 0f;
			}
			flag = false;
			if (PhoneInput.IsPressed() && touchPoint != Vector3.one * -1f)
			{
				if (PhoneInput.IsPressedDown() && (lastclick <= 0.42f || lastclick <= Time.deltaTime) && Vector3.Distance(lastpos, touchPoint) <= 0.1f)
				{
					flag = true;
				}
				lastclick = 0f;
			}
			else if (!PhoneInput.IsPressed() && lastpressed)
			{
				lastpos = touchPoint;
			}
			if (flag)
			{
				lastclick = 1f;
			}
			lastpressed = PhoneInput.IsPressed();
		}
		else
		{
			hold_timer = 0f;
			flag = true;
			input = PhoneInput.GetControlDir();
		}
		lastclick += Time.deltaTime;
		if (waitingforrelease)
		{
			if (input.magnitude <= 0.05f)
			{
				waitingforrelease = false;
			}
			else
			{
				input = Vector2.zero;
			}
		}
		euler.y = input.x * 45f;
		euler.x = input.y * -45f;
		if (!taking_pic)
		{
			cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.Euler(euler), Time.deltaTime * 10f);
		}
		if (flag && !taking_pic && PhoneInput.IsPressedDown())
		{
			StartCoroutine("TakePic");
		}
	}

	private void DoEffects()
	{
		int num = effectIndex;
		if (Input.GetAxisRaw("EffectSwitch") > 0.5f)
		{
			if (canswitch)
			{
				effectIndex++;
				canswitch = false;
			}
		}
		else
		{
			if (!(Input.GetAxisRaw("EffectSwitch") < -0.5f))
			{
				canswitch = true;
				return;
			}
			if (canswitch)
			{
				effectIndex--;
				canswitch = false;
			}
		}
		if (effectIndex >= imageEffects.Count)
		{
			effectIndex = -1;
		}
		else if (effectIndex < -1)
		{
			effectIndex = imageEffects.Count - 1;
		}
		if (effectIndex != num)
		{
			SetEffectActive(num, false);
			SetEffectActive(effectIndex, true);
			ResetMessage();
		}
	}

	public void SetEffectActive(int index, bool isactive)
	{
		if (index >= 0)
		{
			(imageEffects[index] as MonoBehaviour).enabled = isactive;
		}
	}

	public void SetTextureColor(Color col)
	{
		float a = gui_texture.color.a;
		col.a = a;
		gui_texture.color = col;
	}

	public void SetMessage(string message)
	{
		SetMessage(message, 3f);
	}

	public void SetMessage(string message, float wait)
	{
		if (gui_text.text != message)
		{
			gui_text.text = message;
		}
		CancelInvoke("ResetMessage");
		Invoke("ResetMessage", wait);
	}

	public void SetMessage(string message, Color color, float wait)
	{
		gui_text.material.color = color;
		SetMessage(message, wait);
	}

	public void SetMessage(string message, Color color)
	{
		gui_text.material.color = color;
		SetMessage(message);
	}

	public void ResetMessage()
	{
		string arg = "none";
		if (effectIndex != -1)
		{
			arg = string.Format("{0}/{1}", (effectIndex + 1).ToString(), imageEffects.Count.ToString());
		}
		string arg2 = "[,]";
		if (PhoneInput.controltype == PhoneInput.ControlType.Keyboard)
		{
			arg2 = "DPad";
		}
		gui_text.text = string.Format("{1}\n{2} cycle effects-{0}", arg, normal_message, arg2);
		gui_text.material.color = Color.white;
	}

	public override void OnPause()
	{
		cam.gameObject.active = false;
		cam.enabled = false;
	}

	public override void OnResume()
	{
		cam.gameObject.active = true;
		cam.enabled = true;
	}

	public override void OnLoad()
	{
		base.gameObject.SetActiveRecursively(true);
		cam.gameObject.active = true;
		cam.enabled = true;
		cam.gameObject.active = true;
		if ((bool)gui_text)
		{
			gui_text.enabled = true;
		}
		if ((bool)gui_texture)
		{
			gui_texture.enabled = true;
		}
		oldcam.enabled = false;
		oldcam.gameObject.active = false;
		oldmat = PhoneInterface.view_controller.phoneviewscreen.renderer.material;
		PhoneInterface.view_controller.phoneviewscreen.renderer.material = rendmat;
		PhoneInterface.view_controller.phoneviewscreen.renderer.material.mainTexture = rendtex;
		oldmat.mainTextureScale = Vector2.one;
		oldmat.mainTextureOffset = Vector2.zero;
		rendmat.mainTextureScale = Vector2.one;
		rendmat.mainTextureOffset = Vector2.zero;
		string[] array = new string[5] { "Loading EdgeReject filter...", "Reskinning JSR...", "Pretending to be artsy...", "Searching your dreams...", "Disabling sandworms..." };
		string message = array[UnityEngine.Random.Range(0, array.Length)];
		SetMessage(message, Color.Lerp(Color.red, Color.yellow, 5f), 1f);
		hold_timer = 0f;
		waitingforrelease = true;
	}

	public override void OnExit()
	{
		if ((bool)gui_text)
		{
			gui_text.enabled = false;
		}
		if ((bool)gui_texture)
		{
			gui_texture.enabled = false;
		}
		cam.enabled = false;
		cam.gameObject.active = false;
		oldcam.enabled = true;
		oldcam.gameObject.active = true;
		PhoneInterface.view_controller.phoneviewscreen.renderer.material = oldmat;
		base.OnExit();
	}

	private IEnumerator TakePic()
	{
		while (taking_pic)
		{
			yield return null;
		}
		taking_pic = true;
		cam.GetComponent<GUILayer>().enabled = false;
		if ((bool)snapSound)
		{
			AudioSource.PlayClipAtPoint(snapSound, Vector3.zero);
		}
		yield return new WaitForEndOfFrame();
		Texture2D ntex = new Texture2D(480, 800, TextureFormat.RGB24, false);
		RenderTexture.active = rendtex;
		ntex.ReadPixels(new Rect(0f, 0f, 480f, 800f), 0, 0);
		RenderTexture.active = null;
		yield return null;
		byte[] bytes = ntex.EncodeToPNG();
		yield return null;
		string timestr = DateTime.Now.ToString("M-d-yyyy_H-mm-ss");
		// this replace function might be unneeded due to the format string above?
		string filename = timestr.Replace(' ', '_').Replace(':', '-').Replace('/', '-') + ".png";
		File.WriteAllBytes(filename, bytes);
		SetMessage(string.Format("Saved: {0}", filename));
		if (Application.isEditor)
		{
			Debug.Log(string.Format("Took screenshot to: {0}", filename));
		}
		yield return null;
		UnityEngine.Object.Destroy(ntex);
		cam.enabled = false;
		yield return new WaitForSeconds(0.25f);
		cam.enabled = true;
		cam.GetComponent<GUILayer>().enabled = true;
		taking_pic = false;
	}

	public static List<NetPlayer> GetPlayersInView(Camera camr)
	{
		string text = string.Empty;
		List<NetPlayer> list = new List<NetPlayer>();
		foreach (NetPlayer value in Networking.netplayer_dic.Values)
		{
			if (value.networkPlayer == Network.player)
			{
				continue;
			}
			Vector3 vector = value.transform.position + value.transform.up * 2.5f;
			Vector3 vector2 = camr.WorldToViewportPoint(vector);
			if (vector2.x >= 0f && vector2.y >= 0f && vector2.x < 1f && vector2.y < 1f && vector2.z > 0f)
			{
				Vector3 direction = vector - camr.transform.position;
				RaycastHit hitInfo;
				if (!Physics.Raycast(camr.transform.position, direction, out hitInfo, vector2.z))
				{
					list.Add(value);
					string text2 = text;
					text = text2 + value.userName + " " + vector2.x + ", " + vector2.y + "\n";
				}
			}
		}
		return list;
	}
}
