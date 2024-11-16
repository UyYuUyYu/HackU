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
    private bool isSend,isSended;

    bool isChangeScore;
    // Start is called before the first frame update
    void Start()
    {
        isSend=false;
        isSended=false;
        isChangeScore=false;
        myScore=0;
    }
    void Update()
    {

        if(isSend&&isSended)
        {
            SceneManager.LoadScene("Result");
        }
    }
   
    public void SendScore(int _myScore)
    {
        print("スコア送った");
        myScore=_myScore;
        isSend=true;
        photonView.RPC("SendFriendName", RpcTarget.Others, _myScore);
    }


    [PunRPC]
    private void SendFriendName(int _score)
    {
        print("スコア送られた"+_score);
        friendScore=_score;
        isSended=true;
        
    }
}
