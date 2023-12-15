using System.IO;
using UnityEngine;

public class UltraPrinter : MonoBehaviour
{
	private static UltraPrinter _instance;

	public static string file_dir = "Assets/Resources/printing/print_prison/";

	public static string orig_file_dir = "Assets/Resources/printing/";

	public static bool enable_print = true;

	public static UltraPrinter instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType(typeof(UltraPrinter)) as UltraPrinter;
			}
			return _instance;
		}
	}

	public static bool Print(int index)
	{
		return Print(PhoneResourceController.zine_images[index].name);
	}

	public static bool Print(string filename)
	{
		if (!enable_print)
		{
			return true;
		}
		if (!filename.EndsWith(".png"))
		{
			filename += ".png";
		}
		string sourceFileName = orig_file_dir + filename;
		string text = file_dir + filename;
		File.Delete(text);
		File.Copy(sourceFileName, text);
		return true;
	}
}
