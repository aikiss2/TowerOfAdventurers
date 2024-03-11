using System.Collections;
using UnityEngine;

// タウンシーンのキャラ図鑑のキャラ選択フェーズクラス
public class TownChooseCharaPhase : TownPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TownObjects townObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return TownManager.townManager.WaitPlayerInputNoClickRect(playerInput, townObjects.charaNamePanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            int currentID = townObjects.charaNamePanel.CurrentID;

            int enemyIndex = 0;
            // 敵を選択している場合、敵のインデックス値を算出
            if (currentID >= GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count)
            {
                //現在の味方の数で減算し、敵のIndex値を算出
                enemyIndex = currentID - GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count;
            }

            // 戻る選択
            if (townObjects.charaNamePanel.IsLastRow())
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

                // 選択フェーズに戻る処理
                ReturnChoosePhaseProcess(townObjects);

                next = new TownChoosePhase();
            }
            // プレイヤー選択
            else if (currentID < GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.playerPartyDbData.Count)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // スキル選択フェーズに向かう処理
                MoveChooseSkillProcess(townObjects);

                next = new TownChooseSkillPhase();
            }
            // 出会っている敵選択
            else if (GlobalCanvasManager.globalCanvasManager.playerPartyDbSO.encountEnemyList[enemyIndex])
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                // スキル選択フェーズに向かう処理
                MoveChooseSkillProcess(townObjects);

                next = new TownChooseSkillPhase();
            }
            // 出会っていない敵選択
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);

                GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("まだ出会っていません。");

                next = new TownChooseCharaPhase();
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // 選択フェーズに戻る処理
            ReturnChoosePhaseProcess(townObjects);

            next = new TownChoosePhase();
        }
        else
        {
            next = new TownChooseCharaPhase();
        }

        yield return null;
    }


    // 選択フェーズに戻る処理を実行する
    private void ReturnChoosePhaseProcess(TownObjects townObjects)
    {
        // キャラ図鑑の各種パネルを閉じる
        townObjects.charaNamePanel.Close();
        townObjects.charaImagePanel.Close();
        townObjects.charaSkillPanel.Close();
        townObjects.statusPanel.Close();
        townObjects.charaSkillPanel.Close();
        townObjects.charaStoryPanel.Close();

        // 選択フェーズのパネルを開く
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        townObjects.selectButtonPanel.Open(4);
    }


    // スキル選択フェーズに向かう処理を実行する
    private void MoveChooseSkillProcess(TownObjects townObjects)
    {
        // キャラ名パネルを非選択にし、スキルパネルを開く
        townObjects.charaNamePanel.DeSelect();
        townObjects.charaSkillPanel.Open();
    }
}
