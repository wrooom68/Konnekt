using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;

public class Loginscreen : MonoBehaviour {

    [SerializeField] private GameObject OTPpanel;

    [SerializeField] private GameObject recieveOTPpanel;

    [SerializeField] private GameObject setPINpanel;

    [SerializeField] private GameObject checkPINpanel;

    [SerializeField] private InputField forMobileNumber;
    [SerializeField] private InputField forOTPrecieve;

    [SerializeField] private InputField forPIN;
    [SerializeField] private InputField forConfirmPIN;

    [SerializeField] private InputField forCheckPIN;

    FirebaseApp app;
    FirebaseAuth firebaseAuth;
	FirebaseUser user;
	PhoneAuthProvider provider;

	// Use this for initialization
	void Start () 
	{
        OTPpanel.SetActive(false);
        recieveOTPpanel.SetActive(false);
        checkPINpanel.SetActive(false);
        setPINpanel.SetActive(false);
    }

    private void OnEnable()
    {
        AppHandler._instance._OTPautoVerifyFail += AutoVerifyOTPfail;
        AppHandler._instance._OTPscreen += ShowRegisterPage;
        AppHandler._instance._PINset += ShowPINPage;
        AppHandler._instance._PINcheck += ShowConfirmPINPage;
    }

    private void OnDisable()
    {
        AppHandler._instance._OTPautoVerifyFail -= AutoVerifyOTPfail;
        AppHandler._instance._OTPscreen -= ShowRegisterPage;
        AppHandler._instance._PINset -= ShowPINPage;
        AppHandler._instance._PINcheck -= ShowConfirmPINPage;
    }
    // Update is called once per frame
    void Update ()
    {
		if (Input.GetKeyDown (KeyCode.Escape) && recieveOTPpanel.activeSelf) 
		{
			recieveOTPpanel.SetActive (false);
		}
	}

    public void CheckAndSavePin()
    {
        if (forConfirmPIN.text.Equals(forPIN.text))
        {
            Debug.Log("Check 1");

            AppHandler._instance._myData.UserPin = forPIN.text;
            Debug.Log("Check 2");
            ShowConfirmPINPage();
        }
    }

    public void ConfirmPINcheckSuccess()
    {
        if (forCheckPIN.text.Equals(AppHandler._instance._myData.UserPin))
        {
            if (AppHandler._instance._myData._isVerified)
            {
                AppHandler._instance.ShowScene("Main");
            }
            else
            {
                if (AppHandler._instance._myData.UserName.Trim().Equals(""))
                {
                    AppHandler._instance.ShowScene("Newuserform");
                }
                else
                {
                    AppHandler._instance.ShowScene("WaitingLounge");
                }
            }
        }
    }

    void ShowRegisterPage()
    {
        OTPpanel.SetActive(true);
        recieveOTPpanel.SetActive(false);
        checkPINpanel.SetActive(false);
        setPINpanel.SetActive(false);
    }

    void ShowPINPage()
    {
        OTPpanel.SetActive(false);
        recieveOTPpanel.SetActive(false);
        checkPINpanel.SetActive(false);
        setPINpanel.SetActive(true);
    }

    void ShowConfirmPINPage()
    {
        Debug.Log("Check 3");

        OTPpanel.SetActive(false);
        recieveOTPpanel.SetActive(false);
        checkPINpanel.SetActive(true);
        Debug.Log("Check 4");
        setPINpanel.SetActive(false);

        Debug.Log("Check 5");
    }

	public void OnSubmitOTPclick()
    {
        AppHandler._instance.SignInWithPhoneOTP(forOTPrecieve.text);
	}

	public void OnGetOTPclick()
    {
        AppHandler._instance.SignInGetPhoneOTP(forMobileNumber.text);
	}

    void AutoVerifyOTPfail()
    {
        OTPpanel.SetActive(false);
        recieveOTPpanel.SetActive(true);
        checkPINpanel.SetActive(false);
        setPINpanel.SetActive(false);
    }

    public void OnNewUserClick()
    {
        AppHandler._instance.ShowScene("Newuserform");
	}

    public void OnEditorLoginClick()
    {
        AppHandler._instance.LoginInEditor();
    }
}

