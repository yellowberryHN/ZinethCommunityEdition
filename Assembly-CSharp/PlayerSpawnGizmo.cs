using UnityEngine;
using System.Collections;

public class PlayerSpawnGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
	
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
        Gizmos.DrawSphere(transform.position + new Vector3(0, 1.5f, 0), 1.5f);
    }
}