using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// バトルシーンの終了フェーズクラス
public class BattleEndPhase : BattlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        // フェードアウトする
        yield return BattleManager.battleManager.Fadeout();

        // 各種データをセーブする
        GlobalCanvasManager.globalCanvasManager.playerPanel.PlayerSave();

        // 遷移先の指定がある(全滅)場合、そこに遷移する
        if (Utility.nextScene != SceneName.None)
        {
            // イベントをリセットしておく(ボス戦等のイベントを進めさせない)
            Utility.nextEvent = EventTableIndex.None;
            Utility.beforeSceneName = SceneName.Battle;

            // 次シーンの文字列を生成する
            var nextSceneString = Utility.nextScene.ToString();
            Utility.nextScene = SceneName.None;

            // 次のシーンを読み込み、遷移する
            SceneManager.LoadScene(nextSceneString);
        }
        // 遷移先の指定がない(戦闘勝利)場合、バトル前のシーンに戻る
        else
        {
            // イベントが設定されている場合は、プレイヤーデータベースにイベントを設定する(ボスに勝利等)
            if (Utility.nextEvent != EventTableIndex.None)
            {
                GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.eventIndex = Utility.nextEvent;
            }
            Utility.nextEvent = EventTableIndex.None;

            // 次シーンの文字列を生成する
            var beforeSceneNameString = Utility.beforeSceneName.ToString();
            Utility.beforeSceneName = SceneName.Battle;

            // 次のシーンを読み込み、遷移する
            SceneManager.LoadScene(beforeSceneNameString);
        }

        yield return null;
    }
}
