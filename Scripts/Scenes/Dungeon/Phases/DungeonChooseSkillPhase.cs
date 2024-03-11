using System.Collections;
using UnityEngine;
using static SkillSO;

// ダンジョンシーンのマップスキル選択フェーズクラス
public class DungeonChooseSkillPhase : DungeonPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(DungeonObjects dungeonObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return DungeonManager.dungeonManager.WaitPlayerInputNoClickRect(playerInput, dungeonObjects.mapSkillPanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            // キャンセルを選んだ場合
            if ( dungeonObjects.mapSkillPanel.IsCancel() == true)
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

                // マップスキルパネルを閉じ、項目選択パネルを開く
                dungeonObjects.mapSkillPanel.Close();
                dungeonObjects.selectButtonPanel.Open();

                next = new DungeonChoosePhase();
            }
            // スキルを選んだ場合
            else
            {
                
                string reason = "";
                // 選択スキルが使用可能か判定する。使用不可の場合はrezsonに理由が格納される
                // 使用可の場合
                if (dungeonObjects.mapSkillPanel.GetUseable(ref reason))
                {
                    // 選択しているスキルの所持プレイヤーとスキル内容を取得する
                    var player = dungeonObjects.mapSkillPanel.GetNowBattleStatus();
                    player.selectSkill = dungeonObjects.mapSkillPanel.GetNowSkill();

                    // マップスキルを非選択状態にする
                    dungeonObjects.mapSkillPanel.DeSelect();

                    // 味方選択が必要な場合
                    if ((player.selectSkill.TargetType == TargetType.FriendChoose) || (player.selectSkill.TargetType == TargetType.DeadFriendChoose))
                    {
                        SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Decide);

                        // プレイヤーパネルを選択状態で開く
                        GlobalCanvasManager.globalCanvasManager.playerPanel.Open();

                        next = new DungeonChooseTargetPhase();
                    }
                    // 味方選択が不要の場合
                    else
                    {
                        // スキルを実行する
                        yield return player.SkillExecute(GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList(), null, null);

                        // マップスキルパネルを選択状態にする
                        dungeonObjects.mapSkillPanel.Select();

                        next = new DungeonChooseSkillPhase();
                    }
                }
                // 使用不可の場合
                else
                {
                    SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);

                    // 使用不可の理由をダイアログに表示
                    GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog(reason);

                    next = new DungeonChooseSkillPhase();
                }
            }

        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // マップスキルパネルを閉じ、項目選択パネルを開く
            dungeonObjects.mapSkillPanel.Close();
            dungeonObjects.selectButtonPanel.Open();

            next = new DungeonChoosePhase();
        }
        else
        {
            next = new DungeonChooseSkillPhase();
        }

        yield return null;
    }

}
