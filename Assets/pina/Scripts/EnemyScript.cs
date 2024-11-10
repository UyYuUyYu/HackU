using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


//Enemy Movements
public class EnemyScript : MonoBehaviour
{

    [SerializeField]
    private Transform centerPosition;

    [SerializeField]
    private float moveDuration = 2f;

    private void Start()
    {
        if (centerPosition != null)
        {
            // DoTweenで敵を中心に向かって移動させます
            transform.DOMove(centerPosition.position, moveDuration)
                .SetEase(Ease.Linear); // 直線的な動き
        }
        else
        {
            Debug.LogError("Center position is not set.");
        }
    }


}
