using UnityEngine;

public class PhoneViewOpenButton : MonoBehaviour
{
	public Camera cam;

	public Color normalcolor = Color.gray;

	public Color hovercolor = Color.white;

	public PhoneViewController phoneviewcontroller;

	private bool mouseon;

	public Transform icon;

	private Vector3 iconscale;

	private bool guitext_set;

	public TextMesh guitext;

	public Color wantedColor = Color.gray;

	public bool can_use
	{
		get
		{
			if (PhoneController.powerstate == PhoneController.PowerState.open)
			{
				return PhoneController.instance.allow_close;
			}
			return PhoneController.instance.allow_open;
		}
	}

	private void Start()
	{
		if (phoneviewcontroller == null)
		{
			phoneviewcontroller = Object.FindObjectOfType(typeof(PhoneViewController)) as PhoneViewController;
		}
		base.renderer.material.color = normalcolor;
		wantedColor = normalcolor;
		if (icon == null && base.transform.childCount > 0)
		{
			icon = base.transform.GetChild(0);
		}
		if ((bool)icon)
		{
			iconscale = icon.localScale;
		}
	}

	private void Update()
	{
		if (Input.GetButtonDown("CellOpen"))
		{
			dOnMouseEnter();
		}
		if (Input.GetButtonUp("CellOpen"))
		{
			dOnMouseExit();
		}
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (base.collider.Raycast(ray, out hitInfo, 100f))
		{
			if (!mouseon)
			{
				dOnMouseEnter();
			}
			if (Input.GetButtonDown("CellClick"))
			{
				dOnMouseDown();
			}
			mouseon = true;
		}
		else
		{
			if (mouseon)
			{
				dOnMouseExit();
			}
			mouseon = false;
		}
		if ((bool)icon)
		{
			Vector3 vector = Vector3.up * 180f;
			if (phoneviewcontroller.open)
			{
				vector = Vector3.zero;
			}
			if (icon.transform.localEulerAngles != vector)
			{
				icon.transform.localEulerAngles = Vector3.Slerp(icon.transform.localEulerAngles, vector, 1f);
			}
		}
		if ((bool)guitext)
		{
			if (!mouseon)
			{
				base.renderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 3f, 0.5f));
			}
			else
			{
				base.renderer.material.color = Color.red;
			}
			guitext.renderer.material.color = base.renderer.material.color;
			if (phoneviewcontroller.open)
			{
				Object.Destroy(guitext.gameObject);
			}
		}
		else if (base.renderer.material.color != wantedColor)
		{
			base.renderer.material.color = wantedColor;
		}
	}

	private void dOnMouseDown()
	{
		if (can_use)
		{
			phoneviewcontroller.SetOpen(!phoneviewcontroller.open);
		}
	}

	private void dOnMouseEnter()
	{
		if (can_use)
		{
			wantedColor = hovercolor;
		}
		else
		{
			wantedColor = Color.red;
		}
	}

	private void dOnMouseExit()
	{
		wantedColor = normalcolor;
	}
}
