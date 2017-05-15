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
	public const int S_ON = 3;
	public const int S_OFF = 4;
	public const int S_END = 5;

	//Current state
	private int currentState = 0;

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
	public List<GameObject> actuators;

	//Com port input
	public InputField input;

	//intensity percentage
	private string intensity = "0.200";

	//Number of times the sequence has been completed
	private int completedCycles = 0;

	//Intensity log components
	public Text intensityLog;
	public Slider intensitySlider;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (start)
			StateTransition ();
	}

	//State transition handler
	private void StateTransition(){

		//If the 3 seconds routines are not active, manage states
		if (!routineActive) {
			switch (currentState) {
			case S1:
				intensity = "0.200";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S2:
				intensity = "0.600";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S3:
				intensity = "1.000";
				ShowIntesity ();
				currentState = S_ON;
				break;
			case S_ON:
				StartCoroutine (OnRoutine ());
				break;
			case S_OFF:
				StartCoroutine (OffRoutine ());
				break;
			case S_END:
				StopStateMachine ();
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
				break;
			default:
				currentState = S1;
				break;
			}
		}
	}

	//3 seconds routine to activate actuators
	private IEnumerator OnRoutine(){
		routineActive = true;
		string message = "S" + intensity + ":" + "UUUUUUUUUUE";
		int tries = 0;
		serialManager.Write (message);
		string response = serialManager.Read ();
		while (!response.Equals ("ACK") && tries < 5) {
			serialManager.Write (message);
			response = serialManager.Read ();
		}
		ActivateActuators ();
		yield return new WaitForSeconds (3f);
		routineActive = false;
		currentState = S_OFF;
	}

	//3 seconds routin to deactivate motors
	private IEnumerator OffRoutine(){
		routineActive = false;
		string message = "S" + intensity + ":" + "DDDDDDDDDDE";
		int tries = 0;
		serialManager.Write (message);
		string response = serialManager.Read ();
		while (!response.Equals ("ACK") && tries < 5) {
			serialManager.Write (message);
			response = serialManager.Read ();
		}
		DeactivateActuators ();
		yield return new WaitForSeconds (3f);
		if(intensity.Equals("1.000"))
			completedCycles ++;
		if (completedCycles == 3 && intensity.Equals("1.000"))
			currentState = S_END;
		else if (intensity.Equals ("0.200"))
			currentState = S2;
		else if (intensity.Equals ("0.600"))
			currentState = S3;
		else if (intensity.Equals ("1.000"))
			currentState = S1;
		routineActive = false;

	}

	//Visually activate actuators
	private void ActivateActuators(){
		foreach (GameObject motor in actuators)
			motor.GetComponent<MeshRenderer> ().material = active;
	}

	//Visually deactivate all actuators
	private void DeactivateActuators(){
		foreach (GameObject motor in actuators)
			motor.GetComponent<MeshRenderer> ().material = inactive;
	}

	//Start state machine
	public void StartStateMachine(){
		if(serialManager.ActiveConnection())
			start = true;
	}

	//Stop state machine
	public void StopStateMachine(){
		string message = "S0.100:DDDDDDDDDDE";
		serialManager.Write (message);
		string response = serialManager.Read ();
		int tries = 0;
		while (!response.Equals ("ACK") && tries < 5) {
			serialManager.Write (message);
			response = serialManager.Read ();
		}
		currentState = S1;
		start = false;
	}

	//Connect to the glove
	public void Connect(){
		serialManager.portName = input.text;
		serialManager.Initialize ();
		serialManager.StartConnection ();
	}

	//Show intensity to the user
	private void ShowIntesity (){
		intensityLog.text = "Intensidad: " + float.Parse (intensity) * 100 + "%";
		intensitySlider.value = float.Parse (intensity);
	}
}
