using UnityEngine;

public class NewCamera : MonoBehaviour
{
	public Transform target;

	public Transform tempTarget;

	public float distanceBehind = 1.73f;

	public float distance = 13f;

	public float height = 2.75f;

	public float damping = 8f;

	public float rotationDamping = 8f;

	public float hitOffset = 1f;

	public float adjustOffset = 2f;

	public float ignoreDistance = 0.2f;

	public float maxRotationSpeed = 0.01f;

	private Vector3 stuckPoint;

	private Vector3 stuckNormal;

	private bool hasHit;

	private Vector3 cameraPos;

	private Vector3 lastWantedPosition = new Vector3(0f, 0f, 0f);

	private bool snapped;

	private int maxUpdates = 2;

	private int numUpdates = 2;

	private Vector3 snapPos;

	private Quaternion snapRot;

	private float hRotOff = 30f;

	private float vRotOff = 30f;

	private Vector3 baseMousePos;

	private Transform tempRotation;

	private Transform tempBaseRotation;

	public Transform mockCamera;

	public float maxDist = 50f;

	public bool pauseCamera;

	public bool hawkMode;

	private move moveScript;

	private SplineGrinding grindScript;

	private GameObject mainCamera;

	private Vector3 camClickPos = -Vector3.one;

	public static bool use_mouse_look = true;

	public Transform curTarget
	{
		get
		{
			if (tempTarget != null)
			{
				return tempTarget;
			}
			return target;
		}
	}

	private void Awake()
	{
		grindScript = GameObject.Find("GrindPoint").GetComponent<SplineGrinding>();
		mainCamera = GameObject.Find("Main Camera");
		moveScript = GameObject.Find("Player").GetComponent<move>();
		float[] array = new float[32];
		array[11] = 500f;
		mainCamera.camera.layerCullDistances = array;
		tempRotation = Object.Instantiate(target) as Transform;
		tempBaseRotation = Object.Instantiate(target) as Transform;
		mockCamera = Object.Instantiate(target) as Transform;
		SnapCamera();
		mockCamera.position = base.transform.position;
		mockCamera.rotation = base.transform.rotation;
	}

	private void LateUpdate()
	{
		if (snapped && !pauseCamera)
		{
			if (numUpdates > 0)
			{
				numUpdates--;
				return;
			}
			numUpdates = maxUpdates;
			snapped = false;
			hasHit = false;
			base.transform.position = curTarget.TransformPoint(0f, height, 0f - distance);
			base.transform.rotation = Quaternion.LookRotation(curTarget.position - base.transform.position, curTarget.up);
			mockCamera.position = base.transform.position;
			mockCamera.rotation = base.transform.rotation;
		}
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		if (!tempTarget)
		{
			if (!snapped && !pauseCamera && !hawkMode)
			{
				NormalMode(fixedDeltaTime);
				CheckMaxDistance();
			}
			else if (hawkMode)
			{
				HawkMode(fixedDeltaTime);
			}
		}
	}

	public void CheckMaxDistance()
	{
		if (Vector3.Distance(base.transform.position, curTarget.position) > maxDist)
		{
			base.transform.position = curTarget.TransformPoint(0f, height, 0f - distance);
			if ((bool)mockCamera)
			{
				mockCamera.position = base.transform.position;
			}
		}
	}

	private void HawkMode(float deltaTime)
	{
		NormalMode(deltaTime);
	}

	public void NormalMode(float deltaTime)
	{
		Quaternion quaternion = new Quaternion(0f, 0f, 0f, 1f);
		Vector3 to = new Vector3(0f, 0f, 0f);
		if (moveScript.isGrinding)
		{
			if ((bool)grindScript.spline.GetComponent<RailCamera>())
			{
				quaternion = Quaternion.Euler(0f, grindScript.spline.GetComponent<RailCamera>().rotationOffset, 0f);
				to = new Vector3(grindScript.spline.GetComponent<RailCamera>().positionOffset, 0f, 0f);
				if (!grindScript.forward)
				{
					quaternion = Quaternion.Inverse(quaternion);
					to *= -1f;
				}
				mainCamera.transform.localPosition = Vector3.Slerp(mainCamera.transform.localPosition, to, deltaTime * rotationDamping);
			}
		}
		else
		{
			mainCamera.transform.localPosition = Vector3.Slerp(mainCamera.transform.localPosition, to, deltaTime * rotationDamping);
		}
		Quaternion quaternion2 = Quaternion.Euler(Input.GetAxis("RVertical") * (0f - vRotOff), Input.GetAxis("RHorizontal") * hRotOff, 0f);
		if (PhoneController.powerstate == PhoneController.PowerState.open)
		{
			quaternion2 = new Quaternion(0f, 0f, 0f, 1f);
			camClickPos = -Vector3.one;
		}
		else if (use_mouse_look)
		{
			if (Input.GetButton("CellClick"))
			{
				if (camClickPos == -Vector3.one)
				{
					camClickPos = Input.mousePosition;
				}
				else
				{
					Vector3 vector = (Input.mousePosition - camClickPos) / 60f;
					vector = Vector3.ClampMagnitude(vector, 1f);
					quaternion2 = Quaternion.Euler(vector.y * (0f - vRotOff), vector.x * hRotOff, 0f);
				}
			}
			else
			{
				camClickPos = -Vector3.one;
			}
		}
		tempRotation.position = curTarget.position;
		tempRotation.rotation = curTarget.rotation;
		tempBaseRotation.position = curTarget.position;
		tempBaseRotation.rotation = curTarget.rotation * quaternion;
		Quaternion quaternion3 = Quaternion.LookRotation(tempRotation.position - base.transform.position, tempRotation.up);
		Quaternion to2 = Quaternion.LookRotation(tempBaseRotation.position - base.transform.position, tempBaseRotation.up);
		mockCamera.rotation = Quaternion.Slerp(mockCamera.rotation, to2, deltaTime * rotationDamping);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion3 * quaternion2 * quaternion, deltaTime * rotationDamping);
		Vector3 vector2 = curTarget.TransformPoint(0f, height, 0f - distance);
		Vector3 vector3 = curTarget.TransformPoint(0f, height, 0f - distance * 3f);
		Vector3 newCheck;
		Vector3 newPos;
		if (!hasHit)
		{
			newPos = Vector3.Lerp(base.transform.position, vector2, deltaTime * damping);
			newCheck = Vector3.Lerp(base.transform.position, vector3, deltaTime * damping);
		}
		else
		{
			newPos = vector2;
			newCheck = vector3;
		}
		newPos = collisionCheck(newPos, newCheck, deltaTime);
		if (newPos == vector2)
		{
			newPos = Vector3.Lerp(base.transform.position, vector2, deltaTime * damping);
		}
		base.transform.position = newPos;
		mockCamera.position = newPos;
		lastWantedPosition = vector2;
	}

	private Vector3 collisionCheck(Vector3 newPos, Vector3 newCheck, float deltaTime)
	{
		Vector3 vector = newPos;
		RaycastHit hitInfo;
		if (!hasHit)
		{
			if (Physics.Linecast(base.transform.position, newCheck, out hitInfo))
			{
				hasHit = true;
				stuckPoint = hitInfo.point;
				stuckNormal = hitInfo.normal;
				vector = hitInfo.point + hitInfo.normal * adjustOffset;
				vector = Vector3.Lerp(base.transform.position, vector, deltaTime * damping);
			}
		}
		else if (Physics.Linecast(base.transform.position, newPos, out hitInfo))
		{
			if (Vector3.Distance(newPos, lastWantedPosition) >= ignoreDistance)
			{
				stuckPoint = hitInfo.point;
				stuckNormal = hitInfo.normal;
				vector = stuckPoint + stuckNormal * adjustOffset;
			}
			else
			{
				vector = stuckPoint + stuckNormal * adjustOffset;
			}
			vector = Vector3.Lerp(base.transform.position, vector, deltaTime * damping);
		}
		else
		{
			hasHit = false;
		}
		if (Physics.Linecast(curTarget.position, vector, out hitInfo))
		{
			stuckPoint = hitInfo.point;
			stuckNormal = hitInfo.normal;
			vector = Vector3.Lerp(base.transform.position, curTarget.position, deltaTime * damping);
		}
		if (Physics.Linecast(vector + Vector3.up * 3f, vector - Vector3.up * 0.5f, out hitInfo) && hitInfo.collider.tag == "Terrain")
		{
			stuckPoint = hitInfo.point;
			stuckNormal = hitInfo.normal;
			vector = hitInfo.point + hitInfo.normal * (3.5f - hitInfo.distance);
		}
		return vector;
	}

	public void SnapCamera()
	{
		snapped = true;
	}
}
