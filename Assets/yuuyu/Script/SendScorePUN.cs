using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class SendScorePUN : MonoBehaviourPun
{
    public static int myScore;
    public static int friendScore;

    bool isChangeScore;
    // Start is called before the first frame update
    void Start()
    {
        isChangeScore=false;
        myScore=0;
    }
   
    public void SendScore()
    {
        print("スコア送った");
         photonView.RPC("SendFriendName", RpcTarget.Others, myScore);
    }


    [PunRPC]
    private void SendFriendName(int _score)
    {
        print("スコア送られた");
        friendScore=_score;
    }
}
