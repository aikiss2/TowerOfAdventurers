using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// キャラ図鑑のキャラクターイメージ表示用のパネル
public sealed class CharaImagePanel : MonoBehaviour
{
    // 表示するイメージ
    [SerializeField] Image charaImage;


    // ウィンドウを開く
    public void Open()
    {
        gameObject.SetActive(true);
    }


    // ウィンドウを閉じる
    public void Close()
    {
        gameObject.SetActive(false);
    }


    // キャライメージを差し替える
    public void SetCharaImage(Sprite texture)
    {
        charaImage.sprite = texture;
    }
}
