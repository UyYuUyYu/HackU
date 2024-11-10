using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class ScreenshotSender : MonoBehaviourPun
{
    
    public Camera screenshotCamera; // キャプチャに使うカメラ
    public RawImage displayImage;   // 受信した画像を表示するUI (RawImage)

    private byte[] imageData;

    void Update()
    {
        // スペースキーを押したら画像をキャプチャして送信
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("カシャ");
            StartCoroutine(CaptureAndSendScreenshot());
        }
    }

    private IEnumerator CaptureAndSendScreenshot()
    {
        print("capture");
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
        imageData = screenshot.EncodeToPNG();

        ViewShot(imageData);

        // RPCを使って他のプレイヤーに送信
        photonView.RPC("ReceiveScreenshot", RpcTarget.Others, imageData);

        yield return null;
    }

    private void ViewShot(byte[] _imageData)
    {
         // 受信したバイトデータをTexture2Dに変換
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(_imageData); // バイトデータをTexture2Dにロード

        // RawImage UIに表示
        displayImage.texture = receivedTexture;
        displayImage.SetNativeSize(); // 受信した画像に合わせてサイズを調整
    }
    [PunRPC]
    private void ReceiveScreenshot(byte[] _imageData)
    {
        // 受信したバイトデータをTexture2Dに変換
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(_imageData); // バイトデータをTexture2Dにロード

        // RawImage UIに表示
        displayImage.texture = receivedTexture;
        displayImage.SetNativeSize(); // 受信した画像に合わせてサイズを調整
    }
}
