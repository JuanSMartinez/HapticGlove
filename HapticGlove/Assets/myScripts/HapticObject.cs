using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionStay(Collision collision){
		Debug.Log ("Collided with" + collision.collider.name);
	}
}
