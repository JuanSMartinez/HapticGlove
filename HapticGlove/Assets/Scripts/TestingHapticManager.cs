using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class TestingHapticManager : MonoBehaviour {

	//Raycasting distance
	public float rayDistance = 0.01f;

	//Serial Manager
	public SerialManager serialManager;

	//Maximum weight to manipulate in the application in Kg
	public float maxMass = 10f;

	//Maximum velocity to achieve in movement in m/s
	public float maxVelocity = 2f;

	//Array of finger suffixes in the haptic glove
	private string[] suffixes = {"0_2","1_0", "1_2","2_0", "2_2","3_0", "3_2","4_0", "4_2"};

	//Array of touching bones
	public List<GameObject> bones;

	//Palm
	public GameObject palm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Compute haptic values for vibration and serial data
		string serialData = GetHapticValues ();
		//Send data through serial port
		serialManager.Write(serialData);
		//Debug.Log (serialManager.Read ());
		Debug.Log(serialData);
		//Debug.Log (serialManager.Read ());
	}

	private string GetHapticValues(){
		//List of bones in contact with haptic object
		List<GameObject> touchingBones = new List<GameObject> ();

		//Total energy and friction values 
		float totalEnergy = 0;
		float totalFriction = 0;

		//Calculate physics for each touching bone and populate list of touching bones
		foreach (GameObject bone in bones) {
			Ray collisionRay = new Ray (bone.transform.position, Vector3.up);
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
		Ray collisionRayPalm = new Ray (palm.transform.position, Vector3.up);
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
		//float serialPercentage = Mathf.Max(( 0.5f * normalizedEnergy + 0.5f * normalizedFriction), 0.6f);
		float serialPercentage = 1.0f;

		return "S" + serialPercentage.ToString ("0.00") + ":" + controlWord;

	}

	private string CreateControlWord(List<GameObject> bones){
		string control = "";
		foreach (string boneSuffix in suffixes) {
			bool found = false;
			foreach (GameObject bone in bones) {
				if (bone.name.Equals (boneSuffix))
					found = true;
			}
			control += found ? "U" : "D";
		}
		return control;
	}
}
