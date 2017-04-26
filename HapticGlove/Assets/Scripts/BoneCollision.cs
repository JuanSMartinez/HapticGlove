using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine (transform.position, transform.position + GetComponent<Rigidbody>().velocity, Color.blue, 0.5f,false);
	}

	void OnCollisionEnter(Collision collision){
		Debug.DrawLine (collision.contacts [0].point, collision.contacts [0].point + collision.contacts [0].normal, Color.red, 2f,false);
		Debug.Log ("Collision angle: " + Vector3.Angle(GetComponent<Rigidbody>().velocity, -collision.contacts[0].normal));
	}
}
