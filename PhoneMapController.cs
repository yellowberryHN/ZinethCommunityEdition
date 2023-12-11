using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMapController : PhoneMainMenu
{
	public PhoneElement player_marker;

	private Transform player_trans;

	public PhoneElement mission_marker;

	public List<PhoneLabel> location_markers = new List<PhoneLabel>();

	public Transform[] location_trans;

	private Vector3[] location_pos;

	public PhoneLabel scale_label;

	public Texture2D mission_sprite;

	public Texture2D capsule_sprite;

	public Texture2D battle_sprite;

	public Texture2D secret_sprite;

	public PhoneLabel capsule_label;

	public PhoneLabel npc_label;

	public PhoneLabel secrets_label;

	public PhoneElement capsule_icon;

	public PhoneElement npc_icon;

	public PhoneElement secrets_icon;

	public float low_scale = 0.0001f;

	public float hi_scale = 1f;

	public float scale_pow = 0.01f;

	public List<PhoneElement> objective_markers = new List<PhoneElement>();

	public float max_dist = 2048f;

	public float dist_scale = 0.25f;

	public Dictionary<Transform, PhoneElement> marker_dic = new Dictionary<Transform, PhoneElement>();

	private Vector3 mission_marker_scale = Vector3.zero;

	private float maxdist;

	public float scaling = 0.001f;

	public Vector3 offset = new Vector3(8800f, 0f, 200f);

	public float zoom = 0.5f;

	private Transform no_maths;

	public bool dogui;

	public Material mapmaterial;

	private int total_secrets
	{
		get
		{
			return SecretObject.all_list.Count;
		}
	}

	public List<SecretObject> secret_list
	{
		get
		{
			return SecretObject.uncollected_list;
		}
	}

	public List<Capsule> capsule_list
	{
		get
		{
			return Capsule.all_list;
		}
	}

	private int collected_capsules
	{
		get
		{
			return Capsule.collected_list.Count;
		}
	}

	public List<NPCTrainer> npc_list
	{
		get
		{
			return NPCTrainer.all_list;
		}
	}

	private int defeated_trainers
	{
		get
		{
			return NPCTrainer.defeated_list.Count;
		}
	}

	private void Start()
	{
		no_maths = new GameObject().transform;
		no_maths.transform.parent = base.transform;
		no_maths.name = "no_maths";
		if (hide_background)
		{
			HideBackground();
		}
		base.renderer.material.color = Color.black;
		if ((bool)stick_trans)
		{
			stick_trans.renderer.material.color = Color.black;
		}
		if (player_trans == null)
		{
			player_trans = PhoneInterface.player_trans;
		}
		if (player_trans != null)
		{
			offset = player_trans.position;
		}
		for (int i = 0; i < location_markers.Count; i++)
		{
			Object.Destroy(location_markers[i].gameObject);
		}
		location_markers.Clear();
		location_trans = new Transform[0];
		SetupScaling();
		SetupSecrets();
		if ((bool)capsule_icon && (bool)capsule_sprite)
		{
			capsule_icon.renderer.material.mainTexture = capsule_sprite;
		}
		if ((bool)npc_icon && (bool)battle_sprite)
		{
			npc_icon.renderer.material.mainTexture = battle_sprite;
			npc_icon.renderer.material.color = Color.red;
		}
		if ((bool)secrets_icon && (bool)secret_sprite)
		{
			secrets_icon.renderer.material.mainTexture = secret_sprite;
		}
	}

	public override void OnLoad()
	{
		base.OnLoad();
		UpdateButtonSelected();
		buttons[0].OnSelected();
		Playtomic.Log.CustomMetric("tMapOpened", "tPhone", true);
	}

	private void SetupLocations()
	{
	}

	private void SetupSecrets()
	{
	}

	private void SetupCapsules()
	{
	}

	private void SetupNPCs()
	{
	}

	private void SetupScaling()
	{
		scaling = Mathf.Lerp(low_scale, hi_scale, zoom);
		UpdateBack();
	}

	public override void UpdateScreen()
	{
		ZoomControls();
		UpdateMarkers();
		UpdateLabels();
		base.UpdateScreen();
	}

	private void UpdateLabels()
	{
		if ((bool)capsule_label)
		{
			string text = "Capsules: " + collected_capsules + "/" + capsule_list.Count;
			capsule_label.text = text;
			if ((bool)capsule_icon)
			{
				capsule_icon.renderer.material.color = new Color(Random.value, Random.value, Random.value, Random.Range(0f, 0.8f));
			}
		}
		if ((bool)npc_label)
		{
			string text2 = "Trainers: " + defeated_trainers + "/" + npc_list.Count;
			npc_label.text = text2;
			if ((bool)npc_icon)
			{
				npc_icon.renderer.material.color = new Color(1f, 0f, 0f, Random.Range(0.25f, 1f));
			}
		}
		if ((bool)secrets_label)
		{
			string text3 = "Secrets: " + (total_secrets - secret_list.Count) + "/" + total_secrets;
			secrets_label.text = text3;
			secrets_icon.renderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 0.8f));
		}
	}

	private void UpdateBack()
	{
		float num = 2400f;
		Vector2 vector = new Vector2(base.renderer.bounds.size.x, base.renderer.bounds.size.z) / num;
		base.renderer.material.mainTextureScale = vector / scaling;
		Vector2 mainTextureOffset = -base.renderer.material.mainTextureScale * 0.25f;
		base.renderer.material.mainTextureOffset = mainTextureOffset;
	}

	private void ZoomControls()
	{
		Vector2 controlDir = PhoneInput.GetControlDir();
		if (controlDir.magnitude < 0.2f)
		{
			controlDir.y = 0f;
		}
		controlDir.y += Input.GetAxis("Mouse ScrollWheel") * 25f;
		if (controlDir.y != 0f)
		{
			zoom = Mathf.Clamp(zoom + controlDir.y * base.deltatime * 0.5f, 0.01f, 1f);
			scaling = Mathf.Lerp(low_scale, hi_scale, zoom);
			float num = hi_scale - low_scale;
			scaling = low_scale + num * zoom * (zoom / scale_pow);
		}
		if ((bool)scale_label)
		{
			scale_label.text = (zoom * 100f).ToString("0") + "%";
		}
	}

	private void UpdateMissionMarkers()
	{
		if (!(MissionController.GetInstance() != null))
		{
			return;
		}
		List<MissionObjective> focus_objectives = MissionController.focus_objectives;
		if (focus_objectives == null)
		{
			MonoBehaviour.print("null objectives");
		}
		for (int i = 0; i < focus_objectives.Count; i++)
		{
			if (objective_markers.Count <= i)
			{
				PhoneElement phoneElement = Object.Instantiate(mission_marker) as PhoneElement;
				phoneElement.name = "mission_marker";
				phoneElement.renderer.material.color = Color.green;
				phoneElement.transform.parent = player_marker.transform.parent;
				if ((bool)mission_sprite)
				{
					phoneElement.renderer.material.mainTexture = mission_sprite;
				}
				objective_markers.Add(phoneElement);
			}
			if (i == 0)
			{
				objective_markers[i].renderer.material.color = Color.green;
			}
			else
			{
				objective_markers[i].renderer.material.color = Color.blue;
			}
			objective_markers[i].gameObject.active = true;
			MoveMarker(objective_markers[i], focus_objectives[i].objectivePosition, objective_markers[i].transform.forward);
			objective_markers[i].transform.position += Vector3.up * 0.75f;
		}
		for (int j = focus_objectives.Count; j < objective_markers.Count; j++)
		{
			objective_markers[j].gameObject.active = false;
		}
	}

	private void UpdateCapsuleMarkers()
	{
		for (int i = 0; i < capsule_list.Count; i++)
		{
			Capsule capsule = capsule_list[i];
			if (capsule == null)
			{
				capsule_list.Remove(capsule);
				i--;
				continue;
			}
			PhoneElement phoneElement;
			if (!marker_dic.ContainsKey(capsule.transform))
			{
				phoneElement = Object.Instantiate(mission_marker) as PhoneElement;
				phoneElement.name = "capsule_marker_" + capsule.capsule_index;
				phoneElement.renderer.material.color = Color.gray;
				phoneElement.transform.parent = player_marker.transform.parent;
				if ((bool)capsule_sprite)
				{
					phoneElement.renderer.material.mainTexture = capsule_sprite;
				}
				marker_dic.Add(capsule.transform, phoneElement);
				if (mission_marker_scale == Vector3.zero)
				{
					mission_marker_scale = phoneElement.transform.localScale;
				}
			}
			else
			{
				phoneElement = marker_dic[capsule.transform];
			}
			float num = Vector3.Distance(no_maths.position, capsule.transform.position);
			if (capsule.canCollect)
			{
				phoneElement.gameObject.active = true;
				MoveMarker(phoneElement, capsule.transform);
				if (num <= 0f)
				{
					num = 1E-05f;
				}
				float num2 = Mathf.Clamp(max_dist / num * dist_scale, 0f, 1f);
				if (num2 < 0.1f)
				{
					num2 = 0f;
				}
				Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, num2));
				phoneElement.renderer.material.color = color;
			}
			else
			{
				phoneElement.gameObject.active = false;
			}
		}
	}

	private void UpdateNPCMarkers()
	{
		for (int i = 0; i < npc_list.Count; i++)
		{
			NPCTrainer nPCTrainer = npc_list[i];
			if (nPCTrainer == null)
			{
				npc_list.Remove(nPCTrainer);
				i--;
				continue;
			}
			PhoneElement phoneElement;
			if (!marker_dic.ContainsKey(nPCTrainer.transform))
			{
				phoneElement = Object.Instantiate(mission_marker) as PhoneElement;
				phoneElement.name = "npc_marker_" + nPCTrainer.npc_name;
				phoneElement.renderer.material.color = Color.gray;
				phoneElement.transform.parent = player_marker.transform.parent;
				if ((bool)battle_sprite)
				{
					phoneElement.renderer.material.mainTexture = battle_sprite;
				}
				marker_dic.Add(nPCTrainer.transform, phoneElement);
				if (mission_marker_scale == Vector3.zero)
				{
					mission_marker_scale = phoneElement.transform.localScale;
				}
			}
			else
			{
				phoneElement = marker_dic[nPCTrainer.transform];
			}
			float num = Vector3.Distance(no_maths.position, nPCTrainer.transform.position);
			if (nPCTrainer.can_challenge)
			{
				phoneElement.gameObject.active = true;
				MoveMarker(phoneElement, nPCTrainer.transform);
				if (num <= 0f)
				{
					num = 1E-05f;
				}
				float num2 = Mathf.Clamp(max_dist / num * dist_scale, 0f, 1f);
				if (num2 < 0.1f)
				{
					num2 = 0f;
				}
				Color color = new Color(1f, 0f, 0f, Random.Range(num2 * num2, 1f));
				phoneElement.renderer.material.color = color;
			}
			else
			{
				phoneElement.gameObject.active = false;
			}
		}
	}

	private void UpdateSecretMarkers()
	{
		for (int i = 0; i < secret_list.Count; i++)
		{
			SecretObject secretObject = secret_list[i];
			if (secretObject == null)
			{
				secret_list.Remove(secretObject);
				i--;
				continue;
			}
			Transform transform = secretObject.transform;
			PhoneElement phoneElement;
			if (!marker_dic.ContainsKey(transform))
			{
				phoneElement = Object.Instantiate(mission_marker) as PhoneElement;
				phoneElement.renderer.material.color = Color.gray;
				phoneElement.transform.parent = player_marker.transform.parent;
				if ((bool)secret_sprite)
				{
					phoneElement.renderer.material.mainTexture = secret_sprite;
				}
				marker_dic.Add(transform, phoneElement);
			}
			else
			{
				phoneElement = marker_dic[transform];
			}
			float num = Vector3.Distance(no_maths.position, transform.position);
			phoneElement.gameObject.active = true;
			MoveMarker(phoneElement, transform);
			if (num <= 0f)
			{
				num = 1E-05f;
			}
			float f = Mathf.Clamp(max_dist / num * dist_scale, 0f, 1f);
			f = Mathf.Sqrt(f);
			if (f < 0.1f)
			{
				f = 0f;
			}
			Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, f));
			phoneElement.renderer.material.color = color;
		}
	}

	private void UpdateMarkers()
	{
		Vector3 eulerAngles;
		if ((bool)player_trans)
		{
			no_maths.position = player_trans.position;
			eulerAngles = player_trans.eulerAngles;
		}
		else
		{
			no_maths.position = Camera.main.transform.position;
			eulerAngles = Camera.main.transform.eulerAngles;
		}
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		no_maths.eulerAngles = eulerAngles;
		UpdateMissionMarkers();
		UpdateCapsuleMarkers();
		UpdateNPCMarkers();
		UpdateSecretMarkers();
		for (int i = 0; i < location_markers.Count; i++)
		{
			MoveMarker(location_markers[i], location_trans[i]);
		}
	}

	private void MoveMarker(PhoneElement marker, Transform trans)
	{
		MoveMarker(marker, trans.position, trans.forward);
	}

	private void MoveMarker(PhoneElement marker, Vector3 position, Vector3 forward)
	{
		if ((bool)player_trans)
		{
			offset = player_trans.transform.position;
		}
		else
		{
			offset = Camera.main.transform.position;
		}
		Vector3 vector = WorldToLocal(position);
		Vector2 vector2 = new Vector2(vector.x - base.transform.position.x, vector.z - base.transform.position.z);
		if (maxdist <= 0f)
		{
			Transform transform = base.transform.FindChild("RadarBack");
			maxdist = transform.renderer.bounds.size.x * 0.49f;
		}
		vector2 = Vector2.ClampMagnitude(vector2, maxdist);
		vector.x = base.transform.position.x + vector2.x;
		vector.z = base.transform.position.z + vector2.y;
		if (marker.transform.position != vector)
		{
			marker.transform.position = vector;
		}
		if (marker.GetType() != typeof(PhoneLabel))
		{
		}
	}

	private Vector3 WorldToLocal(Vector3 worldpos)
	{
		worldpos = no_maths.InverseTransformPoint(worldpos);
		Vector3 vector = worldpos * scaling;
		vector.y = 1f;
		return vector + base.transform.position;
	}

	private IEnumerator TakePic()
	{
		Camera mapcam = base.gameObject.AddComponent<Camera>();
		Terrain[] terrains = Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
		Bounds bounds = new Bounds(terrains[0].transform.position, Vector3.zero);
		Terrain[] array = terrains;
		foreach (Terrain ter in array)
		{
			bounds.Encapsulate(ter.collider.bounds);
		}
		int scaling = 2;
		int mapwidth = 480 * scaling;
		int mapheight = 800 * scaling;
		Texture2D tex = new Texture2D(mapwidth, mapheight, TextureFormat.RGB24, false);
		Vector3 pos = bounds.center;
		pos.y = 64000f;
		mapcam.transform.position = pos;
		base.camera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		mapcam.far = 68000f;
		if (true)
		{
			mapcam.isOrthoGraphic = true;
			mapcam.orthographicSize = bounds.size.z * (2f / 3f);
		}
		else
		{
			mapcam.transform.position += Vector3.up * 4800f;
		}
		RenderTexture rt = new RenderTexture(mapwidth, mapheight, 24);
		base.camera.targetTexture = rt;
		yield return new WaitForEndOfFrame();
		base.camera.Render();
		RenderTexture.active = rt;
		tex.ReadPixels(new Rect(0f, 0f, mapwidth, mapheight), 0, 0);
		base.camera.targetTexture = null;
		RenderTexture.active = null;
		Object.Destroy(rt);
		Object.Destroy(mapcam);
		byte[] bytes = tex.EncodeToPNG();
		string filename = "test_map.png";
		Debug.Log(string.Format("Took screenshot to: {0}", filename));
		Object.Destroy(tex);
	}
}
