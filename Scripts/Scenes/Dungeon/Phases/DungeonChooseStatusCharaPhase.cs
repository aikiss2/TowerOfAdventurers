using System.Collections;
using UnityEngine;

// ダンジジョンシーンのキャラ図鑑のキャラ選択フェーズクラス
public class DungeonChooseStatusCharaPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return DungeonManager.dungeonManager.WaitPlayerInputNoClickRect(playerInput, dungeonObjects.charaNamePanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = dungeonObjects.charaNamePanel.CurrentID;

            int enemyIndex = 0;
            // 敵を選択している場合、敵のインデックス値を算出
            if (currentID >= GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count)
            {
                //現在の味方の数で減算し、敵のIndex値を算出
                enemyIndex = currentID - GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count;
            }

            // 戻る選択
            if (dungeonObjects.charaNamePanel.IsLastRow())
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

                // 選択フェーズに戻る処理
                ReturnChoosePhaseProcess(dungeonObjects);

                next = new DungeonChoosePhase();
            }
            // プレイヤー選択
            else if (currentID < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // スキル選択フェーズに向かう処理
                MoveChooseSkillProcess(dungeonObjects);

                next = new DungeonChooseStatusSkillPhase();
            }
            // 出会っている敵選択
            else if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[enemyIndex])
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // スキル選択フェーズに向かう処理
                MoveChooseSkillProcess(dungeonObjects);

                next = new DungeonChooseStatusSkillPhase();
            }
            // 出会っていない敵選択
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);

                GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("まだ出会っていません。");

                next = new DungeonChooseStatusCharaPhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // 選択フェーズに戻る処理
            ReturnChoosePhaseProcess(dungeonObjects);

            next = new DungeonChoosePhase();
        }
        else
        {
            next = new DungeonChooseStatusCharaPhase();
        }

        yield return null;
    }


    // 選択フェーズに戻る処理を実行する
    private void ReturnChoosePhaseProcess(DungeonObjects dungeonObjects)
    {
        // キャラ図鑑の各種パネルを閉じる
        dungeonObjects.charaNamePanel.Close();
        dungeonObjects.charaImagePanel.Close();
        dungeonObjects.charaSkillPanel.Close();
        dungeonObjects.statusPanel.Close();
        dungeonObjects.charaSkillPanel.Close();
        dungeonObjects.charaStoryPanel.Close();

        // 選択フェーズのパネルを開く
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        dungeonObjects.selectButtonPanel.Open(2);
    }


    // スキル選択フェーズに向かう処理を実行する
    private void MoveChooseSkillProcess(DungeonObjects dungeonObjects)
    {
        // キャラ名パネルを非選択にし、スキルパネルを開く
        dungeonObjects.charaNamePanel.DeSelect();
        dungeonObjects.charaSkillPanel.Open();
    }
}
