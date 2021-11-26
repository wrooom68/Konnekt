using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonotdestroyHandler : MonoBehaviour {

	public static string prevScene;
	public GameObject backBtnCanvas;

	void Awake(){
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex < 2) {
			backBtnCanvas.SetActive (false);
		} else {
			backBtnCanvas.SetActive (true);
		}
	}

	public void OnBackButtonClick()
	{
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex <= 3 ) {
			Debug.Log ("Application Quits !!!");
			Application.Quit ();
		} else {
			AppHandler._instance.ShowScene(prevScene);
		}
	}


}
