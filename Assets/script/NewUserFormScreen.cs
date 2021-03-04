using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewUserFormScreen : MonoBehaviour {

	public RawImage userProfilePic;
	public InputField userProfileName;
	public InputField userAge;
	public Dropdown userGender;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SelectPicFromDevice()
	{
	
	}

	public void SaveData()
	{
		PlayerPrefs.SetInt ("userformcomplete",1);
		UnityEngine.SceneManagement.SceneManager.LoadScene ("WaitingLounge");
	}
}
