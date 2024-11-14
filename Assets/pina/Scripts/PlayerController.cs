using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private PlayerHealth playerHealth;

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            playerHealth.Damage(1);
            other.gameObject.GetComponent<Enemy>().Die();
        }


    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Ball"))
        {
            playerHealth.Damage(1);
            Destroy(other.gameObject);
        }
    }
}
