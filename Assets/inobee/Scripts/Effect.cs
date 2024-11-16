//effect.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    // エフェクトのプレハブを指定するための変数
    public GameObject effectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // 座標(0, 0, 0)にエフェクトをインスタンス生成
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, new Vector3(0,-5,0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Effect Prefab が設定されていません！");
        }
    }
}
