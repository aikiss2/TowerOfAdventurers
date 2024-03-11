using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スキル屋シーンの返却スキル選択フェーズクラス
public class SukiruyaChooseOffSkillPhase : SukiruyaPhaseBase
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

            // 選択されたスキルをストックスキルにする。
            List <SkillSO> moveAbleSkills = SelectPlayer.GetMoveAbleSkills();
            SkillStockDbData findSkill = GlobalCanvasManager.globalCanvasManager.skillStockDbSO.skillStockDbData.Find(item => item.skill == moveAbleSkills[currentID]);
            findSkill.stockCondition = StockCondition.Stocked;

            // 選択されたスキルを習得していた味方から削除する
            SelectPlayer.DelateSkill(moveAbleSkills[currentID]);
            SelectPlayer.SetSkill();

            // 返却できるスキルがもう無い場合はプレイヤー選択に戻る
            if (SelectPlayer.GetMoveAbleSkills().Count == 0)
            {
                sukiruyaObjects.skillPanel.Close();
                GlobalCanvasManager.globalCanvasManager.playerPanel.Select();

                next = new SukiruyaChooseOffTargetPhase();
            }
            // まだ削除できるスキルがあれば、スキル選択を継続
            else
            {
                // 表示しているスキルをクリアする
                sukiruyaObjects.skillPanel.Clear();

                // パネル内容を変更する場合はWAITを入れる
                yield return new WaitForSeconds(0.05f);

                // 返却可能スキルを表示する
                sukiruyaObjects.skillPanel.CreateSelectableTextWithTp(SelectPlayer.GetMoveAbleSkills());
                sukiruyaObjects.skillPanel.Open();

                // 返却スキル選択フェーズを継続する。選択された味方のIndex値を設定する。
                next = new SukiruyaChooseOffSkillPhase();
                next.SelectPlayer = SelectPlayer;
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // スキルパネルを閉じ、プレイヤーパネルを選択状態にする。
            sukiruyaObjects.skillPanel.Close();
            GlobalCanvasManager.globalCanvasManager.playerPanel.Select();

            next = new SukiruyaChooseOffTargetPhase();
        }
        else
        {
            next = new SukiruyaChooseOffSkillPhase();
            next.SelectPlayer = SelectPlayer;
        }

        yield return null;
    }

}
