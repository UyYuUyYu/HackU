using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMovementScript : MonoBehaviour
{
    public enum EnemyMovementType
    {
        Linear,
        InOutSine,
        InQuad,
        InExpo,
        AttackBot,
        Tank
    }

    [SerializeField]
    private Transform centerPosition;


    [SerializeField]
    private float moveDuration = 5f;
    [SerializeField]
    private EnemyMovementType movementType;

    [SerializeField]
    public GameObject ballPrefab; // Ballオブジェクトのプレハブ

    [SerializeField]
    public GameObject arrowPrefab; // Arrowオブジェクトのプレハブ


    private void Start()
    {
        if (centerPosition == null)
        {
            Debug.LogError("Center position is not set.");
            return;
        }

        switch (movementType)
        {
            case EnemyMovementType.Linear:
                MoveLinear();
                break;
            case EnemyMovementType.InOutSine:
                MoveInOutSine();
                break;
            case EnemyMovementType.InQuad:
                MoveInQuad();
                break;
            case EnemyMovementType.InExpo:
                MoveInExpo();
                break;
            case EnemyMovementType.AttackBot:
                MoveAttackBot();
                break;
            case EnemyMovementType.Tank:
                MoveTank();
                break;
        }
    }


    private void MoveLinear()
    {


        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);

        transform.LookAt(targetPosition);

        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.Linear);
    }

    private void MoveInOutSine()
    {
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);

        transform.LookAt(targetPosition);


        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine);
    }

    private void MoveInQuad()
    {
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);

        transform.LookAt(targetPosition);

        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InQuad);
    }

    private void MoveInExpo()
    {
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);

        transform.LookAt(targetPosition);

        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InExpo);
    }

    private void MoveAttackBot()
    {
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);
        Vector3 oneThirdPosition = (transform.position + targetPosition) * 3 / 4;

        Sequence sequence = DOTween.Sequence();

        // 1. oneThirdPositionへの移動とarrowPrefabの表示をオフにする
        sequence.Append(transform.DOMove(oneThirdPosition, moveDuration * 3 / 4).SetEase(Ease.Linear)
                        .OnStart(() => arrowPrefab.SetActive(false)));


        // 3. 回転させる
        sequence.Append(transform.DORotate(Quaternion.LookRotation(targetPosition - transform.position).eulerAngles, moveDuration / 8)
                        .SetEase(Ease.Linear));

        // 4. 回転が終わった後に点滅を開始
        sequence.AppendCallback(() =>
        {
            arrowPrefab.SetActive(true);

            arrowPrefab.GetComponent<Renderer>().material.DOColor(Color.clear, 0.2f)
                    .SetLoops(10, LoopType.Yoyo)  // 5回の点滅
                    .SetId("ArrowBlinking");
        });

        // 5. 点滅が終了したら色を戻し、最後に目標地点に移動
        sequence.AppendInterval(2f); // 点滅が完了するまでの時間を待機
        sequence.AppendCallback(() =>
        {
            DOTween.Kill("ArrowBlinking"); // 点滅アニメーションを停止
            arrowPrefab.GetComponent<Renderer>().material.color = Color.white; // 色を元に戻す
            arrowPrefab.SetActive(false); // 非表示にする
        });

        // 6. 最後に目標地点に移動
        sequence.Append(transform.DOMove(targetPosition, moveDuration / 8).SetEase(Ease.InQuad));
    }








    private void MoveTank()
    {
        Vector3 targetPosition = new Vector3(centerPosition.position.x, centerPosition.position.y, transform.position.z);
        Vector3 oneThirdPosition = (transform.position + targetPosition) * 2 / 3;

        transform.DOMove(oneThirdPosition, moveDuration * 2 / 3).SetEase(Ease.Linear)
             .OnComplete(() => 
             {
                 InvokeRepeating(nameof(ShootBall), 3f, 3f); // 3秒後から3秒間隔でShootBallを呼び出す


             });
    }

    private void ShootBall()
    {
        // Ballオブジェクトを現在の位置に生成
        GameObject ball = Instantiate(ballPrefab, transform.position + new Vector3(0,0.8f,0), transform.rotation);
        
        // Ballに前方方向の力を加えて放出
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * 20f, ForceMode.Impulse); // 必要に応じて力の強さを調整してください
        }

        // ローカル座標でのDOPunchPositionを繰り返し
        transform.DOPunchPosition(new Vector3(0.4f, 0, 0), 2f, 5, 1f)
                .SetRelative(true);
    }


}
