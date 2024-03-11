using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// バトルシーンのリザルトフェーズクラス
public class BattleResultPhase : BattlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(BattleObjects battleObjects)
    {
        // ダイアログを閉じる
        GlobalCanvasManager.globalCanvasManager.dialog.Close();

        // リザルトパネルを生成する
        BattleManager.battleManager.ResultPanelCreate();

        // 取得した経験値を生存人数で割る
        var alivePlayers = GlobalCanvasManager.globalCanvasManager.playerPanel.GetAlivePlayerList();
        int getExp = Utility.getPlayersExp / alivePlayers.Count;
        Utility.getPlayersExp = 0;

        // 生存する味方に経験値を分配する
        foreach (var one in alivePlayers)
        {
            one.GetExp = getExp;
        }

        // 全員の経験値取得演出が終わるまでループ
        while (true)
        {
            bool isEnd = true;
            foreach (var one in alivePlayers)
            {
                if (one.GetExp != 0)
                {
                    isEnd = false;
                }
            }

            if (isEnd)
            {
                break;
            }

            yield return null;
        }

        // プレイヤーからの入力待ち(何か入力があれば次に進む)
        PlayerInput[] playerInput = new PlayerInput[1];
        yield return BattleManager.battleManager.WaitPlayerInput(playerInput);

        next = new BattleEndPhase();
    }
}
