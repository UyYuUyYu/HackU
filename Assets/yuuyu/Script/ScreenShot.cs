using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{

    [SerializeField] private RawImage displayImage; 
    public static byte[] imageData;


    /*
    [ContextMenu("ViewSreanShot")]
    public void InportImage()
    {
        ViewShot(ScreenShot.imageData);
        displayImage.gameObject.SetActive(true);
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
    */

    //スクショを撮るmethod
    [ContextMenu("SS")]
    public void StartSS()
    {
        StartCoroutine(SS());
    }
    private IEnumerator SS()
    {
        yield return new WaitForEndOfFrame(); // UIも含めたスクリーンショットを撮るため、フレームの終わりまで待機
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        // RawImageのサイズを取得
        RectTransform rawImageRectTransform = displayImage.rectTransform;
        int width = (int)rawImageRectTransform.rect.width;
        int height = (int)rawImageRectTransform.rect.height;

        Texture2D resizedScreenshot = ResizeTexture(screenshot, width, height);

        imageData = resizedScreenshot.EncodeToPNG();
        Destroy(screenshot);
    }

    private Texture2D ResizeTexture(Texture2D sourceTexture, int targetWidth, int targetHeight)
    {
        // 新しいサイズでTexture2Dを作成
        Texture2D resizedTexture = new Texture2D(targetWidth, targetHeight, sourceTexture.format, false);
        Color[] newPixels = resizedTexture.GetPixels(0);

        // リサイズ用にピクセルごとに計算
        float incX = 1.0f / targetWidth;
        float incY = 1.0f / targetHeight;

        for (int i = 0; i < newPixels.Length; i++)
        {
            // 計算して元のテクスチャからピクセルを取得
            float x = incX * (i % targetWidth);
            float y = incY * (i / targetWidth);
            newPixels[i] = sourceTexture.GetPixelBilinear(x, y);
        }

        resizedTexture.SetPixels(newPixels);
        resizedTexture.Apply();
        return resizedTexture;
    }
}