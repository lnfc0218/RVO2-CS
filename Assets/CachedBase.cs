using UnityEngine;
using System.Collections;

public class CachedBase : MonoBehaviour
{

	[HideInInspector]
	public new Rigidbody
	rigidbody;
	[HideInInspector]
	public new Transform
	transform;
	[HideInInspector]
	public new Camera
	camera;

	public virtual void Awake ()
	{
		transform = gameObject.transform;
		if (gameObject.GetComponent<Rigidbody>())
			rigidbody = gameObject.GetComponent<Rigidbody>();
		if (gameObject.GetComponent<Camera>())
			camera = gameObject.GetComponent<Camera>();
		//Debug.Log ("Caching complete.");
	}
}
