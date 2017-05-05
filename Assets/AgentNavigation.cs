using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentNavigation : CachedBase
{
	private const int RAND_MAX = 0x7fff;

	//	public NavNode currentNode;
	//	private Vector3 targetPosition;
	public Vector3 targetPosition;

	public Vector3 TargetPosition
	{
		get { return targetPosition; }
		set { targetPosition = new Vector3(value.x, this.transform.position.y, value.z); }
	}

	// Movement part
	public float speed = 1.4f; // http://en.wikipedia.org/wiki/Preferred_walking_speed
	public float maxSpeed;

	public Vector3 preferredVelocity;
	public Vector3 velocity;

	public float radius = 0.0f;
	public bool disableRVO = false;

	public RVO.RVOAgent RVOAgent;

	// This put transform and rigidbody in cache
	public override void Awake ()
	{
		base.Awake (); //does the caching.
		maxSpeed = speed * 2.1f;

		// Set the radius
		CapsuleCollider capsuleCollider = GetComponent<Collider>() as CapsuleCollider;
		radius = capsuleCollider.radius;

		// Handling RVO
		RVOAgent = new RVO.RVOAgent ();
		RVOAgent.MaxSpeed = speed;
		RVOAgent.Radius = radius;
		RVOAgent.Position = new RVO.Vector2 (transform.position.x, transform.position.z);

		RVO.Simulator.Instance.addAgent (RVOAgent);
	}

	public void OnDrawGizmos ()
	{
		//		var position = transform.position;
		Vector3 position = transform.position;
		Gizmos.color = Color.green;
		Gizmos.DrawLine (position, position + preferredVelocity);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (position, position + velocity);
	}

	void Start ()
	{
		//		maxSpeed = speed * 2.1f;
		//
		//		// Set the radius
		//		CapsuleCollider capsuleCollider = GetComponent<Collider>() as CapsuleCollider;
		//		radius = capsuleCollider.radius;
		//
		//		// Handling RVO
		//		RVOAgent = new RVO.RVOAgent ();
		//		RVOAgent.MaxSpeed = speed;
		//		RVOAgent.Radius = radius;
		//		RVOAgent.Position = new RVO.Vector2 (transform.position.x, transform.position.z);
		//
		//		RVO.Simulator.Instance.addAgent (RVOAgent);
	}

	void OnDestroy ()
	{
		RVO.Simulator.Instance.removeAgent (RVOAgent);
	}

	public static UnityEngine.Vector3 toUnityVector (RVO.Vector2 vector)
	{
		return new UnityEngine.Vector3 (vector.x_, 0, vector.y_);
	}

	public static RVO.Vector2 toRVOVector (UnityEngine.Vector3 vector)
	{
		return new RVO.Vector2 (vector.x, vector.z);
	}

	void Update(){
		Move ();
	}

	// Update is called once per frame
	public void Move ()
	{
		print ("move method is running!");
		velocity = preferredVelocity;

		// RVO will prevent going towards a shop (wall)
		//if (currentNode != targetShopNode)
		if (!disableRVO)
			velocity = toUnityVector (RVOAgent.TargetVelocity);

		// Apply velocity on transform, this is where you multiply by dt (velocity is in m/s)
		transform.localPosition += velocity * Time.deltaTime;

		// Face same direction as velocity
		if (velocity.magnitude > 0.1f) {
			Quaternion targetRotation = Quaternion.LookRotation (velocity);
			var factor = Time.deltaTime < float.Epsilon ? 0 : Time.deltaTime / (Time.deltaTime + 0.1f); // 0.0f = snap, +infinity = don't move
			if (targetRotation != transform.rotation)
				transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRotation, factor);
		}

		// Done after position update, before RVO
		UpdatePreferredVelocity ();

		// Update agent parameters for next frame calculations
		// There may be a 1 frame lag depending on when Simulator is updated
		RVOAgent.PreferredVelocity = toRVOVector (preferredVelocity);
		RVOAgent.Position = toRVOVector (transform.localPosition);
	}



	void UpdatePreferredVelocity ()
	{
		print ("updat preferred velocity method is running!");
		/*if (targetNode && currentNode == targetShopNode) {
						targetPosition = new Vector3 (shopPositionList [0].x, transform.position.y, shopPositionList [0].z);
				}*/

		var currentPosition = transform.position;
		var currentRotation = transform.rotation;

		var direction = (targetPosition - currentPosition).normalized;
		Quaternion targetPosRotation = Quaternion.LookRotation (direction);

		preferredVelocity = targetPosRotation * Vector3.forward * speed;

		// Not sure if this is necessary
		float angle = Random.Range (0, RAND_MAX) * 2.0f * Mathf.PI / RAND_MAX;
		float dist = Random.Range (0, RAND_MAX) * 0.0001f / RAND_MAX;
		preferredVelocity += dist * new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle));
	}





}
