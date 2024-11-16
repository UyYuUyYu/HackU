using DG.Tweening;
using UnityEngine;

public class CutInAnim : MonoBehaviour
{
    public RectTransform imageRect;
    public float slideDuration = 1f; //移動時間
    public float offScreenX = 1500f;
    public float fadeOutX = -1500f; // 画面外の左側のX座標
    public float holdDuration = 1f; //待機時間

    void Awake()
    {
        //初期位置設定
        imageRect.anchoredPosition = new Vector2(offScreenX, imageRect.anchoredPosition.y);

        //シーケンス作成
        Sequence sequence = DOTween.Sequence();

        //スライドイン
        sequence.Append(imageRect.DOAnchorPosX(0, slideDuration).SetEase(Ease.InOutCubic));

        //待機
        sequence.AppendInterval(holdDuration);

        //スライドアウト
        sequence.Append(imageRect.DOAnchorPosX(fadeOutX, slideDuration).SetEase(Ease.InOutCubic));

        //シーケンス再生
        sequence.Play();
    }
}
