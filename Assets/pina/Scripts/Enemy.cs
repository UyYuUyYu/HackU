using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int hp = 100; // 初期HP
    private int maxHp; // 最大HPを保存

    [SerializeField]
    private Renderer enemyRenderer; // オブジェクトのRenderer
    private ScoreManager scoreManager;

    [SerializeField]
    private int enemyPoint;

    private void Start()
    {
        scoreManager=GameObject.Find("GameManager").GetComponent<ScoreManager>();
        // 最大HPを保存
        maxHp = hp;

        // Rendererがアタッチされていない場合、自動取得
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponent<Renderer>();
        }

        if (enemyRenderer == null)
        {
            Debug.LogError("Renderer not found on the enemy object.");
        }
    }

    public void ReduceHp(int damage)
    {
        hp -= damage; // HPを減らす
        Debug.Log($"Enemy HP: {hp}");

        // HPが0未満にならないように制限
        hp = Mathf.Max(hp, 0);

        // HPに応じて色を変化させる
        UpdateColor();

        if (hp <= 0)
        {
            Die(); // HPが0になったら死亡処理
        }
    }

    private void UpdateColor()
    {
        if (enemyRenderer != null)
        {
            // HP割合を計算
            float hpRatio = (float)hp / maxHp;

            // 色を変化 (緑 -> 赤)
            Color newColor = Color.Lerp(Color.red, Color.green, hpRatio);
            enemyRenderer.material.color = newColor;
        }
    }

    public void Die()
    {
        scoreManager.ScoreUp(enemyPoint);
        // 死亡時にオブジェクトを削除
        Destroy(gameObject);
    }
}