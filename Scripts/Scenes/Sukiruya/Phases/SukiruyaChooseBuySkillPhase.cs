using System.Collections;
using UnityEngine;

// スキル屋シーンの購入スキル選択フェーズクラス
public class SukiruyaChooseBuySkillPhase : SukiruyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return SukiruyaManager.sukiruyaManager.WaitPlayerInputNoClickRect(playerInput, sukiruyaObjects.buySkillPanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

            // 選択されたスキルのIndex値を取得
            int currentID = sukiruyaObjects.buySkillPanel.CurrentID;

            // 購入スキルパネルを閉じる
            sukiruyaObjects.buySkillPanel.Close();

            // 購入するかどうかの確認メッセージを表示させ、はい/いいえの回答をプレイヤーから待つ
            bool[] yesAnswer = new bool[1];
            yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalkYesNo(TalkTableIndex.Sukiruya1, yesAnswer);
            // はい
            if (yesAnswer[0] == true)
            {
                var stockSkills = GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetNoBuySkills();

                // お金が足りている場合、購入したスキルをストック状態にする
                if (stockSkills[currentID].skill.SpendGold <= GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold)
                {
                    SoundManager.soundManager.SEPlay(SEType.SeNo38);
                    stockSkills[currentID].stockCondition = StockCondition.Stocked;
                    GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold -= stockSkills[currentID].skill.SpendGold;
                    GlobalCanvasManager.globalCanvasManager.goldPanel.GoldUpdate();
                }
                // お金が足りていない場合、購入できなかった旨を表示する
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                    yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(TalkTableIndex.Sukiruya2);
                } 
            }
            // いいえ
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);
            }

            // 購入スキルがなくなった場合、項目選択フェーズに戻る
            if(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfNoBuySkills().Count == 0)
            {
                sukiruyaObjects.selectButtonPanel.Open();
                next = new SukiruyaChoosePhase();
            }
            // 購入スキルがまだある場合は、購入スキルパネルを表示する
            else
            {
                sukiruyaObjects.buySkillPanel.CreateSelectableTextWithGold(GlobalCanvasManager.globalCanvasManager.skillStockDbSO.GetSkillSoOfNoBuySkills());
                sukiruyaObjects.buySkillPanel.Open();
                next = new SukiruyaChooseBuySkillPhase();
            }

        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // 購入スキルパネルを閉じ、項目選択パネルを開く
            sukiruyaObjects.buySkillPanel.Close();
            sukiruyaObjects.selectButtonPanel.Open();

            next = new SukiruyaChoosePhase();
        }
        else
        {
            next = new SukiruyaChooseBuySkillPhase();
        }

        yield return null;
    }

}
