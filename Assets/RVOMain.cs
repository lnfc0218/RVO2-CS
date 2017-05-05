using UnityEngine;
using System.Collections;
using RVO;
using System.Collections.Generic;

public class RVOMain : MonoBehaviour {

	//    public List<Vector3> pList1 = new List<Vector3>();
	//	public List<Vector3> pList2 = new List<Vector3>();
	// show on inspector
	//	public GameObject[] shopList;



	void Start () {
	}

	void Update () {
		Simulator.Instance.setTimeStep(Time.deltaTime);
		Simulator.Instance.doStep();
		print ("Simulator Updating!");
	}
}
