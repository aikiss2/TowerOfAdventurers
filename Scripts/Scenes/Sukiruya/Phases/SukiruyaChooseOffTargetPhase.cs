using System.Collections;
using UnityEngine;

// スキル屋シーンのスキル返却する味方選択フェーズクラス
public class SukiruyaChooseOffTargetPhase : SukiruyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return SukiruyaManager.sukiruyaManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            // 選択された味方を取得
            var selectPlayer = GlobalCanvasManager.globalCanvasManager.playerPanel.GetSelectPlayer();

            // 返却できるスキルがない場合はプレイヤー選択を継続する
            if (selectPlayer.GetMoveAbleSkills().Count == 0)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("外せるスキルがありません。");
                next = new SukiruyaChooseOffTargetPhase();
            }
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // プレイヤーパネルを非選択にする。
                GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();

                // 返却可能スキルをスキルパネルに表示する
                sukiruyaObjects.skillPanel.CreateSelectableTextWithTp(selectPlayer.GetMoveAbleSkills());
                sukiruyaObjects.skillPanel.Open();

                // 返却するスキル選択フェーズに遷移する。選択された味方のIndex値を設定する。
                next = new SukiruyaChooseOffSkillPhase();
                next.SelectPlayer = selectPlayer;
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // プレイヤーパネルを非選択にし、項目選択パネルを開く
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            sukiruyaObjects.selectButtonPanel.Open(2);

            next = new SukiruyaChoosePhase();
        }
        else
        {
            next = new SukiruyaChooseOffTargetPhase();
        }

        yield return null;
    }

}
