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
        [SerializeField]
        private Color _leftLandmarkColor = Color.green;

        [SerializeField]
        private Color _rightLandmarkColor = Color.green;

        [SerializeField]
        private float _landmarkRadius = 15.0f;

        [SerializeField]
        private Color _connectionColor = Color.white;

        [SerializeField, Range(0, 1)]
        private float _connectionWidth = 1.0f;

        [SerializeField]
        private float cooldownDuration = 0.0f; // クールダウンの時間（秒）

        private bool isCooldownActive = false; // クールダウン中かどうかを管理
        private float cooldownTimer = 0.0f; // クールダウンの残り時間

        [SerializeField]
        private GameObject[] _landmarkPrefabs; // エフェクトの配列を管理

        [SerializeField]
        private Vector3 _effectPositionOffset = Vector3.zero; // エフェクト位置のオフセットをInspectorで編集可能

        [SerializeField]
        private AudioClip[] _effectSounds; // エフェクトに対応する効果音の配列
        private AudioSource _audioSource; // 効果音再生用
        private GameObject _currentEffect = null; // 現在のエフェクトを保持
        private bool _effectPlaying = false; // エフェクトの再生状態を追跡
        public bool IsEffectPlaying => _effectPlaying;

        [SerializeField]
        private UnityEngine.Events.UnityEvent<int> OnValueChanged;

        // 数値変更時にイベントを発火
        private void NotifyValueChanged()
        {
            OnValueChanged?.Invoke(_currentValue);
        }

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        private void Update()
        {
            // クールダウン中のタイマーを更新
            if (isCooldownActive)
            {
                cooldownTimer -= Time.deltaTime; // 経過時間を減算

                if (cooldownTimer <= 0)
                {
                    isCooldownActive = false; // クールダウンを終了
                    cooldownTimer = 0.0f; // タイマーをリセット
                    // Debug.Log("Cooldown ended.");
                }
            }
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

        private void PlayEffectSound(int soundIndex)
        {
            if (_effectSounds != null && soundIndex >= 0 && soundIndex < _effectSounds.Length)
            {
                _audioSource.clip = _effectSounds[soundIndex];
                _audioSource.Play();
            }
            else
            {
                // Debug.LogWarning($"Invalid sound index: {soundIndex}");
            }
        }

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
        public void Draw(IList<NormalizedLandmarkList> targets, bool visualizeZ = false)
        {
            if (ActivateFor(targets))
            {
                ClearGeneratedObjects();

                // 両手を使うポーズの場合の処理
                // 両手を使うポーズの場合の処理
                if (targets.Count >= 2)
                {
                    var leftHand = targets[0];
                    var rightHand = targets[1];

                    // 他の両手ポーズ判定（例: ハートポーズ）
                    if (IsHeartPose(leftHand, rightHand))
                    {
                        Vector3 leftHandPosition = ConvertToWorldPosition(leftHand.Landmark[0]);
                        Vector3 rightHandPosition = ConvertToWorldPosition(rightHand.Landmark[0]);

                        UpdateEffectByValue(4, leftHandPosition, rightHandPosition);
                        // Debug.Log("Heart pose detected.");
                        return;
                    }

                    return; // 両手が認識されているが有効なポーズではない場合、片手ポーズをスキップ
                }

                // 片手ポーズの判定
                for (int i = 0; i < targets.Count; i++)
                {
                    var landmarkList = targets[i];

                    // 手首または指先の位置を取得
                    Vector3 handPosition = ConvertToWorldPosition(landmarkList.Landmark[0]); // 手首

                    if (IsOpenHand(landmarkList))
                    {
                        UpdateEffectByValue(1, handPosition); // パーのエフェクトを生成
                    }
                    else if (IsVSignHand(landmarkList))
                    {
                        UpdateEffectByValue(2, handPosition); // チョキのエフェクトを生成
                    }
                    else if (IsFistHand(landmarkList))
                    {
                        UpdateEffectByValue(3, handPosition); // グーのエフェクトを生成
                    }
                    else if (IsGunPose(landmarkList))
                    {
                        if (IsIndexFingerMovingUpward(landmarkList))
                        {
                            handPosition = ConvertToWorldPosition(landmarkList.Landmark[0]);
                            UpdateEffectByValue(5, handPosition); // 上向きのエフェクト
                            // Debug.Log("銃ポーズ + 上向きが検出されました。エフェクトを再生します。");
                        }
                        else
                        {
                            // Debug.Log("銃ポーズ検出。上向き条件が満たされていません。");
                        }
                    }
                    else
                    {
                        UpdateEffectByValue(0, handPosition); // 無効なエフェクト
                    }
                }
            }
        }

        // 指定の手の状態に基づくエフェクトの描画
        public void UpdateEffectByValue(int newValue, Vector3 handPosition, Vector3? secondaryPosition = null, NormalizedLandmarkList landmarkList = null)
{
    if (isCooldownActive)
    {
        // Debug.Log("Effect is on cooldown. Ignoring input.");
        return; // クールダウン中は何もしない
    }

    if (_currentValue != newValue)
    {
        _currentValue = newValue;

        // 現在のエフェクトを削除
        if (_currentEffect != null)
        {
            Destroy(_currentEffect);
            _currentEffect = null;
        }

        // エフェクト生成位置を決定
        Vector3 effectPosition = handPosition;
        if (secondaryPosition.HasValue)
        {
            effectPosition = Vector3.Lerp(handPosition, secondaryPosition.Value, 0.5f);
        }

        // 銃魔法の場合、方向に基づいて位置を調整
        if (newValue == 5 && landmarkList != null) // 銃魔法に対応
        {
            Vector3 direction = CalculateIndexFingerDirection(landmarkList); // 人差し指の方向を取得

            float offset = 0.2f; // X座標のずらし量
            if (direction.x > 0.1f) // 人差し指が右向きの場合
            {
                effectPosition.x += offset;

            }
            else if (direction.x < -0.1f) // 人差し指が左向きの場合
            {
                effectPosition.x -= offset;

            }
        }

        // 数値に応じたエフェクト生成と効果音再生
        switch (_currentValue)
        {
            case 0:
                break;
            case 1:
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[0], effectPosition);
                PlayEffectSound(0);
                break;
            case 2:
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[1], effectPosition);
                PlayEffectSound(1);
                break;
            case 3:
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[2], effectPosition);
                PlayEffectSound(2);
                break;
            case 4:
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[3], effectPosition);
                PlayEffectSound(3);
                break;
            case 5:
                _currentEffect = GenerateEffectAtPosition(_landmarkPrefabs[4], effectPosition);
                PlayEffectSound(4);
                break;
            default:
                Debug.Log($"Effect for value {_currentValue} is not implemented.");
                break;
        }

        // クールダウンを開始
        StartCooldown();
    }
}

        // 銃のポーズを判定するメソッド
        private bool IsGunPose(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                // Debug.Log("ランドマークが不足しています。");
                return false; // ランドマークが不足している場合は判定しない
            }

            // ランドマークのインデックス
            int wrist = 0;
            int indexTip = 8;
            int middleTip = 12;
            int ringTip = 16;
            int pinkyTip = 20;
            int thumbTip = 4;

            // 手首から各指先までの距離を計算
            float wristToIndex = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[indexTip]
            );
            float wristToMiddle = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[middleTip]
            );
            float wristToRing = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[ringTip]
            );
            float wristToPinky = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[pinkyTip]
            );
            float wristToThumb = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[thumbTip]
            );

            // 条件：親指と人差し指が長く、他の指は短い
            bool isThumbExtended =
                wristToThumb > wristToMiddle
                && wristToThumb > wristToRing
                && wristToThumb > wristToPinky;
            bool isIndexExtended =
                wristToIndex > wristToMiddle
                && wristToIndex > wristToRing
                && wristToIndex > wristToPinky;
            bool isOtherFingersFolded =
                wristToMiddle < wristToIndex
                && wristToRing < wristToIndex
                && wristToPinky < wristToIndex;

            // 条件を満たすか確認
            Debug.Log(
                $"Thumb Extended: {isThumbExtended}, Index Extended: {isIndexExtended}, Other Fingers Folded: {isOtherFingersFolded}"
            );

            // 条件をすべて満たしたら銃ポーズ
            bool isGunPose = isThumbExtended && isIndexExtended && isOtherFingersFolded;

            if (isGunPose)
            {
                Debug.Log("銃ポーズが検出されました。");
            }

            return isGunPose;
        }

        private void StartCooldown()
        {
            isCooldownActive = true;
            cooldownTimer = cooldownDuration; // クールダウン時間を設定
            Debug.Log("Cooldown started.");
        }

        private bool IsHeartPose(NormalizedLandmarkList leftHand, NormalizedLandmarkList rightHand)
        {
            if (
                leftHand == null
                || rightHand == null
                || leftHand.Landmark.Count < 21
                || rightHand.Landmark.Count < 21
            )
            {
                return false; // 両手のランドマークが不足している場合は無効
            }

            // ランドマークのインデックス
            int thumbTip = 4; // 親指先端
            int pinkyTip = 20; // 小指先端
            int indexTip = 8; // 人差し指先端
            int middleTip = 12; // 中指先端
            int ringTip = 16; // 薬指先端

            // 親指と小指のランドマークを取得
            Vector3 leftThumb = ConvertToWorldPosition(leftHand.Landmark[thumbTip]);
            Vector3 rightThumb = ConvertToWorldPosition(rightHand.Landmark[thumbTip]);
            Vector3 leftPinky = ConvertToWorldPosition(leftHand.Landmark[pinkyTip]);
            Vector3 rightPinky = ConvertToWorldPosition(rightHand.Landmark[pinkyTip]);

            // 親指がそれ以外の指から十分離れているか判定
            float leftThumbToIndexDistance = Vector3.Distance(
                leftThumb,
                ConvertToWorldPosition(leftHand.Landmark[indexTip])
            );
            float leftThumbToMiddleDistance = Vector3.Distance(
                leftThumb,
                ConvertToWorldPosition(leftHand.Landmark[middleTip])
            );
            float leftThumbToRingDistance = Vector3.Distance(
                leftThumb,
                ConvertToWorldPosition(leftHand.Landmark[ringTip])
            );
            float rightThumbToIndexDistance = Vector3.Distance(
                rightThumb,
                ConvertToWorldPosition(rightHand.Landmark[indexTip])
            );
            float rightThumbToMiddleDistance = Vector3.Distance(
                rightThumb,
                ConvertToWorldPosition(rightHand.Landmark[middleTip])
            );
            float rightThumbToRingDistance = Vector3.Distance(
                rightThumb,
                ConvertToWorldPosition(rightHand.Landmark[ringTip])
            );

            const float minThumbDistance = 0.1f; // 親指と他の指の最小距離（近すぎる場合は誤認識とみなす）

            bool isLeftThumbSeparated =
                leftThumbToIndexDistance > minThumbDistance
                && leftThumbToMiddleDistance > minThumbDistance
                && leftThumbToRingDistance > minThumbDistance;
            bool isRightThumbSeparated =
                rightThumbToIndexDistance > minThumbDistance
                && rightThumbToMiddleDistance > minThumbDistance
                && rightThumbToRingDistance > minThumbDistance;

            // 親指と小指の距離条件
            float thumbToThumbDistance = Vector3.Distance(leftThumb, rightThumb);
            const float thumbFarThreshold = 0.7f; // 親指同士の距離の最小閾値（かなり離れている必要がある）

            float pinkyToPinkyDistance = Vector3.Distance(leftPinky, rightPinky);
            const float pinkyFarThreshold = 0.7f; // 小指同士の距離の最小閾値（かなり離れている必要がある）

            // 親指が小指より下にあるか判定
            bool isThumbBelowPinky = leftThumb.y < leftPinky.y && rightThumb.y < rightPinky.y;

            // 両手が離れているか判定
            float leftToRightDistance = Vector3.Distance(
                ConvertToWorldPosition(leftHand.Landmark[0]),
                ConvertToWorldPosition(rightHand.Landmark[0])
            );
            const float minHandDistance = 0.8f; // 両手間の最小距離（離れていることを要求）

            // 条件をすべて満たしているかを判定
            return thumbToThumbDistance > thumbFarThreshold
                && // 親指がかなり離れている
                pinkyToPinkyDistance > pinkyFarThreshold
                && // 小指がかなり離れている
                isThumbBelowPinky
                && leftToRightDistance > minHandDistance
                && isLeftThumbSeparated
                && isRightThumbSeparated; // 親指が他の指と離れていることを確認
        }

        // グーを判定するメソッド
        private bool IsFistHand(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                return false; // ランドマークが不足している場合は「グー」ではない
            }

            // ランドマークのインデックス
            int wrist = 0;
            int indexTip = 8;
            int middleTip = 12;
            int ringTip = 16;
            int pinkyTip = 20;
            int thumbTip = 4;

            // 手首と各指先の距離を計算
            float wristToIndex = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[indexTip]
            );
            float wristToMiddle = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[middleTip]
            );
            float wristToRing = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[ringTip]
            );
            float wristToPinky = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[pinkyTip]
            );
            float wristToThumb = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[thumbTip]
            );

            // 指先間の距離を計算
            float indexToMiddle = GetDistance(
                landmarkList.Landmark[indexTip],
                landmarkList.Landmark[middleTip]
            );
            float middleToRing = GetDistance(
                landmarkList.Landmark[middleTip],
                landmarkList.Landmark[ringTip]
            );
            float ringToPinky = GetDistance(
                landmarkList.Landmark[ringTip],
                landmarkList.Landmark[pinkyTip]
            );

            // 親指と人差し指の交差を確認
            float thumbToIndex = GetDistance(
                landmarkList.Landmark[thumbTip],
                landmarkList.Landmark[indexTip]
            );

            // 条件設定
            const float fistThreshold = 0.2f; // 手首から指先の距離が短い（握りこぶしを表現）
            const float fingerCloseThreshold = 0.1f; // 指先同士の距離が近い（指が密集している）
            const float thumbInteractionThreshold = 0.15f; // 親指と人差し指が交差している

            // 条件判定
            bool areFingersCloseToWrist =
                wristToIndex < fistThreshold
                && wristToMiddle < fistThreshold
                && wristToRing < fistThreshold
                && wristToPinky < fistThreshold
                && wristToThumb < fistThreshold;

            bool areFingersCloseTogether =
                indexToMiddle < fingerCloseThreshold
                && middleToRing < fingerCloseThreshold
                && ringToPinky < fingerCloseThreshold;

            bool isThumbInteracting = thumbToIndex < thumbInteractionThreshold;

            // すべての条件を満たす場合に「グー」と判定
            return areFingersCloseToWrist && areFingersCloseTogether && isThumbInteracting;
        }

        // エフェクト生成ヘルパーメソッド
        private GameObject GenerateEffectAtPosition(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
                return null;

            // Z座標を固定
            position.z = -4f;

            var effect = Instantiate(prefab);
            effect.transform.position = position + _effectPositionOffset; // オフセットを適用
            Destroy(effect, 3f); // 3秒後に自動削除
            return effect;
        }

        // NormalizedLandmarkをワールド座標に変換// NormalizedLandmarkをワールド座標に変換
        private Vector3 ConvertToWorldPosition(NormalizedLandmark landmark)
        {
            Vector3 screenPosition = new Vector3(
                (1 - landmark.X) * Screen.width, // X座標を反転
                landmark.Y * Screen.height, // Y座標も必要なら反転
                -4f // Z座標を固定
            );

            return Camera.main.ScreenToWorldPoint(screenPosition);
        }

        // 手のひらの法線を計算するメソッド
        private Vector3 CalculatePalmNormal(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                return Vector3.zero;
            }

            // ランドマークのインデックス
            int wrist = 0; // 手首
            int indexBase = 5; // 人差し指の付け根
            int pinkyBase = 17; // 小指の付け根

            // 手のひらのベクトルを計算
            Vector3 wristPos = ConvertToWorldPosition(landmarkList.Landmark[wrist]);
            Vector3 indexBasePos = ConvertToWorldPosition(landmarkList.Landmark[indexBase]);
            Vector3 pinkyBasePos = ConvertToWorldPosition(landmarkList.Landmark[pinkyBase]);

            // 手のひらの法線ベクトル
            Vector3 palmVector1 = indexBasePos - wristPos;
            Vector3 palmVector2 = pinkyBasePos - wristPos;

            return Vector3.Cross(palmVector1, palmVector2).normalized;
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
            int indexTip = 8; // 人差し指先端
            int middleTip = 12; // 中指先端
            int ringTip = 16; // 薬指先端
            int pinkyTip = 20; // 小指先端
            int thumbTip = 4; // 親指先端

            // 指先と手首の距離を計算
            const float wristThreshold = 0.25f; // 手首から指先の距離の閾値（やや緩めに設定）
            float wristToIndex = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[indexTip]
            );
            float wristToMiddle = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[middleTip]
            );
            float wristToRing = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[ringTip]
            );
            float wristToPinky = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[pinkyTip]
            );
            float wristToThumb = GetDistance(
                landmarkList.Landmark[wrist],
                landmarkList.Landmark[thumbTip]
            );

            if (
                wristToIndex < wristThreshold
                || wristToMiddle < wristThreshold
                || wristToRing < wristThreshold
                || wristToPinky < wristThreshold
                || wristToThumb < wristThreshold
            )
            {
                return false; // 指先が十分に伸びていない場合「パー」ではない
            }

            // 指が十分に開いているかを判定
            return AreFingersSpread(landmarkList);
        }

        // チョキを判定するメソッド（簡易版）
        private bool IsVSignHand(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                // Debug.Log("ランドマークが不足しています。");
                return false;
            }

            // ランドマークのインデックス
            int indexTip = 8; // 人差し指先端
            int middleTip = 12; // 中指先端
            int ringTip = 16; // 薬指先端
            int pinkyTip = 20; // 小指先端
            int thumbTip = 4; // 親指先端

            // 距離を計算
            float indexToMiddle = GetDistance(
                landmarkList.Landmark[indexTip],
                landmarkList.Landmark[middleTip]
            ); // 人差し指と中指の距離
            float indexToRing = GetDistance(
                landmarkList.Landmark[indexTip],
                landmarkList.Landmark[ringTip]
            ); // 人差し指と薬指の距離
            float middleToRing = GetDistance(
                landmarkList.Landmark[middleTip],
                landmarkList.Landmark[ringTip]
            ); // 中指と薬指の距離
            float indexToThumb = GetDistance(
                landmarkList.Landmark[indexTip],
                landmarkList.Landmark[thumbTip]
            ); // 人差し指と親指の距離
            float middleToPinky = GetDistance(
                landmarkList.Landmark[middleTip],
                landmarkList.Landmark[pinkyTip]
            ); // 中指と小指の距離

            // 条件設定
            const float vFingerThreshold = 0.06f; // 人差し指と中指が近接している最大距離
            const float otherFingerMinDistance = 0.08f; // 他の指が離れている最小距離

            // 条件1: 人差し指と中指が適度に近い
            bool isIndexMiddleClose = indexToMiddle < vFingerThreshold;

            // 条件2: 他の指が離れている
            bool areOtherFingersAway =
                indexToRing > otherFingerMinDistance
                && middleToRing > otherFingerMinDistance
                && indexToThumb > otherFingerMinDistance
                && middleToPinky > otherFingerMinDistance;

            // 条件に基づき「チョキ」と判定
            if (isIndexMiddleClose && areOtherFingersAway)
            {
                // Debug.Log("チョキが検出されました。");
                return true;
            }
            else
            {
                // Debug.Log("チョキの条件に一致しません。");
                return false;
            }
        }

        // 人差し指の方向を計算
        private Vector3 CalculateIndexFingerDirection(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                return Vector3.zero;
            }

            // 人差し指の基点と先端
            int indexBase = 5; // 人差し指の付け根
            int indexTip = 8; // 人差し指の先端

            Vector3 basePos = ConvertToWorldPosition(landmarkList.Landmark[indexBase]);
            Vector3 tipPos = ConvertToWorldPosition(landmarkList.Landmark[indexTip]);

            // ベクトルを計算
            Vector3 direction = (tipPos - basePos).normalized;
            return direction;
        }

        // 指が十分に開いているかを判定するメソッド
        private bool AreFingersSpread(NormalizedLandmarkList landmarkList)
        {
            // 指間の距離を確認（人差し指～小指の間）
            const float spreadThreshold = 0.06f; // 指間の距離の閾値（緩めに設定）
            float indexToMiddle = GetDistance(landmarkList.Landmark[8], landmarkList.Landmark[12]);
            float middleToRing = GetDistance(landmarkList.Landmark[12], landmarkList.Landmark[16]);
            float ringToPinky = GetDistance(landmarkList.Landmark[16], landmarkList.Landmark[20]);

            return indexToMiddle > spreadThreshold
                && middleToRing > spreadThreshold
                && ringToPinky > spreadThreshold;
        }

        private bool IsIndexFingerMovingUpward(NormalizedLandmarkList landmarkList)
        {
            if (landmarkList == null || landmarkList.Landmark.Count < 21)
            {
                Debug.Log("ランドマークが不足しています。");
                return false;
            }

            // 人差し指のベクトルを取得
            Vector3 direction = CalculateIndexFingerDirection(landmarkList);

            // 動作条件を緩める：少しでも上向き（y > 0.3）
            bool isUpward = direction.y > 0.565f; // 条件を緩めた

            return isUpward;
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
                if (handLandmarkList != null)
                {
                    handLandmarkList.SetLeftLandmarkColor(leftLandmarkColor);
                }
            }
        }

        private void ApplyRightLandmarkColor(Color rightLandmarkColor)
        {
            foreach (var handLandmarkList in children)
            {
                if (handLandmarkList != null)
                {
                    handLandmarkList.SetRightLandmarkColor(rightLandmarkColor);
                }
            }
        }

        private void ApplyLandmarkRadius(float landmarkRadius)
        {
            foreach (var handLandmarkList in children)
            {
                if (handLandmarkList != null)
                {
                    handLandmarkList.SetLandmarkRadius(landmarkRadius);
                }
            }
        }

        private void ApplyConnectionColor(Color connectionColor)
        {
            foreach (var handLandmarkList in children)
            {
                if (handLandmarkList != null)
                {
                    handLandmarkList.SetConnectionColor(connectionColor);
                }
            }
        }

        private void ApplyConnectionWidth(float connectionWidth)
        {
            foreach (var handLandmarkList in children)
            {
                if (handLandmarkList != null)
                {
                    handLandmarkList.SetConnectionWidth(connectionWidth);
                }
            }
        }
    }
}
