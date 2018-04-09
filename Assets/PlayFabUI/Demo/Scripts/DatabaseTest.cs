using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class DatabaseTest : MonoBehaviour {
    //Settings for what data to get from playfab on login.
    //public GetPlayerCombinedInfoRequestParams InfoRequestParams;

    //Reference to our Authentication service
    private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;
    public Text statusText;

    private void Awake()
    {
        PlayFabSettings.TitleId = "4858"; //your title id goes here.
        _AuthService.Authenticate(Authtypes.Silent);
        Debug.Log(_AuthService.Username);
    }

    // Use this for initialization
    void Start () {
		
	}
   public void SetUserData()
    {

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"Ancestor", "Soulemane"},
            {"Success", "Souley jr"}
        }
        },
        result => AddStatusText("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void DataResults(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("Ancestor") || !result.Data.ContainsKey("Success"))
            AddStatusText("Result: Unsuccessful");
        else
            AddStatusText("Ancestor: " + result.Data["Ancestor"].Value);
            AddStatusText("Successor: " + result.Data["Success"].Value);
    }

    public void Test()
    {
        Debug.Log("TEST");
    }


   public void GetUserData()
    {

        GetUserDataRequest udr = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(udr,DataResults,OnPlayFabError);
        AddStatusText("Accessing Data...");
    }
    // Update is called once per frame
    void Update () {
		
	}
    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.ErrorMessage);
    }
    private List<string> messages = new List<string>();

    void AddStatusText(string text)
    {
        if (messages.Count == 5)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        statusText.text = txt;
    }
}
