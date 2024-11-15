using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class CheckHandNumPUN : MonoBehaviourPun
{
     public int myHundNum,friendHundNum;
     public bool isMatch=false;

     [SerializeField] private GameObject matchEffect;

     void Update()
     {
          if(myHundNum==friendHundNum)
          {
               isMatch=true;
          }
          else
          {
               isMatch=false;
          }

          if(isMatch)
          {
               matchEffect.SetActive(true);
               isMatch=false;
          }

     }


     //相手に自分の手の番号を伝える
     public void SendNumber(int _hundNum)
     {
          print("SendNumInput");
          myHundNum=_hundNum;
          photonView.RPC("SendHundNum", RpcTarget.Others, myHundNum);
     }

     [PunRPC]
     private void SendHundNum(int _myHundNum)
     {
          friendHundNum=_myHundNum;
     }

    
}
