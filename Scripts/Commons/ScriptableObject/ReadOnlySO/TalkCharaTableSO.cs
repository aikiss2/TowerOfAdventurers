using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 会話キャラデータのテーブル
public class TalkCharaTableSO : ScriptableObject
{
    public List<TalkCharaData> talkCharaData = new();
}

[System.Serializable]
// 会話キャラデータ
public class TalkCharaData
{
    // 会話ウィンドウの見出しに表示するキャラ名
    [field: SerializeField] public string Name { get; private set; }

    // 会話ウィンドウに表示するイメージ
    [field: SerializeField] public Sprite Sprite { get; private set; }
}