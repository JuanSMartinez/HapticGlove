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

	//Array of finger suffixes in the haptic glove
	private string[] suffixes = {"0_2","1_0", "1_2","2_0", "2_2","3_0", "3_2","4_0", "4_2"};

	//Maximum weight to manipulate in the application in Kg
	public float maxMass = 10f;

	//Maximum velocity to achieve in movement in m/s
	public float maxVelocity = 2f;

	//Serial data
	private string serialWriteData = "";


	
	// Use this for initialization
	void Start () {
		provider = FindObjectOfType<LeapProvider> ();
		serialManager.Initialize ();
		serialManager.StartConnection ();
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = provider.CurrentFrame;

		foreach (Hand hand in frame.Hands) {
			//Get the bones for the haptic glove used
			List<InteractionBrushBone> bones = FilterBones ();
			//Compute haptic values for vibration and serial data
			string serialData = GetHapticValues (bones, hand.PalmNormal.ToVector3());
			//Send data through serial port
			if (!serialData.Equals (serialWriteData) || GetNumberOfActiveMotors(serialData)==0) {
				serialWriteData = serialData;
				SendDataProtocol (serialWriteData);
			}
	
		}


	}

	private List<InteractionBrushBone> FilterBones(){
		InteractionBrushBone[] totalBones = FindObjectsOfType<InteractionBrushBone> ();
		List<InteractionBrushBone> result = new List<InteractionBrushBone> ();

		//Filter bones for handedness
		foreach (InteractionBrushBone bone in totalBones) {
			if (gloveHandedness == Chirality.Right && bone.name.StartsWith ("BrushHand_R_") || gloveHandedness == Chirality.Left && bone.name.StartsWith ("BrushHand_L_")) {
				string[] boneNameSplit = bone.name.Split(new char[]{'_'});
				if(boneNameSplit[3].Equals("0") || boneNameSplit[3].Equals("2"))
					result.Add (bone);
			}

		}
		return result;
	}
		

	private string GetHapticValues(List<InteractionBrushBone> bones, Vector3 palmDirection){
		//List of bones in contact with haptic object
		List<InteractionBrushBone> touchingBones = new List<InteractionBrushBone> ();

		//Total energy and friction values 
		float totalEnergy = 0;
		float totalFriction = 0;

		//Calculate physics for each touching bone and populate list of touching bones
		foreach (InteractionBrushBone bone in bones) {
			Ray collisionRay = new Ray (bone.transform.position, -bone.transform.up);
			RaycastHit hit;
			if (Physics.Raycast (collisionRay, out hit, rayDistance)) {
				if (hit.collider.CompareTag ("HapticObj")) {
					touchingBones.Add (bone);
					float energy = PhysicsCalculator.GetKineticEnergyOfCollision (bone.transform.GetComponent<Rigidbody> (), hit.transform.GetComponent<Rigidbody> ());
					float friction = PhysicsCalculator.GetFrictionForceOfCollision (hit.normal, hit.transform.GetComponent<Rigidbody> (), hit.collider.material);
					totalEnergy += energy;
					totalFriction += friction;
				}
			}
		}

		//Get the partial control word for only the finger bones
		string controlWord = CreateControlWord (touchingBones);

		//Compute values for the palm actuator and the control signal for the palm
		GameObject palm = gloveHandedness == Chirality.Left ? GameObject.Find("LeftPalm") : GameObject.Find("RightPalm");
		Ray collisionRayPalm = new Ray (palm.transform.position, palmDirection);
		RaycastHit hitPalm;
		if (Physics.Raycast (collisionRayPalm, out hitPalm, rayDistance)) {
			if (hitPalm.collider.CompareTag ("HapticObj")) {
				controlWord += "U";
				float energy = PhysicsCalculator.GetKineticEnergyOfCollision (palm.transform.GetComponent<Rigidbody> (), hitPalm.transform.GetComponent<Rigidbody> ());
				float friction = PhysicsCalculator.GetFrictionForceOfCollision (hitPalm.normal, hitPalm.transform.GetComponent<Rigidbody> (), hitPalm.collider.material);
				totalEnergy += energy;
				totalFriction += friction;
			} else
				controlWord += "D";

		} else {
			controlWord += "D";
		}

		//Calculate average energy and friction
		float averageEnergy = totalEnergy / 10f;
		float averageFriction = totalFriction / 10f;

		//Normalize energy and friction in terms of the maximum kinetic energy and maximum weight 
		float normalizedEnergy = Mathf.Min(averageEnergy, 0.5f*maxMass*maxVelocity*maxVelocity)/ (0.5f*maxMass*maxVelocity*maxVelocity);
		float normalizedFriction = Mathf.Min (averageFriction, maxMass * 9.8f) / (maxMass * 9.8f);

		//percentage of the maximum current, has to be greater than 60%
		float serialPercentage = Mathf.Max(( 0.5f * normalizedEnergy + 0.5f * normalizedFriction), 0.6f);
		//float serialPercentage = 1.0f;
		return "S" + serialPercentage.ToString ("0.000") + ":" + controlWord+"E";

	}

	private string CreateControlWord(List<InteractionBrushBone> bones){
		string control = "";
		foreach (string boneSuffix in suffixes) {
			bool found = false;
			foreach (InteractionBrushBone bone in bones) {
				if (bone.name.Substring (12, 3).Equals (boneSuffix))
					found = true;
			}
			control += found ? "U" : "D";
		}
		return control;
	}

	private void SendDataProtocol(string serialData){
		int tries = 0;
		if(serialManager.Write (serialData)){
			bool proceed = false;
			string response = serialManager.Read ();
			while (!proceed && tries < 5) {
				Debug.Log(response);
				if (response.Equals ("ACK")) {
					proceed = true;
				} else if(response.Equals("NACK")) {
					serialManager.Write (serialWriteData);
				}
				response = serialManager.Read ();
				tries++;
			}

		}
	}

	private int GetNumberOfActiveMotors(string serialData){
		char[] characters = serialData.ToCharArray ();
		int n = 0;
		foreach (char c in characters)
			if (c == 'U')
				n++;
		return n;
	}







}
