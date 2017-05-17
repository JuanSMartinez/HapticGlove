using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FamiliarizationManager : MonoBehaviour {

	//State machine states 
	public const int S1 = 0;
	public const int S2 = 1;
	public const int S3 = 2;
	public const int S4 = 3;
	public const int S_ON = 4;
	public const int S_OFF = 5;
	public const int S_END = 6;

	//Current state
	private int currentState = S1;

	//Control variable that tells that the 3 sec routine is active
	private bool routineActive = false;

	//Control variable to start the state machine
	private bool start = false;

	//Material when motors are active
	public Material active;

	//Material when motors are inactive
	public Material inactive;

	//Serial Manager
	public SerialManager serialManager;

	//Array of motors
	public GameObject[] actuators;

	//intensity percentage
	private string intensity = "0.250";

	//Number of times the sequence has been completed
	private int completedCycles = 0;

	//Intensity log components
	public Text intensityLog;
	public Slider intensitySlider;
	public Text completedcyclesLog;

	//Serial data to write
	private string serialData = "S0.100:DDDDDDDDDDE";


	// Use this for initialization
	void Start () {
		//Connect to the glove
		Connect();
	}
	
	// Update is called once per frame
	void Update () {
		if (start) {
			StateTransition ();
		} else {
			serialData = "S0.100:DDDDDDDDDDE";
		}
		WriteData ();
	}

	//State transition handler
	private void StateTransition(){
		
		//If the 3 seconds routines are not active, manage states
		if (!routineActive) {
			switch (currentState) {
			case S1:
				intensity = "0.250";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S2:
				intensity = "0.500";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S3:
				intensity = "0.750";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S4:
				intensity = "1.000";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S_ON:
				routineActive = true;
				StartCoroutine (OnRoutine ());
				break;
			case S_OFF:
				routineActive = true;
				StartCoroutine (OffRoutine ());
				break;
			case S_END:
				StopStateMachine ();
				break;
			
			}
		}

	}

	//3 seconds routine to activate actuators
	private IEnumerator OnRoutine(){
		string baseMessage = "S" + intensity + ":" + "DDDDDDDDDDE";
		for (int i = 0; i < 10 && start; i++) {
			char[] message = baseMessage.ToCharArray ();
			message [i + 7] = 'U';
			serialData = new string (message);
			ActivateActuator (i);
			yield return new WaitForSeconds (1.5f);
		}
		serialData = "S0.100:DDDDDDDDDDE";
		DeactivateAllActuators ();
		yield return new WaitForSeconds (1f);
		baseMessage = "S" + intensity + ":" + "UUUUUUUUUUE";
		serialData = baseMessage;
		ActivateAllActuators ();
		yield return new WaitForSeconds (3f);
		routineActive = false;
		currentState = S_OFF;

	}

	//3 seconds routin to deactivate motors
	private IEnumerator OffRoutine(){
		string message = "S" + intensity + ":" + "DDDDDDDDDDE";
		//WriteProtocol (message);
		serialData = message;
		DeactivateAllActuators ();
		yield return new WaitForSeconds (3f);
		if(intensity.Equals("1.000"))
			completedCycles ++;
		if (completedCycles == 3 && intensity.Equals ("1.000"))
			currentState = S_END;
		else if (intensity.Equals ("0.250"))
			currentState = S2;
		else if (intensity.Equals ("0.500"))
			currentState = S3;
		else if (intensity.Equals ("0.750"))
			currentState = S4;
		else if (intensity.Equals ("1.000"))
			currentState = S1;
		routineActive = false;

	}

	//Visually activate all actuators
	private void ActivateAllActuators(){
		foreach (GameObject motor in actuators)
			for (int i = 0; i < motor.transform.childCount; i++)
				motor.transform.GetChild (i).GetComponent<MeshRenderer> ().material = active;
	}

	//Visually activate actuator
	private void ActivateActuator(int index){
		for (int i = 0; i < actuators.Length; i++) {
			if(i == index)
				for (int j = 0; j < actuators[i].transform.childCount; j++)
					actuators[i].transform.GetChild (j).GetComponent<MeshRenderer> ().material = active;
			else
				for (int j = 0; j < actuators[i].transform.childCount; j++)
					actuators[i].transform.GetChild (j).GetComponent<MeshRenderer> ().material = inactive;
		}
	}

	//Visually deactivate all actuators
	private void DeactivateAllActuators(){
		foreach (GameObject motor in actuators)
			for (int i = 0; i < motor.transform.childCount; i++)
				motor.transform.GetChild (i).GetComponent<MeshRenderer> ().material = inactive;
	}

	//Start state machine
	public void StartStateMachine(){
		if (serialManager.ActiveConnection ()) {
			start = true;
			completedCycles = 0;
			currentState = S1;
			routineActive = false;
		}
	}

	//Stop state machine
	public void StopStateMachine(){
		start = false;
		StopAllCoroutines ();
		DeactivateAllActuators ();
		intensity = "0.000";
		completedCycles = 0;
		ShowIntesity ();
		currentState = S1;
		routineActive = false;
	}

	//Connect to the glove
	public void Connect(){
		serialManager.portName = SerialConfiguration.serialPort;
		serialManager.Initialize ();
		serialManager.StartConnection ();
	}

	//Show intensity to the user
	private void ShowIntesity (){
		intensityLog.text = "Intensidad: " + float.Parse (intensity) * 100 + "%";
		intensitySlider.value = float.Parse (intensity);
		completedcyclesLog.text = "Ciclos Completados: " + completedCycles;
	}

	//Writing protocol
	private void WriteProtocol(string message){
		int tries = 0;
		serialManager.Write (message);
		string response = serialManager.Read ();
		while (!(response.Equals ("ACK") || response.Equals ("")) && tries < 5) {
			Debug.Log ("Response: " + response);
			serialManager.Write (message);
			response = serialManager.Read ();
			tries++;
		}
	}

	//Simple write data
	private void WriteData(){
		serialManager.Write (serialData);
		Debug.Log (serialManager.Read ());
	}

	//Load Main Menu Scene
	public void LoadMainMenu(){
		StopStateMachine ();
		SceneManager.LoadScene ("MainMenu");
	}
		


}
