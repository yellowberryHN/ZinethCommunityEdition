using UnityEngine;

public class PhoneButtonBgColor : PhoneButton
{
	public Color[] colors;

	private int ind;

	private void Awake()
	{
		if (textmesh == null)
		{
			textmesh = base.gameObject.GetComponent<TextMesh>();
		}
		if (controller == null)
		{
			controller = Object.FindObjectOfType(typeof(PhoneController)) as PhoneController;
		}
		Init();
	}

	private void Start()
	{
		if (colors.Length == 0)
		{
			colors[0] = controller.backcolor;
		}
	}

	public override void OnPressed()
	{
		ind++;
		if (ind >= colors.Length)
		{
			ind = 0;
		}
	}
}
