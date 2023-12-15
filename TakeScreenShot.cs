using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class TakeScreenShot : MonoBehaviour
{
	private int count;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F9))
		{
			StartCoroutine(ScreenshotEncode());
		}
	}

	private IEnumerator ScreenshotEncode()
	{
		yield return new WaitForEndOfFrame();
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
		texture.Apply();
		string filedir = string.Empty;
		if (Environment.UserName == "knipfj")
		{
			filedir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Zineth/";
		}
		string timestr = DateTime.Now.ToString("M-d-yyyy_H-mm-ss");
		string filename = filedir + timestr.Replace(' ', '_').Replace(':', '-').Replace('/', '-') + ".png";
		string tfilename = filename;
		while (File.Exists(tfilename))
		{
			count++;
			tfilename = filename.Replace(".png", "_" + count + ".png");
		}
		yield return 0;
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(filename, bytes);
		count++;
		MonoBehaviour.print(filename);
		UnityEngine.Object.DestroyObject(texture);
	}
}
