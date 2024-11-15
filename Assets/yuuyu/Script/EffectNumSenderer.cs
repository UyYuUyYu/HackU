using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectNumSenderer : MonoBehaviour
{
    [SerializeField] private int effectNum;
    private CheckHandNumPUN  checkHandNumPUN;
    void Awake()
    {
        checkHandNumPUN=GameObject.Find("PunManager").GetComponent<CheckHandNumPUN>();
        checkHandNumPUN.SendNumber(effectNum);
        print("effectAwake");
    }

  
}
