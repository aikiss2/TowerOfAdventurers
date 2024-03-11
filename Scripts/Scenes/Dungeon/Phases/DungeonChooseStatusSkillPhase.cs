using System.Collections;
using UnityEngine;

// ダンジョンシーンのキャラ図鑑のスキル選択フェーズクラス
public class DungeonChooseStatusSkillPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return DungeonManager.dungeonManager.WaitPlayerInputNoClickRect(playerInput, dungeonObjects.charaSkillPanel.GetScrollbarRect());
        // 決定・キャンセル(決定・キェンセルの両方で、キャラ選択フェーズに戻るというキャンセル動作をする)
        if ((playerInput[0] == PlayerInput.Decide) || (playerInput[0] == PlayerInput.Cancel))
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // スキルパネルを非選択にし、キャラ名パネルを選択状態にする
            dungeonObjects.charaSkillPanel.DeSelect();
            dungeonObjects.charaNamePanel.Select();

            next = new DungeonChooseStatusCharaPhase();
        }
        else
        {
            next = new DungeonChooseStatusSkillPhase();
        }

        yield return null;
    }
}
