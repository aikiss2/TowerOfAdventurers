using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スキル屋シーンの開始フェーズクラス
public class SukiruyaStartPhase : SukiruyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // 各種パネルを開く
        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        sukiruyaObjects.selectButtonPanel.Open();

        // BGMを再生する
        SoundManager.soundManager.PlayBgm(SukiruyaManager.sukiruyaManager.DefaultBgm);

        // 選択画面に遷移
        next = new SukiruyaChoosePhase();

        yield return null;
    }
}
