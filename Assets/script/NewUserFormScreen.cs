using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using NativeGalleryNamespace;
using UnityEngine.Networking;
#endif

public class NewUserFormScreen : MonoBehaviour
{
    public Image userProfileSprite;
	public InputField userProfileName;
	public InputField userAge;
	public Dropdown userGender;

    Texture2D _userProfilePic;

	// Use this for initialization
	void Start () {
        if (!string.IsNullOrEmpty(AppHandler._instance._myData.UserName))
        {
            userProfileName.text = AppHandler._instance._myData.UserName;
            userAge.text = AppHandler._instance._myData.Age;
            userGender.value = userGender.options.FindIndex(x => x.text.Equals(AppHandler._instance._myData._gender));

            if (!string.IsNullOrEmpty(AppHandler._instance._myData.ProfileImagePath))
            {
                StartCoroutine(LoadImageFromThePath(AppHandler._instance._myData.ProfileImagePath));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SelectPicFromDevice()
	{
        NativeGallery.GetImageFromGallery(LoadImageFromPath);
	}

    void LoadImageFromPath(string path)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path,-1,false);
        _userProfilePic = texture;
        // Save it into the Image UI's sprite
        userProfileSprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        userProfileSprite.preserveAspect = true;
        //StartCoroutine(LoadImageFromThePath("file://" + path));
    }

    IEnumerator LoadImageFromThePath(string path)
    {
        AppHandler._instance._downloadUrlComplete += DownloadComplete;
        //AppHandler._instance.UploadFile(path);
        Debug.Log("www PATH" + path);
        //WWW www = new WWW(path);//(Application.persistentDataPath + "/" + filename);
        ////Waitng for operation to complete.is this correct?
        //while (!www.isDone)
        //{
        //    Debug.Log("Not done");
        //    yield return null;
        //}

        //if (www == null)
        //{
        //    Debug.Log("www is null");
        //}
        //else
        //{
        //    Debug.Log("www is done");
        //    userProfilePic.texture = www.texture;
        //}

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(path))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log($"{webRequest.error}: {webRequest.downloadHandler.text}");
            }
            else
            {
                // Get the texture out using a helper downloadhandler
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                _userProfilePic = texture;
                // Save it into the Image UI's sprite
                userProfileSprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                userProfileSprite.preserveAspect = true;
            }
        }
    }

    public void UploadComplete(string url)
    {
        AppHandler._instance._myData.ProfileImagePath = url;
        //AppHandler._instance.DownloadFileURL(url);
        AppHandler._instance._uploadcomplete -= UploadComplete;
        _uploadDone = true;
    }

    public void DownloadComplete(string url)
    {
        _uploadDone = true;
        Debug.Log("DownloadComplete done  "+url);
        Debug.Log("DownloadComplete done222");
        AppHandler._instance._myData.ProfileImagePath = url;
        AppHandler._instance._downloadUrlComplete -= DownloadComplete;
    }

    //string path = "";

    //public IEnumerator DownloadImage()
    //{
    //    Debug.Log("DownloadImage done");
    //    WWW www = new WWW(path);//(Application.persistentDataPath + "/" + filename);
    //    //Waitng for operation to complete.is this correct?
    //    while (!www.isDone)
    //    {
    //        Debug.Log("Not done");
    //        yield return null;
    //    }

    //    if (www == null)
    //    {
    //        Debug.Log("www is null");
    //    }
    //    else
    //    {
    //        Debug.Log("www is done");
    //        userProfilePic.texture = www.texture;
    //    }
    //    AppHandler._instance._downloadUrlComplete -= DownloadComplete;
    //    //using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
    //    //{
    //    //    // Request and wait for the desired page.
    //    //    yield return webRequest.SendWebRequest();

    //    //    if (webRequest.isNetworkError || webRequest.isHttpError)
    //    //    {
    //    //        Debug.Log($"{webRequest.error}: {webRequest.downloadHandler.text}");
    //    //    }
    //    //    else
    //    //    {
    //    //        // Get the texture out using a helper downloadhandler
    //    //        Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
    //    //        // Save it into the Image UI's sprite
    //    //        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    //    //    }
    //    //}
    //}


    public void SaveData()
	{
        AppHandler._instance._uploadcomplete += UploadComplete;
        AppHandler._instance._downloadUrlComplete += DownloadComplete;

        if (userProfileName.text.Trim().Length > 2)
        {
            AppHandler._instance._myData.UserName = userProfileName.text.Trim();
        }
        else
        {
            return;
        }

        if (userAge.text.Trim().Length > 1)
        {
            AppHandler._instance._myData.Age = userAge.text.Trim();
        }
        else
        {
            return;
        }

        if (userGender.value > 0)
        {
            AppHandler._instance._myData._gender = userGender.options[userGender.value].text.ToString();
        }
        else
        {
            return;
        }

        if(userProfileSprite.sprite == null)
        {
            return;
        }
        else
        {
            AppHandler._instance.UploadFile("profileImages/ "+AppHandler._instance._myData._mobileNumber+".jpg", _userProfilePic.EncodeToJPG());
        }

        StartCoroutine("SaveDataCoroutine");
    }

    bool _uploadDone = false;

    IEnumerator SaveDataCoroutine()
    {
        while (!_uploadDone)
        {
            yield return null;
        }

        AppHandler._instance.SetUserData();

        if (AppHandler._instance._myData._isVerified)
        {
            AppHandler._instance.ShowScene("Userprofile");
        }
        else
        {
            AppHandler._instance.ShowScene("WaitingLounge");
        }
    }
}
