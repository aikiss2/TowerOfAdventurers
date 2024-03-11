using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 宿屋シーンの開始フェーズクラス
public class YadoyaStartPhase : YadoyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(YadoyaObjects yadoyaObjects)
    {
        // 各種パネルを開く
        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Open();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Open(false);
        yadoyaObjects.selectButtonPanel.Open();

        // BGMを再生する
        SoundManager.soundManager.PlayBgm(YadoyaManager.yadoyaManager.DefaultBgm);

        // 選択画面に遷移
        next = new YadoyaChoosePhase();

        yield return null;
    }
}
