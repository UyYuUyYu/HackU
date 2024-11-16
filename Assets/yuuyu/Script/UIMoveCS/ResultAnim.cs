using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnim : MonoBehaviour
{
    public Text scoreText1;
    public Text scoreText2;
    public Image rankImage;
    public int finalScore1 = 20;
    public int finalScore2 = 15;
    public float scoreCountDuration = 2f; //スコアカウントアップ時間
    public float rankEffectDuration = 1f; //ランク演出時間
    private Vector3 initialScale; //ランク初期スケール
    public Color glowColor = Color.yellow; //ランク発光色

    void SetText()
    {
        scoreText1.text = "0";
        scoreText2.text = "0";
        initialScale = rankImage.transform.localScale; //初期スケール保存
        rankImage.gameObject.SetActive(false); //ランク非表示

        //カウントアップDOTween
        DOTween.To(() => 0, x => UpdateScoreTexts(x), Mathf.Max(finalScore1, finalScore2), scoreCountDuration)
            .OnComplete(() => ShowRankImage()); //カウントアップ終了後にランク表示
    }

    // スコア
    void UpdateScoreTexts(int score)
    {
        scoreText1.text = Mathf.Min(score, finalScore1).ToString();
        scoreText2.text = Mathf.Min(score, finalScore2).ToString();
    }
    //ランク演出
    void ShowRankImage()
    {
        rankImage.gameObject.SetActive(true); //ランクを表示
        rankImage.color = Color.white; //初期色

        Sequence rankSequence = DOTween.Sequence();

        //拡大演出
        rankImage.transform.localScale = initialScale * 0.5f;
        rankSequence.Append(rankImage.transform.DOScale(initialScale * 1.5f, rankEffectDuration / 2).SetEase(Ease.OutBack));
        rankSequence.Append(rankImage.transform.DOScale(initialScale, rankEffectDuration / 2).SetEase(Ease.OutBounce));

        //発光演出
        rankSequence.Join(rankImage.DOColor(glowColor, rankEffectDuration / 2).SetLoops(2, LoopType.Yoyo));

        rankSequence.Play();
    }
    
    public void InputScore12(int _score1,int _score2)
    {
        finalScore1=_score1;
        finalScore2=_score2;
        SetText();
    }
}
