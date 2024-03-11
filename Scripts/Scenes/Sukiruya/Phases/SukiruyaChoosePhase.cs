using System.Collections;
using UnityEngine;

public class SukiruyaChoosePhase : SukiruyaPhaseBase
{
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return SukiruyaManager.sukiruyaManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = sukiruyaObjects.selectButtonPanel.CurrentID;

            // 購入
            if (currentID == 0)
            {
                // 未購入スキルがない場合は、購入できない旨を表示し、本フェーズを継続する
                if (GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfNoBuySkills().Count == 0)
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                    GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("購入できるスキルがありません。");
                    next = new SukiruyaChoosePhase();
                }
                // 未購入スキルがある場合は、購入スキル選択フェーズに遷移する
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                    // 項目選択パネルを閉じる
                    sukiruyaObjects.selectButtonPanel.Close();

                    // 未購入スキルを購入スキルパネルに表示する
                    sukiruyaObjects.buySkillPanel.CreateSelectableTextWithGold(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfNoBuySkills());
                    sukiruyaObjects.buySkillPanel.Open();

                    next = new SukiruyaChooseBuySkillPhase();
                }
            }
            // 習得
            else if (currentID == 1 )
            {
                // ストックスキルがない場合は、習得できない旨を表示し、本フェーズを継続する
                if (GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfStockSkills().Count == 0)
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                    GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("習得できるスキルがありません。");
                    next = new SukiruyaChoosePhase();
                }
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                    // 項目選択パネルを閉じる
                    sukiruyaObjects.selectButtonPanel.Close();

                    // ストックスキルをスキルパネルに表示する
                    sukiruyaObjects.skillPanel.CreateSelectableTextWithTp(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfStockSkills());
                    sukiruyaObjects.skillPanel.Open();

                    next = new SukiruyaChooseSkillPhase();
                }
            }
            // 返却
            else if (currentID == 2)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 項目選択パネルを閉じ、プレイヤーパネルを開く
                sukiruyaObjects.selectButtonPanel.Close();
                GlobalCanvasManager.globalCanvasManager.playerPanel.Open();

                next = new SukiruyaChooseOffTargetPhase();
            }
            // 会話
            else if ( currentID == 3)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 選択パネルを非選択にする
                sukiruyaObjects.selectButtonPanel.DeSelect();

                // 会話を表示する
                yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(TalkTableIndex.Sukiruya_Talk);

                // 選択パネルを選択状態にする
                sukiruyaObjects.selectButtonPanel.Select();

                next = new SukiruyaChoosePhase();
            }
            // 戻る
            else if(currentID == 4)
            {
                SoundManager.soundManager.SEPlay(SEType.SeNo40);

                // フェードアウトしてから、タウンシーンに遷移
                yield return SukiruyaManager.sukiruyaManager.Fadeout();
                Utility.nextScene = SceneName.Town;

                next = new SukiruyaEndPhase();
            }
            else
            {
                next = new SukiruyaChoosePhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // デフォルトボタン選択(戻る)
            sukiruyaObjects.selectButtonPanel.ManualSelectButton(4);

            next = new SukiruyaChoosePhase();
        }
        else
        {
            next = new SukiruyaChoosePhase();
        }

        yield return null;
    }

}
