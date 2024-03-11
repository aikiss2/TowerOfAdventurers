using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// UIを閉じるイベント(ただし、フェードアウトの黒画よりも前面に位置するようにする ※エンディング用)
public class UI_HideEvent : IEventTask
{
    // UIを閉じる
    public IEnumerator RunEvent(int index, SceneManagerBase sceneManager)
    {
        GlobalCanvasManager.globalCanvasManager.yesNoButtonPanel.Close();
        GlobalCanvasManager.globalCanvasManager.dialog.Close();
        GlobalCanvasManager.globalCanvasManager.playerPanel.Close();
        GlobalCanvasManager.globalCanvasManager.goldPanel.Close();

        // フェードアウトの黒画よりも前面に位置するようにレイヤー変更 ※エンディング用
        GlobalCanvasManager.globalCanvasManager.GetComponent<Canvas>().sortingLayerName = "Fade";

        yield break;
    }
}
