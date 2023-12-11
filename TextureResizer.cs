using UnityEngine;

public class TextureResizer : MonoBehaviour
{
	public Renderer rend;

	private Texture tex;

	private Transform trans;

	public float scale = 1f;

	public Vector2 max_size = Vector2.one;

	public bool done;

	public PhoneButton parbutton;

	private void Start()
	{
		if (!rend)
		{
			rend = base.renderer;
		}
		trans = rend.transform;
		if (!parbutton)
		{
			parbutton = trans.parent.GetComponent<PhoneButton>();
		}
	}

	private void Update()
	{
		if (rend.enabled)
		{
			if (tex == null || rend.material.mainTexture != tex)
			{
				done = false;
			}
			if (!done)
			{
				Check();
			}
		}
		else
		{
			done = false;
		}
	}

	public void ResizeSoon()
	{
	}

	public void Check()
	{
		tex = rend.material.mainTexture;
		if (rend.material.mainTexture != null && (!parbutton || parbutton.wantedscale == parbutton.transform.localScale) && PhoneTweetButton.finished_dl_dict.ContainsKey(tex) && PhoneTweetButton.finished_dl_dict[tex])
		{
			Resize();
		}
	}

	public void Resize()
	{
		if (rend.material.mainTexture != null)
		{
			float x = tex.width;
			float y = tex.height;
			Vector2 normalized = new Vector2(x, y).normalized;
			normalized *= scale;
			Transform parent = trans.parent;
			trans.parent = null;
			trans.localScale = new Vector3(normalized.x, 1f, normalized.y);
			trans.parent = parent;
			done = true;
		}
	}
}
