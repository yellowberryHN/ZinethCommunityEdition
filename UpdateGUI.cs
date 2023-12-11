using UnityEngine;

public class UpdateGUI : MonoBehaviour
{
	private void OnLevelWasLoaded(int level)
	{
		if (Application.loadedLevelName != "Loader 3")
		{
			base.enabled = false;
		}
	}

	private void OnGUI()
	{
		if (DLCControl.instance != null)
		{
			DLCControl.instance.DoGUI();
		}
	}
}
