using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ダンジョンシーンの項目選択フェーズクラス
public class DungeonChoosePhase : DungeonPhaseBase
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
            int currentID = dungeonObjects.selectButtonPanel.CurrentID;

            //スキル
            if (currentID == 0)
            {
                // マップスキルパネルを開く
                bool isOpen = dungeonObjects.mapSkillPanel.Open(GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList());

                // マップスキルがあり、パネルを開いた場合はスキル選択フェーズに遷移する
                if (isOpen == true)
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);
                    dungeonObjects.selectButtonPanel.Close();
                    next = new DungeonChooseSkillPhase();
                }
                // マップスキルがなく、パネルを開かなかった場合は、開けなかった旨のダイアログを表示
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                    GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("マップで使えるスキルがありません。");
                    next = new DungeonChoosePhase();
                }

            }
            // キャラ辞典
            else if (currentID == 1)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // 各種パネルを閉じる
                dungeonObjects.selectButtonPanel.Close();
                GlobalCanvasManager.globalCanvasManager.goldPanel.Close();
                GlobalCanvasManager.globalCanvasManager.playerPanel.Close();

                // キャラ辞典のキャラ名称のリストを作成する
                List<string> charaNameList = new();

                // プレイヤーキャラを追加
                foreach (var one in GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData)
                {
                    charaNameList.Add(one.playerName);
                }

                // 敵キャラは出会ってるキャラは名称を、出会っていないキャラは？？？を追加する
                int cnt = 0;
                foreach (var one in GlobalCanvasManager.globalCanvasManager.enemyStatusTableSO.enemyStatusTableData)
                {
                    if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[cnt])
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
                dungeonObjects.charaImagePanel.SetCharaImage(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].FullBodyTexture);
                dungeonObjects.charaImagePanel.Open();

                dungeonObjects.statusPanel.SetPlayerStatus(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0]);
                dungeonObjects.statusPanel.Open();

                dungeonObjects.charaSkillPanel.CreateSelectableText(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].skills);
                dungeonObjects.charaSkillPanel.Open();
                dungeonObjects.charaSkillPanel.DeSelect();

                dungeonObjects.charaStoryPanel.SetText(GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData[0].story);
                dungeonObjects.charaStoryPanel.Open();

                dungeonObjects.charaNamePanel.CreateSelectableText(charaNameList.ToArray());
                dungeonObjects.charaNamePanel.Open();
                dungeonObjects.charaNamePanel.Select();

                next = new DungeonChooseStatusCharaPhase();
            }
            // キャンセル
            else if (currentID == 2) 
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

                // マップ移動フェーズに戻る処理
                ReturnMapMoveProcess(dungeonObjects);

                // ウィンドウを消した直後にキャラが動かないように少しウェイトする
                yield return new WaitForSeconds(0.2f);

                // マップのキャラクターを動けるようにする
                DungeonCharacterDontMoveSet(false, dungeonObjects);

                next = new DungeonMapMovePhase();
            }
            else
            {
                next = new DungeonChoosePhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // マップ移動フェーズに戻る処理
            ReturnMapMoveProcess(dungeonObjects);

            // マップのキャラクターを動けるようにする
            DungeonCharacterDontMoveSet(false, dungeonObjects);

            next = new DungeonMapMovePhase();
        }
        else
        {
            next = new DungeonChoosePhase();
        }

        yield return null;
    }


    // マップ移動フェーズに戻る処理
    private void ReturnMapMoveProcess(DungeonObjects dungeonObjects)
    {
        // 各種パネルを閉じる
        dungeonObjects.selectButtonPanel.Close();
        GlobalCanvasManager.globalCanvasManager.dialog.Close();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Close();

        // メニューボタンを表示する
        dungeonObjects.openMenuButton.gameObject.SetActive(true);
    }

}
