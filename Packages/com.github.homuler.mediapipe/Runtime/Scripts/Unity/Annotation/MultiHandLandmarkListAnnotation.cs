// MultiHandLandmarkListAnnotation.cs
// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
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
    [SerializeField] private GameObject _landmarkPrefab; // 追加: 生成するオブジェクトのPrefab
    [SerializeField] private GameObject _landmarkPrefab2; // 追加: 生成するオブジェクトのPrefab

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




// エフェクトを手の位置に生成するメソッド
public void Draw(IList<NormalizedLandmarkList> targets, bool visualizeZ = false)
{
    if (ActivateFor(targets))
    {
        ClearGeneratedObjects(); // 前回生成したオブジェクトを削除
        if (targets.Count == 2 && AreHandsTogether(targets)) // 両手が近づいたときの判定
        {
            UnityEngine.Debug.Log("Both hands together: Special effect triggered");
            GenerateEffectForBothHands(targets); // 両手用のエフェクトを生成
        }
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                var landmarkList = targets[i];

                if (IsOpenHand(landmarkList)) // 「パー」の時にエフェクトを生成
                {
                    UnityEngine.Debug.Log($"Hand {i}: Open hand detected");
                    GenerateEffectAtHandPosition(landmarkList); // 手の位置にエフェクト生成
                }
            }
        }

        CallActionForAll(targets, (annotation, target) =>
        {
            if (annotation != null) { annotation.Draw(target, visualizeZ); }
        });
    }
}

// 両手が近いかどうかを判定するメソッド
private bool AreHandsTogether(IList<NormalizedLandmarkList> targets)
{
    if (targets.Count < 2) return false;

    // 両手の重心を計算
    Vector3 leftHandPosition = GetHandCenter(targets[0]);
    Vector3 rightHandPosition = GetHandCenter(targets[1]);

    // 両手の距離を計算
    const float handsTogetherThreshold = 0.2f; // 距離の閾値（適宜調整）
    float distance = Vector3.Distance(leftHandPosition, rightHandPosition);

    return distance < handsTogetherThreshold;
}

// 両手の重心位置にエフェクトを生成
private void GenerateEffectForBothHands(IList<NormalizedLandmarkList> targets)
{
    if (_landmarkPrefab == null) return;

    // 両手の重心を計算
    Vector3 leftHandPosition = GetHandCenter(targets[0]);
    Vector3 rightHandPosition = GetHandCenter(targets[1]);
    Vector3 centerPosition = (leftHandPosition + rightHandPosition) / 2.0f; // 中央位置

    // 特別なエフェクトを生成
    var specialEffect = Instantiate(_landmarkPrefab2); // 別のエフェクトにしたい場合は新しいPrefabを指定
    specialEffect.transform.position = centerPosition; // 両手の中央位置に配置
    _generatedObjects.Add(specialEffect);
}

// 手の重心を計算するメソッド
private Vector3 GetHandCenter(NormalizedLandmarkList landmarkList)
{
    Vector3 center = Vector3.zero;
    foreach (var landmark in landmarkList.Landmark)
    {
        center += new Vector3(landmark.X, landmark.Y, landmark.Z);
    }
    return center / landmarkList.Landmark.Count; // 平均値を取得
}

// エフェクトを手の位置に生成するメソッド
private void GenerateEffectAtHandPosition(NormalizedLandmarkList landmarkList)
{
    if (_landmarkPrefab == null) return;

    // 手の重心（平均位置）を計算
    Vector3 handPosition = GetHandCenter(landmarkList);

    // エフェクトを生成
    var effect = Instantiate(_landmarkPrefab);
    effect.transform.position = handPosition; // 手の重心位置に配置
    _generatedObjects.Add(effect); // 後で削除できるようリストに保存
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
