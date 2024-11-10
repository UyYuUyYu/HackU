using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;

public class Test : MonoBehaviour
{
    private MultiHandLandmarkListAnnotation multiHandLandmarkListAnnotation; // エフェクト管理クラスの参照

    // Start is called before the first frame update
    void Start()
    {
        // Scene内のMultiHandLandmarkListAnnotationを検索して参照を取得
        multiHandLandmarkListAnnotation = FindObjectOfType<MultiHandLandmarkListAnnotation>();

        if (multiHandLandmarkListAnnotation == null)
        {
            Debug.LogError("MultiHandLandmarkListAnnotation not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (multiHandLandmarkListAnnotation != null) // Nullチェック
        {
            if (multiHandLandmarkListAnnotation.IsEffectPlaying)
            {
                Debug.Log("今やってる");
            }
            else
            {
                Debug.Log("今やってない");
            }
        }
    }
}