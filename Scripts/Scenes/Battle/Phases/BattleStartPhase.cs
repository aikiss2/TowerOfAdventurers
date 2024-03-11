using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// バトルシーンの開始フェーズクラス
public class BattleStartPhase : BattlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        // 各種パネルを開く
        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Close();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);

        // 敵パネルは非選択にする
        battleObjects.enemyPanel.DeSelect();

        // 生存している味方のスキルパネルを開く
        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
        battleObjects.skillPanel.CreateSelectableTextWithTp(alivePlayers[0].skills);
        battleObjects.skillPanel.Open(alivePlayers[0]);

        // 背景をセットし、BGMを再生する
        battleObjects.battleBackground.sprite = GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBackground;
        SoundManager.soundManager.PlayBgm(GlobalCanvasManager.globalCanvasManager.encountEnemyDbSO.battleBgm);

        // 選択画面に遷移
        next = new BattleChooseSkillPhase();

        yield return null;
    }
}
