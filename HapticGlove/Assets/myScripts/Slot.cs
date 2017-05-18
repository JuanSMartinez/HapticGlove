using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

	//Slot index
	public int slotIndex;

	//Game manager
	public BlocksGameManager manager;

	//Haptic object contained
	private GameObject hapticObjContained;

	//slot is finally placed
	private bool slotPlaced = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (!slotPlaced && other.gameObject.CompareTag ("HapticObj")) {
			hapticObjContained = other.gameObject;
			slotPlaced = manager.CheckSlot (hapticObjContained.GetComponent<Block> ().GetObjectType (), slotIndex);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.CompareTag ("HapticObj")) {
			if (slotPlaced && manager.CheckSlot (other.gameObject.GetComponent<Block> ().GetObjectType (), slotIndex)) {
				slotPlaced = false;
				manager.ClearReferenceObj (slotIndex);
			}
		}

	}

	public bool IsCorrect(){
		return slotPlaced;
	}

	public void ResetSlot(){
		slotPlaced = false;
	}


}
