using UnityEngine;

public class PhoneMenuLine : MonoBehaviour
{
	private enum ButtonDirection
	{
		up,
		down,
		left,
		right
	}

	public LineRenderer liner;

	public PhoneButton start;

	public PhoneButton end;

	private ButtonDirection dir;

	public Transform drawer;

	private float offset = 0.1f;

	private void Awake()
	{
		base.renderer.enabled = false;
		if (!drawer)
		{
			drawer = base.transform.GetChild(0).transform;
		}
		if (base.name.EndsWith("U"))
		{
			dir = ButtonDirection.up;
		}
		if (base.name.EndsWith("D"))
		{
			dir = ButtonDirection.down;
		}
		if (base.name.EndsWith("L"))
		{
			dir = ButtonDirection.left;
		}
		if (base.name.EndsWith("R"))
		{
			dir = ButtonDirection.right;
		}
	}

	private void Update()
	{
		if (PhoneInput.controltype != PhoneInput.ControlType.Keyboard && (!start || !start.force_mouse_menulines))
		{
			drawer.renderer.enabled = false;
		}
		else
		{
			DoPositions();
		}
	}

	private void DoPositions()
	{
		if ((bool)start && (bool)end)
		{
			drawer.renderer.enabled = true;
			Vector3 pos = GetPos();
			if (base.transform.position != pos)
			{
				base.transform.position = pos;
			}
		}
		else
		{
			drawer.renderer.enabled = false;
		}
	}

	private Vector3 GetPos()
	{
		Vector3 direction = GetDirection(dir);
		Bounds bounds = start.GetBounds();
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			zero[i] = direction[i] * bounds.size[i] / 2f;
		}
		zero += direction * offset;
		zero += bounds.center;
		return zero + base.transform.up * 1f;
	}

	private Vector3 GetDirection(ButtonDirection direction)
	{
		switch (direction)
		{
		case ButtonDirection.up:
			return base.transform.parent.forward;
		case ButtonDirection.down:
			return -base.transform.parent.forward;
		case ButtonDirection.left:
			return -base.transform.parent.right;
		case ButtonDirection.right:
			return base.transform.parent.right;
		default:
			return Vector3.zero;
		}
	}
}
