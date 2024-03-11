using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// キャラ図鑑のキャラ説明用のパネル
public sealed class CharaStoryPanel : MonoBehaviour
{
    // キャラ説明テキスト
    [SerializeField] TextMeshProUGUI text;


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


    // キャラ説明を差し替える
    public void SetText(string story)
    {
        text.text = story;
    }
}
