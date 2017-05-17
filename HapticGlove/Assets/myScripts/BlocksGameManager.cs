using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksGameManager : MonoBehaviour {

	//Reference object material when the haptic object is in the correct spot
	public Material correct;

	//Reference object material when the haptic object is misplaced
	public Material incorrect;

	//Score of a single run, has to reach 4 
	private int runScore = 0;

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
		for (int i = 0; i < referencePositions.Length; i++)
			referenceObjs [i].transform.position = referencePositions [i];
	}

	//Method triggered when a haptic object hits a slot
	public void CheckSlot(int hapticObjType, int slotIndex){
		GameObject referenceObject = referenceObjs [slotIndex];
		if (hapticObjType == referenceObject.GetComponent<Block> ().GetObjectType ()) {
			//The haptic object is correctly placed
			referenceObject.GetComponent<MeshRenderer>().material = correct;
			runScore++;
			if (runScore == 4)
				WinRound ();
		}
	}

	//Method triggered when a haptic object leaves a slot
	public void SlotLeft(int slotIndex){
		referenceObjs [slotIndex].GetComponent<MeshRenderer> ().material = incorrect;
		runScore--;
		runScore = Mathf.Max (0, runScore);
	}

	//The user won the round
	private void WinRound(){
		Debug.Log ("Round won");
		runScore = 0;
	}


}
