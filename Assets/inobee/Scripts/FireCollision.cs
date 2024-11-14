using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCollision : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {
        // トリガーされたパーティクルを取得
        int numInside = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);

        for (int i = 0; i < numInside; i++)
        {
            // パーティクルのコライダーを取得
            ParticleSystem.Particle particle = insideParticles[i];
            Collider[] colliders = Physics.OverlapSphere(particle.position, 0.1f);

            foreach (var collider in colliders)
            {
                // タグで判定
                if (collider.CompareTag("Enemy"))
                {
                    Debug.Log("hit");

                    // Enemyスクリプトを取得してHPを減らす
                    var enemyScript = collider.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.ReduceHp(3);
                    }
                }
            }
        }

        // 更新されたパーティクルを反映
        particleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, insideParticles);
    }
}
