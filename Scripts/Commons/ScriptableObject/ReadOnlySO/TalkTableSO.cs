using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 会話データのテーブル
public class TalkTableSO : ScriptableObject
{
    public List<TalkTableData> talkTableData = new();
}

[System.Serializable]
// 会話データ
public class TalkTableData
{
    // 会話データのインデックス
    public TalkTableIndex index;

    // 会話データが、どのストーリーの進捗状況に対応した会話か
    public StoryStage storyStage;

    // 会話データが、どのイベントに対応した会話か
    public EventTalk eventTalk;

    // 会話データが発生したあとに、新たにセットする会話データ
    public EventTalk setEventTalk;

    // 会話データを繰り返し発生させるか(true:繰り返す)
    public bool loop;

    // シーン移動したら、またこの会話を発生させるか(true:リセットし、再び発生させるようにする)
    public bool sceneReset;

    // 会話するキャラ(キャラ名とキャライメージを指定)
    public TalkChara chara;

    // 会話するキャラを、会話ウィンドウのどの位置に表示させるか
    public TalkSide side;

    // 会話内容
    public string talk;
}