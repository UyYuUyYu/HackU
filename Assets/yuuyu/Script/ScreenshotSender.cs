using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

public class ScreenshotSender : MonoBehaviourPun
{
    public Camera screenshotCamera; // キャプチャに使うカメラ
    public RawImage displayImage;   // 受信した画像を表示するUI (RawImage)

    void Update()
    {
        // スペースキーを押したら画像をキャプチャして送信
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureAndSendScreenshot());
        }
    }

    private IEnumerator CaptureAndSendScreenshot()
    {
        // RenderTextureを用意
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenshotCamera.targetTexture = renderTexture;
        screenshotCamera.Render();

        // Texture2Dに変換
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // カメラのRenderTextureを解除
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // PNGフォーマットにエンコード
        byte[] imageData = screenshot.EncodeToPNG();

        // RPCを使って他のプレイヤーに送信
        photonView.RPC("ReceiveScreenshot", RpcTarget.Others, imageData);

        yield return null;
    }

    [PunRPC]
    private void ReceiveScreenshot(byte[] imageData)
    {
        // 受信したバイトデータをTexture2Dに変換
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(imageData); // バイトデータをTexture2Dにロード

        // RawImage UIに表示
        displayImage.texture = receivedTexture;
        displayImage.SetNativeSize(); // 受信した画像に合わせてサイズを調整
    }
}
