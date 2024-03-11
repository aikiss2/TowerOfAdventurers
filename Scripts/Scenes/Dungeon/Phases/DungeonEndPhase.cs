using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーンの切り替えに必要

// ダンジョンシーンの終了フェーズクラス
public class DungeonEndPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // 次のシーンに、遷移元の情報をセットする
        Utility.beforeSceneName = DungeonManager.dungeonManager.NowScene;

        // 次のシーンの情報を取得する
        int nextSceneNo = dungeonObjects.mapPlayer.NextSceneNo;
        string nextSceneName;

        // バトルの場合
        if(nextSceneNo == Define.DUNGEON_NEXT_BATTLE_SCENE_NO)
        {
            // マップキャラクターを記憶する
            DungeonManager.dungeonManager.MapAllCharacterMemory();

            // 戦闘曲が設定されていなければ、通常戦闘曲を設定する
            if(GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm == BGMType.None)
            {
                GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm = DungeonManager.dungeonManager.NormalBattleBgm;
            }

            // 戦闘背景が設定されていなければ、通常背景を設定する
            if (GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBackground == null)
            {
                GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBackground = DungeonManager.dungeonManager.NormalBattleBackground;
            }

            // 遷移先をバトルに設定する
            nextSceneName = SceneName.Battle.ToString();
        }
        // バトル以外の遷移
        else // バトル遷移以外であれば、
        {
            // マップキャラクター位置の一時保存データを消去する
            Utility.MapEnemyAndCharacterClear();

            // トークテーブルをリセットする(バトルではリセットしない)
            GlobalCanvasManager.globalCanvasManager.talkWindow.TalkTableSceneReset();

            // 遷移先の情報を設定する
            nextSceneName = DungeonManager.dungeonManager.sceneChangeTableSO.sceneChangeTableData[nextSceneNo].NextSceneName.ToString();
            Utility.nextSceneObjectNumber = DungeonManager.dungeonManager.sceneChangeTableSO.sceneChangeTableData[nextSceneNo].NextObjectNumber;
        }

        // 各種データをセーブする
        GlobalCanvasManager.globalCanvasManager.playerPanel.PlayerSave();

        SceneManager.LoadScene(nextSceneName);

        yield return null;
    }
}
