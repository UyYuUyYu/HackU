using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectNumSenderer : MonoBehaviour
{
    [SerializeField] private int effectNum;
    private CheckHandNumPUN  checkHandNumPUN;
    void Awake()
    {
        if(effectNum==4)
        {
            GameObject.Find("GameManager").GetComponent<PlayerHealth>().Heal(2);
        }
        checkHandNumPUN=GameObject.Find("PunManager").GetComponent<CheckHandNumPUN>();
        checkHandNumPUN.SendNumber(effectNum);
        
    }

  
}
