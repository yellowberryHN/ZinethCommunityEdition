using UnityEngine;

public class PhoneViewHomeButton : MonoBehaviour
{
	public Camera cam;

	public Color normalcolor = Color.gray;

	public Color hovercolor = Color.white;

	public PhoneViewController phoneviewcontroller;

	private bool mouseon;

	private Transform icon;

	private Vector3 iconscale;

	public bool can_use
	{
		get
		{
			return PhoneController.instance.allow_home;
		}
	}

	private void Start()
	{
		if (phoneviewcontroller == null)
		{
			phoneviewcontroller = Object.FindObjectOfType(typeof(PhoneViewController)) as PhoneViewController;
		}
		base.renderer.material.color = normalcolor;
		if (icon == null && base.transform.GetChildCount() > 0)
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
		if (Input.GetButtonDown("CellHome"))
		{
			if ((bool)icon)
			{
				icon.localScale = iconscale * 0.25f;
			}
			dOnMouseEnter();
		}
		if (Input.GetButtonUp("CellHome"))
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
		if ((bool)icon && icon.localScale != iconscale)
		{
			icon.localScale = Vector3.Slerp(icon.localScale, iconscale, Time.deltaTime * 10f);
		}
	}

	private void dOnMouseDown()
	{
		if (can_use)
		{
			if ((bool)icon)
			{
				icon.localScale = iconscale * 0.25f;
			}
			phoneviewcontroller.OnHomeButton();
		}
	}

	private void dOnMouseEnter()
	{
		if (can_use)
		{
			base.renderer.material.color = hovercolor;
		}
		else
		{
			base.renderer.material.color = Color.red;
		}
	}

	private void dOnMouseExit()
	{
		base.renderer.material.color = normalcolor;
	}
}
