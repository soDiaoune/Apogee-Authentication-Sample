using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ExampleView : MonoBehaviour {
    public GameObject LoginSuccessObject;
    public GameObject LoginFailedObject;
    public Text LoginFailedText;
    private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;

    private string DeviceId;

    // Use this for initialization
    void Start()
    {
        GetAndroidDeviceInfo();
        SetupGooglePlayGames();
        LoginFailedText.text = "";
    }

    public void SignIn()
    {

        Debug.Log("Social Local User Auth.");
        Social.localUser.Authenticate((success) =>
        {
            if (success)
            {
                Debug.Log("Auth was a success!");
                try
                {
                    var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    _AuthService.AuthTicket = serverAuthCode;
                    _AuthService.Authenticate(Authtypes.Google);

                    /*
                    Debug.Log("Get Id Token");
                    PlayGamesPlatform.Instance.GetIdToken((idToken) =>
                    {
                        Debug.Log("Login to playfab with google.");
                        var loginRequest = new PlayFab.ClientModels.LoginWithGoogleAccountRequest()
                        {
                            TitleId = PlayFab.PlayFabSettings.TitleId,
                            CreateAccount = true,
                            ServerAuthCode = idToken
                        };
                        PlayFab.PlayFabClientAPI.LoginWithGoogleAccount(loginRequest, (result) =>
                        {
                            LoginFailedObject.SetActive(false);
                            LoginSuccessObject.SetActive(true);
                        }, OnPlayFabError);
                    });
                    */
                }
                catch(Exception e)
                {
                    Debug.Log("Error Getting Id Token: " + e.Message);
                }
            }
            else
            {
#if !UNITY_EDITOR
                LoginFailedText.text = "Authentication Failed: Make sure you add your account to testers. ";
#else
                LoginFailedText.text = "Authentication Failed: You can't use Google Auth in Editor - http://goo.gl/pHjc3N";
#endif
                LoginFailedObject.SetActive(true);
            }
        });

    }

    private void OnPlayFabError(PlayFabError obj)
    {
        Debug.Log(obj.GenerateErrorReport());
        LoginFailedText.text = obj.GenerateErrorReport();
        LoginFailedObject.SetActive(true);
        LoginSuccessObject.SetActive(false);
    }

    void GetAndroidDeviceInfo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        DeviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
#endif
    }

    void SetupGooglePlayGames()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
#endif
    }

}
