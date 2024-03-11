using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タウンシーンの「出発」「宿屋」「スキル屋」・・・選択フェーズクラス
public class TownChoosePhase : TownPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TownObjects townObjects)
    {
        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return TownManager.townManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = townObjects.selectButtonPanel.CurrentID;

            // 出発
            if (currentID == 0 )
            {
                SoundManager.soundManager.SEPlay(SEType.SeNo37);

                // フェードアウトしてから、ダンジョンシーンに遷移
                yield return TownManager.townManager.Fadeout();
                Utility.nextScene = SceneName.Dungeon1_1;

                next = new TownEndPhase();
            }
            // 宿屋
            else if(currentID == 1 )
            {
                SoundManager.soundManager.SEPlay(SEType.SeNo39);

                // フェードアウトしてから、宿屋シーンに遷移
                yield return TownManager.townManager.Fadeout();
                Utility.nextScene = SceneName.Yadoya;

                next = new TownEndPhase();
            }
            // スキル屋
            else if (currentID == 2)
            {
                SoundManager.soundManager.SEPlay(SEType.SeNo40);

                // フェードアウトしてから、スキル屋シーンに遷移
                yield return TownManager.townManager.Fadeout();
                Utility.nextScene = SceneName.Sukiruya;

                next = new TownEndPhase();
            }
            // キャラ辞典
            else if (currentID == 3)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 各種パネルを閉じる
                townObjects.selectButtonPanel.Close();
                GlobalCanvasManager.globalCanvasManager.goldPanel.Close();
                GlobalCanvasManager.globalCanvasManager.playerPanel.Close();

                // キャラ辞典のキャラ名称のリストを作成する
                List<string> charaNameList = new();

                // プレイヤーキャラ名を追加
                foreach( var one in GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData)
                {
                    charaNameList.Add(one.playerName);
                }

                // 敵キャラは出会ってるキャラは名称を、出会っていないキャラは？？？を追加する
                int cnt = 0;
                foreach( var one in GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData)
                {
                    if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[cnt] == true)
                    {
                        charaNameList.Add(one.Name);
                    }
                    else
                    {
                        charaNameList.Add("？？？");
                    }
                    cnt++;
                }

                // 最下段に戻るボタンも追加
                charaNameList.Add("戻る");

                // キャラ辞典の各パネルに初期値として主人公のデータを設定する
                townObjects.charaImagePanel.SetCharaImage(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].FullBodyTexture);
                townObjects.charaImagePanel.Open();

                townObjects.statusPanel.SetPlayerStatus(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0]);
                townObjects.statusPanel.Open();

                townObjects.charaSkillPanel.CreateSelectableText(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].skills);
                townObjects.charaSkillPanel.Open();
                townObjects.charaSkillPanel.DeSelect();

                townObjects.charaStoryPanel.SetText(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].story);
                townObjects.charaStoryPanel.Open();

                townObjects.charaNamePanel.CreateSelectableText(charaNameList.ToArray());
                townObjects.charaNamePanel.Open();

                next = new TownChooseCharaPhase();
            }
            // オプション
            else if (currentID == 4)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // オプション画面を開き、オプションフェーズに遷移
                townObjects.selectButtonPanel.DeSelect();
                townObjects.optionSettingPanel.SetOptionSettingWindow(GlobalCanvasManager.globalCanvasManager.optionSettingDbSO);
                townObjects.optionSettingPanel.Open();

                next = new TownOptionSettingPhase();
            }
            else
            {
                next = new TownChoosePhase();
            }

        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // デフォルトボタン選択(出発)
            townObjects.selectButtonPanel.ManualSelectButton(0);

            next = new TownChoosePhase();
        }
        else
        {
            next = new TownChoosePhase();
        }

        yield return null;
    }

}
