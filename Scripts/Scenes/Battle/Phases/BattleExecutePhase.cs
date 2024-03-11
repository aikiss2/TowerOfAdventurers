using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// バトルシーンのスキル実行フェーズクラス
public class BattleExecutePhase : BattlePhaseBase
{
    // バトルキャラクターを攻撃順に並べたリスト
    private readonly List<BattleCharacter> attackOrder = new();

    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        // スキルパネルを閉じる
        battleObjects.skillPanel.Close();

        // バトルキャラクターを攻撃順に並べる
        AttackOrderSort(battleObjects);

        bool isEscape = false;

        foreach (var attacker in attackOrder)
        {
            // このフェーズで死亡した敵キャラは攻撃できないようにする(プレイヤーキャラは削除されないためnullにならない)
            if (attacker == null)
            {
                continue;
            }

            // このフェーズで死亡した味方キャラは攻撃できないようにする
            if (attacker.IsDead == true) 
            {
                continue;
            }

            // ダイアログに実行者とスキル名称を表示する
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog($"{attacker.Name}の{attacker.selectSkill.Name}");

            // 味方実行時は前にせりだし、敵実行時は暗くする
            yield return attacker.SkillStart();

            // スキル実行
            yield return attacker.SkillExecute(GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList(), battleObjects.enemyPanel.GetEnemyList(), battleObjects.screenEffectCanvas);

            // 演出が終わるまで待つ
            while (true)
            {
                bool isExecuting = false;
                // 味方で演出実行中かチェックする
                foreach (var one in GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList())
                {
                    if (one.IsExecuting == true)
                    {
                        isExecuting = true;
                    }
                }

                bool enemyDelate = false;
                // 敵で演出実行中かチェックする。倒されている場合は経験値/ゴールドを加算する
                foreach (var one in battleObjects.enemyPanel.GetEnemyList())
                {
                    if (one.IsDead == true)
                    {
                        Utility.getPlayersExp += one.Exp; 
                        Utility.getGold += one.Gold;
                        enemyDelate = true;
                    }
                    else if (one.IsExecuting == true)
                    {
                        isExecuting = true;
                    }
                }

                // 敵が倒されている場合は敵パネルから削除する
                if (enemyDelate == true) {
                    battleObjects.enemyPanel.DelateSelectableImage();
                }

                // 演出実行中のキャラクターがいなければ、ループを抜ける
                if (isExecuting == false)
                {
                    break;
                }

                yield return null;
            }

            // 味方は前のせりだしを元に戻し、敵は暗くしたのを元に戻す
            yield return attacker.SkillEnd();

            // 敵か味方のどちらか居なくなれば、スキル実行は途中で終了する
            if ((GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList().Count == 0) || (battleObjects.enemyPanel.GetEnemyList().Count == 0))
            {
                break;
            }
            // 逃走した場合は、スキル実行は途中で終了する
            else if (attacker.IsEscape == true)
            {
                attacker.IsEscape = false;
                isEscape = true;
                break;
            }
        }

        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();

        // 逃走した場合は終了フェーズに遷移する
        if (isEscape == true)
        {
            next = new BattleEndPhase();
        }
        // 味方の生存数が0の場合は全滅演出する
        else if (alivePlayers.Count == 0)
        {
            // 全滅BGMを再生する
            SoundManager.soundManager.PlayBgm(BGMType.BgmNo12, false);

            // ダイアログを表示
            GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("パーティが戦闘不能になった・・・");

            // プレイヤーからの入力待ち(何か入力があれば次に進む)
            PlayerInput[] playerInput = new PlayerInput[1];
            yield return BattleManager.battleManager.WaitPlayerInput(playerInput);

            // 強制的に宿屋に戻るため、マップキャラの情報を消去しておく
            Utility.MapEnemyAndCharacterClear();

            // 宿屋シーンに遷移する
            Utility.nextScene = SceneName.Yadoya;

            next = new BattleEndPhase();
        }
        // 敵が0の場合はリザルトフェーズに遷移する
        else if (battleObjects.enemyPanel.GetEnemyList().Count == 0)
        {
            // バトル勝利BGMを再生する
            SoundManager.soundManager.PlayBgm(BGMType.BgmNo15, false);

            next = new BattleResultPhase();
        }
        // 味方も敵も生存している場合は、生存している味方のスキルパネルを表示する
        else
        {
            battleObjects.skillPanel.CreateSelectableTextWithTp(alivePlayers[0].skills);
            battleObjects.skillPanel.Open(alivePlayers[0]);
            next = new BattleChooseSkillPhase();
        }

        yield return null;
    }


    // バトルキャラクターを攻撃順に並べる
    private void AttackOrderSort(BattleObjects battleObjects)
    {
        // 生存している味方と敵を取得
        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
        var enemies = battleObjects.enemyPanel.GetEnemyList();

        // 味方と敵を同一リストに結合
        attackOrder.AddRange(alivePlayers);
        attackOrder.AddRange(enemies);

        // 順番を決めるためのAGIを算出する(AGI値の20%を上下に変動させる)
        foreach (var one in attackOrder)
        {
            int range = (int) one.AGI / 5;
            one.OrderAGI = one.AGI + Random.Range(0, (range * 2)+1 ) - range;
        }

        // 算出したAGI値で降順に並び替え
        attackOrder.Sort((a, b) => b.OrderAGI - a.OrderAGI);
    }
}
