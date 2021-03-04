using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class Loginscreen : MonoBehaviour {

	public GameObject recieveOTPpanel;

	public InputField forMobileNumber;
	public InputField forOTPrecieve;

	public bool yesAuthenticated = false;


	FirebaseAuth firebaseAuth;
	FirebaseUser user;
	PhoneAuthProvider provider;

	// Handle initialization of the necessary firebase modules:
	void InitializeFirebase() {
		Debug.Log("Setting up Firebase Auth");
		firebaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
	}
	// Use this for initialization
	void Start () 
	{
		recieveOTPpanel.SetActive (false);
		InitializeFirebase ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) && recieveOTPpanel.activeSelf) 
		{
			recieveOTPpanel.SetActive (false);
		}
		
	}

	public void OnSubmitOTPclick(){
		DonotdestroyHandler.prevLvlNum = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
		Credential credential =
			provider.GetCredential(PlayerPrefs.GetString("verifyid"), forOTPrecieve.text);

		if (credential != null) {
			firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
				if (task.IsFaulted) {
					Debug.LogError("SignInWithCredentialAsync encountered an error: " +
						task.Exception);
					return;
				}

				FirebaseUser newUser = task.Result;
				Debug.Log("User signed in successfully");
				// This should display the phone number.
				Debug.Log("Phone number: " + newUser.PhoneNumber);
				// The phone number providerID is 'phone'.
				Debug.Log("Phone provider ID: " + newUser.ProviderId);
				if (yesAuthenticated) {
					UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
				} else {
					if (PlayerPrefs.GetInt ("userformcomplete",0) == 0) {
						UnityEngine.SceneManagement.SceneManager.LoadScene ("Newuserform");
					} else {
						UnityEngine.SceneManagement.SceneManager.LoadScene ("WaitingLounge");
					}
				}
			});


		}
		
	}

	public void OnGetOTPclick(){
		
		string phoneNumber ="+91"+ forMobileNumber.text;
		uint phoneAuthTimeoutMs = 120000;
		provider = PhoneAuthProvider.GetInstance(firebaseAuth);
		Debug.Log (phoneNumber + ".....Phone number");
		provider.VerifyPhoneNumber(phoneNumber, phoneAuthTimeoutMs, null,
			verificationCompleted: (credential) => {
				// Auto-sms-retrieval or instant validation has succeeded (Android only).
				// There is no need to input the verification code.
				// `credential` can be used instead of calling GetCredential().
				Debug.Log (".....auto verify");
				if (yesAuthenticated) {
					UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
				} else {
					if (PlayerPrefs.GetInt ("userformcomplete",0) == 0) {
						UnityEngine.SceneManagement.SceneManager.LoadScene ("Newuserform");
					} else {
						UnityEngine.SceneManagement.SceneManager.LoadScene ("WaitingLounge");
					}
				}
			},
			verificationFailed: (error) => {
				// The verification code was not sent.
				// `error` contains a human readable explanation of the problem.
				Debug.Log (".....failed because ..." + error);
			},
			codeSent: (id, token) => {
				// Verification code was successfully sent via SMS.
				// `id` contains the verification id that will need to passed in with
				// the code from the user when calling GetCredential().
				// `token` can be used if the user requests the code be sent again, to
				// tie the two requests together.
				Debug.Log("inside codesent");
				PlayerPrefs.SetString("verifyid",id);
//				PlayerPrefs.SetString("verifytoken",token);
				recieveOTPpanel.SetActive (true);
			},
			codeAutoRetrievalTimeOut: (id) => {
				// Called when the auto-sms-retrieval has timed out, based on the given
				// timeout parameter.
				// `id` contains the verification id of the request that timed out.
				Debug.Log("inside timeout");
			});

		Debug.Log ("Out of town");
	}

	public void OnFBloginclick(){
		DonotdestroyHandler.prevLvlNum = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;

//		string accessToken;
//		Firebase.Auth.Credential credential =
//			Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
//		firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
//			if (task.IsCanceled) {
//				Debug.LogError("SignInWithCredentialAsync was canceled.");
//				return;
//			}
//			if (task.IsFaulted) {
//				Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
//				return;
//			}
//
//			Firebase.Auth.FirebaseUser newUser = task.Result;
//			Debug.LogFormat("User signed in successfully: {0} ({1})",
//				newUser.DisplayName, newUser.UserId);
//		});


		if (yesAuthenticated) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Main");
		} else {
			if (PlayerPrefs.GetInt ("userformcomplete",0) == 0) {
				UnityEngine.SceneManagement.SceneManager.LoadScene ("Newuserform");
			} else {
				UnityEngine.SceneManagement.SceneManager.LoadScene ("WaitingLounge");
			}
		}
	}

	public void OnNewUserClick(){
		DonotdestroyHandler.prevLvlNum = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Newuserform");
	}
}

