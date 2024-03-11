using System.Collections;
using UnityEngine;

// 宿屋シーンの「泊まる」「会話」「戻る」の選択フェーズクラス
public class YadoyaChoosePhase : YadoyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(YadoyaObjects yadoyaObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return YadoyaManager.yadoyaManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = yadoyaObjects.selectButtonPanel.CurrentID;

            //泊まる
            if (currentID == 0)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 選択パネルを非選択にする
                yadoyaObjects.selectButtonPanel.DeSelect();

                // 泊まるかどうかの確認メッセージを表示させ、はい/いいえの回答をプレイヤーから待つ
                bool[] yesAnswer = new bool[1];
                yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalkYesNo(TalkTableIndex.Yadoya1, yesAnswer);
                // はい
                if (yesAnswer[0] == true)
                {
                    // 所持金が宿屋料金を上回っている場合は料金を払い、下回っている場合は会話を表示し、無料で泊まる
                    if(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold >= YadoyaManager.yadoyaManager.YadoyaSpendGold)
                    {
                        GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.partyGold -= YadoyaManager.yadoyaManager.YadoyaSpendGold;
                        GlobalCanvasManager.globalCanvasManager.goldPanel.GoldUpdate();
                    }
                    else
                    {
                        yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(TalkTableIndex.Yadoya2);
                    }

                    // １泊BGMを鳴らしながら暗転させ、数秒待つ
                    SoundManager.soundManager.PlayBgm(BGMType.BgmNo13, false);
                    yield return YadoyaManager.yadoyaManager.Fadeout();
                    yield return new WaitForSeconds(5f);

                    // フェードインしながら通常BGMにし、回復SEを鳴らす
                    YadoyaManager.yadoyaManager.Fadein();
                    SoundManager.soundManager.PlayBgm(YadoyaManager.yadoyaManager.DefaultBgm);
                    yield return new WaitForSeconds(0.3f);
                    SoundManager.soundManager.SEPlay(SEType.SeNo1);

                    // 味方プレイヤー全員を回復させる
                    foreach (BattleCharacter one in GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList())
                    {
                        if (one.IsDead)
                        {
                            one.Rebirth();
                        }

                        one.Heal(Define.HP_MAX_LIMIT, true);
                        one.HealTp(Define.TP_MAX_LIMIT);
                    }
                }
                // いいえ
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);
                }

                // 選択パネルを選択状態にする
                yadoyaObjects.selectButtonPanel.Select();

                next = new YadoyaChoosePhase();
            }
            // 会話
            else if (currentID == 1)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 選択パネルを非選択にする
                yadoyaObjects.selectButtonPanel.DeSelect();

                // 会話を表示する
                yield return GlobalCanvasManager.globalCanvasManager.talkWindow.TypeTalk(TalkTableIndex.Yadoya_Talk);

                // 選択パネルを選択状態にする
                yadoyaObjects.selectButtonPanel.Select();

                next = new YadoyaChoosePhase();
            }
            // 戻る
            else if(currentID == 2)
            {
                SoundManager.soundManager.SEPlay(SEType.SeNo39);

                // フェードアウトしてから、タウンシーンに遷移
                yield return YadoyaManager.yadoyaManager.Fadeout();
                Utility.nextScene = SceneName.Town;

                next = new YadoyaEndPhase();
            }
            else
            {
                next = new YadoyaChoosePhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // デフォルトボタン選択(戻る)
            yadoyaObjects.selectButtonPanel.ManualSelectButton(2);

            next = new YadoyaChoosePhase();
        }
        else
        {
            next = new YadoyaChoosePhase();
        }

        yield return null;
    }

}
