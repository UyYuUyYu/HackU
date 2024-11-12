using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class MatchngDisplayScript : MonoBehaviourPun
{
    
    [SerializeField] RawImage myDisplayImage,friendDisplayImage;

    [SerializeField] private Text myNameText,friendNameText;


    [ContextMenu("SendFriendImageAndName")]
    public void SendFriendImageAndName()
    {
        photonView.RPC("ReceiveName", RpcTarget.Others, NameInputPUN.myName);
        photonView.RPC("ReceiveScreenshot", RpcTarget.Others, ScreenShot.imageData);
        friendDisplayImage.gameObject.SetActive(true);
    
    }

    [ContextMenu("InportImageAndName")]
    public void InportImageAndName()
    {
        myNameText.text=NameInputPUN.myName;
        ViewShot(ScreenShot.imageData);
        myDisplayImage.gameObject.SetActive(true);
    }

    private void ViewShot(byte[] _imageData)
    {
         // 受信したバイトデータをTexture2Dに変換
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(_imageData); // バイトデータをTexture2Dにロード

        // RawImage UIに表示
        myDisplayImage.texture = receivedTexture;
        myDisplayImage.SetNativeSize(); // 受信した画像に合わせてサイズを調整
    }
    [PunRPC]
    private void ReceiveScreenshot(byte[] _imageData)
    {
        // 受信したバイトデータをTexture2Dに変換
        Texture2D receivedTexture = new Texture2D(1, 1);
        receivedTexture.LoadImage(_imageData); // バイトデータをTexture2Dにロード

        // RawImage UIに表示
        friendDisplayImage.texture = receivedTexture;
        friendDisplayImage.SetNativeSize(); // 受信した画像に合わせてサイズを調整
    }

    [PunRPC]
    private void ReceiveName(string _name)
    {
        friendNameText.text=_name;
    }
}
