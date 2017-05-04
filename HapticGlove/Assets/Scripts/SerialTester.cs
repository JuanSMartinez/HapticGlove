using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialTester : MonoBehaviour {

	//Message text
	public UnityEngine.UI.Text textInput;

	//Response message from the serial port
	public UnityEngine.UI.Text response;

	//Serial manager
	public SerialManager serialManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (serialManager.readingEnabled)
			response.text = serialManager.Read ();
	}

	//Send serial data
	public void Send(){
		serialManager.Write (textInput.text);
	}
}
