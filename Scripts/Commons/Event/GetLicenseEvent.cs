using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 冒険者証取得イベント
public class GetLicenseEvent : IEventTask
{
    // 
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        // BGMを再生する
        SoundManager.soundManager.PlayBgm(BGMType.BgmNo22, false);

        GlobalCanvasManager.globalCanvasManager.dialog.Open();
        GlobalCanvasManager.globalCanvasManager.dialog.TypeDialog("冒険者証を手に入れた！");

        // 加入BGMの終了まで待つ
        yield return new WaitForSeconds(6.2f);

        GlobalCanvasManager.globalCanvasManager.dialog.Close();

        // クリアイベントのBGMを再生する
        SoundManager.soundManager.PlayBgm(BGMType.BgmNo21);

        yield break;
    }
}
