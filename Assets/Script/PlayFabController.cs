using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using PlayFab.Json;
using System.Linq;
public class PlayFabController : MonoBehaviour {

    public string PlayFabId;
    [SerializeField] GetPlayerCombinedInfoRequestParams InfoRequestParams;

    public string PlayerName;
    public List<CharacterResult> Characters;
    //public List<ItemData> ItemDatas;

    [SerializeField] InputField inputName;
    [SerializeField] Button btn;
    [SerializeField] Button questBtn;

    [SerializeField] List<ItemInstance> userInventry;

    [SerializeField] PlayerProfileModel playerProfileModel;

    [Serializable]
    public class ItemDate {

        public string ItemId;
        public int Power;
        public string Type;
        public int Gold;
    }

    public List<ItemDate> itemDateList = new List<ItemDate>();
    public string quests;
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
        //PlayFabAuthService.Instance.Authenticate(Authtypes.RegisterPlayFabAccount);
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
        yield return new WaitForSeconds(3.0f);
        //UpdateUserdata();
        //UpdateCharacterDate();
        GetUserData();

        btn.onClick.AddListener(InitPlayer);

    }

    private void Update() {
        InputValueChanged();
    }


    private void PlayFabLogin_OnLoginSuccess(LoginResult result) {

        if (result.NewlyCreated)
            Debug.Log("新規");
        else
            Debug.Log("既存ユーザー");


        PlayerName = result.InfoResultPayload.UserData["Name"].Value;
        Characters = result.InfoResultPayload.CharacterList;

        //itemDateList = JsonHelper.ListFromJson<ItemDate>(result.InfoResultPayload.TitleData["ItemDate"]);

        bool facebookAuth = playerProfileModel.LinkedAccounts.Any(x => x.Platform == PlayFab.ClientModels.LoginIdentityProvider.Facebook);
        Debug.Log("facebookAuth" + facebookAuth);
        Debug.Log("Login Success!");

        // タイトルデータの取得
        itemDateList = PlayFabSimpleJson.DeserializeObject<List<ItemDate>>(result.InfoResultPayload.TitleData["ItemDate"]);

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

    #region プレイヤー表示名の更新
    private void UpdateUserTitleDisplayName() {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = inputName.text
        }, result => {
            Debug.Log("PlayerName:" + result.DisplayName);

        }, error => Debug.LogError(error.GenerateErrorReport()));

    }
    #endregion

    #region プレイヤーの初期化
    void InitPlayer() {
        var request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                {"Exp","0" },
                {"Rank","1" }
            }
        };

        //コールバックを持つメソッド
        PlayFabClientAPI.UpdateUserData(request
            //成功時
            , result => {
                Debug.Log("プレイヤーの初期化完了");
                UpdateUserTitleDisplayName();
                //失敗時
            }, eroor => Debug.LogError(eroor.GenerateErrorReport()));
    }
    #endregion

    public void InputValueChanged() {
        btn.interactable = IsValidName();
    }

    bool IsValidName() {
        return !string.IsNullOrWhiteSpace(inputName.text)
            && 3 <= inputName.text.Length
            && inputName.text.Length <= 10;
    }


    //void UpdateCharacterDate() {

    //    CharacterResult characterResult = new CharacterResult();
    //    characterResult.CharacterId = "0";
    //    characterResult.CharacterName = "dragon";
    //    characterResult.CharacterType = "Type";
    //    var changes = new Dictionary<string, string> {
    //        { "0",JsonHelper.ClassToJson(characterResult) }
    //    };

    //    PlayFabClientAPI.UpdateCharacterData(new UpdateCharacterDataRequest {
    //        Data = changes
    //    }, result => {
    //        Debug.Log("Update UpdateCharacterDate Success!!");
    //    }, error => {
    //        Debug.Log(error.GenerateErrorReport());
    //    });
    //}

    void GetUserData() {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
        }, result => {
            //UserQuestDatas = JsonHelper.ListFromJson<UserQuestData>(result.Data["Quests"].Value);//PlayFabSimpleJson.DeserializeObject<List<UserQuestDatas>>(result.Data["Quests"].Value);
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    void GetQuestsData() {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest() {

        }, result => {
            
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


    private void GetUserInventry() {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest {
        }, (result) =>
        {
            userInventry = result.Inventory;
            Debug.Log("GetUserInventory Success!!");
        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }


    
}