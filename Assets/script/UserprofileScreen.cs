using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserprofileScreen : MonoBehaviour
{
    public Image _userProfilePic;
    public Text userProfileName;
    public Text userAge;
    public Text userGender;

    // Use this for initialization
    void Start ()
    {
        userProfileName.text = AppHandler._instance._myData.UserName;
        userAge.text = AppHandler._instance._myData.Age;
        userGender.text = AppHandler._instance._myData._gender;

        if (!string.IsNullOrEmpty(AppHandler._instance._myData.ProfileImagePath))
        {
            AppHandler._instance._downloadUrlComplete += DownloadComplete;
            AppHandler._instance.DownloadFileURL(AppHandler._instance._myData.ProfileImagePath);
            //StartCoroutine(LoadImageFromThePath(AppHandler._instance._myData.ProfileImagePath));
        }
    }

	public void OnLogoutClick()
	{
        AppHandler._instance.OnLogoutClick();
        AppHandler._instance.ShowScene("Login");
	}

    public void OnEditProfileClick()
    {
        AppHandler._instance.ShowScene("Newuserform");
    }

    public void DownloadComplete(string url)
    {
        Debug.Log("DownloadComplete done  " + url);
        Debug.Log("DownloadComplete done222");
        AppHandler._instance._myData.ProfileImagePath = url;
        AppHandler._instance._downloadUrlComplete -= DownloadComplete;
        StartCoroutine(LoadImageFromThePath(url));
    }

    IEnumerator LoadImageFromThePath(string path)
    {

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
                // Save it into the Image UI's sprite
                _userProfilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                _userProfilePic.preserveAspect = true;
            }
        }
    }
}
