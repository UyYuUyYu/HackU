using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class ResultManager : MonoBehaviourPun
{
    private string resurtMyName,resurtFriendName;
    [SerializeField] Text resurtMyNameText,resurtFriendNameText;
    // Start is called before the first frame update
    void SetName()
    {
        SetFriendName();
        SetNameText();

    }
    void SetNameText()
    {
        resurtMyNameText.text=resurtMyName;
        resurtFriendNameText.text=resurtFriendName;
    }

    void SetFriendName()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            resurtFriendName = PhotonNetwork.PlayerList[1].NickName;
        }
        else
        {
            resurtFriendName= PhotonNetwork.PlayerList[0].NickName;
        }
    }



}
