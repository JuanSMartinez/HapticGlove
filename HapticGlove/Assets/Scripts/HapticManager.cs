using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using Leap.Unity;
using Leap;
using AssemblyCSharp;

public class HapticManager : MonoBehaviour {

	//Raycasting distance
	public float rayDistance = 0.01f;

	//Glove selection
	public Chirality gloveHandedness;

	//Leap provider
	LeapProvider provider;

	//Serial Manager
	public SerialManager serialManager;
	
	// Use this for initialization
	void Start () {
		provider = FindObjectOfType<LeapProvider> ();
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = provider.CurrentFrame;

		foreach (Hand hand in frame.Hands) {
				//Get the bones for the haptic glove used
				List<InteractionBrushBone> bones = FilterBones ();
				//Compute haptic values for vibration
				GetHapticValues (bones, hand.PalmNormal.ToVector3());
		}

	}

	private List<InteractionBrushBone> FilterBones(){
		InteractionBrushBone[] totalBones = FindObjectsOfType<InteractionBrushBone> ();
		List<InteractionBrushBone> result = new List<InteractionBrushBone> ();

		//Filter bones for handedness
		foreach (InteractionBrushBone bone in totalBones) {
			if (gloveHandedness == Chirality.Right && bone.name.StartsWith ("BrushHand_R_")||gloveHandedness==Chirality.Left && bone.name.StartsWith ("BrushHand_L_"))
				result.Add (bone);
			
		}
		return result;
	}
		

	private void GetHapticValues(List<InteractionBrushBone> bones, Vector3 palmDirection){
		foreach (InteractionBrushBone bone in bones) {
			Ray collisionRay = new Ray (bone.transform.position, -bone.transform.up);
			RaycastHit hit;
			if (Physics.Raycast (collisionRay, out hit, rayDistance)) {
				if (hit.collider.CompareTag ("HapticObj")) {
					float energy = PhysicsCalculator.GetKineticEnergyOfCollision (bone.transform.GetComponent<Rigidbody> (), hit.transform.GetComponent<Rigidbody> ());
					float friction = PhysicsCalculator.GetFrictionForceOfCollision (hit.normal, hit.transform.GetComponent<Rigidbody> (), hit.collider.material);
					Debug.Log ("Hit info for bone " + bone.name + ": Energy: " + energy + ". Friction: " + friction);
				}
			}
		}

		//Compute values for the palm actuator
		GameObject palm = gloveHandedness == Chirality.Left ? GameObject.Find("LeftPalm") : GameObject.Find("RightPalm");
		Ray collisionRayPalm = new Ray (palm.transform.position, palmDirection);
		RaycastHit hitPalm;
		if (Physics.Raycast (collisionRayPalm, out hitPalm, rayDistance)) {
			if (hitPalm.collider.CompareTag ("HapticObj")) {
				serialManager.Write ("ON");
				float energy = PhysicsCalculator.GetKineticEnergyOfCollision (palm.transform.GetComponent<Rigidbody> (), hitPalm.transform.GetComponent<Rigidbody> ());
				float friction = PhysicsCalculator.GetFrictionForceOfCollision (hitPalm.normal, hitPalm.transform.GetComponent<Rigidbody> (), hitPalm.collider.material);
				Debug.Log ("Hit info for bone " + palm.name + ": Energy: " + energy + ". Friction: " + friction);
			}

		}
		else
			serialManager.Write ("OFF");
	}
}
