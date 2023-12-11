using UnityEngine;

public class PhoneOpenPromp : MonoBehaviour
{
	private GUIText guitext;

	private TextMesh textmesh;

	public Color color = Color.black;

	public bool animate_color;

	private void Start()
	{
		guitext = GetComponent<GUIText>();
		if ((bool)guitext)
		{
			guitext.material.color = color;
		}
		textmesh = GetComponent<TextMesh>();
		if ((bool)textmesh)
		{
			textmesh.renderer.material.color = color;
		}
	}

	private void Update()
	{
		if (animate_color && (bool)textmesh)
		{
			textmesh.renderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 3f, 0.5f));
		}
		if (PhoneController.powerstate == PhoneController.PowerState.open)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
