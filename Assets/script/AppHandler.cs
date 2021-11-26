using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using System.Threading.Tasks;

public class AppHandler : MonoBehaviour
{
    public static AppHandler _instance = null;

    private void Awake()
    {
        _instance = this;
        InitializeFirebase();
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    #region "Firebase Login"

    FirebaseApp app;
    FirebaseAuth auth;
    FirebaseUser user;
    PhoneAuthProvider provider;

    public delegate void OTPscreen();
    public OTPscreen _OTPscreen;

    public delegate void PINset();
    public PINset _PINset;

    public delegate void PINcheck();
    public PINcheck _PINcheck;

    public delegate void OTPautoVerifyFail();
    public OTPautoVerifyFail _OTPautoVerifyFail;

    public void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                InitFirebaseDatabase();
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(string.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                _OTPscreen?.Invoke();
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                OnSuccessLogin();
            }
        }
        else
        {
            _OTPscreen?.Invoke();
        }
    }

    public void SignInGetPhoneOTP(string mobileNumber)
    {
        string phoneNumber = "+91" + mobileNumber;
        uint phoneAuthTimeoutMs = 120000;
        provider = PhoneAuthProvider.GetInstance(auth);
        Debug.Log(phoneNumber + ".....Phone number");
        AppHandler._instance._myData._mobileNumber = mobileNumber;
        provider.VerifyPhoneNumber(phoneNumber, phoneAuthTimeoutMs, null,
            verificationCompleted: (credential) =>
            {
                // Auto-sms-retrieval or instant validation has succeeded (Android only).
                // There is no need to input the verification code.
                // `credential` can be used instead of calling GetCredential().
                Debug.Log(".....auto verify");
                OnSuccessLogin();
            },
            verificationFailed: (error) =>
            {
                // The verification code was not sent.
                // `error` contains a human readable explanation of the problem.
                Debug.Log(".....failed because ..." + error);
            },
            codeSent: (id, token) =>
            {
                // Verification code was successfully sent via SMS.
                // `id` contains the verification id that will need to passed in with
                // the code from the user when calling GetCredential().
                // `token` can be used if the user requests the code be sent again, to
                // tie the two requests together.
                Debug.Log("inside codesent");
                PlayerPrefs.SetString("verifyid", id);
                //				PlayerPrefs.SetString("verifytoken",token);
                _OTPautoVerifyFail?.Invoke();
            },
            codeAutoRetrievalTimeOut: (id) =>
            {
                // Called when the auto-sms-retrieval has timed out, based on the given
                // timeout parameter.
                // `id` contains the verification id of the request that timed out.
                Debug.Log("inside timeout");
            });
    }

    public void SignInWithPhoneOTP(string OTPrecieved)
    {
        Credential credential =
            provider.GetCredential(PlayerPrefs.GetString("verifyid"), OTPrecieved);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
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
            OnSuccessLogin();
        });
    }

    void OnSuccessLogin()
    {
        _myData.UserId = user.UserId;
        FetchUserData();
    }


    public void LoginInEditor()
    {
        auth.SignInWithEmailAndPasswordAsync("ruchirdarji8@gmail.com", "123456789").ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    #endregion

    #region "Firebase Logout"

    public void OnLogoutClick()
    {
        auth.SignOut();
    }

    #endregion

    #region "Scene Transition"
    public void ShowScene(string sceneName)
    {
        DonotdestroyHandler.prevScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    #endregion

    #region "Firebase Database"

    public PlayerData _myData;

    DatabaseReference _fireReference;

    void InitFirebaseDatabase()
    {
        _fireReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void FetchUserData()
    {
        Debug.Log("inside FetchUserData");

        FirebaseDatabase.DefaultInstance
        .GetReference("users").Child(auth.CurrentUser.UserId)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("inside FetchUserData error");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("check 1");
                DataSnapshot snapshot = task.Result;
                if (snapshot != null)
                {
                    Debug.Log("check 2");
                    if (!snapshot.Exists)
                    {
                        Debug.Log("check 3");
                        _PINset?.Invoke();
                    }
                    else
                    {
                        Debug.Log("check 4");
                        _myData = JsonUtility.FromJson<PlayerData>(snapshot.GetRawJsonValue());
                        if (_myData.UserPin.Trim().Equals(""))
                        {
                            Debug.Log("inside FetchUserData _PINset");
                            _PINset?.Invoke();
                        }
                        else
                        {
                            Debug.Log("inside FetchUserData _PINcheck");
                            _PINcheck?.Invoke();
                        }
                    }
                }
                else
                {
                    Debug.Log("inside FetchUserData null");
                    _PINset?.Invoke();
                }
            }
        });
    }

    public void SetUserData()
    {
        _fireReference.Child("users").Child(user.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(_myData));
    }

    #endregion

    #region "Firebase Storage"

    public delegate void OnUploadComplete(string url);
    public OnUploadComplete _uploadcomplete;

    public delegate void OnDownloadUrlComplete(string url);
    public OnDownloadUrlComplete _downloadUrlComplete;

    public void UploadFile(string childpathString, byte[] _data)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        StorageReference storageRef = storage.RootReference;

        // Create a reference to the file you want to upload
        StorageReference childRef = storageRef.Child(childpathString);// ("profileImages/"+auth.CurrentUser.UserId+".jpg");

        // Upload the file to the path "images/rivers.jpg"
        childRef.PutBytesAsync(_data)
            .ContinueWith((Task<StorageMetadata> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
            // Uh-oh, an error occurred!
        }
                else
                {
            // Metadata contains file metadata such as size, content-type, and download URL.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                    //Debug.Log("md5 hash = " + "gs://konnekt4march.appspot.com"+profileImagesRef.Path);
                    _uploadcomplete?.Invoke(childRef.Path);
                }
            });
    }

    public void DownloadFileURL(string url)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        // Create a reference from a Google Cloud Storage URI
        StorageReference gsReference =
            storage.GetReferenceFromUrl("gs://konnekt4march.appspot.com" + url);

        // Fetch the download URL
        gsReference.GetDownloadUrlAsync().ContinueWith(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                _downloadUrlComplete?.Invoke(task.Result.ToString());
                // ... now download the file via WWW or UnityWebRequest.
            }
        });
    }



    #endregion

}
