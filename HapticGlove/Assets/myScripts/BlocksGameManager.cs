using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlocksGameManager : MonoBehaviour {

	//Reference object material when the haptic object is in the correct spot
	public Material correct;

	//Reference object material when the haptic object is misplaced
	public Material incorrect;

	//User total score
	public int gameScore = 0;

	//Total time in seconds
	public float totalTime = 90f;

	//Score log
	public Text scoreLog;

	//Time log
	public Text timeLog;

	//Game state log
	public Text stateLog;

	//Slots
	public Slot[] slots;

	//Game on
	private bool gameOn = false;

	//A routine is active
	private bool routineActive = false;

	//Slots reference object positions
	private Vector3[] referencePositions = {
		new Vector3(-0.181f,-0.181f,1.883f),
		new Vector3(0.01f,-0.181f,1.883f),
		new Vector3(0.203f,-0.181f,1.883f),
		new Vector3(0.394f,-0.181f,1.883f)
	};

	//Array of reference objects
	public GameObject[] referenceObjs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOn && !routineActive) {
			CheckSlots ();
			totalTime -= Time.deltaTime;
			CheckTime ();
		}
	}

	//Fisher Yates Shuffle
	private void RandomizeArray(){
		for (int i = referenceObjs.Length - 1; i > 0; i--) {
			int r = Random.Range (0, i);
			GameObject temp = referenceObjs [i];
			referenceObjs [i] = referenceObjs [r];
			referenceObjs [r] = temp;
		}
	}

	//Place reference objects in random positions
	private void RandomPlaceObjects(){
		RandomizeArray ();
		for (int i = 0; i < referencePositions.Length; i++) {
			referenceObjs [i].transform.position = referencePositions [i];
			referenceObjs [i].GetComponent<MeshRenderer> ().material = incorrect;
		}
	}

	//Method triggered when a haptic object hits a slot
	public bool CheckSlot(int hapticObjType, int slotIndex){
		if (gameOn) {
			GameObject referenceObject = referenceObjs [slotIndex];
			if (hapticObjType == referenceObject.GetComponent<Block> ().GetObjectType ()) {
				//The haptic object is correctly placed
				referenceObject.GetComponent<MeshRenderer> ().material = correct;
				return true;
			
			} else {
				return false;
			}
		}
		return false;
	}

	//Clears a reference obj
	public void ClearReferenceObj(int slotIndex){
		if(gameOn)
			referenceObjs [slotIndex].GetComponent<MeshRenderer> ().material = incorrect;
	}

	//The user won the round
	private void WinRound(){
		RandomPlaceObjects ();
		gameScore++;
		scoreLog.text = "Rondas Completadas: " + gameScore;
	}

	//Check slots condition
	private void CheckSlots(){
		int score = 0;
		foreach (Slot slot in slots)
			score += slot.IsCorrect () ? 1 : 0;
		if (score == slots.Length) {
			StartCoroutine (RandomizeRoutine ());
		}
	}

	private IEnumerator RandomizeRoutine(){
		routineActive = true;
		yield return new WaitForSeconds (1f);
		WinRound ();
		yield return new WaitForSeconds (1f);
		foreach (Slot slot in slots)
			slot.ResetSlot ();
		routineActive = false;
	}

	//Check time
	private void CheckTime(){
		float min = (totalTime / 60)%60;
		float sec = totalTime % 60;
		if (totalTime < 0) {
			//Game over
			gameOn = false;
			timeLog.text = "Tiempo Finalizado";
			stateLog.text = "";
		}
		else
			timeLog.text = "Tiempo Restante: " + min.ToString("00") + ":" + sec.ToString("00");
	}

	//Start a game
	public void GameStart(){
		gameOn = true;
		totalTime = 90f;
		gameScore = 0;
		stateLog.text = "START!!";
	}


}
