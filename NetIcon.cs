using UnityEngine;

public class NetIcon : MonoBehaviour
{
	public AudioClip collect_sound;

	public float life = 5f;

	public Vector3 world_pos;

	public GUITexture guitex;

	public Texture2D tex;

	public Vector2 tex_size = Vector2.one * 64f;

	public Vector2 start_tex_size = Vector2.zero;

	public bool can_collect;

	public NetworkPlayer owner;

	private void Update()
	{
		if (start_tex_size == Vector2.zero)
		{
			start_tex_size = tex_size;
		}
		life -= Time.deltaTime;
		if (life <= 1.1f)
		{
			Kill();
		}
		else if (life <= 2f)
		{
			tex_size = Vector2.Lerp(tex_size, tex_size.normalized * 4f, Time.deltaTime * (life * 4f));
		}
		UpdatePos();
	}

	private void UpdatePos()
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(world_pos);
		guitex.enabled = true;
		if (vector.z < 0f)
		{
			vector.y = 0f;
		}
		vector.x -= tex_size.x / 2f;
		vector.y -= tex_size.y / 2f;
		vector.x = Mathf.Clamp(vector.x, 0f, (float)Screen.width - tex_size.x);
		vector.y = Mathf.Clamp(vector.y, 0f, (float)Screen.height - tex_size.y);
		guitex.pixelInset = new Rect(vector.x, vector.y, tex_size.x, tex_size.y);
	}

	public static NetIcon AddNetIcon(NetworkPlayer owner_player, Vector3 position, Texture2D tex, Vector2 size)
	{
		GameObject gameObject = new GameObject("neticon");
		gameObject.transform.position = position;
		GameObject gameObject2 = new GameObject("subobj");
		gameObject2.transform.position = Vector3.zero;
		gameObject2.transform.localScale = Vector3.zero;
		gameObject2.transform.parent = gameObject.transform;
		GUITexture gUITexture = gameObject2.AddComponent<GUITexture>();
		gUITexture.texture = tex;
		NetIcon netIcon = gameObject2.AddComponent<NetIcon>();
		netIcon.owner = owner_player;
		netIcon.world_pos = position;
		netIcon.guitex = gUITexture;
		netIcon.tex = tex;
		netIcon.tex_size = size;
		if (tex.name == "pizza")
		{
			netIcon.can_collect = true;
			netIcon.life = 20f;
		}
		if (netIcon.owner == Network.player)
		{
			netIcon.can_collect = false;
		}
		if (netIcon.can_collect)
		{
			CollisionChecker collisionChecker = gameObject.AddComponent<CollisionChecker>();
			collisionChecker.TriggerEnterDelegate = netIcon.OnTriggerEnter;
			SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = 8f;
			netIcon.collect_sound = Networking.piz_clip;
		}
		return netIcon;
	}

	public void Kill()
	{
		if ((bool)base.transform.parent)
		{
			Object.Destroy(base.transform.parent.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollect()
	{
		if (Application.isEditor)
		{
			MonoBehaviour.print("grabbing some piz");
		}
		if (collect_sound != null)
		{
			AudioSource.PlayClipAtPoint(collect_sound, Camera.main.transform.position);
		}
		if ((bool)Networking.my_net_player)
		{
			Networking.my_net_player.pizzaScore++;
			Networking.my_net_player.DoSetPizzaScore();
		}
		Kill();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (can_collect && other.name == "Player")
		{
			OnCollect();
		}
	}
}
