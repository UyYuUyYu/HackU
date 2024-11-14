 using Photon.Pun;
 using Photon.Realtime;
 using UnityEngine;
 using System.Collections;
 using UnityEngine.SceneManagement;
 using System;

public class PunMain : MonoBehaviourPunCallbacks
{
    public string playerName{get;set;}
    public string friendName{get;set;}

    private MatchngDisplayScript matchngDisplayScript;

    void Start()
    {

        if(SceneManager.GetActiveScene().name=="MatchWait")
        {
            matchngDisplayScript=this.GetComponent<MatchngDisplayScript>();
            ConentPUNServer();
        } 
        
    }
    

    #region Method
    [ContextMenu("StartPUN")]
    public void ConentPUNServer() 
    {
        print("aa");
        SetNickName();
        PhotonNetwork.ConnectUsingSettings();
    }
    public void DisconectPUNServer()
    {
        PhotonNetwork.Disconnect(); 
        print(ScreenShot.imageData);
        Array.Clear(ScreenShot.imageData, 0, ScreenShot.imageData.Length);
        SendScorePUN.myScore=0;
        NameInputPUN.myName="Player";
        SceneManager.LoadScene("Start");
    }

    public void Restart()
    {
        SendScorePUN.myScore=0;
        SceneManager.LoadScene("MatchWait");
    }

    public void SetNickName()
    {
        PhotonNetwork.NickName = playerName;
    }
    #endregion

  
    
    #region PUNCallBucks
    public override void OnConnectedToMaster() 
    {
        // ランダムなルームに参加する
        PhotonNetwork.JoinRandomRoom();
    }

    // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
    public override void OnJoinRandomFailed(short returnCode, string message) 
    {
        // ルームの参加人数を2人に設定する
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // ルームへの参加が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom() 
    {
        print("room入った");
        if (PhotonNetwork.IsMasterClient)
        {
            print("Master");
            matchngDisplayScript.InportImageAndName();
        }
        else
        {
            matchngDisplayScript.InportImageAndName();
            matchngDisplayScript.SendFriendImageAndName();
            StartCoroutine("WaitMainScene");
        }
            
    }
    // ルームに人が入ってきた時に呼ばれるコールバック
    public override void OnPlayerEnteredRoom (Player newPlayer)
    {
        print("人が来た");
        if(PhotonNetwork.IsMasterClient)
        {
            matchngDisplayScript.SendFriendImageAndName();
            StartCoroutine("WaitMainScene");
        }
    }

    // Photonのサーバーから切断された時に呼ばれるコールバック
    public override void OnDisconnected(DisconnectCause cause) 
    {
        Debug.Log($"サーバーとの接続が切断されました: {cause.ToString()}");
    }
    #endregion


    private IEnumerator WaitMainScene()
    {
        print("a");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main");

    }
    
}