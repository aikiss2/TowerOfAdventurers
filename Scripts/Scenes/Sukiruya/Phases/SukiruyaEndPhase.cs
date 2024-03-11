﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーンの切り替えに必要

// スキル屋シーンの終了フェーズクラス
public class SukiruyaEndPhase : SukiruyaPhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(SukiruyaObjects sukiruyaObjects)
    {
        // 次のシーンに、遷移元の情報をセットする
        Utility.beforeSceneName = SceneName.Sukiruya;

        // トークテーブルをリセットし、各種データをセーブする
        GlobalCanvasManager.globalCanvasManager.talkWindow.TalkTableSceneReset();
        GlobalCanvasManager.globalCanvasManager.playerPanel.PlayerSave();

        // 次シーンの文字列を生成する
        var nextSceneString = Utility.nextScene.ToString();
        Utility.nextScene = SceneName.None;

        // 次のシーンを読み込み、遷移する
        SceneManager.LoadScene(nextSceneString);

        yield return null;
    }
}
