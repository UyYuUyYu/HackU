using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingAnim : MonoBehaviour
{
    private const float DURATION = 1f;

    void Start()
    {
        Image[] circles = GetComponentsInChildren<Image>();
        var size = (int)Mathf.Sqrt(circles.Length);
        for (var i = 0; i < circles.Length; i++)
        {
            var x = i % size - (float)(size - 1) / 2;
            var y = (int)(i / size) - (float)(size - 1) / 2;
            circles[i].rectTransform.anchoredPosition += new Vector2(x, y) * 50f;
            circles[i].DOFade(0f, DURATION / 2).SetLoops(-1, LoopType.Yoyo).SetDelay(Random.Range(0f, DURATION));
        }
    }
}
