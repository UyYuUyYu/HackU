using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGageController : MonoBehaviour
{
    [SerializeField] private GameObject hpBar;
    public int max=100;
    public float current;
    [ContextMenu("HP")]
    public void HPP()
    {
         hpBar.GetComponent<Image>().fillAmount = current / max;
    }
    public void HPSet (float current) {
        //ImageというコンポーネントのfillAmountを取得して操作する
        hpBar.GetComponent<Image>().fillAmount = current / max;
    }

}