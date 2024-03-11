using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// イベントデータのテーブル
public class EventTableSO : ScriptableObject
{
    public List<EventData> eventTableData = new();
}

[System.Serializable]
// イベントデータ
public class EventData
{
    // イベントデータのインデックス
    public EventTableIndex index;

    // イベント種別
    public EventType eventType;

    // イベント種別がゴールド取得時に取得量
    public int amount;

    // イベント種別がスキル取得時に取得スキル
    public SkillTable skill;

    // イベント種別が会話時の会話データ
    public TalkTableIndex talkIndex;

    // イベント種別がBGM再生時のBGMデータ
    public BGMType bgmType;

    // イベント種別がSE再生時のSEデータ
    public SEType seType;
}