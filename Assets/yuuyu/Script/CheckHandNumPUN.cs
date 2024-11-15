using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class CheckHandNumPUN : MonoBehaviourPun
{
     public int myHundNum,friendHundNum;

     [SerializeField] private GameObject matchEffect;

     //相手に自分の手の番号を伝える
     public void SendNumber(int _hundNum)
     {
          myHundNum=_hundNum;
          photonView.RPC("SendHundNum", RpcTarget.Others, myHundNum);
     }

     [PunRPC]
     private void SendHundNum(int _friendNum)
     {
          friendHundNum=_friendNum;
          if(myHundNum==friendHundNum)
          {
               print("マッチしたよ");
               Instantiate(matchEffect,new Vector3(0,-5,0),Quaternion.identity);
          }
          
          print("自分の"+myHundNum);
          print("相手の"+friendHundNum);
     }

    
}
