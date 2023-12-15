using UnityEngine;

public class SpawnPoint
{
	public int currentState;

	public Vector3 spawnPos;

	public Quaternion spawnRot;

	public Vector3 velocity;

	public string animationState;

	public string animationName;

	public float animationTime;

	public float animationSpeed;

	public Vector3 cameraPosition;

	public Quaternion cameraRotation;

	public int gracePeriod;

	public float volume;

	public float currentVelocity;

	public float passedTime;

	public float offSet;

	public Spline spline;

	public bool forward;

	public Vector3 wallNormal;

	public float airTime;

	public Vector3 hawkPos;

	public Quaternion hawkRot;

	public float timeFollowed;

	public float swoopDistance;

	public float startSwoopDistance;

	public bool inBounds;

	public bool targetEngaged;

	public bool hasSwoopedIn;

	public SpawnPoint(int _currentState, Vector3 _spawnPos, Quaternion _spawnRot, Vector3 _velocity, string _animationState, string _animationName, float _animationTime, float _animationSpeed, Vector3 _cameraPosition, Quaternion _cameraRotation, int _gracePeriod, float _volume, float _airTime)
	{
		currentState = _currentState;
		spawnPos = _spawnPos;
		spawnRot = _spawnRot;
		velocity = _velocity;
		animationState = _animationState;
		animationName = _animationName;
		animationTime = _animationTime;
		animationSpeed = _animationSpeed;
		cameraPosition = _cameraPosition;
		cameraRotation = _cameraRotation;
		gracePeriod = _gracePeriod;
		volume = _volume;
		currentVelocity = 0f;
		passedTime = 0f;
		offSet = 0f;
		spline = null;
		forward = false;
		wallNormal = new Vector3(0f, 0f, 0f);
		airTime = _airTime;
		Transform transform = PhoneInterface.hawk.transform;
		hawkPos = transform.position;
		hawkRot = transform.rotation;
		timeFollowed = transform.GetComponent<HawkBehavior>().timeFollowed;
		swoopDistance = transform.GetComponent<HawkBehavior>().swoopDistance;
		startSwoopDistance = transform.GetComponent<HawkBehavior>().startSwoopDistance;
		inBounds = transform.GetComponent<HawkBehavior>().inBounds;
		targetEngaged = transform.GetComponent<HawkBehavior>().targetEngaged;
		hasSwoopedIn = transform.GetComponent<HawkBehavior>().hasSwoopedIn;
	}
}
