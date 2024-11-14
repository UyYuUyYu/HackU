using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class CheckHandNumPUN : MonoBehaviourPun
{
     public int myHundNum,friendHundNum;
     public bool isBigEffect=false;

     void Update()
     {
          if(myHundNum==friendHundNum)
          {
               isBigEffect=true;
          }
          else
          {
               isBigEffect=false;
          }
     }


     //相手に自分の手の番号を伝える
     public void SendNumber(int _hundNum)
     {
          myHundNum=_hundNum;
          photonView.RPC("SendHundNum", RpcTarget.Others, myHundNum);
     }

     [PunRPC]
     private void SendHundNum(int _myHundNum)
     {
          friendHundNum=_myHundNum;
     }

    
}
