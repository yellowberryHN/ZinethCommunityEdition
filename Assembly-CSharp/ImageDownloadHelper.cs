using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDownloadHelper : MonoBehaviour
{
	public string my_url;

	private static List<ImageDownloadHelper> free_list = new List<ImageDownloadHelper>();

	public bool busy;

	private static Dictionary<string, Texture2D> image_dict
	{
		get
		{
			return PhoneTweetButton.image_dict;
		}
	}

	private static Dictionary<Texture, bool> finished_dl_dict
	{
		get
		{
			return PhoneTweetButton.finished_dl_dict;
		}
	}

	public static ImageDownloadHelper DownLoadImage(string url)
	{
		ImageDownloadHelper imageDownloadHelper = ((free_list.Count <= 0) ? new GameObject("image_downloader").AddComponent<ImageDownloadHelper>() : free_list[0]);
		imageDownloadHelper.DownloadImage(url);
		return imageDownloadHelper;
	}

	public static Texture2D NewImage(string image_name)
	{
		if (image_dict.ContainsKey(image_name))
		{
			return image_dict[image_name];
		}
		Texture2D texture2D = new Texture2D(64, 64);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		image_dict.Add(image_name, texture2D);
		finished_dl_dict.Add(texture2D, false);
		DownLoadImage(image_name);
		return texture2D;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		free_list.Add(this);
	}

	public void DownloadImage(string url)
	{
		StartCoroutine(Co_DownloadImage(url));
	}

	public IEnumerator Co_DownloadImage(string url)
	{
		busy = true;
		my_url = url;
		if (free_list.Contains(this))
		{
			free_list.Remove(this);
		}
		WWW web = new WWW(url);
		yield return web;
		if (web.error != null)
		{
			Debug.LogWarning("Error downloading image " + url + "(" + web.error + ")");
			image_dict.Remove(url);
		}
		else
		{
			image_dict[url].Resize(web.texture.width, web.texture.height);
			web.LoadImageIntoTexture(image_dict[url]);
			finished_dl_dict[image_dict[url]] = true;
		}
		free_list.Add(this);
		busy = false;
	}

	public static void DeleteImage(string url)
	{
		if (image_dict.ContainsKey(url))
		{
			Texture2D texture2D = image_dict[url];
			if (finished_dl_dict.ContainsKey(texture2D))
			{
				finished_dl_dict.Remove(texture2D);
				image_dict.Remove(url);
				Object.Destroy(texture2D);
			}
		}
	}
}
