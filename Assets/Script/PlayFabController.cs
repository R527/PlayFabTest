using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayFabController : MonoBehaviour {

    public string PlayFabId;
    [SerializeField] GetPlayerCombinedInfoRequestParams InfoRequestParams;

    public string PlayerName;
    public List<CharacterResult> Characters;
    //public List<ItemData> ItemDatas;

    [Serializable]
    public class UserQuestData {
        public int id;
        public int Score;
        public UserQuestData (int id,int Score) {
            this.id = id;
            this.Score = Score;
        }
    }
    public List<UserQuestData> UserQuestDatas = new List<UserQuestData>();

    void OnEnable() {
        PlayFabAuthService.OnLoginSuccess += PlayFabLogin_OnLoginSuccess;
    }

    IEnumerator Start() {

        InfoRequestParams.GetUserData = true; // プレイヤーデータを取得する
        InfoRequestParams.GetTitleData = true; // タイトルデータを取得する
        InfoRequestParams.GetCharacterList = true; // キャラクターの一覧を取得する

        PlayFabAuthService.Instance.InfoRequestParams = InfoRequestParams;
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
        yield return new WaitForSeconds(3.0f);
        UpdateUserdata();
        //GetUserData();
    }

    private void PlayFabLogin_OnLoginSuccess(LoginResult result) {
        PlayerName = result.InfoResultPayload.UserData["Name"].Value;
        Characters = result.InfoResultPayload.CharacterList;

        Debug.Log("Login Success!");

    }
    private void OnDisable() {
        PlayFabAuthService.OnLoginSuccess -= PlayFabLogin_OnLoginSuccess;
    }

    //void GetUserData() {
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
    //        PlayFabId = PlayFabId
    //    }, result => {
    //        Debug.Log(result.Data["Name"].Value);
    //        Debug.Log("ログイン成功");
    //    }, error => {
    //        Debug.Log(error.GenerateErrorReport());
    //        Debug.Log("ログイン失敗");

    //    });
    //}

    

    void GetUserData() {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
        }, result => {
            UserQuestDatas = JsonHelper.ListFromJson<UserQuestData>(result.Data["Quests"].Value);//PlayFabSimpleJson.DeserializeObject<List<UserQuestDatas>>(result.Data["Quests"].Value);
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    void UpdateUserdata() {
        UserQuestData user = new UserQuestData(3, 500);

        var changes = new Dictionary<string, string> { 
            { "Quests", JsonHelper.ClassToJson(user) }
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest {
            Data = changes
        }, result => {
            Debug.Log("Update UserQuest Success!!");
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }
    
}