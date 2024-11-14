// MultiHandLandmarkListAnnotation.cs
// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
  using Color = UnityEngine.Color;
#pragma warning restore IDE0065

  public sealed class MultiHandLandmarkListAnnotation : ListAnnotation<HandLandmarkListAnnotation>
  {
    [SerializeField] private Color _leftLandmarkColor = Color.green;
    [SerializeField] private Color _rightLandmarkColor = Color.green;
    [SerializeField] private float _landmarkRadius = 15.0f;
    [SerializeField] private Color _connectionColor = Color.white;
    [SerializeField, Range(0, 1)] private float _connectionWidth = 1.0f;
    [SerializeField] private GameObject[] _landmarkPrefabs; // エフェクトの配列を管理
    [SerializeField] private Vector3 _effectPositionOffset = Vector3.zero; // エフェクト位置のオフセットをInspectorで編集可能
    private GameObject _currentEffect = null; // 現在のエフェクトを保持
    private bool _effectPlaying = false; // エフェクトの再生状態を追跡
    public bool IsEffectPlaying => _effectPlaying;
    [SerializeField] private UnityEngine.Events.UnityEvent<int> OnValueChanged;

// 数値変更時にイベントを発火
private void NotifyValueChanged()
{
    OnValueChanged?.Invoke(_currentValue);
}


    private List<GameObject> _generatedObjects = new List<GameObject>(); // 生成したオブジェクトのリスト

    private IList<ClassificationList> _handedness; // 手の形状を保存するリスト

#if UNITY_EDITOR
    private void OnValidate()
    {
      if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
      {
        ApplyLeftLandmarkColor(_leftLandmarkColor);
        ApplyRightLandmarkColor(_rightLandmarkColor);
        ApplyLandmarkRadius(_landmarkRadius);
        ApplyConnectionColor(_connectionColor);
        ApplyConnectionWidth(_connectionWidth);
      }
    }
#endif

    public void SetLeftLandmarkColor(Color leftLandmarkColor)
    {
      _leftLandmarkColor = leftLandmarkColor;
      ApplyLeftLandmarkColor(_leftLandmarkColor);
    }

    public void SetRightLandmarkColor(Color rightLandmarkColor)
    {
      _rightLandmarkColor = rightLandmarkColor;
      ApplyRightLandmarkColor(_rightLandmarkColor);
    }

    public void SetLandmarkRadius(float landmarkRadius)
    {
      _landmarkRadius = landmarkRadius;
      ApplyLandmarkRadius(_landmarkRadius);
    }

    public void SetConnectionColor(Color connectionColor)
    {
      _connectionColor = connectionColor;
      ApplyConnectionColor(_connectionColor);
    }

    public void SetConnectionWidth(float connectionWidth)
    {
      _connectionWidth = connectionWidth;
      ApplyConnectionWidth(_connectionWidth);
    }

    public void SetHandedness(IList<ClassificationList> handedness)
    {
      _handedness = handedness;
      var count = handedness == null ? 0 : handedness.Count;
      for (var i = 0; i < Mathf.Min(count, children.Count); i++)
      {
        children[i].SetHandedness(handedness[i]);
      }
      for (var i = count; i < children.Count; i++)
      {
        children[i].SetHandedness((IList<Classification>)null);
      }
    }

// エフェクト管理用の数値
private int _currentValue = -1;

// 数値に応じたエフェクトの切り替え
public void UpdateEffectByValue(int newValue)
{
    // 数値が変更された場合のみ処理
    if (_currentValue != newValue)
    {
        Debug.Log($"Value changed: {_currentValue} -> {newValue}");
        _currentValue = newValue;

        // 現在のエフェクトを削除
        if (_currentEffect != null)
        {
            Destroy(_currentEffect);
            _currentEffect = null;
        }

        // 数値に応じたエフェクト生成
        switch (_currentValue)
        {
            case 0:
                Debug.Log("No effect activated.");
                break;
            case 1:
                Debug.Log("Activating 'Open Hand' effect.");
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[0], new Vector3(0,0,-4));
                break;
            case 2:
                Debug.Log("Activating 'V Sign' effect.");
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[1], new Vector3(0,0,-4));
                break;
            default:
                Debug.Log($"Effect for value {_currentValue} is not implemented.");
                break;
        }
    }
}

// エフェクト生成ヘルパーメソッド
private GameObject GenerateEffectAtPosition(GameObject prefab, Vector3 position)
{
    if (prefab == null) return null;
    position.z = -4f;

    var effect = Instantiate(prefab);
    effect.transform.position = position + _effectPositionOffset; // オフセットを適用
    Destroy(effect, 3f); // 3秒後に自動削除
    return effect;
}



// Draw メソッドの修正版
public void Draw(IList<NormalizedLandmarkList> targets, bool visualizeZ = false)
{
    if (ActivateFor(targets))
    {
        ClearGeneratedObjects();

        for (int i = 0; i < targets.Count; i++)
        {
            var landmarkList = targets[i];
            if (IsOpenHand(landmarkList))
            {
                UpdateEffectByValue(1); // パーの場合
            }
            else if (IsVSignHand(landmarkList))
            {
                UpdateEffectByValue(2); // チョキの場合
            }
            else
            {
                UpdateEffectByValue(0); // 何も検出されない場合
            }
        }

        CallActionForAll(targets, (annotation, target) =>
        {
            if (annotation != null) { annotation.Draw(target, visualizeZ); }
        });
    }
}
// NormalizedLandmarkをワールド座標に変換
private Vector3 ConvertToWorldPosition(NormalizedLandmark landmark)
{
    // 正規化された座標 (0~1) をスクリーン座標に変換
    Vector3 screenPosition = new Vector3(
        landmark.X * Screen.width,
        (1 - landmark.Y) * Screen.height, // Y座標は上下逆のため反転
        landmark.Z * 5.0f // 深度のスケール調整（適宜変更）
    );

    // スクリーン座標をワールド座標に変換
    return Camera.main.ScreenToWorldPoint(screenPosition);
}

// エフェクトを手の位置に生成するメソッド(やる)


// エフェクト再生状態を追跡するコルーチン
private IEnumerator TrackEffectStatus(GameObject effect)
{
    _effectPlaying = true;

    // パーティクルシステムがある場合、それを監視
    var particleSystem = effect.GetComponent<ParticleSystem>();
    if (particleSystem != null)
    {
        while (particleSystem.isPlaying)
        {
            yield return null; // 再生中は待機
            Debug.Log("これずっと出てる？");
        }
    }

    // 再生が終了したらログを出力し、フラグをリセット
    Debug.Log("Effect has finished playing.");
    _effectPlaying = false;

    // エフェクトを削除
    if (effect != null)
    {
        Destroy(effect);
    }

    _currentEffect = null; // 参照をクリア
}
// 指先の距離を計算して「パー」か判定するメソッド
private bool IsOpenHand(NormalizedLandmarkList landmarkList)
{
    if (landmarkList == null || landmarkList.Landmark.Count < 21)
    {
        return false; // ランドマークが不足している場合は「パー」ではない
    }

    // ランドマークのインデックス
    int wrist = 0;
    int indexTip = 8;    // 人差し指先端
    int middleTip = 12;  // 中指先端
    int ringTip = 16;    // 薬指先端
    int pinkyTip = 20;   // 小指先端
    int thumbTip = 4;    // 親指先端

    // 指先と手首の距離を計算
    const float wristThreshold = 0.25f; // 手首から指先の距離の閾値（やや緩めに設定）
    float wristToIndex = GetDistance(landmarkList.Landmark[wrist], landmarkList.Landmark[indexTip]);
    float wristToMiddle = GetDistance(landmarkList.Landmark[wrist], landmarkList.Landmark[middleTip]);
    float wristToRing = GetDistance(landmarkList.Landmark[wrist], landmarkList.Landmark[ringTip]);
    float wristToPinky = GetDistance(landmarkList.Landmark[wrist], landmarkList.Landmark[pinkyTip]);
    float wristToThumb = GetDistance(landmarkList.Landmark[wrist], landmarkList.Landmark[thumbTip]);

    if (wristToIndex < wristThreshold ||
        wristToMiddle < wristThreshold ||
        wristToRing < wristThreshold ||
        wristToPinky < wristThreshold ||
        wristToThumb < wristThreshold)
    {
        return false; // 指先が十分に伸びていない場合「パー」ではない
    }

    // 指が十分に開いているかを判定
    return AreFingersSpread(landmarkList);
}

// チョキを判定するメソッド（改良版）
// チョキを判定するメソッド（簡易版）
private bool IsVSignHand(NormalizedLandmarkList landmarkList)
{
    if (landmarkList == null || landmarkList.Landmark.Count < 21)
    {
        Debug.Log("ランドマークが不足しています。");
        return false;
    }

    // ランドマークのインデックス
    int indexTip = 8;    // 人差し指先端
    int middleTip = 12;  // 中指先端
    int ringTip = 16;    // 薬指先端
    int pinkyTip = 20;   // 小指先端
    int thumbTip = 4;    // 親指先端

    // 距離を計算
    float indexToMiddle = GetDistance(landmarkList.Landmark[indexTip], landmarkList.Landmark[middleTip]); // 人差し指と中指の距離
    float indexToRing = GetDistance(landmarkList.Landmark[indexTip], landmarkList.Landmark[ringTip]);     // 人差し指と薬指の距離
    float middleToRing = GetDistance(landmarkList.Landmark[middleTip], landmarkList.Landmark[ringTip]);   // 中指と薬指の距離
    float indexToThumb = GetDistance(landmarkList.Landmark[indexTip], landmarkList.Landmark[thumbTip]);   // 人差し指と親指の距離
    float middleToPinky = GetDistance(landmarkList.Landmark[middleTip], landmarkList.Landmark[pinkyTip]); // 中指と小指の距離

    // 条件設定
    const float vFingerThreshold = 0.06f;  // 人差し指と中指が近接している最大距離
    const float otherFingerMinDistance = 0.08f; // 他の指が離れている最小距離

    // 条件1: 人差し指と中指が適度に近い
    bool isIndexMiddleClose = indexToMiddle < vFingerThreshold;

    // 条件2: 他の指が離れている
    bool areOtherFingersAway =
        indexToRing > otherFingerMinDistance &&
        middleToRing > otherFingerMinDistance &&
        indexToThumb > otherFingerMinDistance &&
        middleToPinky > otherFingerMinDistance;

    // 条件に基づき「チョキ」と判定
    if (isIndexMiddleClose && areOtherFingersAway)
    {
        Debug.Log("チョキが検出されました。");
        return true;
    }
    else
    {
        Debug.Log("チョキの条件に一致しません。");
        return false;
    }
}


// 指が十分に開いているかを判定するメソッド
private bool AreFingersSpread(NormalizedLandmarkList landmarkList)
{
    // 指間の距離を確認（人差し指～小指の間）
    const float spreadThreshold = 0.06f; // 指間の距離の閾値（緩めに設定）
    float indexToMiddle = GetDistance(landmarkList.Landmark[8], landmarkList.Landmark[12]);
    float middleToRing = GetDistance(landmarkList.Landmark[12], landmarkList.Landmark[16]);
    float ringToPinky = GetDistance(landmarkList.Landmark[16], landmarkList.Landmark[20]);

    return indexToMiddle > spreadThreshold &&
          middleToRing > spreadThreshold &&
          ringToPinky > spreadThreshold;
}

// 2点間の距離を計算するヘルパーメソッド
private float GetDistance(NormalizedLandmark p1, NormalizedLandmark p2)
{
    float dx = p1.X - p2.X;
    float dy = p1.Y - p2.Y;
    float dz = p1.Z - p2.Z;
    return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
}

    private void ClearGeneratedObjects()
    {
      foreach (var obj in _generatedObjects)
      {
        if (obj != null)
        {
          Destroy(obj);
        }
      }
      _generatedObjects.Clear();
    }

    protected override HandLandmarkListAnnotation InstantiateChild(bool isActive = true)
    {
      var annotation = base.InstantiateChild(isActive);
      annotation.SetLeftLandmarkColor(_leftLandmarkColor);
      annotation.SetRightLandmarkColor(_rightLandmarkColor);
      annotation.SetLandmarkRadius(_landmarkRadius);
      annotation.SetConnectionColor(_connectionColor);
      annotation.SetConnectionWidth(_connectionWidth);
      return annotation;
    }

    private void ApplyLeftLandmarkColor(Color leftLandmarkColor)
    {
      foreach (var handLandmarkList in children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetLeftLandmarkColor(leftLandmarkColor); }
      }
    }

    private void ApplyRightLandmarkColor(Color rightLandmarkColor)
    {
      foreach (var handLandmarkList in children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetRightLandmarkColor(rightLandmarkColor); }
      }
    }

    private void ApplyLandmarkRadius(float landmarkRadius)
    {
      foreach (var handLandmarkList in children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetLandmarkRadius(landmarkRadius); }
      }
    }

    private void ApplyConnectionColor(Color connectionColor)
    {
      foreach (var handLandmarkList in children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetConnectionColor(connectionColor); }
      }
    }

    private void ApplyConnectionWidth(float connectionWidth)
    {
      foreach (var handLandmarkList in children)
      {
        if (handLandmarkList != null) { handLandmarkList.SetConnectionWidth(connectionWidth); }
      }
    }
  }
}
