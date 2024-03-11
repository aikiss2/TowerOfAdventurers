using System.Collections;
using UnityEngine;

// スキル屋シーンのスキル習得プレイヤー選択フェーズクラス
public class SukiruyaChooseTargetPhase : SukiruyaPhaseBase
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
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

            // 選択された味方を取得
            int currentID = GlobalCanvasManager.globalCanvasManager.playerPanel.CurrentID;
            var players = GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList();

            // 選択されたスキルを味方に習得させ、スキルを「使用中」にする。
            var stockSkills = GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetStockSkills();
            players[currentID].skills.Add(stockSkills[SelectSkillIndex].skill);
            stockSkills[SelectSkillIndex].stockCondition = StockCondition.Use;
            players[currentID].SetSkill();

            // プレイヤーパネルを非選択にする。
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();

            // ストックスキルが0になったら、項目選択フェーズまで戻る。
            if(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfStockSkills().Count == 0)
            {
                sukiruyaObjects.selectButtonPanel.Open(1);
                next = new SukiruyaChoosePhase();
            }
            // ストックスキルがまだあれば、習得スキル選択フェーズに戻る。
            else
            {
                // ストックスキルをスキルパネルに表示する
                sukiruyaObjects.skillPanel.CreateSelectableTextWithTp(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfStockSkills());
                sukiruyaObjects.skillPanel.Open();

                next = new SukiruyaChooseSkillPhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // プレイヤーパネルを非選択にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();

            // ストックスキルをスキルパネルに表示する
            sukiruyaObjects.skillPanel.CreateSelectableTextWithTp(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfStockSkills());
            sukiruyaObjects.skillPanel.Open();

            next = new SukiruyaChooseSkillPhase();
        }
        else
        {
            next = new SukiruyaChooseTargetPhase();
            next.SelectSkillIndex = SelectSkillIndex;
        }

        yield return null;
    }

}
