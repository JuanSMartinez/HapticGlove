using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

	//Slot index
	public int slotIndex;

	//Game manager
	public BlocksGameManager manager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("HapticObj")) {
			manager.CheckSlot (other.GetComponent<Block> ().GetObjectType (), slotIndex);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.CompareTag ("HapticObj")) {
			manager.SlotLeft (slotIndex);
		}
	}
}
