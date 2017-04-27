using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using Leap.Unity.Interaction;
using Leap.Unity;
using Leap;

public class BoneCollision : MonoBehaviour {
	


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionStay(Collision collision){
		Debug.Log ("Collision: " + collision.transform.name);
		//Debug.Log("Collision energy: " + PhysicsCalculator.GetKineticEnergyOfCollision(collision.rigidbody, GetComponent<Rigidbody>()) + " Friction: " + PhysicsCalculator.GetFrictionForceOfCollision(-collision.contacts[0].normal, GetComponent<Rigidbody>(), GetComponent<Collider>().material));
	}
}
