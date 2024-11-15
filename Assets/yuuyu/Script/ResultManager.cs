using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class ResultManager : MonoBehaviourPun
{
    [SerializeField] private int row,hight,mid;
    [SerializeField] private GameObject rankS,rankA,rankB,rankC;
    private string resurtMyName,resurtFriendName;
    [SerializeField] Text resurtMyNameText,resurtFriendNameText,resultMyScore,resurtFriendScore;
    // Start is called before the first frame update
    [ContextMenu("Set")]
    void SetName()
    {
        SetFriendName();
        SetNameText();

    }
    void SetNameText()
    {
        resurtMyName=PhotonNetwork.NickName;
        resurtMyNameText.text=resurtMyName;
        resurtFriendNameText.text=resurtFriendName;
    }

    void SetFriendName()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("名前出したい");
            resurtFriendName = PhotonNetwork.PlayerList[1].NickName;
        }
        else
        {
            resurtFriendName= PhotonNetwork.PlayerList[0].NickName;
        }
    }

    void ScoreHantei()
    {
        int allSocre;
        allSocre=SendScorePUN.myScore+SendScorePUN.friendScore;
        if(allSocre<row)
        {
            rankC.SetActive(true);
            print("c");
        }
        else if((allSocre>row)&&(allSocre<mid))
        {
            rankB.SetActive(true);
            print("b");
        }
        else if((allSocre>mid)&&(allSocre<hight))
        {
            rankA.SetActive(true);
            print("a");
        }
        else if(allSocre>hight)
        {
            rankS.SetActive(true);
            print("s");
        }
    }



}
