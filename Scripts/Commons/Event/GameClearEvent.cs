using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ゲームクリアイベント
public class GameClearEvent : IEventTask
{
    // ゲームクリア時の処理を実行
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        // 会話を表示
        var talkIndex = GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData[index].talkIndex;
        yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(talkIndex, false);

        // フェードアウトの裏に戻し、画面を真っ暗にする
        GlobalCanvasManager.globalCanvasManager.GetComponent<Canvas>().sortingLayerName = "UI";

        // イベントクリア
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO.done.Count; i++)
        {
            GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO.done[i] = false;
        }

        // トーククリア
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done.Count; i++)
        {
            GlobalCanvasManager.globalCanvasManager.talkMemoryDbSO.done[i] = false;
        }

        // 3人目の仲間削除
        if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count == 3)
        {
            GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Remove(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[2]);
            GlobalCanvasManager.globalCanvasManager.playerPanel.DestroyPlayer(2);
        }

        // 3人目の仲間のスキル削除(一番目のスキルのみ残す)。ゲーム2週目以降は3人目の初期スキル(一番目は除く)はストックに入れられるため、仲間にしていなくてもスキル削除する
        if (GlobalCanvasManager.globalCanvasManager.playerPartyDbData_player3.skills.Count > 1)
        {
            GlobalCanvasManager.globalCanvasManager.playerPartyDbData_player3.skills.RemoveRange(1, GlobalCanvasManager.globalCanvasManager.playerPartyDbData_player3.skills.Count - 1);
        }

        // その他の仲間スキルを削除(一番目のスキルのみ残す)
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count; i++)
        {
            GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[i].skills.RemoveRange(1, GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[i].skills.Count - 1);
        }

        // パーティ情報初期化
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.storyStage = StoryStage.Dungeon1;
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventIndex = EventTableIndex.None;
        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventTalk = EventTalk.None;

        // 初期の敵設定
        GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm = BGMType.BgmNo7;

        // 「はじめから」の敵の数を1人にするようリスト削除
        GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.RemoveRange(1, GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Count - 1);
        for (int i = 0; i < GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers.Count; i++)
        {
            GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.encountEnemyNumbers[i] = 0;
        }

        // ストックスキル初期化
        foreach (var one in GlobalCanvasManager.globalCanvasManager.skillStockDbSO.skillStockDbData)
        {
            if (one.skill.SkillTableNo == SkillTable.Cut3
                || one.skill.SkillTableNo == SkillTable.Cut5
                || one.skill.SkillTableNo == SkillTable.Cut8
                || one.skill.SkillTableNo == SkillTable.Heal2
                || one.skill.SkillTableNo == SkillTable.Water2
                || one.skill.SkillTableNo == SkillTable.Wind1
                || one.skill.SkillTableNo == SkillTable.Soil1
                || one.skill.SkillTableNo == SkillTable.Buff2
                || one.skill.SkillTableNo == SkillTable.Buff3)
            {
                one.stockCondition = StockCondition.NoGet;
            }
            else
            {
                one.stockCondition = StockCondition.NoBuy;
            }
        }

        // セーブする
        GlobalCanvasManager.globalCanvasManager.playerPanel.PlayerSave();

        // タイトルに遷移する
        SceneManager.LoadScene(SceneName.Title.ToString());

        // ここでWAITしている間にタイトルに戻る
        yield return new WaitForSeconds(10f);
    }
}
