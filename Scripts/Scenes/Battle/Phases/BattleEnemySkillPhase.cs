using System.Collections;
using UnityEngine;
using static SkillSO;

// バトルシーンの敵キャラクターのスキル選択フェーズクラス
public class BattleEnemySkillPhase : BattlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        var enemies = battleObjects.enemyPanel.GetEnemyList();
        // 全ての敵キャラクターでスキルを選択する
        foreach (var one in enemies)
        {
            // このランダム値でスキルを決定する
            int randomCount = Random.Range(0, 100);

            // スキル選択確率にミスがあった場合に備え、先頭スキルを設定しておく
            one.selectSkill = one.skills[0]; 

            int oddsStack = 0;
            // ランダム値がoddsStack未満になったときにスキル決定
            for (int i=0; i<one.skills.Count; i++)
            {
                oddsStack += one.enemySkillOdds[i];
                if(randomCount < oddsStack)
                {
                    one.selectSkill = one.skills[i];
                    break;
                }
            }

            var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
            // 相手指定のスキルの場合は、ランダムで相手を選ぶ
            if (one.selectSkill.TargetType == TargetType.OpponentChoose)
            {
                randomCount = Random.Range(0, alivePlayers.Count);
                one.chooseTarget = alivePlayers[randomCount];
            }
            // 味方選択のスキルの場合は、ランダムで味方を選ぶ
            else if (one.selectSkill.TargetType == TargetType.FriendChoose)
            {
                randomCount = Random.Range(0, enemies.Count);
                one.chooseTarget = enemies[randomCount];
            }

            // スキルのターゲットを確定させる
            one.SetTarget();
        }

        next = new BattleExecutePhase();

        yield return null;
    }
}
