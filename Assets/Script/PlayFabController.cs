﻿using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabController : MonoBehaviour {
    void Start() {
        //PlayFabClientAPI.LoginWithCustomID(
        //    new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true }
        //, result => Debug.Log("おめでとうございます！ログイン成功です！")
        //, error => Debug.Log("ログイン失敗...(´；ω；｀)"));

        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
    }
}