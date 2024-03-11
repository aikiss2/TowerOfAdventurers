using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// プレイヤーパーティのデータベース
public class PlayerPartyDbSO : ScriptableObject
{
    // 所持ゴールド
    public int partyGold;

    // ストーリー進捗
    public StoryStage storyStage;

    // 実施するイベント
    public EventTableIndex eventIndex;

    // イベント後の専用会話
    public EventTalk eventTalk;

    // プレイヤーの味方情報
    public List<PlayerPartyDbDataSO> playerPartyDbData = new();

    // 出会った敵キャラ(true:出会った、false:出会っていない)
    public List<bool> encountEnemyList = new();
}