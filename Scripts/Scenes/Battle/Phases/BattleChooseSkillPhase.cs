using System.Collections;
using UnityEngine;
using static SkillSO;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BattleChooseSkillPhase : BattlePhaseBase
{
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        // プレイヤーからの入力待ち(スクロールバー領域の左クリックは決定として処理しないよう指定)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return BattleManager.battleManager.WaitPlayerInputNoClickRect(playerInput, battleObjects.skillPanel.GetScrollbarRect());
        // 決定
        if (playerInput[0] == PlayerInput.Decide)
        {
            // 選択スキルが使用可能な場合
            if (battleObjects.skillPanel.GetUseable())
            {
                // 決定音ではうるさいため、選択音
                SoundManager.soundManager.SEPlay(SEType.SeNo2); 

                int currentID = battleObjects.skillPanel.CurrentID;

                // 選択スキルを決定する。また、選択スキルを記憶する
                var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
                alivePlayers[NowPlayerIndex].selectSkill = alivePlayers[NowPlayerIndex].skills[currentID];
                alivePlayers[NowPlayerIndex].memorySkill = alivePlayers[NowPlayerIndex].skills[currentID];

                // スキルパネルを閉じる
                battleObjects.skillPanel.Close();

                // 音の２度出しをなくすためWAITを入れる。
                // ３体敵の左端を選んでる状態で攻撃選択した際、スキルパネルを消してから猶予を与えれば、この待ちの間に無音のまま左端敵を選択できる。
                yield return null;

                // 味方選択スキルの場合、プレイヤーパネルを開く
                if ((alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.FriendChoose) || (alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.DeadFriendChoose))
                {
                    GlobalCanvasManager.globalCanvasManager.playerPanel.Open();
                }
                // 敵選択スキルの場合、敵パネルを開く
                else if (alivePlayers[NowPlayerIndex].selectSkill.TargetType == TargetType.OpponentChoose)
                {
                    battleObjects.enemyPanel.Open();
                }

                next = new BattleChooseTargetPhase();
                next.NowPlayerIndex = NowPlayerIndex;
            }
            // 選択スキルが使用不可の場合
            else
            {
                SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Reject);
                GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("TPが足りません");

                next = new BattleChooseSkillPhase();
                next.NowPlayerIndex = NowPlayerIndex;
            }
        }
        // キャンセル
        else if (playerInput[0] == PlayerInput.Cancel)
        {
            SoundManager.soundManager.InputReactionSEPlay(InputReactionSE.Cancel);

            // スキル選択中の味方を戻る(先頭の味方の場合は変わらず)
            if (NowPlayerIndex > 0) 
            {
                NowPlayerIndex--;
            }

            // 表示しているスキルをクリアする
            battleObjects.skillPanel.Clear();

            // パネル内容を変更する場合はWAITを入れる
            yield return new WaitForSeconds(0.05f);

            // スキル選択中の味方が所持しているスキルを表示する
            var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
            battleObjects.skillPanel.CreateSelectableTextWithTp(alivePlayers[NowPlayerIndex].skills);
            battleObjects.skillPanel.Open(alivePlayers[NowPlayerIndex]);

            next = new BattleChooseSkillPhase();
            next.NowPlayerIndex = NowPlayerIndex;
        }
        else
        {
            next = new BattleChooseSkillPhase();
        }

        yield return null;
    }

}
