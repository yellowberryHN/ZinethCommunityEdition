using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	public float dragStartVelocity;

	public float dragMaxVelocity;

	public float maxVelocity;

	public float maxDrag;

	private float originalDrag;

	private Rigidbody rb;

	private float sqrDragStartVelocity;

	private float sqrDragVelocityRange;

	private float sqrMaxVelocity;

	public Player()
	{
		maxDrag = 1f;
	}

	public virtual void Awake()
	{
		originalDrag = rigidbody.drag;
		rb = rigidbody;
		Initialize(dragStartVelocity, dragMaxVelocity, maxVelocity, maxDrag);
	}

	public virtual void Initialize(float dragStartVelocity, float dragMaxVelocity, float maxVelocity, float maxDrag)
	{
		this.dragStartVelocity = dragStartVelocity;
		this.dragMaxVelocity = dragMaxVelocity;
		this.maxVelocity = maxVelocity;
		this.maxDrag = maxDrag;
		sqrDragStartVelocity = dragStartVelocity * dragStartVelocity;
		sqrDragVelocityRange = dragMaxVelocity * dragMaxVelocity - sqrDragStartVelocity;
		sqrMaxVelocity = maxVelocity * maxVelocity;
	}

	public virtual void FixedUpdate()
	{
		rb.AddForce(new Vector3(0f, 0f, 10f));
		Vector3 velocity = rb.velocity;
		float y = velocity.y;
		float sqrMagnitude = velocity.sqrMagnitude;
		if (!(sqrMagnitude <= sqrDragStartVelocity))
		{
			rigidbody.drag = Mathf.Lerp(originalDrag, maxDrag, Mathf.Clamp01((sqrMagnitude - sqrDragStartVelocity) / sqrDragVelocityRange));
			if (!(sqrMagnitude <= sqrMaxVelocity))
			{
				rb.velocity = velocity.normalized * maxVelocity;
			}
		}
		else
		{
			rb.drag = originalDrag;
		}
		rb.AddForce(new Vector3(0f, -9.8f, 0f));
	}
	
	public void Start()
	{
		Debug.Log("Player");
	}

	public virtual void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			rigidbody.AddForce(transform.up * 10f, ForceMode.Impulse);
		}
		if (Input.GetButtonDown("Right Boost"))
		{
			rigidbody.AddForce(transform.right * 25f, ForceMode.Impulse);
		}
		if (Input.GetButtonDown("Left Boost"))
		{
			rigidbody.AddForce(-transform.right * 25f, ForceMode.Impulse);
		}
	}

	public virtual void Main()
	{
	}
}
