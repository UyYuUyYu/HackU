using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class NameInputPUN : MonoBehaviourPunCallbacks
{
    
    [SerializeField] private Text inputFieldText;
    public static string myName;
    // Start is called before the first frame update
    void Start()
    {
        myName = "Player";
    }
   
    [ContextMenu("NameSet")]
    public void StartButton()
    {
        PhotonNetwork.NickName= inputFieldText.text;
        myName = inputFieldText.text;
        print(PhotonNetwork.NickName);
        
    }

 
    
}
