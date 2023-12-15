using System.Collections;
using UnityEngine;

public class UnlockZineTrigger : MonoBehaviour
{
	public bool use_mat_texture;

	public Texture2D tex;

	public string tex_url;

	public int ind;

	public bool destroy_on_activate = true;

	public bool auto_add_secret = true;

	public SecretObject secret;

	private void Awake()
	{
		if (secret == null)
		{
			secret = GetComponent<SecretObject>();
		}
		if (secret == null && auto_add_secret)
		{
			secret = base.gameObject.AddComponent<SecretObject>();
		}
		if (secret != null)
		{
			secret.send_activate_to.Add(base.gameObject);
		}
	}

	public void SecretFound()
	{
		AddZine();
	}

	public void AddZine()
	{
		if (use_mat_texture && base.renderer.material.mainTexture != null)
		{
			PhoneMemory.UnlockZine(base.renderer.material.mainTexture as Texture2D);
		}
		else if (tex != null)
		{
			PhoneMemory.UnlockZine(tex);
		}
		else if (!string.IsNullOrEmpty(tex_url))
		{
			GameObject gameObject = new GameObject();
			UnlockZineTrigger unlockZineTrigger = gameObject.AddComponent<UnlockZineTrigger>();
			PhoneMemory.UnlockZine(unlockZineTrigger.NewImage(tex_url));
		}
		else
		{
			PhoneMemory.UnlockZine(ind);
		}
	}

	public virtual void Activate()
	{
		if (Application.isEditor)
		{
			MonoBehaviour.print("activated trigger " + base.gameObject.name);
		}
		AddZine();
		if ((bool)secret)
		{
			secret.Found();
		}
		DoGUI();
		if (destroy_on_activate)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void DoGUI()
	{
		AudioSource.PlayClipAtPoint(MissionController.get_capsule_sound, Vector3.zero);
		MissionGUIText missionGUIText = MissionGUIText.Create("Found a new Zine!", new Vector3(0.024f, 0.2857143f, 0f), Vector3.one * 10f);
		missionGUIText.color = Color.blue;
		missionGUIText.velocity = Vector3.up * 0.15f;
		missionGUIText.stopAfter = 0.15f;
		missionGUIText.lifeTime = 2f;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Player")
		{
			Activate();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player")
		{
			Activate();
		}
	}

	public Texture2D NewImage(string url)
	{
		if (PhoneTweetButton.image_dict.ContainsKey(url))
		{
			return PhoneTweetButton.image_dict[url];
		}
		int num = 128;
		Texture2D texture2D = new Texture2D(num, num);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		PhoneTweetButton.image_dict.Add(url, texture2D);
		PhoneTweetButton.finished_dl_dict.Add(texture2D, false);
		StartCoroutine("DownloadImage", url);
		return texture2D;
	}

	private IEnumerator DownloadImage(string url)
	{
		WWW web = new WWW(url);
		yield return web;
		if (web.error != null)
		{
			Debug.LogWarning("Error downloading image " + url + "(" + web.error + ")");
		}
		else
		{
			PhoneTweetButton.image_dict[url].Resize(web.texture.width, web.texture.height);
			Debug.Log("Resizing...");
			web.LoadImageIntoTexture(PhoneTweetButton.image_dict[url]);
			PhoneTweetButton.finished_dl_dict[PhoneTweetButton.image_dict[url]] = true;
		}
		Object.Destroy(base.gameObject);
	}
}
