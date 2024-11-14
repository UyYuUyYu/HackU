using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    private WebCamTexture webCamTexture;

    void Start()
    {
        // WebCamTextureのインスタンスを作成
        webCamTexture = new WebCamTexture();

        // このスクリプトがアタッチされているオブジェクトのテクスチャにカメラ映像を設定
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = webCamTexture;
        }

        // UIのRawImageにカメラ映像を表示する場合
        RawImage rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = webCamTexture;
        }

        // カメラ映像を開始
        webCamTexture.Play();
    }

    void OnDestroy()
    {
        // カメラ映像を停止
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}
