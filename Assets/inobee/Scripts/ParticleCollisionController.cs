using System.Collections;
using UnityEngine;

public class ParticleCollisionController : MonoBehaviour
{
    public ParticleSystem particleSystem; // 操作するParticleSystem
    public float delayBeforeCollision = 12.0f; // 当たり判定を有効にするまでの遅延時間

    void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        // 初期状態でCollision Moduleを無効化
        var collision = particleSystem.collision;
        collision.enabled = false;

        // 一定時間後にCollisionを有効化
        StartCoroutine(EnableParticleCollisionAfterDelay());
    }

    private IEnumerator EnableParticleCollisionAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeCollision); // 指定時間待機

        // Collision Moduleを有効化
        var collision = particleSystem.collision;
        collision.enabled = true;

        Debug.Log("Particle collision enabled.");
    }
}