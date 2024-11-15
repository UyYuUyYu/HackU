using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHP = 100; // 最大HPはInspectorで変更してください
    private int currentHP;

    [SerializeField]
    private Text hpText;
    private HPGageController hPGageController;

    private void Start()
    {
        hPGageController=this.GetComponent<HPGageController>();
        currentHP = maxHP;
        UpdateHPText();
        Debug.Log("Player HP: " + currentHP);
    }

    public void Damage(int amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        UpdateHPText();

        Camera.main.transform.DOShakePosition(1f,0.5f);
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHPText();
        Debug.Log("Player HP after Heal: " + currentHP);
    }

    private void UpdateHPText()
    {
        hPGageController.HPSet(currentHP);
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHP;
        }
    }
}

