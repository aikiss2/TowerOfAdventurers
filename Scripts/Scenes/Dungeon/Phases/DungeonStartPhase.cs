using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ダンジョンシーンの開始フェーズクラス
public class DungeonStartPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // BGMを再生する
        SoundManager.soundManager.PlayBgm(DungeonManager.dungeonManager.DefaultBgm);

        // マップキャラクターのうち、イベント終了後は表示しないキャラを無効化する
        foreach (var one in dungeonObjects.mapCharacters)
        {
            var destroyEventIndex = one.GetComponent<MapCharaCommonParameter>().DestroyEventIndex;
            if (destroyEventIndex != EventTableIndex.None)
            {
                int index = 0;
                foreach (var eventTable in GlobalCanvasManager.globalCanvasManager.eventTableSO.eventTableData)
                {
                    if ((eventTable.index == destroyEventIndex) && (GlobalCanvasManager.globalCanvasManager.eventMemoryDbSO.done[index] == true))
                    {
                        one.gameObject.SetActive(false);
                        break;
                    }

                    index++;
                }
            }
        }

        // イベントが設定されている場合はイベントを実施する
        if(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventIndex != EventTableIndex.None)
        {
            // マップのキャラクターを動かなくする
            DungeonCharacterDontMoveSet(true, dungeonObjects);

            // イベントを実行する
            EventTask eventTask = new ();
            yield return eventTask.RunEvent(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventIndex, DungeonManager.dungeonManager);
            GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventIndex = EventTableIndex.None;

            // マップのキャラクターを動けるようにする
            DungeonCharacterDontMoveSet(false, dungeonObjects);
        }

        // メニューボタンの表示や、各種パネルの非表示を実行
        dungeonObjects.openMenuButton.PointerExitDeSelectSet(true);
        dungeonObjects.openMenuButton.gameObject.SetActive(true);
        GlobalCanvasManager.globalCanvasManager.dialog.Close();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Close();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);

        next = new DungeonMapMovePhase();

        yield return null;
    }
}
