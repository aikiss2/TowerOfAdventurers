using System.Collections;
using UnityEngine;
using static SkillSO;

// バトルシーンのスキル対象選択フェーズクラス
public class BattleChooseTargetPhase : BattlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
        // ターゲット選択が不要のスキルの場合は次のフェーズに遷移する
        if ( (alivePlayers[NowPlayerIndex].selectSkill.TargetType != TargetType.FriendChoose) 
            && (alivePlayers[NowPlayerIndex].selectSkill.TargetType != TargetType.OpponentChoose) 
            && (alivePlayers[NowPlayerIndex].selectSkill.TargetType != TargetType.DeadFriendChoose))
        {
            // 味方・敵パネルを非選択にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            battleObjects.enemyPanel.DeSelect();

            // 次のフェーズに進む処理を実施する
            NextPhaseProcess(battleObjects);

            yield break;
        }

        // プレイヤーからの入力待ち
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return BattleManager.battleManager.WaitPlayerInput(playerInput);
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            // 決定音ではうるさいため、選択音
            SoundManager.soundManager.SEPlay(SEType.SeNo2); 

            // 味方選択
            if ((alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.FriendChoose)
                || (alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.DeadFriendChoose))
            {
                alivePlayers[NowPlayerIndex].chooseTarget = GlobalCanvasManager.globalCanvasManager.playerPanel.GetSelectPlayer() ;
            }
            // 敵選択
            else if(alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.OpponentChoose)
            {
                alivePlayers[NowPlayerIndex].chooseTarget = battleObjects.enemyPanel.GetSelectEnemy();
            }

            // 味方・敵パネルを非選択にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            battleObjects.enemyPanel.DeSelect();

            // 次のフェーズに進む処理を実施する
            NextPhaseProcess(battleObjects);
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // 味方・敵パネルを非選択にする
            GlobalCanvasManager.globalCanvasManager.playerPanel.DeSelect();
            battleObjects.enemyPanel.DeSelect();

            // スキル選択中の味方のスキルを表示する
            battleObjects.skillPanel.CreateSelectableTextWithTp(alivePlayers[NowPlayerIndex].skills);
            battleObjects.skillPanel.Open(alivePlayers[NowPlayerIndex]);

            // スキル選択フェーズに戻る
            next = new BattleChooseSkillPhase();
            next.NowPlayerIndex = NowPlayerIndex;
        }
        else
        {
            next = new BattleChooseTargetPhase();
            next.NowPlayerIndex = NowPlayerIndex;
        }

        yield return null;
    }


    // 次のフェーズに進む処理を実施する
    private void NextPhaseProcess(BattleObjects battleContext)
    {
        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();

        // ターゲットを確定させる
        alivePlayers[NowPlayerIndex].SetTarget();

        // スキル選択中の味方を次に進める
        NowPlayerIndex++;

        // スキル選択中の味方が最後尾だった場合、敵スキル選択フェーズに移行する
        if (NowPlayerIndex >= alivePlayers.Count)
        {
            next = new BattleEnemySkillPhase();
        }
        // まだスキル未選択の味方がいる場合は、次の味方のスキルを表示する
        else
        {
            battleContext.skillPanel.CreateSelectableTextWithTp(alivePlayers[NowPlayerIndex].skills);
            battleContext.skillPanel.Open(alivePlayers[NowPlayerIndex]);

            next = new BattleChooseSkillPhase();
            next.NowPlayerIndex = NowPlayerIndex;
        }
    }
}
