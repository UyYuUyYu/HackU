using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
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
        PhotonNetwork.NickName= myName;
        print(PhotonNetwork.NickName);
        SceneManager.LoadScene("MatchWait");
        
    }
    public void OnEndEdit()
    {
        print("入力完了");
        myName = inputFieldText.text;
    }

 
    
}
