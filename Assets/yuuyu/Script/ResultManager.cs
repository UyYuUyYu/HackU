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
    void Start()
    {

        SetNameText();
        ScoreHantei();

    }
    void SetNameText()
    {
        resurtMyNameText.text=NameInputPUN.myName;
        resurtFriendNameText.text=NameInputPUN.friendName;
    }

   

    void ScoreHantei()
    {
        this.GetComponent<ResultAnim>().InputScore12(SendScorePUN.myScore,SendScorePUN.friendScore);
        //resultMyScore.text=SendScorePUN.myScore.ToString();
        //resurtFriendScore.text=SendScorePUN.friendScore.ToString();

        int allSocre;
        allSocre=SendScorePUN.myScore+SendScorePUN.friendScore;
        print(allSocre);
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
