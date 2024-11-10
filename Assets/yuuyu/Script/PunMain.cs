 using Photon.Pun;
 using Photon.Realtime;
 using UnityEngine;

public class PunMain : MonoBehaviourPunCallbacks
{
    public string playerName{get;set;}
    public string enemyName{get;set;}

    void Start()
    {
        ConentPUNServer();
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

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom() 
    {
            
    }

    // Photonのサーバーから切断された時に呼ばれるコールバック
    public override void OnDisconnected(DisconnectCause cause) 
    {
        Debug.Log($"サーバーとの接続が切断されました: {cause.ToString()}");
    }
    #endregion


    
}