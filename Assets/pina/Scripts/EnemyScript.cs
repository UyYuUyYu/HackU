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
    private float moveDuration = 5f;

    private void Start()
{
    if (centerPosition != null)
    {
        // 現在のZ座標を保持します
        float fixedZ = transform.position.z;

        // centerPositionのX, Y座標に固定したZ座標を設定
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, fixedZ);

        // DoTweenでZ座標を固定しながら移動させます
        transform.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.Linear); // 直線的な動き
    }
    else
    {
        Debug.LogError("Center position is not set.");
    }
}


}
