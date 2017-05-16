using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class IntensityEvaluation : MonoBehaviour {

	//State machine constants
	public const int S1 = 1;
	public const int S_ON = 2;
	public const int S_WAIT = 3;
	public const int S_END = 4;

	//Control variable that tells that the 3 sec routine is active
	private bool routineActive = false;

	//Start variable
	private bool start = false;

	//Variable to wait for the user response
	private bool response = false;

	//Current state
	private int currentState = S1;

	//Material when motors are active
	public Material active;

	//Material when motors are inactive
	public Material inactive;

	//Serial Manager
	public SerialManager serialManager;

	//Array of motors
	public GameObject[] actuators;

	//Com port input
	public InputField comPort;

	//Completed runs
	private int runs = 0;

	//Serial data
	private string serialData = "S0.100:DDDDDDDDDDE";

	//Possible intensity values
	private string[] intensities = {"0.250", "0.500", "0.750", "1.000"};

	//Current intensity
	private string intensity = "0.250";

	//Text inputs for user information
	public InputField nameInput;
	public InputField ageInput;
	public InputField sexInput;

	//User log 
	public Text userLog;

	//File base path
	private static string BASE_PATH;

	//Stream writer of the text file
	private StreamWriter writer;

	//Total number of runs
	public int totalRuns = 30;

	// Use this for initialization
	void Start () {
		BASE_PATH = Application.dataPath;

	}
	
	// Update is called once per frame
	void Update () {
		if (start) {
			StateTransition ();
			WriteData ();
		}
	}

	//Manage state machine
	private void StateTransition(){
		if (!routineActive) {
			switch (currentState) {
			case S1:
				GetRandomIntesity ();
				currentState = S_ON;
				break;
			case S_ON:
				routineActive = true;
				StartCoroutine (OnRoutine ());
				break;
			case S_WAIT:
				
				if (response) {
					if (runs == totalRuns)
						currentState = S_END;
					else
						currentState = S1;
					response = false;
				}
				break;
			case S_END:
				serialData = "S0.100:DDDDDDDDDDE";
				runs = 0;
				Close ();
				break;
			default:
				currentState = S1;
				break;
			}

		}
	}

	//Get a random intensity
	private void GetRandomIntesity(){
		intensity = intensities [Random.Range (0, 4)];
	}

	//Answer feedback coroutine
	private IEnumerator Feedback(string userAnswer){
		userLog.text = "Respuesta: " + userAnswer + ". Respuesta Correcta: " + float.Parse (intensity) * 100 + "%";
		yield return new WaitForSeconds (3f);
		userLog.text = "Respuesta:";
		response = true;
	}

	//On routine
	private IEnumerator OnRoutine(){
		serialData = "S" + intensity + ":UUUUUUUUUUE";
		ActivateAllActuators ();
		yield return new WaitForSeconds (3f);
		serialData = "S" + intensity + ":DDDDDDDDDDE";
		currentState = S_WAIT;
		routineActive = false;
		DeactivateAllActuators ();

	}

	//Simple write data
	private void WriteData(){
		serialManager.Write (serialData);
		Debug.Log (serialManager.Read ());
	}

	//Visually activate all actuators
	private void ActivateAllActuators(){
		foreach (GameObject motor in actuators)
			for (int i = 0; i < motor.transform.childCount; i++)
				motor.transform.GetChild (i).GetComponent<MeshRenderer> ().material = active;
	}

	//Visually deactivate all actuators
	private void DeactivateAllActuators(){
		foreach (GameObject motor in actuators)
			for (int i = 0; i < motor.transform.childCount; i++)
				motor.transform.GetChild (i).GetComponent<MeshRenderer> ().material = inactive;
	}

	//User answers 25%
	public void Answer25(){
		if (writer != null && currentState == S_WAIT) {
			runs++;
			writer.WriteLine ("" + runs + ",0.250," + intensity);
			StartCoroutine (Feedback ("25%"));
		}
	}

	//User answers 50%
	public void Answer50(){
		if (writer != null && currentState == S_WAIT) {
			runs++;
			writer.WriteLine ("" + runs + ",0.500," + intensity);
			StartCoroutine (Feedback ("50%"));
		}
	}

	//User answers 75%
	public void Answer75(){
		if (writer != null && currentState == S_WAIT) {
			runs++;
			writer.WriteLine ("" + runs + ",0.750," + intensity);
			StartCoroutine (Feedback ("75%"));
		}
	}

	//User answers 100%
	public void Answer100(){
		if (writer != null && currentState == S_WAIT) {
			runs++;
			writer.WriteLine ("" + runs + ",1.000," + intensity);
			StartCoroutine (Feedback ("100%"));
		}
	}

	//Starts the evaluation
	public void StartEvaluation(){
		string name = nameInput.text;
		string age = ageInput.text;
		string sex = sexInput.text;

		bool dataOk = !name.Equals("") && !age.Equals("") && !sex.Equals("");

		if (dataOk) {
			//Create a new file
			if (File.Exists (BASE_PATH + "_" + name + ".txt"))
				File.Delete (BASE_PATH + "_" + name + ".txt");
			writer = File.CreateText(BASE_PATH + "_" + "_" + name + ".txt");
			writer.WriteLine (name + "," + age + "," + sex);
			writer.WriteLine ("Run,User Answer,Correct Answer");
			currentState = S1;
			start = true;
		}

	}
	//Connect to the glove
	public void Connect(){
		serialManager.portName = comPort.text;
		serialManager.Initialize ();
		serialManager.StartConnection ();
	}


	//Close the file
	public void Close(){
		if (writer != null) {
			userLog.text = "Prueba Terminada";
			writer.Close ();
			writer = null;
			start = false;
			runs = 0;
		}
	}

}
