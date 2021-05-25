using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;

public class PlayFabController : MonoBehaviour {

    public string PlayFabId;

    void OnEnable() {
        PlayFabAuthService.OnLoginSuccess += PlayFabLogin_OnLoginSuccess;
    }

    IEnumerator Start() {
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
        yield return new WaitForSeconds(3.0f);
        GetUserData();
    }

    private void PlayFabLogin_OnLoginSuccess(LoginResult result) {
        Debug.Log("Login Success!");

    }
    private void OnDisable() {
        PlayFabAuthService.OnLoginSuccess -= PlayFabLogin_OnLoginSuccess;
    }

    void GetUserData() {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            PlayFabId = PlayFabId
        }, result => {
            Debug.Log(result.Data["Name"].Value);
            Debug.Log("ログイン成功");
        }, error => {
            Debug.Log(error.GenerateErrorReport());
            Debug.Log("ログイン失敗");

        });
    }
}