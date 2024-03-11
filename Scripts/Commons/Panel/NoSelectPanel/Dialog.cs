using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ダイアログクラス
public class Dialog : MonoBehaviour
{
    // ダイアログに表示するテキスト
    [SerializeField] TextMeshProUGUI text;


    // ダイアログに文字列を表示する
    public void TypeDialog(string line)
    {
        text.text = line;
    }


    // ダイアログを開く
    public void Open()
    {
        gameObject.SetActive(true);
    }


    // ダイアログを閉じる
    public void Close()
    {
        text.text = "";
        gameObject.SetActive(false);
    }
}
