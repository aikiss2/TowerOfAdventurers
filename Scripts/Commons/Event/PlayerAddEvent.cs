using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// プレイヤー3加入イベント
public class PlayerAddEvent : IEventTask
{
    // プレイヤー3を加入する
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        // プレイヤー3を加入する
        GlobalCanvasManager.globalCanvasManager.playerPanel.AddPlayer3();

        // 加入BGMを再生する
        SoundManager.soundManager.PlayBgm(BGMType.BgmNo20, false);

        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("ヒカリが仲間になった！");

        // 加入BGMの終了まで待つ
        yield return new WaitForSeconds(8.2f);

        GlobalCanvasManager.globalCanvasManager.dialog.Close();

        // ダンジョンBGMを再生する
        SoundManager.soundManager.PlayBgm(BGMType.BgmNo6);

        yield break;
    }
}
