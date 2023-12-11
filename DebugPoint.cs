using UnityEngine;

public class DebugPoint : MonoBehaviour
{
	public Vector3 velocity;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position, new Vector3(1f, 1f, 1f));
	}
}
