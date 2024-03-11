using System.Collections;
using UnityEngine;

// ダンジョンシーンのスキル対象選択フェーズクラス
public class DungeonChooseTargetPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return DungeonManager.dungeonManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            // 選択しているスキルの所持プレイヤーを取得する
            var player = dungeonObjects.mapSkillPanel.GetNowBattleStatus();

            // スキル対象を確定させる
            player.chooseTarget = GlobalCanvasManager.globalCanvasManager.playerPanel.GetSelectPlayer();
            player.SetTarget();

            // スキルを実行する
            yield return player.SkillExecute(GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList(), null, null);

            // プレイヤーパネルを非選択にし、マップスキルパネルを選択状態にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            dungeonObjects.mapSkillPanel.Select();

            next = new DungeonChooseSkillPhase();
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // プレイヤーパネルを非選択にし、マップスキルパネルを選択状態にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            dungeonObjects.mapSkillPanel.Select();

            next = new DungeonChooseSkillPhase();
        }
        else
        {
            next = new DungeonChooseTargetPhase();
        }

        yield return null;
    }

}
