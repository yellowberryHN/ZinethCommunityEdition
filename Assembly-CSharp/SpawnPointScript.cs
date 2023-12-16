using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
	private enum State
	{
		Normal,
		Grinding,
		WallRiding
	}

	public Transform player;

	public Transform cameraTrans;

	private Transform playerAnimation;

	public static int spawnNum = 200;

	public static float spawnGap = 0f;

	public bool canRespawn = true;

	public DebugPoint[] debug_checkpoints;

	private float currentGap = spawnGap;

	private List<SpawnPoint> spawnList = new List<SpawnPoint>();

	private List<GameObject> oldSpawns = new List<GameObject>();

	private List<float> oldSpawnDecays = new List<float>();

	private int currentLoc;

	public bool isRespawning;

	private float lastTime;

	private float lastSpeed;

	private float lastRewindTime;

	private float rewindGap = spawnGap;

	private float timePassed;

	private SplineGrinding splineGrinding;

	private string inputString = string.Empty;

	private string totalInput = string.Empty;

	private float resetTime = 2f;

	private float maxResetTime = 2f;

	private static SpawnPointScript _instance;

	public static float trailTime = 0f;

	public bool loadPackage;

	public bool loaded;

	private WWW web;

	public static string[] bundlenames = new string[0];

	private GameObject bundleObj;

	private static AssetBundle curBundle;

	public byte[] curBundleBytes;

	public string curBundleName;

	private float stopRecordingTimer;

	private move moveScript
	{
		get
		{
			return PhoneInterface.player_move;
		}
	}

	public static SpawnPointScript instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(SpawnPointScript)) as SpawnPointScript;
			}
			return _instance;
		}
	}

	public static string filePath
	{
		get
		{
			return Path.Combine(Application.dataPath, Path.Combine("whatever", "Bundles"));
		}
	}

	private void Awake()
	{
		if (trailTime == 0f)
		{
			trailTime = PhoneInterface.playerTrail.trailList[0].time;
		}
		StartCoroutine(DoAwake());
	}

	public static void GetBundles()
	{
		string text = filePath;
		if (Directory.Exists(text))
		{
			bundlenames = Directory.GetFiles(text);
			return;
		}
		Debug.Log("creating directory... " + text);
		Directory.CreateDirectory(text);
	}

	public static bool HasBundle(string nam)
	{
		GetBundles();
		string[] array = bundlenames;
		foreach (string path in array)
		{
			if (Path.GetFileName(path) == nam)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator DoAwake()
	{
		GetBundles();
		if (loadPackage && bundlenames.Length > 0)
		{
			yield return LoadBundleByInd(0);
			yield break;
		}
		SpawnPlayerAtStart();
		loaded = true;
	}

	private IEnumerator LoadBundleByInd(int ind)
	{
		if (ind < bundlenames.Length)
		{
			yield return LoadBundle(bundlenames[ind]);
		}
		Debug.LogWarning("bundle ind out of bounds!!!! " + ind);
	}

	private IEnumerator LoadBundleBytes(string nam, byte[] data)
	{
		loaded = false;
		string path = Path.Combine(filePath, nam);
		Debug.Log(path);
		File.WriteAllBytes(path, data);
		Debug.Log(data.Length);
		AssetBundleCreateRequest request = AssetBundle.CreateFromMemory(data);
		yield return request;
		AssetBundle bundle = request.assetBundle;
		if (bundle == null)
		{
			Debug.LogWarning("bundle is null...");
		}
		else
		{
			if ((bool)bundleObj)
			{
				Object.Destroy(bundleObj.gameObject);
			}
			if ((bool)curBundle)
			{
				curBundle.Unload(true);
			}
			curBundle = bundle;
			curBundleBytes = data;
			curBundleName = nam;
			bundleObj = Object.Instantiate(curBundle.mainAsset) as GameObject;
		}
		loaded = true;
		yield return null;
		SpawnPlayerAtStart();
	}

	private IEnumerator LoadBundle(string nam)
	{
		loaded = false;
		string fpath = nam;
		bool local = true;
		if (!nam.StartsWith("http"))
		{
			fpath = "file://" + Path.Combine(filePath, nam);
		}
		else
		{
			local = false;
		}
		web = new WWW(fpath);
		MonoBehaviour.print("grabbin " + web.url);
		yield return web;
		if (web.error != null)
		{
			Debug.LogWarning("error: " + web.error);
			ShowError("error: " + web.error + "\n" + nam);
		}
		else if (web.assetBundle == null)
		{
			ShowError("no bundle: " + nam);
		}
		else
		{
			if ((bool)bundleObj)
			{
				Object.Destroy(bundleObj.gameObject);
			}
			if ((bool)curBundle)
			{
				curBundle.Unload(true);
			}
			curBundle = web.assetBundle;
			curBundleBytes = web.bytes;
			curBundleName = nam;
			if (!local)
			{
				string path = Path.Combine(filePath, nam.Substring(nam.LastIndexOf("/") + 1));
				Debug.Log(path);
				if (!File.Exists(path))
				{
					File.WriteAllBytes(path, curBundleBytes);
				}
			}
			bundleObj = Object.Instantiate(curBundle.mainAsset) as GameObject;
			Debug.Log("instantiated... " + bundleObj);
		}
		yield return null;
		SpawnPlayerAtStart();
		loaded = true;
	}

	private void ShowError(string err)
	{
		MissionGUIText missionGUIText = MissionGUIText.Create(err, new Vector3(0.024f, 0.5714286f, 0f), Vector3.one * 4f);
		missionGUIText.color = Color.red;
		missionGUIText.velocity = Vector3.up * -0.05f;
		missionGUIText.stopAfter = 0.5f;
		missionGUIText.lifeTime = 4f;
	}

	public void SpawnPlayerAtStart()
	{
		if (splineGrinding == null)
		{
			splineGrinding = GameObject.Find("GrindPoint").GetComponent<SplineGrinding>();
		}
		if (playerAnimation == null)
		{
			playerAnimation = GameObject.Find("main_character").transform;
		}
		if (moveScript.isGrinding)
		{
			splineGrinding.bail();
		}
		if ((bool)PhoneInterface.hawk && PhoneInterface.hawk.targetHeld)
		{
			PhoneInterface.hawk.Drop();
		}
		if (isRespawning)
		{
			StopRewind();
		}
		Transform transform = base.transform;
		if ((bool)GameObject.Find("PlayerSpawn"))
		{
			transform = GameObject.Find("PlayerSpawn").transform;
		}
		if (Application.loadedLevelName == "test")
		{
			if ((bool)PhoneInterface.hawk)
			{
				PhoneInterface.hawk.dropLocation.transform.position = transform.transform.position + Vector3.up * 30f;
				PhoneInterface.hawk.dropLocation.transform.rotation = transform.transform.rotation;
				if ((bool)PhoneInterface.hawk.tempHawkRend)
				{
					Object.Destroy(PhoneInterface.hawk.tempHawkRend.gameObject);
					PhoneInterface.hawk.fly.renderer.enabled = true;
					PhoneInterface.hawk.carry.renderer.enabled = true;
				}
				GameObject gameObject = GameObject.Find("HawkObject");
				if ((bool)gameObject)
				{
					GameObject gameObject2 = Object.Instantiate(gameObject) as GameObject;
					PhoneInterface.hawk.tempHawkRend = gameObject2;
					gameObject2.transform.parent = PhoneInterface.hawk.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					Debug.Log("added hawk obj...");
					PhoneInterface.hawk.fly.renderer.enabled = false;
					PhoneInterface.hawk.carry.renderer.enabled = false;
				}
			}
			bool flag = false;
			MusicManager.override_vol = 1f;
			if (Application.loadedLevelName == "test" && (bool)GameObject.Find("MusicObject"))
			{
				flag = true;
			}
			if ((bool)MusicManager.instance)
			{
				if (flag)
				{
					MusicManager.instance.enabled = false;
					MusicManager.override_vol = 0f;
					MusicManager.instance.audio.Stop();
				}
				else
				{
					MusicManager.instance.enabled = true;
					MusicManager.override_vol = 1f;
					if (!MusicManager.instance.audio.isPlaying)
					{
						MusicManager.instance.audio.Play();
					}
				}
				MusicManager.base_vol = MusicManager.base_vol;
			}
			SplineGrinding.instance.RefreshRails();
		}
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		string animationState = "Idle";
		string animationName = "Idle";
		float animationTime = 0f;
		float animationSpeed = 0f;
		int currentGracePeriod = splineGrinding.currentGracePeriod;
		float volume = player.GetComponent<AudioSource>().volume;
		float airTime = moveScript.airTime;
		if (player.rigidbody.isKinematic)
		{
			player.rigidbody.isKinematic = false;
			player.rigidbody.velocity = Vector3.zero;
			player.rigidbody.isKinematic = true;
		}
		else
		{
			player.rigidbody.velocity = Vector3.zero;
		}
		spawnList.Clear();
		NewCamera component = cameraTrans.GetComponent<NewCamera>();
		cameraTrans.rotation = rotation;
		cameraTrans.position = position - component.distanceBehind * 5f * cameraTrans.forward + component.height * Vector3.up * 1.4f;
		spawnList.Add(new SpawnPoint(0, position, rotation, new Vector3(0f, 0f, 0f), animationState, animationName, animationTime, animationSpeed, cameraTrans.position, cameraTrans.rotation, currentGracePeriod, volume, airTime));
		spawnNum = Mathf.FloorToInt(player.GetComponent<PlayerTrail>().decayTime / Time.fixedDeltaTime);
		ReSpawn(0);
	}

	public void LoadAndSpawn(string nam, byte[] data)
	{
		StopCoroutine("LoadBundle");
		StopCoroutine("LoadBundleBytes");
		if ((bool)bundleObj)
		{
			Object.Destroy(bundleObj.gameObject);
		}
		if ((bool)curBundle)
		{
			curBundle.Unload(true);
		}
		Debug.Log("loading bytes..." + data.Length);
		StartCoroutine(LoadBundleBytes(nam, data));
	}

	public void LoadAndSpawn(string bundlename)
	{
		StopCoroutine("LoadBundle");
		StopCoroutine("LoadBundleBytes");
		if ((bool)bundleObj)
		{
			Object.Destroy(bundleObj.gameObject);
		}
		if ((bool)curBundle)
		{
			curBundle.Unload(true);
		}
		Debug.Log("starting to load bundle...");
		StartCoroutine(LoadBundle(bundlename));
	}

	private void Update()
	{
		if (loaded && canRespawn)
		{
			if (isRespawning)
			{
				CheckStopRewind();
			}
			else if (Input.GetButtonDown("Rewind"))
			{
				GameObject.Find("Main Camera").GetComponent<MotionBlur>().enabled = true;
				GameObject.Find("Camera Holder").GetComponent<SoundManager>().StopSound();
				player.GetComponent<MusicManager>().Pause();
				ReparentSpawns();
				lastRewindTime = rewindGap;
				isRespawning = true;
				GameObject.Find("Holder").GetComponent<PlayerGraphic>().isRespawning = true;
				player.GetComponent<move>().freezeControls = true;
				player.GetComponent<Rigidbody>().isKinematic = true;
				cameraTrans.GetComponent<NewCamera>().pauseCamera = true;
				splineGrinding.paused = true;
				splineGrinding.grindTimer = splineGrinding.maxGrindTimer;
				DropPoint(true);
				splineGrinding.bail();
				currentLoc = spawnList.Count - 1;
				TempMovePlayer(currentLoc);
			}
		}
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		float time = Time.time;
		if (!loaded)
		{
			return;
		}
		resetTime -= fixedDeltaTime;
		if (resetTime <= 0f)
		{
			totalInput = string.Empty;
		}
		if (canRespawn)
		{
			if (!isRespawning)
			{
				spawnGap -= time - lastTime;
				lastTime = time;
				if (!moveScript.isGrinding && player.rigidbody.velocity.magnitude < 1f && Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
				{
					stopRecordingTimer += Time.fixedDeltaTime;
				}
				else
				{
					stopRecordingTimer = 0f;
				}
				if (spawnGap <= 0f && stopRecordingTimer < 0.25f)
				{
					DropPoint();
					spawnGap = currentGap;
				}
			}
			if (isRespawning)
			{
				stopRecordingTimer = 0f;
				timePassed += fixedDeltaTime;
				GetRewindInput();
			}
		}
		else
		{
			stopRecordingTimer = 0f;
		}
	}

	private void ReparentSpawns()
	{
		PlayerTrail component = player.GetComponent<PlayerTrail>();
		Color trailColor = component.color;
		for (int i = 0; i < component.trailList.Count; i++)
		{
			GameObject gameObject = new GameObject("spawnHolder");
			gameObject.layer = 2;
			TrailRenderer trailRenderer = gameObject.AddComponent<TrailRenderer>();
			trailRenderer.material = component.trailMaterial;
			trailRenderer.startWidth = component.startWidth;
			trailRenderer.endWidth = component.endWidth;
			trailRenderer.time = trailTime;
			trailRenderer.material.color = trailColor;
			oldSpawns.Add(component.holderObjectList[i]);
			oldSpawnDecays.Add(component.decayTime);
			component.holderObjectList[i].transform.parent = null;
			gameObject.transform.position = component.trailHolderList[i].position;
			gameObject.transform.parent = component.trailHolderList[i];
			component.trailList[i] = gameObject.GetComponent<TrailRenderer>();
			component.trailList[i].time = 0f;
			component.holderObjectList[i] = gameObject;
			component.lastPointList[i] = gameObject.transform.position;
			timePassed = 0f;
		}
		for (int j = 0; j < oldSpawns.Count; j++)
		{
			if (oldSpawns[j] == null)
			{
				Object.Destroy(oldSpawns[j]);
				oldSpawns.RemoveAt(j);
				oldSpawnDecays.RemoveAt(j);
				j--;
			}
			else
			{
				oldSpawnDecays[j] = oldSpawns[j].GetComponent<TrailRenderer>().time;
				oldSpawns[j].GetComponent<TrailRenderer>().time = 999999f;
			}
		}
	}

	private void ReactivateSpawns()
	{
		int num = 0;
		for (num = 0; num < player.GetComponent<PlayerTrail>().trailList.Count; num++)
		{
			int count = player.GetComponent<PlayerTrail>().trailList.Count;
			int index = num + oldSpawns.Count - count;
			oldSpawns[index].GetComponent<TrailRenderer>().time = timePassed + oldSpawnDecays[index];
			oldSpawns[index].GetComponent<TrailRenderer>().autodestruct = true;
			player.GetComponent<PlayerTrail>().trailList[num].time = oldSpawnDecays[index];
		}
		for (num = 0; num < oldSpawns.Count; num++)
		{
			if (oldSpawns[num] == null)
			{
				Object.Destroy(oldSpawns[num]);
				oldSpawns.RemoveAt(num);
				oldSpawnDecays.RemoveAt(num);
				num--;
			}
			else
			{
				oldSpawns[num].GetComponent<TrailRenderer>().time = timePassed + oldSpawnDecays[num];
			}
		}
	}

	public Vector3 GetWallNormal()
	{
		return spawnList[currentLoc].wallNormal;
	}

	private void CheckStopRewind()
	{
		if (Input.GetButtonDown("Skate"))
		{
			StopRewind();
		}
	}

	public void StopRewind()
	{
		player.GetComponent<MusicManager>().PlayForward();
		ReactivateSpawns();
		isRespawning = false;
		GameObject.Find("Holder").GetComponent<PlayerGraphic>().isRespawning = false;
		cameraTrans.GetComponent<NewCamera>().pauseCamera = false;
		splineGrinding.paused = false;
		cameraTrans.GetComponent<NewCamera>().mockCamera.position = cameraTrans.position;
		cameraTrans.GetComponent<NewCamera>().mockCamera.rotation = cameraTrans.rotation;
		lastTime = Time.time;
		ReSpawn(currentLoc);
		if (spawnList.Count > 0)
		{
			spawnList.Remove(spawnList[spawnList.Count - 1]);
		}
		lastRewindTime = rewindGap;
		GameObject.Find("Main Camera").GetComponent<MotionBlur>().enabled = false;
	}

	private void GetRewindInput()
	{
		lastRewindTime -= Time.deltaTime;
		if (!(lastRewindTime <= 0f))
		{
			return;
		}
		if (Input.GetButton("Rewind"))
		{
			currentLoc--;
			if (currentLoc < 0)
			{
				currentLoc = 0;
				GameObject.Find("Camera Holder").GetComponent<SoundManager>().PauseRewind();
				player.GetComponent<MusicManager>().Pause();
			}
			else
			{
				GameObject.Find("Camera Holder").GetComponent<SoundManager>().PlayRewind();
				player.GetComponent<MusicManager>().PlayReversed();
			}
			TempMovePlayer(currentLoc);
			lastRewindTime = rewindGap;
		}
		else if (Input.GetButton("UnRewind"))
		{
			currentLoc++;
			if (currentLoc >= spawnList.Count)
			{
				currentLoc = spawnList.Count - 1;
				GameObject.Find("Camera Holder").GetComponent<SoundManager>().PauseRewind();
				player.GetComponent<MusicManager>().Pause();
			}
			else
			{
				GameObject.Find("Camera Holder").GetComponent<SoundManager>().PlayRewind();
				player.GetComponent<MusicManager>().PlayForward();
			}
			TempMovePlayer(currentLoc);
			lastRewindTime = rewindGap;
		}
		else
		{
			GameObject.Find("Camera Holder").GetComponent<SoundManager>().PauseRewind();
			player.GetComponent<MusicManager>().Pause();
		}
	}

	private void TempMovePlayer(int pos)
	{
		moveScript.freezeControls = true;
		moveScript.wallRiding = false;
		player.GetComponent<Rigidbody>().isKinematic = true;
		moveScript.airTime = spawnList[pos].airTime;
		if (spawnList[pos].currentState == 0)
		{
			moveScript.isGrinding = false;
			splineGrinding.isGrinding = false;
		}
		else if (spawnList[pos].currentState == 1)
		{
			moveScript.isGrinding = true;
			splineGrinding.isGrinding = true;
		}
		else if (spawnList[pos].currentState == 2)
		{
			moveScript.isGrinding = false;
			splineGrinding.isGrinding = false;
		}
		cameraTrans.position = spawnList[pos].cameraPosition;
		cameraTrans.rotation = spawnList[pos].cameraRotation;
		player.position = spawnList[pos].spawnPos;
		player.rotation = spawnList[pos].spawnRot;
		if (spawnList[pos].currentState != 2)
		{
		}
		splineGrinding.currentGracePeriod = spawnList[pos].gracePeriod;
		player.GetComponent<AudioSource>().volume = spawnList[pos].volume;
		playerAnimation.animation.Play(spawnList[pos].animationName);
		AnimationState animationState = playerAnimation.animation[spawnList[pos].animationName];
		animationState.time = spawnList[pos].animationTime;
		lastSpeed = animationState.speed;
		animationState.speed = 0f;
		HawkBehavior hawk = PhoneInterface.hawk;
		hawk.transform.position = spawnList[pos].hawkPos;
		hawk.transform.rotation = spawnList[pos].hawkRot;
		hawk.timeFollowed = spawnList[pos].timeFollowed;
		hawk.swoopDistance = spawnList[pos].swoopDistance;
		hawk.startSwoopDistance = spawnList[pos].startSwoopDistance;
		hawk.inBounds = spawnList[pos].inBounds;
		hawk.targetEngaged = spawnList[pos].targetEngaged;
		hawk.hasSwoopedIn = spawnList[pos].hasSwoopedIn;
	}

	public SpawnPoint GetCurrentSpawnPoint()
	{
		return spawnList[currentLoc];
	}

	public Vector3 GetCurrentVelocity()
	{
		return spawnList[currentLoc].velocity;
	}

	public float GetRailVelocity()
	{
		return spawnList[currentLoc].currentVelocity;
	}

	private void ReSpawn(int pos)
	{
		TempMovePlayer(pos);
		moveScript.airTime = spawnList[pos].airTime;
		if (spawnList[pos].currentState == 0)
		{
			moveScript.freezeControls = false;
			player.GetComponent<Rigidbody>().isKinematic = false;
			player.GetComponent<Rigidbody>().velocity = spawnList[pos].velocity;
			splineGrinding.bail();
		}
		else if (spawnList[pos].currentState == 1)
		{
			splineGrinding.spline = spawnList[pos].spline;
			splineGrinding.currentVelocity = spawnList[pos].currentVelocity;
			splineGrinding.passedTime = spawnList[pos].passedTime;
			splineGrinding.offSet = spawnList[pos].offSet;
			splineGrinding.movePlayerToGrindPoint();
			splineGrinding.forward = spawnList[pos].forward;
		}
		else if (spawnList[pos].currentState == 2)
		{
			moveScript.wallRiding = true;
			moveScript.freezeControls = false;
			player.GetComponent<Rigidbody>().isKinematic = false;
			player.GetComponent<Rigidbody>().velocity = spawnList[pos].velocity;
			moveScript.wallNormal = spawnList[pos].wallNormal;
			splineGrinding.bail();
		}
		playerAnimation.animation[spawnList[pos].animationName].speed = spawnList[pos].animationSpeed;
		moveScript.lastAnim = spawnList[pos].animationState;
		int num;
		for (num = pos + 1; num < spawnList.Count; num++)
		{
			spawnList.RemoveAt(num);
			num--;
		}
		if (pos == 0)
		{
			spawnList.RemoveAt(0);
		}
	}

	private void DropPoint(bool alwaysAdd = false)
	{
		Vector3 position = player.position;
		Quaternion rotation = player.rotation;
		Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
		string animationState = string.Empty;
		string empty = string.Empty;
		float animationTime = 0f;
		float animationSpeed = 0f;
		float airTime = moveScript.airTime;
		bool flag = false;
		AnimationState animationState2 = playerAnimation.animation[moveScript.animName];
		if ((bool)animationState2)
		{
			flag = true;
			animationState = moveScript.lastAnim;
			animationTime = animationState2.time;
			empty = animationState2.name;
			animationSpeed = animationState2.speed;
		}
		if (flag)
		{
			int num = 0;
			if (moveScript.isGrinding)
			{
				num = 1;
			}
			else if (moveScript.wallRiding)
			{
				num = 2;
			}
			int currentGracePeriod = splineGrinding.currentGracePeriod;
			float volume = player.GetComponent<AudioSource>().volume;
			SpawnPoint spawnPoint = new SpawnPoint(num, position, rotation, velocity, animationState, empty, animationTime, animationSpeed, cameraTrans.position, cameraTrans.GetComponent<NewCamera>().mockCamera.rotation, currentGracePeriod, volume, airTime);
			switch (num)
			{
			case 1:
				spawnPoint.currentVelocity = splineGrinding.currentVelocity;
				spawnPoint.passedTime = splineGrinding.passedTime;
				spawnPoint.offSet = splineGrinding.offSet;
				spawnPoint.spline = splineGrinding.spline;
				spawnPoint.forward = splineGrinding.forward;
				break;
			case 2:
				spawnPoint.wallNormal = moveScript.wallNormal;
				break;
			}
			if (spawnList.Count >= spawnNum && !alwaysAdd)
			{
				spawnList.Remove(spawnList[0]);
			}
			spawnList.Add(spawnPoint);
		}
		else
		{
			Debug.LogWarning("couldnt find animation: " + moveScript.animName);
		}
	}

	public void ClearSpawns()
	{
		spawnList = new List<SpawnPoint>();
		currentLoc = 0;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position, new Vector3(1f, 1f, 1f));
	}

	private void GetDebugInput()
	{
		totalInput += inputString;
		resetTime = maxResetTime;
	}

	private void JumpToDebug(string _loc)
	{
		int num = int.Parse(_loc);
		if (num < debug_checkpoints.Length)
		{
			DebugRespawn(num);
		}
	}

	private void DebugRespawn(int loc)
	{
		if (isRespawning)
		{
			StopRewind();
		}
		moveScript.freezeControls = false;
		moveScript.wallRiding = false;
		moveScript.isGrinding = false;
		player.GetComponent<Rigidbody>().isKinematic = false;
		splineGrinding.isGrinding = false;
		player.position = debug_checkpoints[loc].transform.position;
		player.rotation = debug_checkpoints[loc].transform.rotation;
		player.GetComponent<Rigidbody>().velocity = debug_checkpoints[loc].velocity;
	}
}
