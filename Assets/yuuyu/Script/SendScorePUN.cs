using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
   
    public void SendScore(int _myScore)
    {
        print("スコア送った");
        myScore=_myScore;
        photonView.RPC("SendFriendName", RpcTarget.Others, _myScore);
    }


    [PunRPC]
    private void SendFriendName(int _score)
    {
        print("スコア送られた"+_score);
        friendScore=_score;
        SceneManager.LoadScene("Result");
    }
}
