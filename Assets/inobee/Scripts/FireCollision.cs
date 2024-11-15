using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        // タグで判定
        if (!other.CompareTag("Enemy")) return;
        Debug.Log("hit");

        

        // 事前にEnemyコンポーネントを取得
        var enemyScript = other.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.ReduceHp(3); // 必要最小限の処理を呼び出す
        }
    }
}