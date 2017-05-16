using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestMotorBeahavior : MonoBehaviour {

	//Motor is active
	private bool motorActive = false;

	//Material when motor is active
	public Material active;

	//Material when motor is inactive
	public Material inactive;

	// Use this for initialization
	void Start () {
		SetColor ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown(){
		motorActive = !motorActive;
		SetColor ();
	}

	void SetColor(){
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild (i).GetComponent<MeshRenderer> ().material = motorActive ? active : inactive;
	}

	public bool MotorActive(){
		return motorActive;
	}
}
