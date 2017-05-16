using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using Leap;


public class PalmManager : MonoBehaviour {

	LeapProvider provider;
	GameObject leftPalmSphere;
	GameObject rightPalmSphere;

	// Use this for initialization
	void Start () {
		provider = FindObjectOfType<LeapProvider>();
		leftPalmSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		rightPalmSphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		leftPalmSphere.name = "LeftPalm";
		rightPalmSphere.name = "RightPalm";
		leftPalmSphere.GetComponent<MeshRenderer> ().enabled = false;
		rightPalmSphere.GetComponent<MeshRenderer> ().enabled = false;
		rightPalmSphere.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		leftPalmSphere.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		leftPalmSphere.AddComponent<Rigidbody> ();
		rightPalmSphere.AddComponent<Rigidbody> ();
		leftPalmSphere.GetComponent<Rigidbody> ().useGravity = false;
		leftPalmSphere.GetComponent<Rigidbody> ().mass = 0.4f;
		leftPalmSphere.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rightPalmSphere.GetComponent<Rigidbody> ().useGravity = false;
		rightPalmSphere.GetComponent<Rigidbody> ().mass = 0.4f;
		rightPalmSphere.GetComponent<Rigidbody> ().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = provider.CurrentFrame;
		if (frame.Hands.Count > 0) {
			foreach (Hand hand in frame.Hands) {
				if (hand.IsLeft) {
					leftPalmSphere.transform.position = hand.PalmPosition.ToVector3 ();
					leftPalmSphere.transform.localScale = hand.PalmWidth * Vector3.one/2f;

				} else {
					rightPalmSphere.transform.position = hand.PalmPosition.ToVector3 ();
					rightPalmSphere.transform.localScale = hand.PalmWidth * Vector3.one/2f;

				}

			}
		} 
	}
}
