using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll ();
		Invoke ("LoadNextScene", 3);
	}

	void LoadNextScene(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Login");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
