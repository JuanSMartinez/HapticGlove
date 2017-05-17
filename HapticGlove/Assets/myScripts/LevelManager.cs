using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	//Serial com port input field
	public InputField serialInput;

	//Set label
	public Text serialSetLabel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Loads the familiarization scene
	public void LoadFamiliarizationScene(){
		SceneManager.LoadScene ("Familiarization");
	}

	//Loads the perception testing scene
	public void LoadPerceptionTestScene(){
		SceneManager.LoadScene ("IntensityPerception");
	}

	//Load the blocks demo scene
	public void LoadBlocksDemo(){
		SceneManager.LoadScene ("Blocks");
	}

	//Set the serial com port in the global settings
	public void SetSerialPort(){
		SerialConfiguration.serialPort = serialInput.text;
		StartCoroutine (ShowSetMessage ());
	}

	//Routine to tell the user the serial port has been set 
	private IEnumerator ShowSetMessage(){
		serialSetLabel.text = "Puerto COM configurado!";
		yield return new WaitForSeconds (3f);
		serialSetLabel.text = "";
	}


}
