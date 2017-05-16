using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialManager : MonoBehaviour {

	//COM port
	public string portName = "COM6";

	//Serial port for communication
	private SerialPort port;

	//Baud rate for communication
	public int baudRate = 9600;

	//Read timeout
	public int timeOut = 50;

	//Message string from serial port
	private string messageRead = "";

	//Control variable to tell if reading is enabled
	public bool readingEnabled = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(readingEnabled && port != null && port.IsOpen)
			StartCoroutine (AsynchronousReadFromSerial((string s)=>ReadCallback(s), ()=> Fail(), 50f));
	}

	//Initialize port settings
	public void Initialize(){
		port = new SerialPort (portName, baudRate);
		port.ReadTimeout = timeOut;
		port.WriteTimeout = timeOut;

	}

	//Start connection
	public void StartConnection(){
		port.Open ();
		Debug.Log ("Port opened");

	}

	//Write a line to the serial buffer
	//Returns true if data was sent, false otherwise
	public bool Write(string message){
		if(port != null && port.IsOpen){
			Debug.Log ("Sending: " + message);
			port.Write (message);
			port.BaseStream.Flush ();
			return true;
		}
		return false;
	}

	public void WriteRoutine(string message){
		StartCoroutine(AsynchronousWrite(message, 100f));
	}

	void OnDestroy(){
		if(port != null && port.IsOpen)
			port.Close ();
		Debug.Log ("Closed port");
	}


	//Read from serial port
	public string Read(){
		if (readingEnabled)
			return messageRead;
		else
			return "No Reading Enabled";
	}

	//Read callback
	void ReadCallback(string message ){
		messageRead = message;

	}

	//Time out callback
	void Fail(){

	}

	//Checks if there is an active connection
	public bool ActiveConnection(){
		return (port != null && port.IsOpen);
	}

	//Asynchronous read coroutine from serial port
	public IEnumerator AsynchronousReadFromSerial(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
		DateTime initialTime = DateTime.Now;
		DateTime nowTime;
		TimeSpan diff = default(TimeSpan);

		string dataString = null;

		do {
			try {
				dataString = port.ReadLine();
			}
			catch (TimeoutException) {
				dataString = null;
			}

			if (dataString != null)
			{
				callback(dataString);
				yield return null;
			} else
				yield return new WaitForSeconds(0.05f);

			nowTime = DateTime.Now;
			diff = nowTime - initialTime;

		} while (diff.Milliseconds < timeout);

		if (fail != null)
			fail();
		yield return null;
	}

	public IEnumerator AsynchronousWrite(string message, float miliseconds){
		yield return new WaitForSeconds (miliseconds / 1000.0f);
		if (port != null && port.IsOpen) {
			port.Write (message);
			port.BaseStream.Flush ();
		}

	}
}
