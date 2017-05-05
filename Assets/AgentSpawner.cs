using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {

	public int numberOfAgents;
	public float radius;
	public GameObject agentPrefab;
	// Use this for initialization
	void Awake () {
		//		float deltaAngle = 2f * Mathf.PI / numberOfAgents;

		//		for(int i =0; i < numberOfAgents; i++){
		for(int i = numberOfAgents; i > 0; i--){
			float angle = Mathf.Lerp (0f, 2f * Mathf.PI, (float) i/numberOfAgents);
			//			print (angle);
			Vector3 pos = new Vector3 (radius * Mathf.Cos(angle), 1f, radius * Mathf.Sin(angle));
			GameObject temp = Instantiate (agentPrefab, pos, Quaternion.identity);
			temp.name = "Agent " + i;
			Vector3 targetPosition = new Vector3 (- radius * Mathf.Cos(angle), 1f, - radius * Mathf.Sin(angle));
			temp.GetComponent<AgentNavigation> ().TargetPosition = targetPosition;
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
