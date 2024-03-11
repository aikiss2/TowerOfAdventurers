using System.Collections;
using UnityEngine;

// スキル屋シーンの習得スキル選択フェーズクラス
public class SukiruyaChooseSkillPhase : SukiruyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return SukiruyaManager.sukiruyaManager.WaitPlayerInputNoClickRect(playerInput, sukiruyaObjects.skillPanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

            // 選択されたスキルのIndex値を取得
            int currentID = sukiruyaObjects.skillPanel.CurrentID;

            // スキルパネルを閉じ、プレイヤーパネルを開く
            sukiruyaObjects.skillPanel.Close();
            GlobalCanvasManager.globalCanvasManager.playerPanel.Open();

            // スキルを習得させる味方選択フェーズに遷移する。選択されたスキルのIndex値を設定する。
            next = new SukiruyaChooseTargetPhase();
            next.SelectSkillIndex = currentID;
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // スキルパネルを閉じ、項目選択パネルを開く
            sukiruyaObjects.skillPanel.Close();
            sukiruyaObjects.selectButtonPanel.Open(1);

            next = new SukiruyaChoosePhase();
        }
        else
        {
            next = new SukiruyaChooseSkillPhase();
        }

        yield return null;
    }

}
