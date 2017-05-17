using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	//Object shapes as constants
	public const int SPHERE = 0;
	public const int CYLINDER = 1;
	public const int CUBE = 2;

	//Block object type
	public int objectType = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetObjectType(){
		return objectType;
	}
}
