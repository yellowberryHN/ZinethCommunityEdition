using UnityEngine;

public class CactusPlacer : MonoBehaviour
{
	public int num = 1000;

	public int minWidth;

	public int maxWidth = 10000;

	public int minLength;

	public int maxLength = 10000;

	public Transform prefab;

	private int height = 10000;

	private static int staticNum = 40;

	public int currentNum;

	private Transform playerRef;

	public static CactusPlacer instance;

	public Transform mirageCatPrefab;

	public float mirageChance = 0.01f;

	private void Awake()
	{
		instance = this;
		playerRef = PhoneInterface.player_trans;
		CactusBehavior.cactusBreaks = 0;
		CactusBehavior.recentCactusBreaks = 0;
	}

	private void FixedUpdate()
	{
		SpawnCactus();
	}

	private void SpawnCactus()
	{
		int min = -2000;
		int max = 2000;
		int min2 = -800;
		int max2 = 800;
		int min3 = 1200;
		int max3 = 2500;
		int min4 = 150;
		int max4 = 1000;
		if (currentNum >= staticNum)
		{
			return;
		}
		Vector3 vector = ((Random.Range(0, 10) <= 3) ? (playerRef.position + -playerRef.forward * Random.Range(min4, max4) + playerRef.right * Random.Range(min2, max2)) : (playerRef.position + playerRef.forward * Random.Range(min3, max3) + playerRef.right * Random.Range(min, max)));
		RaycastHit hitInfo;
		if (Physics.Linecast(vector + Vector3.up * height * 2f, vector + Vector3.down * height, out hitInfo) && hitInfo.collider.name == "Terrain")
		{
			Transform original = prefab;
			if ((bool)mirageCatPrefab && Random.Range(0f, 100f) <= mirageChance * 100f)
			{
				original = mirageCatPrefab;
			}
			Transform transform = Object.Instantiate(original, new Vector3(hitInfo.point.x, hitInfo.point.y - 1f, hitInfo.point.z), Quaternion.identity) as Transform;
			transform.parent = base.transform;
			currentNum++;
		}
	}
}
