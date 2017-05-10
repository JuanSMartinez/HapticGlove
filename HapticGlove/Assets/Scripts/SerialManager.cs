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
		port = new SerialPort (portName, baudRate);
		port.ReadTimeout = timeOut;
		port.Open ();
	}
	
	// Update is called once per frame
	void Update () {
		if(readingEnabled)
			StartCoroutine (AsynchronousReadFromSerial((string s)=>ReadCallback(s), ()=> Debug.Log("Time out"), 100f));
	}

	//Start connection
	public void StartConnection(){
		port.Open ();
	}

	//Write a line to the serial buffer
	public void Write(string message){
		
		port.WriteLine (message);
		//port.BaseStream.Flush ();
	}

	void OnDestroy(){
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
}
