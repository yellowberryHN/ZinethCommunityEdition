using System.Collections.Generic;
using UnityEngine;

public class PhoneTweetButton : PhoneButton
{
	private PhoneElement render_element;

	public PhoneLabel sender_label;

	public PhoneLabel username_label;

	public PhoneLabel bodytext_label;

	public Renderer image_renderer;

	public bool new_effect;

	public PhoneLabel new_label;

	public bool auto_update_new = true;

	public Renderer images_icon;

	public Renderer links_icon;

	private PhoneMail _my_mail;

	public bool resize;

	public string url_str = string.Empty;

	public static Dictionary<string, Texture2D> image_dict = new Dictionary<string, Texture2D>();

	public static Dictionary<Texture, bool> finished_dl_dict = new Dictionary<Texture, bool>();

	public int image_size = 48;

	public Texture2D image_tex
	{
		get
		{
			if (!image_renderer)
			{
				return null;
			}
			return image_renderer.material.mainTexture as Texture2D;
		}
		set
		{
			if ((bool)image_renderer)
			{
				image_renderer.material.mainTexture = value;
			}
		}
	}

	public PhoneMail my_mail
	{
		get
		{
			if (_my_mail == null)
			{
				_my_mail = MailController.FindMail(id_info);
			}
			return _my_mail;
		}
		set
		{
			_my_mail = value;
			if (_my_mail != null)
			{
				id_info = _my_mail.id;
			}
			else
			{
				Debug.LogWarning("null mail...");
			}
		}
	}

	private void Awake()
	{
		Init();
		normal_scale = base.transform.localScale;
		wantedscale = normal_scale;
	}

	public override void Init()
	{
		back_normal_color = PhoneMemory.settings.mailColor;
		back_selected_color = Color.Lerp(back_normal_color, Color.white, 0.6f);

		// recolor mail
		if (name == "SingleTweet" && my_mail != null)
		{
			if ((bool)username_label)
			{
				var titleBack = base.gameObject.transform.FindChild("TitleBack");
				titleBack.gameObject.renderer.material.color = Color.Lerp(back_normal_color, back_normal_color * 2f, 0.2f);
			}
			if ((bool)bodytext_label)
			{
				var bodyBack = base.gameObject.transform.FindChild("BodyBack");
				bodyBack.gameObject.renderer.material.color = Color.Lerp(back_normal_color, back_normal_color * 2f, 0.2f);
			}
		}
		
		base.Init();
	}

	public override void OnLoad()
	{
		base.OnLoad();
		if (my_mail != null)
		{
			if (my_mail.color != Color.clear)
			{
				back_normal_color = my_mail.color;
				back_selected_color = Color.Lerp(back_normal_color, Color.white, 0.6f);
				SetBackColor(back_normal_color);
			}
			if (my_mail.image_url != string.Empty && (bool)image_tex)
			{
				GetImage(my_mail.image_url);
			}
			if ((bool)sender_label)
			{
				sender_label.text = my_mail.sender;
			}
			if ((bool)username_label)
			{
				username_label.text = my_mail.subject;
			}
			if ((bool)bodytext_label)
			{
				bodytext_label.text = my_mail.body;
			}
			if ((bool)new_label)
			{
				if (new_effect && my_mail.is_new)
				{
					new_label.renderer.enabled = true;
				}
				else
				{
					new_label.renderer.enabled = false;
				}
			}
			if ((bool)images_icon)
			{
				images_icon.enabled = my_mail.media_urls.Count > 0;
			}
			if ((bool)links_icon)
			{
				links_icon.enabled = my_mail.link_urls.Count > 0;
			}
		}
		if (resize && (bool)background_box)
		{
			background_box.renderer.bounds.Encapsulate(bodytext_label.textmesh.renderer.bounds);
		}
	}

	public override void OnUpdate()
	{
		if (new_effect && auto_update_new)
		{
			DoNewEffect();
		}
		base.OnUpdate();
	}

	public virtual void DoNewEffect()
	{
		if ((bool)new_label)
		{
			new_label.renderer.enabled = my_mail.is_new;
		}
	}

	public virtual void GetImage(string url)
	{
		if ((bool)image_tex)
		{
			image_tex = NewImage(url);
		}
	}

	public virtual Texture2D NewImage(string image_name)
	{
		if (image_dict.ContainsKey(image_name))
		{
			return image_dict[image_name];
		}
		Texture2D texture2D = new Texture2D(image_size, image_size);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		image_dict.Add(image_name, texture2D);
		finished_dl_dict.Add(texture2D, false);
		ImageDownloadHelper.DownLoadImage(image_name);
		return texture2D;
	}

	public override void OnSelected()
	{
		if ((bool)textmesh)
		{
			textscale = text_size + Mathf.Min(text_size * 0.2f, 0.1f);
		}
		if ((bool)background_box)
		{
			SetBackColor(back_selected_color);
		}
		SetBorderActive(always_use_background_border);
		if (expand_on_select)
		{
			wantedscale = normal_scale * expand_size;
		}
	}

	public override void OnUnSelected()
	{
		if ((bool)textmesh)
		{
			textscale = text_size;
		}
		if ((bool)background_box)
		{
			SetBackColor(back_normal_color);
		}
		SetBorderActive(always_use_background_border);
		if (expand_on_select)
		{
			wantedscale = normal_scale;
		}
	}
}
