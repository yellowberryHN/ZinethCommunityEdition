using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
	public Camera m_Camera;

	public bool amActive;

	public bool autoInit = true;

	public GameObject myContainer;

	public Vector3 offset;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		if (autoInit && !amActive)
		{
			Init();
		}
	}

	private void Init()
	{
		if (autoInit)
		{
			m_Camera = Camera.mainCamera;
			amActive = true;
		}
		if (myContainer == null)
		{
			myContainer = new GameObject();
			myContainer.name = "GRP_" + base.transform.gameObject.name;
			myContainer.transform.position = base.transform.position;
			myContainer.transform.parent = base.transform.parent;
		}
		base.transform.parent = myContainer.transform;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.y = 0f;
		base.transform.localEulerAngles = localEulerAngles;
	}

	private void Update()
	{
		if (m_Camera == null)
		{
			m_Camera = Camera.mainCamera;
		}
		if (amActive)
		{
			myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
		}
	}

	private void OnDisable()
	{
		if (base.gameObject.active)
		{
			Object.Destroy(myContainer);
		}
	}
}
