using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particletrigger : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        // パーティクルシステムを取得
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        // トリガーされたパーティクルの数を取得
        List<ParticleSystem.Particle> triggeredParticles = new List<ParticleSystem.Particle>();
        int numParticles = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, triggeredParticles);

        // 各パーティクルごとに処理を実行
        for (int i = 0; i < numParticles; i++)
        {
            // パーティクルの現在の位置を取得
            Vector3 particlePosition = triggeredParticles[i].position;

            // コライダー内のオブジェクトを取得するためにOverlapSphereを使用
            Collider[] hitColliders = Physics.OverlapSphere(particlePosition, 0.5f); // 半径0.5で範囲検索
            foreach (var collider in hitColliders)
            {
                // "Enemy"タグを持つオブジェクトか判定
                if (collider.CompareTag("Enemy"))
                {
                    Debug.Log("hit");

                    // Enemyコンポーネントを取得してダメージを与える
                    var enemyScript = collider.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.ReduceHp(3); // 必要最小限の処理を呼び出す
                    }
                }
            }
        }

        // 更新したパーティクル情報をパーティクルシステムに反映
        particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, triggeredParticles);
    }
}
