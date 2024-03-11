using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// バトル結果表示パネル
public class ResultPlayerPanel : MonoBehaviour
{
    // 各プレイヤーのバトル結果のプレハブ
    [SerializeField] PlayerResultFrame playerResultFramePrefab;


    // バトル結果表示を開始する
    private void Start()
    {
        StartCoroutine(ResultSequence());
    }


    // バトル結果をシーケンス処理で表示する
    private IEnumerator ResultSequence()
    {
        List<PlayerResultFrame> resultFrameList = new();

        // 各プレイヤーの結果を時間差をつけながら順番に生成する
        foreach (var one in GlobalCanvasManager.globalCanvasManager.playerPanel.GetPlayerList())
        {
            var playerResultFrame = Instantiate(playerResultFramePrefab, transform);
            playerResultFrame.SetPlayer(one);

            resultFrameList.Add(playerResultFrame);

            yield return new WaitForSeconds(0.1f);
        }

        // 一旦WAITする(余韻)
        yield return new WaitForSeconds(0.5f);

        // 経験値取得を開始する
        foreach (var one in resultFrameList)
        {
            one.StartGetExp();
        }

        yield break;
    }

}
