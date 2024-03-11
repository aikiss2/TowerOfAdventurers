using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タウンシーンの開始フェーズクラス
public class TownStartPhase : TownPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TownObjects townObjects)
    {
        // 各種パネルを開く
        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        townObjects.selectButtonPanel.Open();

        // BGMを再生する
        SoundManager.soundManager.PlayBgm(TownManager.townManager.DefaultBgm);

        // 選択画面に遷移
        next = new TownChoosePhase();

        yield return null;
    }
}
