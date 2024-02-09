using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
	public enum PowerState
	{
		closed,
		open,
		off
	}

	public Transform phoneback;

	public Camera phonecam;

	public Camera phoneviewcam;

	public PhoneViewController phoneviewcontroller;

	// TODO: this needs to be stubbed out, as it's serialized
	public TwitterDemo demo;

	public Color backcolor = Color.gray;

	public string startscreen = "Main_Menu";

	public PhoneScreen curscreen;

	public PhoneButton buttonprefab;

	public PhoneMenuLine[] menulines;

	private static Transform trans;

	public static Vector3 presspos = Vector3.zero;

	public static PowerState powerstate = PowerState.off;

	public static ParticleSystem particlesys;

	public ParticleSystem particleSystemPrefab;

	public PhoneMemory phonememory;

	public string returnname;

	public bool allow_home = true;

	public bool allow_open = true;

	public bool allow_close = true;

	public bool open_at_start;

	public static bool _use_fixed_update = false;

	public Dictionary<string, PhoneScreen> screendict = new Dictionary<string, PhoneScreen>();

	public List<PhoneScreen> bgscreens = new List<PhoneScreen>();

	private static PhoneController _instance;

	private PhoneParser parser;

	public bool load_tut = true;

	private static ParticleSystem[] part_array = new ParticleSystem[10];

	private int ringcount;

	private float ringinterval = 1f;

	private float ringtimer;

	private int vibratecount;

	private float vibrateinterval = 0.5f;

	private float vibratetimer;

	public GameObject _buttonBackPrefab;
	
	private GameObject phonescreenbottom;

	private GameObject phonescreentop;

	private GameObject phonebuttonopen;

	private GameObject phonebuttonhome;

	public static float deltatime
	{
		get
		{
			if (_use_fixed_update)
			{
				return Time.fixedDeltaTime;
			}
			return Time.deltaTime;
		}
	}

	public static PhoneController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(PhoneController)) as PhoneController;
			}
			return _instance;
		}
	}

	public static GameObject buttonBackPrefab
	{
		get
		{
			return instance._buttonBackPrefab;
		}
	}

	public static bool DoPhoneCommand(string command)
	{
		return instance.DoCommand(command);
	}

	private void Start()
	{
		trans = base.transform;
		PhoneTextController.buttonprefab = buttonprefab;
		if (parser == null)
		{
			parser = base.gameObject.AddComponent<PhoneParser>();
		}
		parser.Init(this);
		if (phoneback == null)
		{
			phoneback = GameObject.Find("PhoneBack").transform;
		}
		if (phonecam == null)
		{
			phonecam = GameObject.Find("PhoneCamera").camera;
		}
		if (phoneviewcam == null)
		{
			phoneviewcam = GameObject.Find("PhoneViewCamera").camera;
		}
		if (phonescreentop == null)
		{
			phonescreentop = GameObject.Find("PhoneScreenTop");
			if (phonebuttonopen == null)
			{
				phonebuttonopen = phonescreentop.transform.FindChild("PhoneScreenOpenButton").gameObject;
			}
		}
		if (phonescreenbottom == null)
		{
			phonescreenbottom = GameObject.Find("PhoneScreenBottom");
			if (phonebuttonhome == null)
			{
				phonebuttonhome = phonescreenbottom.transform.FindChild("PhoneScreenHomeButton").gameObject;
			}
		}
		if (particlesys == null)
		{
			particlesys = particleSystemPrefab;
		}
		if (phoneviewcontroller == null)
		{
			phoneviewcontroller = Object.FindObjectOfType(typeof(PhoneViewController)) as PhoneViewController;
		}
		PhoneInput.phonecontroller = this;
		PhoneInput.phonescenecamera = phonecam;
		PhoneInput.phonescreencamera = phoneviewcam;
		phonecam.gameObject.active = false;
		if ((bool)GameObject.Find("PhoneScreen"))
		{
			PhoneInput.phonescreencollider = GameObject.Find("PhoneScreen").collider;
		}
		BootUp();
		if (open_at_start)
		{
			Invoke("OpenUp", 0.1f);
		}
	}

	private void OpenUp()
	{
		PhoneInterface.view_controller.SetOpen(true, true);
	}

	private void BootUp()
	{
		presspos = trans.position;
		SetColors();
		SetupScreenDict();
		InitScreens();
		if (load_tut)
		{
			LoadScreenQuiet("Tutorial");
		}
		else
		{
			LoadScreenQuiet(startscreen);
		}
		powerstate = PowerState.closed;
	}

	private void SetupScreenDict()
	{
		screendict.Clear();
		PhoneScreen[] componentsInChildren = base.transform.GetComponentsInChildren<PhoneScreen>();
		foreach (PhoneScreen phoneScreen in componentsInChildren)
		{
			screendict.Add(phoneScreen.screenname, phoneScreen);
		}
	}

	private void InitScreens()
	{
		foreach (PhoneScreen value in screendict.Values)
		{
			if (value.controller == null)
			{
				value.controller = this;
			}
			value.Init();
			if (value.bgscreen)
			{
				bgscreens.Add(value);
			}
			else
			{
				value.gameObject.SetActiveRecursively(false);
			}
		}
	}

	private void Update()
	{
		Input.GetButtonDown("InvertPhoneStick");
		if (!_use_fixed_update)
		{
			switch (powerstate)
			{
			case PowerState.open:
				Update_Open();
				break;
			case PowerState.closed:
				Update_Closed();
				break;
			case PowerState.off:
				Update_Off();
				break;
			}
		}
	}

	private void Update_Open()
	{
		HandleRinging();
		PhoneInput.DetectControlType();
		UpdateScreen();
		UpdateBgScreens();
	}

	private void Update_Closed()
	{
		HandleRinging();
	}

	private void Update_Off()
	{
	}

	public bool DoCommand(string command)
	{
		return parser.DoStringCommand(command);
	}

	private void UpdateScreen()
	{
		if ((bool)curscreen)
		{
			curscreen.UpdateScreen();
		}
	}

	private void UpdateBgScreens()
	{
		foreach (PhoneScreen bgscreen in bgscreens)
		{
			bgscreen.UpdateScreen();
		}
	}

	public PhoneScreen getScreen(string screen)
	{
		if (screendict.ContainsKey(screen))
		{
			return screendict[screen];
		}
		return null;
	}

	public bool LoadScreenQuiet(string screenname)
	{
		PhoneScreen screen = getScreen(screenname);
		if (screen != null)
		{
			LoadScreenQuiet(screen);
			return true;
		}
		Debug.LogWarning("tried to load nonexisting screen: " + screenname);
		return false;
	}

	public bool LoadScreen(string screenname)
	{
		PhoneScreen screen = getScreen(screenname);
		if (screen != null)
		{
			LoadScreen(screen);
			return true;
		}
		Debug.LogWarning("tried to load nonexisting screen: " + screenname);
		return false;
	}

	public void LoadScreen(PhoneScreen screen)
	{
		if (curscreen != null && curscreen.screenname == startscreen && (bool)screen.clip)
		{
			PhoneAudioController.PlayAudioClip(screen.clip, SoundType.menu);
		}
		LoadScreenQuiet(screen);
	}

	public void LoadScreenQuiet(PhoneScreen screen)
	{
		PhoneScreen phoneScreen = curscreen;
		curscreen = screen;
		if (phoneScreen != null)
		{
			returnname = phoneScreen.screenname;
			phoneScreen.OnExit();
		}
		screen.transform.position = trans.position;
		if (screen.clearparticles)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
		}
		screen.OnLoad();
	}

	public bool LoadPrevious()
	{
		return LoadScreen(returnname);
	}

	public void SetColors()
	{
		SetBackColor();
		SetPhoneColor();
		//SetPhoneButtonColor();
		SetPhoneButtonHighlight();
	}
	
	public void SetBackColor()
	{
		Color backgroundColor = PhoneMemory.settings.backgroundColor;
		if (Application.platform != RuntimePlatform.Android)
		{
			backgroundColor.a = 0.9f;
		}
		phoneback.renderer.material.color = backgroundColor;
	}

	public void SetBackColor(Color color)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			color.a = 0.9f;
		}
		phoneback.renderer.material.color = color;
	}

	public void SetPhoneColor()
	{
		SetPhoneColor(PhoneMemory.settings.phoneColor);
	}
	
	public void SetPhoneColor(Color color)
	{
		var phoneparts = new[]
		{
			phonescreentop,
			phonescreenbottom
		};
		foreach (var part in phoneparts)
		{
			var meshRenderer = part.transform.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.material.SetColor("_Color", color);
			}
		}
	}
	
	public void SetPhoneButtonColor()
	{
		SetPhoneButtonColor(PhoneMemory.settings.phoneButtonColor);
	}
	
	
	// TODO: this doesn't persist very well, look into it.
	public void SetPhoneButtonColor(Color color)
	{
		phonebuttonopen.transform.GetComponent<PhoneViewOpenButton>().normalcolor = color;
		phonebuttonopen.transform.GetComponent<PhoneViewOpenButton>().wantedColor = color;
		phonebuttonhome.transform.GetComponent<PhoneViewHomeButton>().normalcolor = color;
	}
	
	public void SetPhoneButtonHighlight()
	{
		SetPhoneButtonHighlight(PhoneMemory.settings.phoneButtonHighlightColor);
	}
	
	public void SetPhoneButtonHighlight(Color color)
	{
		var phoneparts = new[]
		{
			phonebuttonopen.transform.FindChild("PhoneSpritePlane").gameObject,
			phonebuttonhome.transform.FindChild("PhoneSpritePlane").gameObject
		};
		foreach (var part in phoneparts)
		{
			var meshRenderer = part.transform.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.material.SetColor("_Color", color);
			}
		}
	}

	public void OnScreenOpen()
	{
		OnScreenOpen(false);
	}

	public void OnScreenOpen(bool force)
	{
		if (!allow_open && !force)
		{
			return;
		}
		powerstate = PowerState.open;
		phonecam.gameObject.active = true;
		StopRinging();
		PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_open, SoundType.menu);
		if ((bool)curscreen)
		{
			curscreen.OnResume();
		}
		if ((bool)PhoneInterface.player_move)
		{
			Animation animation = PhoneInterface.player_move.model.animation;
			if (animation["PhoneArm"] != null)
			{
				animation["PhoneArm"].speed = Mathf.Abs(animation["PhoneArm"].speed);
				animation["PhoneArm"].wrapMode = WrapMode.ClampForever;
				animation["PhoneArm"].weight = 0.5f;
				animation["PhoneArm"].enabled = true;
				animation.CrossFade("PhoneArm");
			}
		}
		phonecam.enabled = true;
		phonecam.gameObject.active = true;
	}

	public void OnScreenClose()
	{
		OnScreenClose(false);
	}

	public void OnScreenClose(bool force)
	{
		if (!allow_close && !force)
		{
			return;
		}
		powerstate = PowerState.closed;
		PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_close, SoundType.menu);
		if ((bool)curscreen)
		{
			curscreen.OnPause();
		}
		if ((bool)PhoneInterface.player_move)
		{
			Animation animation = PhoneInterface.player_move.model.animation;
			if (animation["PhoneArm"] != null)
			{
				animation["PhoneArm"].speed = 0f - Mathf.Abs(animation["PhoneArm"].speed);
				animation["PhoneArm"].wrapMode = WrapMode.Once;
				animation["PhoneArm"].time = animation["PhoneArm"].length;
				animation.CrossFade("PhoneArm");
			}
		}
		phonecam.enabled = false;
	}

	public void OnPowerOff()
	{
		phonecam.gameObject.active = false;
	}

	public void OnHomePressed()
	{
		OnHomePressed(false);
	}

	public void OnHomePressed(bool force)
	{
		if (powerstate == PowerState.open)
		{
			if (allow_home || force)
			{
				PhoneAudioController.PlayAudioClip(PhoneAudioController.audcon.clip_back, SoundType.menu);
				presspos = trans.position + trans.forward * -4f;
				LoadScreenQuiet(startscreen);
				getScreen(startscreen).OnHomeLoad();
			}
		}
		else if (powerstate == PowerState.closed)
		{
			StopRinging();
		}
	}

	public static void EmitPartsMenu(Vector3 pos, int amount)
	{
		EmitParts(pos, amount, PhoneMemory.settings.particleColor);
	}

	public static void EmitParts(Vector3 pos, int amount)
	{
		EmitParts(pos, amount, Color.black);
	}

	public static void EmitParts(Vector3 pos, int amount, Color color)
	{
		ParticleSystem particleSystem = MakeParts(pos);
		particleSystem.startColor = color;
		particleSystem.renderer.material.color = color;
		particleSystem.Emit(amount);
	}

	private static ParticleSystem MakeParts(Vector3 pos)
	{
		for (int i = 0; i < part_array.Length; i++)
		{
			if (part_array[i] == null)
			{
				ParticleSystem component = (Object.Instantiate(particlesys.gameObject) as GameObject).GetComponent<ParticleSystem>();
				component.transform.position = pos;
				component.transform.parent = trans;
				part_array[i] = component;
				return component;
			}
			if (!part_array[i].isPlaying)
			{
				part_array[i].transform.position = pos;
				return part_array[i];
			}
		}
		Debug.LogWarning("hey! you are trying to use more particle systems than the array has room for! wow...");
		return null;
	}

	public void OnNewMessage()
	{
		ringcount = 6;
		ringtimer = 0f;
	}

	public void OnNewMessage(int rings)
	{
		ringcount += rings;
		if (ringcount > 3)
		{
			ringcount = 3;
		}
		ringtimer = 0f;
	}

	public void StopRinging()
	{
		if ((bool)PhoneAudioController.gobj_ring)
		{
			Object.Destroy(PhoneAudioController.gobj_ring.gameObject);
		}
		vibratecount = 0;
		ringcount = 0;
		phoneviewcam.transform.localPosition = Vector3.zero;
		phoneviewcontroller.SetLightBrightness(0f);
	}

	public void HandleRinging()
	{
		if (ringcount < 0)
		{
			return;
		}
		if (!PhoneAudioController.gobj_ring)
		{
			if (ringtimer <= 0f && ringcount > 0)
			{
				Ring();
			}
			ringtimer -= deltatime;
		}
		else
		{
			if (vibratecount > 0 && vibratetimer <= 0f)
			{
				Rumble();
			}
			vibratetimer -= deltatime;
		}
		if ((bool)phoneviewcam)
		{
			phoneviewcam.transform.localPosition = Random.insideUnitSphere * XInput.GetPhoneVibrateForce().magnitude * 0.15f;
		}
		if ((bool)phoneviewcontroller)
		{
			phoneviewcontroller.SetLightBrightness(XInput.GetPhoneVibrateForce().magnitude * 5f);
		}
	}

	private void Rumble()
	{
		XInput.AddPhoneVibrateForce(0.5f, 1f, 0.25f);
		vibratetimer = vibrateinterval;
	}

	private void Ring()
	{
		if (PhoneAudioController.StartRinging())
		{
			ringcount--;
		}
		ringtimer = ringinterval;
		vibratecount = 2;
		vibratetimer = 0f;
	}

	
}
