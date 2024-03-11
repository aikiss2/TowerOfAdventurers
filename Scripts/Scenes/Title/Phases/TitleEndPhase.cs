using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーンの切り替えに必要

// タイトルシーンの終了フェーズクラス
public class TitleEndPhase : TitlePhaseBase
{
    // 本フェーズの実行メソッド
    public override IEnumerator Execute(TitleObjects titleObjects)
    {
        // バトルシーンに遷移する場合は、戦闘終了時にダンジョンに戻るよう設定する(遷移元をダンジョンにする)
        if(Utility.nextScene == SceneName.Battle)
        {
            Utility.beforeSceneName = SceneName.Dungeon1_1;
        }
        // バトルシーン以外の遷移は遷移元をタイトルシーンにする。
        else
        {
            Utility.beforeSceneName = SceneName.Title;
        }

        // 次シーンの文字列を生成する
        var nextSceneString = Utility.nextScene.ToString();
        Utility.nextScene = SceneName.None;

        // 次のシーンを読み込み、遷移する
        SceneManager.LoadScene(nextSceneString);

        yield return null;
    }
}
