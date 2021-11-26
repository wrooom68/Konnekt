using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mainscreen : MonoBehaviour {

	public GameObject recentPanel;
	public GameObject malePanel;
	public GameObject femalePanel;

	public GameObject SearchPanel;

	// Use this for initialization
	void Start () {
		recentPanel.SetActive (true);
		malePanel.SetActive (false);
		femalePanel.SetActive (false);
		SearchPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnProfileClick(){
		AppHandler._instance.ShowScene ("Userprofile");
	}

	public void OnMalePanelClick(){
		recentPanel.SetActive (false);
		malePanel.SetActive (true);
		femalePanel.SetActive (false);
	}

	public void OnFemalePanelClick(){
		recentPanel.SetActive (false);
		malePanel.SetActive (false);
		femalePanel.SetActive (true);
	}

	public void OnRecentPanelClick(){
		recentPanel.SetActive (true);
		malePanel.SetActive (false);
		femalePanel.SetActive (false);
	}

	public void OnSearchClick(){
		SearchPanel.SetActive (true);
	}

	public void OnSearchBackClick(){
		SearchPanel.SetActive (false);
	}

	public void OnCandidateClick(){
        AppHandler._instance.ShowScene("Candidateprofile");
	}
}
