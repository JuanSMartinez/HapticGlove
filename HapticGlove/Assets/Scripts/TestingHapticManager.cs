using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.UI;

public class TestingHapticManager : MonoBehaviour {

	//Voltage Vp log
	public Text log;

	//Com port input
	public InputField input;

	//Percentage slider
	public Slider slider;

	//Serial Manager
	public SerialManager serialManager;

	//Array of finger suffixes in the haptic glove
	private string[] suffixes = {"0_2","1_0", "1_2","2_0", "2_2","3_0", "3_2","4_0", "4_2"};

	//Array of touching bones
	public List<GameObject> bones;

	//Palm
	public GameObject palm;

	//Serial percentage 
	private float serialPercentage;

	//Serial data
	private string serialWriteData = "";

	// Use this for initialization
	void Start () {
		serialPercentage = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		//Compute haptic values for vibration and serial data

		string serialData = GetHapticValues ();

		//Send data through serial port
		if (!serialData.Equals (serialWriteData)) {
			serialWriteData = serialData;
			if(serialManager.Write (serialData)){
				/*bool sent = false;
				while (!sent) {
					if (serialManager.Read ().Equals ("ACK"))
						sent = true;
					else
						serialManager.Write (serialData);
				}*/
				Debug.Log(serialManager.Read ());
			}
		}
	}

	private string GetHapticValues(){
		//List of bones in contact with haptic object
		List<GameObject> touchingBones = new List<GameObject> ();

		//Calculate physics for each touching bone and populate list of touching bones
		foreach (GameObject bone in bones) 
			if (bone.GetComponent<TestMotorBeahavior>().MotorActive()) 
				touchingBones.Add (bone);

		//Get the partial control word for only the finger bones
		string controlWord = CreateControlWord (touchingBones);

		//Compute values for the palm actuator and the control signal for the palm
		if (palm.GetComponent<TestMotorBeahavior>().MotorActive()) {
				controlWord += "U";
			} else
				controlWord += "D";

		float Is = serialPercentage * 183.0f * GetNumberOfActiveMotors(controlWord) / (10.0f);
		float objV = (Is - 19.580882300717f) / 32.555248527564f;
		log.text = "Vp: " + objV.ToString ("0.00") + "V";
		return "S" + serialPercentage.ToString ("0.000") + ":" + controlWord + "E";

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

	public void ValueChangeCheck(){
		serialPercentage = slider.value;
	}

	public void Connect(){
		
		serialManager.portName = input.text;
		serialManager.Initialize ();
		serialManager.StartConnection ();
	}

	private int GetNumberOfActiveMotors(string controlWord){
		char[] characters = controlWord.ToCharArray ();
		int n = 0;
		foreach (char c in characters)
			if (c.Equals ('U'))
				n++;
		return n;
	}
}
